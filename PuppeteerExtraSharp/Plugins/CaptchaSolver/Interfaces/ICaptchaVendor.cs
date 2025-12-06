using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerSharp;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;

public interface ICaptchaVendor
{
    CaptchaVendor Vendor { get; }
    public Task<bool> WaitForCaptchasAsync(IPage page, TimeSpan timeout);
    public Task<CaptchaResponse> FindCaptchasAsync(IPage page);
    public Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas);
    public Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(IPage page, ICollection<CaptchaSolution> solutions);
    public Task HandleOnPageCreatedAsync(IPage page);
    void ProcessResponseAsync(IPage page, object? send, ResponseCreatedEventArgs e);
}
