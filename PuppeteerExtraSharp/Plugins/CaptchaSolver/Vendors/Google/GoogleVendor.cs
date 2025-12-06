using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Helpers;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.Google;

public class GoogleVendor(ICaptchaSolverProvider provider, CaptchaOptions options) : ICaptchaVendor
{
    public CaptchaVendor Vendor => CaptchaVendor.Google;

    public async Task<bool> WaitForCaptchasAsync(IPage page, TimeSpan timeout)
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

    public async Task<CaptchaResponse> FindCaptchasAsync(IPage page)
    {
        await LoadScriptAsync(page);
        return await page.EvaluateExpressionAsync<CaptchaResponse>("window.reScript.findRecaptchas()");
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
                Version = captcha.CaptchaType == CaptchaType.score
                    ? CaptchaVersion.RecaptchaV3
                    : CaptchaVersion.RecaptchaV2,
                MinScore = options.MinScore,
                Vendor = CaptchaVendor.Google
            });

            solutions.Add(new CaptchaSolution
            {
                Id = captcha.Id,
                Vendor = CaptchaVendor.Google,
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
            @"(solutions) => {return window.reScript.enterRecaptchaSolutions(solutions)}",
            solutions);

        if (result is null)
        {
            throw new NullReferenceException("EnterCaptchaSolutionsAsync failed, result is null");
        }

        return result;
    }

    public Task HandleOnPageCreatedAsync(IPage page) => Task.CompletedTask;

    public void ProcessResponseAsync(IPage page, object send, ResponseCreatedEventArgs e)
    {
    }

    private Task LoadScriptAsync(IPage page)
    {
        return page.EnsureEvaluateFunctionAsync(
            $"{GetType().Namespace}.{nameof(CaptchaVendor.Google)}Script.js", options);
    }
}