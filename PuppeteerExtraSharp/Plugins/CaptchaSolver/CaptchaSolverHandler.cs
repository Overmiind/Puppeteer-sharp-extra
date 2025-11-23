using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Helpers;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver;

public class CaptchaSolverHandler(ICaptchaSolverProvider provider, CaptchaSolverOptions options) : ICaptchaSolverHandler
{
    private readonly CaptchaSolverOptions _options = options ?? new CaptchaSolverOptions();
    private ICaptchaVendorHandler? _activeHandler;

    public CaptchaVendor? Vendor { get; }
    public async Task<bool> WaitForCaptchasAsync(IPage page, TimeSpan timeout)
    {
        foreach (var (vendor, options) in _options.EnabledVendors)
        {
            var handler = Helpers.Helpers.CreateHandler(vendor, provider, _options, page);
            if (handler is null)
                continue;

            var handled = await handler.WaitForCaptchasAsync(timeout);
            if (handled)
            {
                // Support only one active handler at a time
                _activeHandler = handler;
                return true;
            }
        }

        return false;
    }

    public async Task<CaptchaResponse> FindCaptchasAsync(IPage page)
    {
        if (_activeHandler is null)
        {
            return new CaptchaResponse
            {
                Error = "No active captcha handler found",
            };
        }

        await LoadScriptAsync(page, _options);
        return await _activeHandler.FindCaptchasAsync();
    }

    public async Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas)
    {
        await LoadScriptAsync(page, _options);
        return await _activeHandler.SolveCaptchasAsync(captchas);
    }

    public async Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(IPage page, ICollection<CaptchaSolution> solutions)
    {
        return await _activeHandler.EnterCaptchaSolutionsAsync(solutions);
    }

    private Task LoadScriptAsync(IPage page, params object[] args)
    {
        if (_activeHandler is null)
            return Task.CompletedTask;

        var recaptchaScriptName = $"{GetType().Namespace}.Vendors.{_activeHandler.Vendor}.{_activeHandler.Vendor}Script.js";
        var script = ResourcesReader.ReadFile(recaptchaScriptName);
        return page.EnsureEvaluateFunctionAsync(recaptchaScriptName, script, args);
    }
}
