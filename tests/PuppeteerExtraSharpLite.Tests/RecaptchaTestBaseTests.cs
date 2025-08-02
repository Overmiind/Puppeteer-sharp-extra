using Microsoft.Extensions.DependencyInjection;

using PuppeteerExtraSharpLite.Plugins.Recaptcha;
using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;
using PuppeteerExtraSharpLite.Tests.Recaptcha;

namespace PuppeteerExtraSharpLite.Tests;

public class RecaptchaTestBaseTests : RecaptchaTestBase {
    [Fact]
    public void ServiceProvider_ShouldBeConfigured() {
        // Act & Assert
        Assert.NotNull(ServiceProvider);
    }

    [Fact]
    public void ServiceProvider_ShouldResolveMockRecaptchaProvider() {
        // Act
        var provider = ServiceProvider.GetService<IRecaptchaProvider>();

        // Assert
        Assert.NotNull(provider);
        Assert.IsType<MockRecaptchaProvider>(provider);
    }

    [Fact]
    public void CreateRecaptchaPlugin_ShouldReturnValidPlugin() {
        // Act
        var plugin = CreateRecaptchaPlugin();

        // Assert
        Assert.NotNull(plugin);
        Assert.IsType<RecaptchaPlugin>(plugin);
    }

    [Fact]
    public async Task CreateRecaptchaPlugin_ShouldUseMockProvider() {
        // Arrange
        var plugin = CreateRecaptchaPlugin();
        var provider = ServiceProvider.GetRequiredService<IRecaptchaProvider>();

        // Act
        var result = await provider.GetSolution("test", "test");

        // Assert
        Assert.Equal("MOCK_CAPTCHA_TOKEN", result);
    }
}
