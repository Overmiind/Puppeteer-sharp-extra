using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Helpers;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.Cloudflare;

public class CloudflareVendor(ICaptchaSolverProvider provider, CaptchaOptions options) : ICaptchaVendor
{
    public CaptchaVendor Vendor => CaptchaVendor.Cloudflare;

    public async Task<bool> WaitForCaptchasAsync(IPage page, TimeSpan timeout)
    {
        var handle =
            await page.QuerySelectorAsync(
                "script[src*=\"challenges.cloudflare.com/turnstile\"], script[src*=\"/turnstile/v0/api.js\"]");

        var hasRecaptchaScriptTag = handle != null;

        if (!hasRecaptchaScriptTag) return false;

        const string selector = "div.cf-turnstile[data-sitekey], input[name='cf-turnstile-response']";

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

    public async Task<CaptchaResponse> FindCaptchasAsync(IPage page)
    {
        await LoadScriptAsync(page);
        return await page.EvaluateExpressionAsync<CaptchaResponse>("window.cfScript.findCaptchas()");
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
                Vendor = CaptchaVendor.Cloudflare
            });

            solutions.Add(new CaptchaSolution
            {
                Id = captcha.Id,
                Vendor = CaptchaVendor.Cloudflare,
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
            @"(solutions) => {return window.cfScript.enterCaptchaSolutions(solutions)}",
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
            $"{GetType().Namespace}.{nameof(CaptchaVendor.Cloudflare)}Script.js", options);
    }
}