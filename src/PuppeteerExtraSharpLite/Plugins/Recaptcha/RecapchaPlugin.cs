using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

public class RecaptchaPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(RecaptchaPlugin);
    private readonly Recaptcha _recaptcha;

    public RecaptchaPlugin(IRecaptchaProvider provider, CaptchaOptions? opt = null) : base() {
        _recaptcha = new Recaptcha(provider, opt ?? new CaptchaOptions());
    }

    public async Task<RecaptchaResult> SolveCaptchaAsync(IPage page) {
        return await _recaptcha.Solve(page);
    }

    public async Task OnPageCreated(IPage page) {
        await page.SetBypassCSPAsync(true);
    }
}