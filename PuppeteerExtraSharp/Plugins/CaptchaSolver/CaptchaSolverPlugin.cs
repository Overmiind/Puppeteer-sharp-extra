using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver;

public class CaptchaSolverPlugin : PuppeteerExtraPlugin
{
    private readonly ICaptchaSolver _solver;
    private readonly CaptchaOptionsScope _optionsScope;

    public CaptchaSolverPlugin(
        ICaptchaSolverProvider provider, // Capsolver, TwoCaptcha
        CaptchaOptions options = null) : base("captcha-solver")
    {
        _optionsScope = new CaptchaOptionsScope(options);
        _solver = new CaptchaSolver(provider, _optionsScope);
    }

    public async Task<CaptchaResponse> FindCaptchaAsync(
        IPage page, 
        CaptchaOptions optionsOverride = null)
    {
        using var scope = _optionsScope.CreateScope(optionsOverride ?? _optionsScope.Current);

        return await FindCaptchaInternalAsync(page);
    }

    public async Task<EnterCaptchaSolutionsResult> SolveCaptchaAsync(
        IPage page,
        CaptchaOptions optionsOverride = null)
    {
        using var scope = _optionsScope.CreateScope(optionsOverride ?? _optionsScope.Current);

        var captchaResponse = await FindCaptchaInternalAsync(page);

        if (!captchaResponse.Captchas.Any())
        {
            return new EnterCaptchaSolutionsResult()
            {
                Error = "No captchas found"
            };
        }

        if (_optionsScope.Current.ThrowOnError && !string.IsNullOrWhiteSpace(captchaResponse.Error))
        {
            throw new CaptchaException(page.Url, captchaResponse.Error);
        }

        var filteredCaptchas = FilterCaptchas(captchaResponse.Captchas, _optionsScope.Current);

        var captchas = filteredCaptchas.unfiltered.ToList();
        if (captchas.Count == 0)
        {
            return new EnterCaptchaSolutionsResult()
            {
                Error = "No captchas found or all captchas have been filtered",
                Filtered = filteredCaptchas.filtered,
            };
        }

        var solvedCaptchas = await _solver.SolveCaptchasAsync(page, captchas);
        var entered = await _solver.EnterCaptchaSolutionsAsync(page, solvedCaptchas);

        // TODO
        var first = entered.FirstOrDefault();
        var result = new EnterCaptchaSolutionsResult()
        {
            Filtered = filteredCaptchas.filtered,
            Solved = first?.Solved,
            Error = first?.Error,
        };

        if (_optionsScope.Current.ThrowOnError && !string.IsNullOrWhiteSpace(result.Error))
        {
            throw new CaptchaException(page.Url, result.Error);
        }

        return result;
    }

    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        await page.SetBypassCSPAsync(true);
        await _solver.OnPageCreatedAsync(page);
    }

    private async Task<CaptchaResponse> FindCaptchaInternalAsync(IPage page)
    {
        var captchaVendors = await _solver.WaitForCaptchasAsync(page, _optionsScope.Current.CaptchaWaitTimeout);

        if (captchaVendors.Count == 0)
        {
            return new CaptchaResponse
            {
                Captchas = [],
                Error = "No captchas found"
            };
        }

        // TODO: Only the first vendor is handled for now (useful for almost all cases, except when a page contains more than two captcha vendors (e.g., Google + Cloudflare))
        var found = await _solver.FindCaptchasAsync(captchaVendors.Take(1).ToHashSet(), page);

        return found.FirstOrDefault();
    }

    private (ICollection<Captcha> unfiltered, ICollection<FilteredCaptcha> filtered) FilterCaptchas(
        ICollection<Captcha> captchas, CaptchaOptions options)
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
                        Vendor = captcha.Vendor,
                        Captcha = captcha,
                        FilteredReason = "solveInvisibleChallenges"
                    });
                    continue;
                case CaptchaType.invisible when
                    !captcha.HasActiveChallengePopup &&
                    !options.SolveInactiveChallenges:
                    filteredCaptchas.Add(new FilteredCaptcha()
                    {
                        Vendor = captcha.Vendor,
                        Captcha = captcha,
                        FilteredReason = "solveInactiveChallenges"
                    });
                    continue;
                case CaptchaType.score when !options.SolveScoreBased:
                    filteredCaptchas.Add(new FilteredCaptcha()
                    {
                        Vendor = captcha.Vendor,
                        Captcha = captcha,
                        FilteredReason = "solveScoreBased"
                    });
                    continue;
                case CaptchaType.checkbox when !captcha.IsInViewport && options.SolveInViewportOnly:
                    filteredCaptchas.Add(new FilteredCaptcha()
                    {
                        Vendor = captcha.Vendor,
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