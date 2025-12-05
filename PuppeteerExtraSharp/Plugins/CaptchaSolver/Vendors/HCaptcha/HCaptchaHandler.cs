using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerSharp;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.HCaptcha;

public class HCaptchaHandler(ICaptchaSolverProvider provider, CaptchaSolverOptions options, IPage page) : ICaptchaVendorHandler
{
    public CaptchaVendor Vendor => CaptchaVendor.HCaptcha;

    public async Task<bool> WaitForCaptchasAsync(TimeSpan timeout)
    {
        var handle = await page.QuerySelectorAsync("script[src*=\"js.hcaptcha.com/1/api.js\"], script[src*=\"hcaptcha.com/1/api.js\"]");
        var hasRecaptchaScriptTag = handle != null;

        if (!hasRecaptchaScriptTag) return false;

        const string selector =
            "iframe[src*='assets.hcaptcha.com/captcha/v1/'], " +
            "iframe[src*='newassets.hcaptcha.com/captcha/v1/']";
        try
        {
            var exist = await page.WaitForSelectorAsync(
                selector,
                new WaitForSelectorOptions
                {
                    Timeout = (int)timeout.TotalMilliseconds
                });

            return exist != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CaptchaResponse> FindCaptchasAsync()
    {
        return await page.EvaluateExpressionAsync<CaptchaResponse>("window.hcaptchaScript.findCaptchas()");
    }

    public async Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(ICollection<Captcha> captchas)
    {
        throw new NotSupportedException(
            $"hCaptcha solving support temporarily disabled.");
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
                Vendor = CaptchaVendor.HCaptcha
            });

            solutions.Add(new CaptchaSolution
            {
                Id = captcha.Id,
                Vendor = "hcaptcha",
                Payload = payload,
            });
        }

        return solutions;
    }

    public async Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(ICollection<CaptchaSolution> solutions)
    {
        var result = await page.EvaluateFunctionAsync<EnterCaptchaSolutionsResult>(
            @"(solutions) => {return window.hcaptchaScript.enterCaptchaSolutions(solutions)}",
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
