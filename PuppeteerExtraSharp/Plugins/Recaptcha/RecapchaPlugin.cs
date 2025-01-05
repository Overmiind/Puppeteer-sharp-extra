using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha;

public class RecaptchaPlugin : PuppeteerExtraPlugin
{
    private readonly Recaptcha _recaptcha;

    public RecaptchaPlugin(IRecaptchaProvider provider, CaptchaOptions opt = null) 
        : base("recaptcha")
    {
        _recaptcha = new Recaptcha(provider, opt ?? new CaptchaOptions());
    }

    public async Task<RecaptchaResult> SolveCaptchaAsync(IPage page)
    {
        return await _recaptcha.Solve(page);
    }

    public override async Task OnPageCreated(IPage page)
    {
        await page.SetBypassCSPAsync(true);
    }
}
