using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.Cloudflare;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.GeeTest;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.Google;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.HCaptcha;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver;

internal class CaptchaSolver : ICaptchaSolver
{
    private readonly CaptchaOptionsScope _optionsScope;
    private readonly HashSet<ICaptchaVendor> _captchaVendorHandlers;

    public CaptchaSolver(ICaptchaSolverProvider provider, CaptchaOptionsScope optionsScope)
    {
        _optionsScope = optionsScope;
        _captchaVendorHandlers = GetCaptchaVendorHandlers(provider, _optionsScope.Current);
    }

    public async Task<ICollection<CaptchaVendor>> WaitForCaptchasAsync(IPage page, TimeSpan timeout)
    {
        var result = new List<CaptchaVendor>();

        foreach (var vendor in FilterCaptchaVendors(_optionsScope.Current.EnabledVendors))
        {
            var handled = await vendor.WaitForCaptchasAsync(page, timeout);
            if (handled)
            {
                result.Add(vendor.Vendor);
            }
        }

        return result;
    }

    public async Task<ICollection<CaptchaResponse>> FindCaptchasAsync(HashSet<CaptchaVendor> vendors, IPage page)
    {
        var result = new List<CaptchaResponse>();

        foreach (var captchaVendor in FilterCaptchaVendors(vendors))
        {
            var captcha = await captchaVendor.FindCaptchasAsync(page);

            result.Add(captcha);
        }

        return result;
    }

    public async Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas)
    {
        var result = new List<CaptchaSolution>();

        var groupByVendor = captchas.GroupBy(e => e.Vendor);
        foreach (var groupedCapchas in groupByVendor)
        {
            var vedorHandler = FindVendor(groupedCapchas.Key);

            var solved = await vedorHandler.SolveCaptchasAsync(page, groupedCapchas.ToList());
            result.AddRange(solved);
        }

        return result;
    }

    public async Task<ICollection<EnterCaptchaSolutionsResult>> EnterCaptchaSolutionsAsync(IPage page, ICollection<CaptchaSolution> solutions)
    {
        var result = new List<EnterCaptchaSolutionsResult>();
        var groupByVendor = solutions.GroupBy(e => e.Vendor);

        foreach (var groupedSolutions in groupByVendor)
        {
            var vendorHandler = FindVendor(groupedSolutions.Key);
            
            var entered = await vendorHandler.EnterCaptchaSolutionsAsync(page, groupedSolutions.ToList());
            result.Add(entered);
        }

        return result;
    }

    public async Task OnPageCreatedAsync(IPage page)
    {
        foreach (var vendor in _captchaVendorHandlers)
        {
            await vendor.HandleOnPageCreatedAsync(page);
        }
    }

    private IEnumerable<ICaptchaVendor> FilterCaptchaVendors(HashSet<CaptchaVendor> enabledVendors)
    {
        return _captchaVendorHandlers.Where(e => enabledVendors.Contains(e.Vendor));
    }

    private ICaptchaVendor FindVendor(CaptchaVendor vendor)
    {
        var handler = _captchaVendorHandlers.SingleOrDefault(x => x.Vendor == vendor);

        if (handler is null)
        {
            throw new NotSupportedException($"Captcha vendor {vendor} is not supported.");
        }

        return handler;
    }

    private HashSet<ICaptchaVendor> GetCaptchaVendorHandlers(
        ICaptchaSolverProvider provider,
        CaptchaOptions options)
    {
        return
        [
            new CloudflareVendor(provider, options),
            new HCaptchaVendor(provider, options),
            new GoogleVendor(provider, options),
            new GeeTestVendor(provider, _optionsScope)
        ];
    }
}