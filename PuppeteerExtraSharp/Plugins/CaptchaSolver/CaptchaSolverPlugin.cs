using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerSharp;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver;

public class CaptchaSolverPlugin : PuppeteerExtraPlugin
{
    private readonly ICaptchaSolverProvider _provider;
    private readonly ICaptchaSolverHandler _handler;
    private readonly CaptchaSolverOptions _defaultOptions;

    public CaptchaSolverPlugin(
        ICaptchaSolverProvider provider, // Capsolver, TwoCaptcha
        CaptchaSolverOptions options = null,
        ICaptchaSolverHandler handler = null) : base("captcha-solver")
    {
        _defaultOptions = options ?? new CaptchaSolverOptions();
        _handler = handler ?? new CaptchaSolverHandler(provider, _defaultOptions);
        _provider = provider;
    }

    public async Task<EnterCaptchaSolutionsResult> SolveCaptchaAsync(IPage page,
        CaptchaSolverOptions optionsOverride = null)
    {
        var options = optionsOverride ?? _defaultOptions;

        var hasCaptchas = await _handler.WaitForCaptchasAsync(page, options.CaptchaWaitTimeout);

        if (!hasCaptchas)
        {
            return new EnterCaptchaSolutionsResult()
            {
                Error = "No captchas found"
            };
        }

        var captchaResponse = await _handler.FindCaptchasAsync(page);

        if (options.ThrowOnError)
        {
            throw new CaptchaException(page.Url, captchaResponse.Error);
        }

        var filteredCaptchas = FilterCaptchas(captchaResponse.Captchas, options);

        var captchas = filteredCaptchas.unfiltered.ToList();
        if (captchas.Count == 0)
        {
            return new EnterCaptchaSolutionsResult()
            {
                Error = "No captchas found or all captchas have been filtered",
                Filtered = filteredCaptchas.filtered,
            };
        }

        var solvedCaptchas = await _handler.SolveCaptchasAsync(page, captchas);
        var result = await _handler.EnterCaptchaSolutionsAsync(page, solvedCaptchas);
        result.Filtered = filteredCaptchas.filtered;

        if (options.ThrowOnError && string.IsNullOrWhiteSpace(result.Error))
        {
            throw new CaptchaException(page.Url, result.Error);
        }

        return result;
    }
    
    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        await page.SetBypassCSPAsync(true);
        foreach (var vendor in _defaultOptions.EnabledVendors.Keys)
        {
            var handler = Helpers.Helpers.CreateHandler(vendor, _provider, _defaultOptions, page);
            if (handler is null)
                continue;

            _ = handler.HandleOnPageCreatedAsync();
        }
    }

    private (ICollection<Captcha> unfiltered, ICollection<FilteredCaptcha> filtered) FilterCaptchas(ICollection<Captcha> captchas, CaptchaSolverOptions options)
    {
        var filteredCaptchas = new List<FilteredCaptcha>();
        var unfilteredCaptchas = new List<Captcha>();
        foreach (var captcha in captchas)
        {
            switch (captcha.CaptchaType)
            {
                case CaptchaType.invisible when !options.SolveInvisibleChallenges:
                    filteredCaptchas.Add(new FilteredCaptcha()
                    {
                        Captcha = captcha,
                        FilteredReason = "solveInvisibleChallenges"
                    });
                    continue;
                case CaptchaType.invisible when
                    !captcha.HasActiveChallengePopup &&
                    !options.SolveInactiveChallenges:
                    filteredCaptchas.Add(new FilteredCaptcha()
                    {
                        Captcha = captcha,
                        FilteredReason = "solveInactiveChallenges"
                    });
                    continue;
                case CaptchaType.score when !options.SolveScoreBased:
                    filteredCaptchas.Add(new FilteredCaptcha()
                    {
                        Captcha = captcha,
                        FilteredReason = "solveScoreBased"
                    });
                    continue;
                case CaptchaType.checkbox when !captcha.IsInViewport && options.SolveInViewportOnly:
                    filteredCaptchas.Add(new FilteredCaptcha()
                    {
                        Captcha = captcha,
                        FilteredReason = "solveInViewportOnly"
                    });
                    continue;
                default:
                    unfilteredCaptchas.Add(captcha);
                    break;
            }
        }

        return (unfilteredCaptchas, filteredCaptchas);
    }
}
