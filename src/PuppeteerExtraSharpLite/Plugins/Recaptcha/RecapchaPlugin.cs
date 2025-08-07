using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

public class RecaptchaPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(RecaptchaPlugin);
    private readonly Recaptcha _recaptcha;

    public RecaptchaPlugin(IRecaptchaProvider provider) : this(provider, CaptchaOptions.Default) { }

    public RecaptchaPlugin(IRecaptchaProvider provider, CaptchaOptions opt) {
        _recaptcha = new Recaptcha(provider, opt);
    }

    public async Task<RecaptchaResult> SolveCaptchaAsync(IPage page) {
        return await _recaptcha.Solve(page);
    }

    public async Task OnPageCreated(IPage page) {
        await page.SetBypassCSPAsync(true);
    }
}