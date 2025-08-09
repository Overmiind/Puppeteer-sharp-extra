using PuppeteerExtraSharpLite.Plugins.Recaptcha;
using PuppeteerExtraSharpLite.Plugins.Stealth;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Stealth_Plugin_PlugStandardEvasions_ShouldNot_BeDetected() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin()).Register(StealthPlugin.GetStandardEvasions());

        await using var browser = await pluginManager.LaunchAsync();
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var webdriver = await page.EvaluateExpressionAsync<bool>("navigator.webdriver");
        Assert.False(webdriver);

        var headlessUserAgent = await page.EvaluateExpressionAsync<string>("window.navigator.userAgent");
        Assert.DoesNotContain("Headless", headlessUserAgent);

        var webDriverOverridden =
            await page.EvaluateExpressionAsync<bool>(
                "Object.getOwnPropertyDescriptor(navigator.__proto__, 'webdriver') !== undefined");
        Assert.True(webDriverOverridden);

        var plugins = await page.EvaluateExpressionAsync<int>("navigator.plugins.length");
        Assert.NotEqual(0, plugins);
    }

    [Fact]
    public async Task Stealth_Plugin_LaunchTest() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new StealthPlugin());

        await using var browser = await pluginManager.LaunchAsync();
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://bot.sannysoft.com");
        await page.ScreenshotAsync("Stealth.png", new ScreenshotOptions() {
            FullPage = true,
            Type = ScreenshotType.Png,
        });
    }

    [Fact]
    public void Stealth_Plugin_StandardEvasionContracts_Names_Should_MatchTypes() {
        var evasions = StealthPlugin.StandardEvasionsContracts;

        foreach (var evasion in evasions) {
            var instance = evasion.Factory();
            Assert.Equal(evasion.PluginName, instance.Name);
        }
    }

    [Fact]
    public void Stealth_Plugin_GetStandardEvasions_Creates_AllStandardEvasions_NoOverrides() {
        var evasions = StealthPlugin.StandardEvasionsContracts;

        var instances = StealthPlugin.GetStandardEvasions();

        Assert.Equal(evasions.Length, instances.Length);
    }

    [Fact]
    public void Stealth_Plugin_GetStandardEvasions_Creates_AllStandardEvasions_AndOverrides() {
        var evasions = StealthPlugin.StandardEvasionsContracts;

        int defaultConcurrencyLevel = new HardwareConcurrencyPlugin().ConcurrencyLevel;
        const int customConcurrencyLevel = 16;

        Assert.NotEqual(customConcurrencyLevel, defaultConcurrencyLevel);

        var p = new HardwareConcurrencyPlugin(customConcurrencyLevel);

        var instances = StealthPlugin.GetStandardEvasions(p);

        Assert.Equal(evasions.Length, instances.Length); // override should not create extra

        var match = instances.SingleOrDefault(i => i is HardwareConcurrencyPlugin);

        Assert.NotNull(match); // If match is null, override didn't work correctly

        // Ensure found match has current parameters (override args)
        Assert.Equal(customConcurrencyLevel, (match as HardwareConcurrencyPlugin)!.ConcurrencyLevel);
    }

    [Fact]
    public void Stealth_Plugin_GetStandardEvasions_Creates_AllStandardEvasions_UnrelatedOverrides() {
        var evasions = StealthPlugin.StandardEvasionsContracts;

        var p = new RecaptchaPlugin(); // unrelated to stealth

        var instances = StealthPlugin.GetStandardEvasions(p);

        // unrelated should not override, so count should be bigger than 1
        Assert.Equal(evasions.Length + 1, instances.Length);
    }
}