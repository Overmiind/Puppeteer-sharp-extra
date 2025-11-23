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

public class GoogleHandler(ICaptchaSolverProvider provider, CaptchaSolverOptions options) : ICaptchaVendorHandler
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
        return await page.EvaluateExpressionAsync<CaptchaResponse>("window.reScript.findRecaptchas()");
    }

    public async Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas)
    {
        var solutions = new List<CaptchaSolution>();
        foreach (var captcha in captchas)
        {
            var solution = await provider.GetSolutionAsync(new GetCaptchaSolutionRequest
            {
                Action = captcha.Action,
                DataS = captcha.S,
                IsEnterprise = captcha.IsEnterprise,
                IsInvisible = captcha.IsInvisible,
                PageUrl = captcha.Url,
                SiteKey = captcha.Sitekey,
                Version = captcha.CaptchaType == CaptchaType.score ? CaptchaVersion.RecaptchaV3 : CaptchaVersion.RecaptchaV2,
                MinScore = options.MinScore,
            });

            solutions.Add(new CaptchaSolution
            {
                Id = captcha.Id,
                Text = solution,
            });
        }

        return solutions;
    }

    public async Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(IPage page, ICollection<CaptchaSolution> solutions)
    {
        var solutionArgs = solutions.Select(s => new
        {
            id = s.Id,
            text = s.Text,
        });

        var result = await page.EvaluateFunctionAsync<EnterCaptchaSolutionsResult>(
            @"(solutions) => {return window.reScript.enterRecaptchaSolutions(solutions)}",
            solutionArgs);

        if (result is null)
        {
            throw new NullReferenceException("EnterCaptchaSolutionsAsync failed, result is null");
        }

        return result;
    }
}
