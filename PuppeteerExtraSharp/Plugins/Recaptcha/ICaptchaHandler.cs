using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver;
using PuppeteerExtraSharp.Plugins.Recaptcha.Models;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha;

[Obsolete($"Use {nameof(CaptchaSolverPlugin)} instead. This plugin will be removed in a future version.", UrlFormat = "https://github.com/Overmiind/Puppeteer-sharp-extra/tree/master/PuppeteerExtraSharp/Plugins/CaptchaSolver")]
public interface ICaptchaHandler
{
    public Task<bool> WaitForCaptchasAsync(IPage page, TimeSpan timeout);
    public Task<CaptchaResponse> FindCaptchasAsync(IPage page);
    public Task<ICollection<CaptchaSolution>> SolveCaptchasAsync(IPage page, ICollection<Captcha> captchas);
    public Task<EnterCaptchaSolutionsResult> EnterCaptchaSolutionsAsync(IPage page, ICollection<CaptchaSolution> solutions);
}