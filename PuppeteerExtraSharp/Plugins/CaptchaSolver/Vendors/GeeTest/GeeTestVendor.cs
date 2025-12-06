using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Helpers;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.GeeTest;

public class GeeTestVendor(ICaptchaSolverProvider provider, CaptchaOptionsScope options) : ICaptchaVendor
{
    public CaptchaVendor Vendor => CaptchaVendor.GeeTest;

    public async Task<bool> WaitForCaptchasAsync(IPage page, TimeSpan timeout)
    {
        IElementHandle handle = null;
        var selector =
            "script[src*=\"static.geetest.com\"],script[src*=\"api.geetest.com\"],script[src*=\"gcaptcha4.geetest.com\"]";

        try
        {
            handle = await page.WaitForSelectorAsync(
                selector,
                new WaitForSelectorOptions
                {
                    Timeout = (int)timeout.TotalMilliseconds
                });
        }
        catch
        {
            return false;
        }

        var hasCaptchaScriptTag = handle != null;

        if (!hasCaptchaScriptTag) return false;

        selector =
            "div.geetest_holder, " +
            "div[class*='geetest'], " +
            "input[name='geetest_challenge'], " +
            "input[name='geetest_validate'], " +
            "input[name='geetest_seccode']";

        try
        {
            var exist = await page.WaitForSelectorAsync(
                selector,
                new WaitForSelectorOptions
                {
                    Timeout = (int)timeout.TotalMilliseconds
                });

            if (exist == null) return false;

            await page.WaitForFunctionAsync(
                "() => { return window.__geetest_challenge || window.__geetest_captcha_id; }",
                new WaitForFunctionOptions
                {
                    Timeout = (int)timeout.TotalMilliseconds,
                    PollingInterval = 200
                });

            await Task.Delay(2000);
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
        return await page.EvaluateExpressionAsync<CaptchaResponse>("window.geeTestScript.findCaptchas()");
    }

    public async Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas)
    {
        var solutions = new List<CaptchaSolution>();
        foreach (var captcha in captchas)
        {
            var payload = await provider.GetSolutionAsync(new GetCaptchaSolutionRequest
            {
                Action = captcha.Action,
                DataS = captcha.S,
                IsEnterprise = captcha.IsEnterprise,
                IsInvisible = captcha.IsInvisible,
                PageUrl = captcha.Url,
                SiteKey = captcha.Sitekey,
                Version = captcha.Version == "v4" || !string.IsNullOrEmpty(captcha.CaptchaId)
                    ? CaptchaVersion.GeeTestV4
                    : CaptchaVersion.GeeTestV3,
                MinScore = options.Current.MinScore,
                Gt = captcha.Gt,
                Challenge = captcha.Challenge,
                CaptchaId = captcha.CaptchaId,
                Vendor = CaptchaVendor.GeeTest
            });

            solutions.Add(new CaptchaSolution
            {
                Id = captcha.Id,
                Vendor = CaptchaVendor.GeeTest,
                Payload = payload,
            });
        }

        return solutions;
    }

    public async Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(IPage page,
        ICollection<CaptchaSolution> solutions)
    {
        await LoadScriptAsync(page);
        var result = await page.EvaluateFunctionAsync<EnterCaptchaSolutionsResult>(
            @"(solutions) => {return window.geeTestScript.enterCaptchaSolutions(solutions)}",
            solutions);

        if (result is null)
        {
            throw new NullReferenceException("EnterCaptchaSolutionsAsync failed, result is null");
        }

        return result;
    }

    public async Task HandleOnPageCreatedAsync(IPage page)
    {
        await page.EnsureEvaluateExpressionOnNewDocumentAsync($"{GetType().Namespace}.{nameof(CaptchaVendor.GeeTest)}InterceptorScript.js");
        page.Response += (sender, args) => ProcessResponseAsync(page, sender, args);
    }

    public async void ProcessResponseAsync(IPage page, object send, ResponseCreatedEventArgs e)
    {
        var url = e.Response.Url;

        if (url != null && (url.Contains("gee-test/init-params") || url.Contains("gcaptcha4.geetest.com")))
        {
            try
            {
                var headers = e.Response.Headers?["Content-Type"];
                var uri = new Uri(url);
                await page.EvaluateExpressionAsync($"window.__geetest_url = '{url}'");

                if (headers?.Contains("application/json") == true)
                {
                    var response = await e.Response.TextAsync().ConfigureAwait(false);
                    if (string.IsNullOrEmpty(response)) return;

                    using var doc = JsonDocument.Parse(response);
                    var json = doc.RootElement;

                    // Try to extract gt and challenge from various response formats
                    json.TryGetProperty("gt", out var gt);
                    json.TryGetProperty("challenge", out var challenge);

                    if (gt.ValueKind == JsonValueKind.String && challenge.ValueKind == JsonValueKind.String)
                    {
                        // _initEndpoint = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";

                        await page.EvaluateExpressionAsync($"window.__geetest_gt = '{gt.GetString()}'");
                        await page.EvaluateExpressionAsync($"window.__geetest_challenge = '{challenge.GetString()}'");
                    }
                }


                if (headers?.Contains("text/javascript") == true)
                {
                    var queryParams = HttpUtility.ParseQueryString(uri.Query);
                    var captchaId = queryParams["captcha_id"];
                    await page.EvaluateExpressionAsync($"window.__geetest_captcha_id = '{captchaId}'");
                }
            }
            catch (Exception ex)
            {
                if (options.Current.Debug)
                {
                    await page.EvaluateExpressionAsync(
                        $"console.error('[GeeTest] ProcessResponse error: {ex.Message}')");
                }
            }
        }
    }

    private Task LoadScriptAsync(IPage page)
    {
        return page.EnsureEvaluateFunctionAsync(
            $"{GetType().Namespace}.{nameof(CaptchaVendor.GeeTest)}Script.js", options.Current);
    }
}