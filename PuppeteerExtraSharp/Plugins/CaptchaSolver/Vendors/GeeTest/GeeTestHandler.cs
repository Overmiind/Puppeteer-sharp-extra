using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerSharp;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.GeeTest;

public class GeeTestHandler(ICaptchaSolverProvider provider, CaptchaSolverOptions options, IPage page) : ICaptchaVendorHandler
{
    private string? _initEndpoint;
    private string? _gt;

    public CaptchaVendor Vendor => CaptchaVendor.GeeTest;

    public async Task<bool> WaitForCaptchasAsync(TimeSpan timeout)
    {
        var selector = "script[src*=\"static.geetest.com\"],script[src*=\"api.geetest.com\"],script[src*=\"gcaptcha4.geetest.com\"]";

        var handle = await page.WaitForSelectorAsync(
            selector,
            new WaitForSelectorOptions
            {
                Timeout = (int)timeout.TotalMilliseconds
            });

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
                "() => { const challengeInput = document.querySelector(\"input[name='geetest_challenge']\"); const hasWindowChallenge = window.__geetest_challenge && window.__geetest_challenge.length > 0; const hasInputChallenge = challengeInput && challengeInput.value && challengeInput.value.length > 0; return hasWindowChallenge || hasInputChallenge; }",
                new WaitForFunctionOptions
                {
                    Timeout = (int)timeout.TotalMilliseconds,
                    PollingInterval = 200
                });

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CaptchaResponse> FindCaptchasAsync()
    {
        return await page.EvaluateExpressionAsync<CaptchaResponse>("window.geeTestScript.findCaptchas()");
    }

    public async Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(ICollection<Captcha> captchas)
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
                Version = captcha.CaptchaType == CaptchaType.score ? CaptchaVersion.RecaptchaV3 : CaptchaVersion.RecaptchaV2,
                MinScore = options.MinScore,
                Gt = captcha.Gt,
                Challenge = captcha.Challenge,
                CaptchaId = captcha.CaptchaId,
                Vendor = CaptchaVendor.GeeTest
            });

            solutions.Add(new CaptchaSolution
            {
                Id = captcha.Id,
                Vendor = "geetest",
                Payload = payload,
            });
        }

        return solutions;
    }

    public async Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(ICollection<CaptchaSolution> solutions)
    {
        var result = await page.EvaluateFunctionAsync<EnterCaptchaSolutionsResult>(
            @"(solutions) => {return window.geeTestScript.enterCaptchaSolutions(solutions)}",
            solutions);

        if (result is null)
        {
            throw new NullReferenceException("EnterCaptchaSolutionsAsync failed, result is null");
        }

        return result;
    }
    public async void ProcessResponseAsync(object send, ResponseCreatedEventArgs e)
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
                        _initEndpoint = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";

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
                // Log error if debug is enabled
                if (options.Debug)
                {
                    await page.EvaluateExpressionAsync($"console.error('[GeeTest] ProcessResponse error: {ex.Message}')");
                }
            }
        }
    }
}
