using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerSharp;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;

public interface ICaptchaVendorHandler
{
    CaptchaVendor Vendor { get; }
    public Task<bool> WaitForCaptchasAsync(IPage page, TimeSpan timeout);
    public Task<CaptchaResponse> FindCaptchasAsync(IPage page);
    public Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas);
    public Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(IPage page, ICollection<CaptchaSolution> solutions);
}
