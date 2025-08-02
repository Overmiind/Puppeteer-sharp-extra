using PuppeteerSharp;

using Task = System.Threading.Tasks.Task;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha.AntiCaptcha;

[Collection("Captcha")]
public class AntiCaptchaTests : RecaptchaTestBase {
    [Fact]
    public async Task ShouldThrowCaptchaExceptionWhenCaptchaNotFound() {
        var plugin = CreateRecaptchaPlugin();

        await using var browser = await LaunchWithPluginAsync(plugin);

        var page = await browser.NewPageAsync();
        await page.GoToAsync("https://lessons.zennolab.com/ru/index");
        var result = await plugin.SolveCaptchaAsync(page);
        Assert.NotNull(result.Exception);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ShouldSolveCaptchaWithSubmitButton() {
        var plugin = CreateRecaptchaPlugin();
        var browser = await LaunchWithPluginAsync(plugin);

        var page = await browser.NewPageAsync();
        await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_simple.php?level=low");
        var result = await plugin.SolveCaptchaAsync(page);

        Assert.Null(result.Exception);

        var button = await page.QuerySelectorAsync("input[type='submit']");
        await button.ClickAsync();

        // Wait for the success verification element to appear
        await page.WaitForSelectorAsync("div[id='main'] div[class='description'] h2", new WaitForSelectorOptions { Timeout = 10000 });
        await CheckSuccessVerify(page);
    }

    [Fact]
    public async Task ShouldSolveCaptchaWithCallback() {
        var plugin = CreateRecaptchaPlugin();
        var browser = await LaunchWithPluginAsync(plugin);
        var page = await browser.NewPageAsync();
        await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_nosubmit.php?level=low");
        var result = await plugin.SolveCaptchaAsync(page);

        Assert.Null(result.Exception);

        // Wait for the success verification element to appear
        await page.WaitForSelectorAsync("div[id='main'] div[class='description'] h2", new WaitForSelectorOptions { Timeout = 10000 });
        await CheckSuccessVerify(page);
    }

    private async Task CheckSuccessVerify(IPage page) {
        var successElement = await page.QuerySelectorAsync("div[id='main'] div[class='description'] h2");
        var elementValue = await (await successElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();
        Assert.NotNull(successElement);
        Assert.Equal("Успешная верификация!", elementValue);
    }
}