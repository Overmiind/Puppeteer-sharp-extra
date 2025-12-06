using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerSharp;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;

public interface ICaptchaSolver
{
    public Task<ICollection<CaptchaVendor>> WaitForCaptchasAsync(IPage page, TimeSpan timeout);
    public Task<ICollection<CaptchaResponse>> FindCaptchasAsync(HashSet<CaptchaVendor> vendors, IPage page);
    public Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas);
    public Task<ICollection<EnterCaptchaSolutionsResult>> EnterCaptchaSolutionsAsync(IPage page, ICollection<CaptchaSolution> solutions);
    public Task OnPageCreatedAsync(IPage page);
}
