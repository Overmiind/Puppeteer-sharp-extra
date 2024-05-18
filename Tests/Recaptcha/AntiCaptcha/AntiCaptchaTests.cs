using Extra.Tests.Properties;
using PuppeteerExtraSharp.Plugins.Recaptcha;
using PuppeteerSharp;
using Xunit;
using Xunit.Abstractions;
using Task = System.Threading.Tasks.Task;

namespace Extra.Tests.Recaptcha.AntiCaptcha
{
    [Collection("Captcha")]
    public class AntiCaptchaTests : BrowserDefault
    {
        private readonly ITestOutputHelper _logger;

        public AntiCaptchaTests(ITestOutputHelper _logger)
        {
            this._logger = _logger;
        }

        [Fact]
        public async void ShouldThrowCaptchaExceptionWhenCaptchaNotFound()
        {
            var plugin = new RecaptchaPlugin(new PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.AntiCaptcha(Resources.AntiCaptchaKey));

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
            var plugin = new RecaptchaPlugin(new PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.AntiCaptcha(Resources.AntiCaptchaKey));
            var browser = await LaunchWithPluginAsync(plugin);

            var page = await browser.NewPageAsync();
            await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_simple.php?level=low");
            var result = await plugin.SolveCaptchaAsync(page);
            
            Assert.Null(result.Exception);
            
            var button = await page.QuerySelectorAsync("input[type='submit']");
            await button.ClickAsync();

            await Task.Delay(1000);
            await CheckSuccessVerify(page);
        }

        [Fact]
        public async void ShouldSolveCaptchaWithCallback()
        {
            var plugin = new RecaptchaPlugin(new PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.AntiCaptcha(Resources.AntiCaptchaKey));
            var browser = await LaunchWithPluginAsync(plugin);
            var page = await browser.NewPageAsync();
            await page.GoToAsync("https://lessons.zennolab.com/captchas/recaptcha/v2_nosubmit.php?level=low");
            var result = await plugin.SolveCaptchaAsync(page);

            Assert.Null(result.Exception);

            await Task.Delay(1000);
            await CheckSuccessVerify(page);
        }

        private async Task CheckSuccessVerify(IPage page)
        {
            var successElement = await page.QuerySelectorAsync("div[id='main'] div[class='description'] h2");
            var elementValue = await (await successElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();
            Assert.NotNull(successElement);
            Assert.Equal("Успешная верификация!", elementValue);
        }
    }
}
