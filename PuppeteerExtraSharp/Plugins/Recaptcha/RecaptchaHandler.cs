using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Helpers;
using PuppeteerExtraSharp.Plugins.Recaptcha.Models;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha;

public class RecaptchaHandler(IRecaptchaProvider provider, RecaptchaSolveOptions options) : ICaptchaHandler
{
    private readonly RecaptchaSolveOptions _options = options ?? new RecaptchaSolveOptions();
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
                    // Use TotalMilliseconds to avoid truncating to 0-999 ms component
                    Timeout = (int)timeout.TotalMilliseconds,
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
        await LoadScriptAsync(page, _options);
        // Double-check presence of helper and attempt one more load if missing (e.g., after navigation)
        var hasScript = await page.EvaluateExpressionAsync<bool>("typeof window.reScript !== 'undefined'");
        if (!hasScript)
        {
            await LoadScriptAsync(page, _options);
        }
        var captchaResponse = await page.EvaluateExpressionAsync<CaptchaResponse>("window.reScript.findRecaptchas()");
        return captchaResponse;
    }

    public async Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas)
    {
        await LoadScriptAsync(page, _options);

        var solutions = new List<CaptchaSolution>();
        foreach (var captcha in captchas)
        {
            var solution = await provider.GetSolutionAsync(new GetRecaptchaSolutionRequest()
            {
                Action = captcha.Action,
                DataS = captcha.S,
                IsEnterprise = captcha.IsEnterprise,
                IsInvisible = captcha.IsInvisible,
                PageUrl = captcha.Url,
                SiteKey = captcha.Sitekey,
                Version = captcha.CaptchaType  == CaptchaType.score ? CaptchaVersion.V3 : CaptchaVersion.V2,
                MinV3RecaptchaScore = _options.MinV3RecaptchaScore,
            });

            solutions.Add(new CaptchaSolution
            {
                Id = captcha.Id,
                Text = solution,
            });
        }

        return solutions;
    }

    public async Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(IPage page,
        ICollection<CaptchaSolution> solutions)
    {
        // Ensure helper is available before invoking it
        await LoadScriptAsync(page, _options);
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

    private async Task LoadScriptAsync(IPage page, params object[] args)
    {
        var recaptchaScriptName = GetType().Namespace + ".Scripts.RecaptchaScript.js";
        var script = ResourcesReader.ReadFile(recaptchaScriptName);
        await page.EnsureEvaluateFunctionAsync(recaptchaScriptName, script, args);

        // Wait until the global helper is available to avoid race conditions
        try
        {
            await page.WaitForFunctionAsync(
                "() => typeof window.reScript !== 'undefined'",
                new WaitForFunctionOptions
                {
                    PollingInterval = 100,
                    Timeout = 5000,
                });
        }
        catch
        {
            // If this fails, caller will attempt to reload. No throw here to stay resilient.
        }
    }
}