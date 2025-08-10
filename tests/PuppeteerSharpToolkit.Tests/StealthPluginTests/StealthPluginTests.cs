using PuppeteerSharpToolkit.Plugins;
using PuppeteerSharpToolkit.Plugins.Recaptcha;

using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Tests.StealthPluginTests;

public partial class StealthPluginTests {
    [Fact]
    public async Task Stealth_Plugin_CannotInject_Utils_MoreThanOnce() {
        await using var browser = await Puppeteer.LaunchAsync(new() {
            Headless = true
        });
        var context = await browser.CreateBrowserContextAsync();

        // Tests that all events were executed
        bool eventExecuted = false;

        browser.TargetCreated += async (sender, args) => {
            // Inject utils x3 (utils should check if it exists before continuing)
            if (args.Target.Type is TargetType.Page) {
                var page = await args.Target.PageAsync();
                await Stealth.RegisterUtilsAsync(page);
                await Stealth.RegisterUtilsAsync(page);
                await Stealth.RegisterUtilsAsync(page);
                eventExecuted = true;
            }
        };

        await using var page = await context.NewPageAsync();

        // Wait for events to execute
        await Task.Delay(500, TestContext.Current.CancellationToken);

        var utilsIsUndefined = await page.EvaluateFunctionAsync<bool>("() => typeof globalThis.utils === 'undefined'");
        Assert.False(utilsIsUndefined);

        Assert.True(eventExecuted);
    }

    [Fact]
    public async Task Stealth_Plugin_PlusStandardEvasions_ShouldNot_BeDetected() {
        var pluginManager = new PluginManager();
        pluginManager.Register(Stealth.GetStandardEvasions());

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await Task.Delay(500, TestContext.Current.CancellationToken);

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

        await using var browser = await pluginManager.LaunchAsync();
        var context = await browser.CreateBrowserContextAsync();
        await using var page = await context.NewPageAsync();

        await page.GoToAsync("https://bot.sannysoft.com");
        await page.ScreenshotAsync("Stealth.png", new ScreenshotOptions() {
            FullPage = true,
            Type = ScreenshotType.Png,
        });
    }

    [Fact]
    public void Stealth_Plugin_StandardEvasionContracts_Names_Should_MatchTypes() {
        var evasions = Stealth.StandardEvasionsContracts;

        foreach (var evasion in evasions) {
            var instance = evasion.Factory();
            Assert.Equal(evasion.PluginName, instance.Name);
        }
    }

    [Fact]
    public void Stealth_Plugin_GetStandardEvasions_Creates_AllStandardEvasions_NoOverrides() {
        var evasions = Stealth.StandardEvasionsContracts;

        var instances = Stealth.GetStandardEvasions();

        Assert.Equal(evasions.Length, instances.Length);
    }

    [Fact]
    public void Stealth_Plugin_GetStandardEvasions_Creates_AllStandardEvasions_AndOverrides() {
        var evasions = Stealth.StandardEvasionsContracts;

        int defaultConcurrencyLevel = new HardwareConcurrencyPlugin().ConcurrencyLevel;
        const int customConcurrencyLevel = 16;

        Assert.NotEqual(customConcurrencyLevel, defaultConcurrencyLevel);

        var p = new HardwareConcurrencyPlugin(customConcurrencyLevel);

        var instances = Stealth.GetStandardEvasions(p);

        Assert.Equal(evasions.Length, instances.Length); // override should not create extra

        var match = instances.SingleOrDefault(i => i is HardwareConcurrencyPlugin);

        Assert.NotNull(match); // If match is null, override didn't work correctly

        // Ensure found match has current parameters (override args)
        Assert.Equal(customConcurrencyLevel, (match as HardwareConcurrencyPlugin)!.ConcurrencyLevel);
    }

    [Fact]
    public void Stealth_Plugin_GetStandardEvasions_Creates_AllStandardEvasions_UnrelatedOverrides() {
        var evasions = Stealth.StandardEvasionsContracts;

        var p = new RecaptchaPlugin(); // unrelated to stealth

        var instances = Stealth.GetStandardEvasions(p);

        // unrelated should not override, so count should be bigger than 1
        Assert.Equal(evasions.Length + 1, instances.Length);
    }
}
