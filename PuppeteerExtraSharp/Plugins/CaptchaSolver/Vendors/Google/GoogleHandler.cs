using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerSharp;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.Google;

public class GoogleHandler(ICaptchaSolverProvider provider, CaptchaSolverOptions options, IPage page) : ICaptchaVendorHandler
{
    public CaptchaVendor Vendor => CaptchaVendor.Google;

    public async Task<bool> WaitForCaptchasAsync(TimeSpan timeout)
    {
        var handle = await page.QuerySelectorAsync(
            "script[src*=\"/recaptcha/api.js\"], script[src*=\"/recaptcha/enterprise.js\"]");
        var hasRecaptchaScriptTag = handle != null;


        if (!hasRecaptchaScriptTag) return false;

        try
        {
            await page.WaitForFunctionAsync(
                "() => Object.keys((window.___grecaptcha_cfg || {}).clients || {}).length > 0",
                options: new WaitForFunctionOptions
                {
                    PollingInterval = 500,
                    Timeout = timeout.Milliseconds,
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
        return await page.EvaluateExpressionAsync<CaptchaResponse>("window.reScript.findRecaptchas()");
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
                Vendor = CaptchaVendor.Google
            });

            solutions.Add(new CaptchaSolution
            {
                Id = captcha.Id,
                Vendor = "google",
                Payload = payload,
            });
        }

        return solutions;
    }

    public async Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(ICollection<CaptchaSolution> solutions)
    {
        var result = await page.EvaluateFunctionAsync<EnterCaptchaSolutionsResult>(
            @"(solutions) => {return window.reScript.enterRecaptchaSolutions(solutions)}",
            solutions);

        if (result is null)
        {
            throw new NullReferenceException("EnterCaptchaSolutionsAsync failed, result is null");
        }

        return result;
    }

    public Task HandleOnPageCreatedAsync() => Task.CompletedTask;

    public void ProcessResponseAsync(object send, ResponseCreatedEventArgs e) { }
}
