using PuppeteerExtraSharpLite.Plugins.Recaptcha;
using PuppeteerExtraSharpLite.Tests.Utils;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha.TwoCaptcha;

[Collection("Captcha")]
public class TwoCaptchaProviderTest : RecaptchaTestBase {
    [Fact]
    public async Task ShouldResolveCaptchaInGooglePage() {
        Assert.SkipUnless(Helper.TryGetEnvironmentVariable("TwoCaptchaProvider", out var antiCaptchaKey),
            "TwoCaptchaProvider environment variable is not set. Skipping test.");

        using var client = new HttpClient();
        var provider = new Plugins.Recaptcha.Provider._2Captcha.TwoCaptcha(client, antiCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        await using var browser = await LaunchWithPluginAsync(plugin);

        var page = (await browser.PagesAsync())[0];

        await page.GoToAsync("https://www.google.com/recaptcha/api2/demo");

        await plugin.SolveCaptchaAsync(page);
        var button = await page.QuerySelectorAsync("input[id='recaptcha-demo-submit']");
        await button.ClickAsync();
        await page.WaitForNavigationAsync();
        var successElement = await page.QuerySelectorAsync("div[class='recaptcha-success']");

        Assert.NotNull(successElement);
    }

    [Fact]
    public async Task ShouldSolveInvisibleCaptcha() {
        Assert.SkipUnless(Helper.TryGetEnvironmentVariable("TwoCaptchaProvider", out var antiCaptchaKey),
            "TwoCaptchaProvider environment variable is not set. Skipping test.");

        using var client = new HttpClient();
        var provider = new Plugins.Recaptcha.Provider._2Captcha.TwoCaptcha(client, antiCaptchaKey);
        var plugin = new RecaptchaPlugin(provider);

        await using var browser = await LaunchWithPluginAsync(plugin);

        var page = (await browser.PagesAsync())[0];

        await page.GoToAsync("https://recaptcha-demo.appspot.com/recaptcha-v2-invisible.php");

        var result = await plugin.SolveCaptchaAsync(page);

        Assert.Null(result.Exception);
        await page.WaitForNavigationAsync();
        var elements = await page.QuerySelectorAllAsync("main h2");

        Assert.Equal(2, elements.Length);

        var elementPropery = await (await elements[1].GetPropertyAsync("textContent")).JsonValueAsync<string>();
        Assert.Equal("Success!", elementPropery);

    }
}