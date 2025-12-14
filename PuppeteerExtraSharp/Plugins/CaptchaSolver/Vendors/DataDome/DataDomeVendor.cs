using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Helpers;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.DataDome;

public class DataDomeVendor(ICaptchaSolverProvider provider, CaptchaOptionsScope options) : ICaptchaVendor
{
    public CaptchaVendor Vendor => CaptchaVendor.DataDome;

    public async Task<bool> WaitForCaptchasAsync(IPage page, TimeSpan timeout)
    {
        // First check for DataDome script tags
        var scriptSelector = "script[src*='datadome'], script[src*='captcha-delivery.com']";

        IElementHandle handle;
        try
        {
            handle = await page.WaitForSelectorAsync(
                scriptSelector,
                new WaitForSelectorOptions
                {
                    Timeout = (int)timeout.TotalMilliseconds
                });
        }
        catch
        {
            return false;
        }

        if (handle == null) return false;

        // Wait for captcha elements to appear (iframe or container)
        var captchaSelector =
            "iframe[src*='geo.captcha-delivery.com'], " +
            "iframe[src*='datadome'], " +
            "iframe[src*='captcha-delivery'], " +
            "#datadome-captcha, " +
            ".datadome-captcha";

        try
        {
            var exist = await page.WaitForSelectorAsync(
                captchaSelector,
                new WaitForSelectorOptions
                {
                    Timeout = (int)timeout.TotalMilliseconds
                });

            if (exist == null) return false;

            // Wait for dynamic parameters to be captured by the interceptor
            await page.WaitForFunctionAsync(
                "() => { return window.__datadome_cid || window.__datadome_captcha_url; }",
                new WaitForFunctionOptions
                {
                    Timeout = (int)timeout.TotalMilliseconds,
                    PollingInterval = 200
                });

            // Allow time for parameters to stabilize
            await Task.Delay(1000);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CaptchaResponse> FindCaptchasAsync(IPage page)
    {
        await LoadScriptAsync(page);
        return await page.EvaluateExpressionAsync<CaptchaResponse>("window.dataDomeScript.findCaptchas()");
    }

    public async Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas)
    {
        var currentOptions = options.Current;

        // DataDome requires a proxy - check if configured
        if (!currentOptions.HasProxy)
        {
            throw new InvalidOperationException(
                "DataDome captcha solving requires a proxy. " +
                "Please configure ProxyAddress, ProxyPort (and optionally ProxyLogin/ProxyPassword) in CaptchaOptions.");
        }

        // Get the browser's actual User-Agent - this MUST match what the solving service uses
        var browserUserAgent = await page.EvaluateExpressionAsync<string>("navigator.userAgent");

        if (currentOptions.Debug)
        {
            await page.EvaluateExpressionAsync(
                $"console.log('[DataDome] Browser UserAgent: {EscapeJs(browserUserAgent)}')");
        }

        var solutions = new List<CaptchaSolution>();
        foreach (var captcha in captchas)
        {
            // Skip captchas without a valid captcha URL - these are likely block pages without a solvable captcha
            if (string.IsNullOrEmpty(captcha.DataDomeCaptchaUrl))
            {
                if (currentOptions.Debug)
                {
                    await page.EvaluateExpressionAsync(
                        "console.log('[DataDome] Skipping captcha - no captcha URL found (likely a block page)')");
                }
                continue;
            }

            try
            {
                var payload = await provider.GetSolutionAsync(new GetCaptchaSolutionRequest
                {
                    PageUrl = captcha.Url,
                    Vendor = CaptchaVendor.DataDome,
                    Version = CaptchaVersion.DataDome,
                    // DataDome-specific fields
                    DataDomeCaptchaUrl = captcha.DataDomeCaptchaUrl,
                    DataDomeCid = captcha.DataDomeCid,
                    DataDomeHash = captcha.DataDomeHash,
                    // Use browser's actual UserAgent, not the one from JS (they should match)
                    DataDomeUserAgent = browserUserAgent,
                    DataDomeReferer = captcha.DataDomeReferer ?? page.Url,
                    // Proxy settings (required for DataDome)
                    ProxyType = currentOptions.ProxyType ?? "http",
                    ProxyAddress = currentOptions.ProxyAddress,
                    ProxyPort = currentOptions.ProxyPort,
                    ProxyLogin = currentOptions.ProxyLogin,
                    ProxyPassword = currentOptions.ProxyPassword,
                    ProxySessionId = currentOptions.ProxySessionId,
                });

                solutions.Add(new CaptchaSolution
                {
                    Id = captcha.Id,
                    Vendor = CaptchaVendor.DataDome,
                    Payload = payload,
                });
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("blocked captcha url") ||
                                                   ex.Message.Contains("ERROR_INVALID_TASK_DATA"))
            {
                // This captcha URL is blocked or unsupported by the solver - skip it
                if (currentOptions.Debug)
                {
                    await page.EvaluateExpressionAsync(
                        $"console.log('[DataDome] Captcha URL blocked or unsupported by solver: {EscapeJs(ex.Message)}')");
                }
                continue;
            }
        }

        return solutions;
    }

    public async Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(IPage page,
        ICollection<CaptchaSolution> solutions)
    {
        await LoadScriptAsync(page);

        var result = await page.EvaluateFunctionAsync<DataDomeEnterSolutionsResult>(
            @"(solutions) => {return window.dataDomeScript.enterCaptchaSolutions(solutions)}",
            solutions);

        if (result is null)
        {
            throw new NullReferenceException("EnterCaptchaSolutionsAsync failed, result is null");
        }

        // DO NOT reload here - let the caller handle it
        // The NeedsReload flag will be exposed via EnterCaptchaSolutionsResult

        if (result.NeedsReload && result.Solved != null && result.Solved.Any(s => s.IsSolved == true))
        {
            // Small delay to ensure the cookie is properly set before caller reloads
            await Task.Delay(500);

            if (options.Current.Debug)
            {
                await page.EvaluateExpressionAsync(
                    "console.log('[DataDome] Cookie set, caller should reload the page')");
            }
        }

        return new EnterCaptchaSolutionsResult
        {
            Solved = result.Solved,
            Error = result.Error,
            NeedsReload = result.NeedsReload
        };
    }

    /// <summary>
    /// Internal result class that includes NeedsReload flag from JavaScript
    /// </summary>
    private class DataDomeEnterSolutionsResult
    {
        public ICollection<CaptchaSolved> Solved { get; set; } = new List<CaptchaSolved>();
        public string Error { get; set; }
        public bool NeedsReload { get; set; }
    }

    public async Task HandleOnPageCreatedAsync(IPage page)
    {
        // Inject interceptor script early to capture DataDome parameters
        await page.EnsureEvaluateExpressionOnNewDocumentAsync(
            $"{GetType().Namespace}.{nameof(CaptchaVendor.DataDome)}InterceptorScript.js");
        page.Response += (sender, args) => ProcessResponseAsync(page, sender, args);
    }

    public async void ProcessResponseAsync(IPage page, object? send, ResponseCreatedEventArgs e)
    {
        var url = e.Response.Url;

        // Intercept DataDome captcha URLs to extract parameters
        if (url != null && (url.Contains("geo.captcha-delivery.com") ||
                           url.Contains("datadome.co") ||
                           url.Contains("captcha-delivery")))
        {
            try
            {
                var uri = new Uri(url);
                var queryParams = HttpUtility.ParseQueryString(uri.Query);

                // Store captcha URL for solving
                await page.EvaluateExpressionAsync($"window.__datadome_captcha_url = '{EscapeJs(url)}'");

                // Extract common parameters
                var cid = queryParams["cid"];
                var hash = queryParams["hash"] ?? queryParams["hsh"];
                var t = queryParams["t"];
                var s = queryParams["s"];
                var referer = queryParams["referer"];

                if (!string.IsNullOrEmpty(cid))
                    await page.EvaluateExpressionAsync($"window.__datadome_cid = '{EscapeJs(cid)}'");
                if (!string.IsNullOrEmpty(hash))
                    await page.EvaluateExpressionAsync($"window.__datadome_hash = '{EscapeJs(hash)}'");
                if (!string.IsNullOrEmpty(t))
                    await page.EvaluateExpressionAsync($"window.__datadome_t = '{EscapeJs(t)}'");
                if (!string.IsNullOrEmpty(s))
                    await page.EvaluateExpressionAsync($"window.__datadome_s = '{EscapeJs(s)}'");
                if (!string.IsNullOrEmpty(referer))
                    await page.EvaluateExpressionAsync($"window.__datadome_referer = '{EscapeJs(referer)}'");

                if (options.Current.Debug)
                {
                    await page.EvaluateExpressionAsync(
                        $"console.log('[DataDome] Captured params: cid={cid}, hash={hash}, t={t}')");
                }
            }
            catch (Exception ex)
            {
                if (options.Current.Debug)
                {
                    await page.EvaluateExpressionAsync(
                        $"console.error('[DataDome] ProcessResponse error: {EscapeJs(ex.Message)}')");
                }
            }
        }
    }

    private Task LoadScriptAsync(IPage page)
    {
        return page.EnsureEvaluateFunctionAsync(
            $"{GetType().Namespace}.{nameof(CaptchaVendor.DataDome)}Script.js", options.Current);
    }

    private static string EscapeJs(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }
}
