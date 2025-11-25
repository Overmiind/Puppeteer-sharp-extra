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
    public Task<bool> WaitForCaptchasAsync(TimeSpan timeout);
    public Task<CaptchaResponse> FindCaptchasAsync();
    public Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(ICollection<Captcha> captchas);
    public Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(ICollection<CaptchaSolution> solutions);
    public Task HandleOnPageCreatedAsync();
    void ProcessResponseAsync(object? send, ResponseCreatedEventArgs e);
}
