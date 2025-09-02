using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Models;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha;

public interface ICaptchaHandler
{
    public Task<bool> WaitForCaptchasAsync(IPage page, TimeSpan timeout);
    public Task<CaptchaResponse> FindCaptchasAsync(IPage page);
    public Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas);
    public Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(IPage page, ICollection<CaptchaSolution> solutions);
}