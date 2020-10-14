using PuppeteerExtraSharp.Plugins.Recaptcha;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha;
using PuppeteerSharp;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Extra.Tests
{
    public class RecaptchaPluginTest : BrowserDefault
    {
        [Fact]
        public async void ShouldThrowCaptchaExceptionWhenCaptchaNotFound()
        {
            var plugin = new RecaptchaPlugin(new AntiCaptcha(Resources.AntiCaptchaKey));

            var browser = await LaunchWithPluginAsync(plugin);

            var page = await browser.NewPageAsync();
            await page.GoToAsync("https://lessons.zennolab.com/ru/index");
            var result = await plugin.SolveCaptchaAsync(page);

            Assert.NotNull(result.Exception);
            Assert.False(result.IsSuccess);
            //await browser.CloseAsync();
        }

        [Fact]
        public async Task ShouldSolveCaptchaWithSubmitButton()
        {
            var plugin = new RecaptchaPlugin(new AntiCaptcha(Resources.AntiCaptchaKey));
            var browser = await LaunchWithPluginAsync(plugin);

            var page = await browser.NewPageAsync();
            var test = page.GoToAsync("https://google.com").Result;
            await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_simple.php?level=low");
            var result = await plugin.SolveCaptchaAsync(page);
            Assert.Null(result.Exception);
            var button = await page.QuerySelectorAsync("input[type='submit']");

            await button.ClickAsync();
            await page.WaitForTimeoutAsync(1000);
            await CheckSuccessVerify(page);
        }

        [Fact]
        public async void ShouldSolveCaptchaWithCallback()
        {
            var plugin = new RecaptchaPlugin(new AntiCaptcha(Resources.AntiCaptchaKey));
            var browser = await LaunchWithPluginAsync(plugin);
            var page = await browser.NewPageAsync();
            await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_nosubmit.php?level=low");
            var result = await plugin.SolveCaptchaAsync(page);

            Assert.Null(result.Exception);

            await page.WaitForTimeoutAsync(1000);
            await CheckSuccessVerify(page);
        }

        private async Task CheckSuccessVerify(Page page)
        {
            var successElement = await page.QuerySelectorAsync("div[id='main'] div[class='description'] h2");
            var elementValue = await (await successElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();
            Assert.NotNull(successElement);
            Assert.Equal("Успешная верификация!", elementValue);
        }
    }
}
