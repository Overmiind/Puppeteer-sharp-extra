using PuppeteerExtraSharpLite.Tests.Recaptcha;

namespace PuppeteerExtraSharpLite.Tests.Recaptcha;

public class MockRecaptchaProviderTests {
    [Fact]
    public async Task GetSolution_ShouldReturnMockToken() {
        // Arrange
        var provider = new MockRecaptchaProvider();

        // Act
        var result = await provider.GetSolution("test-key", "https://example.com");

        // Assert
        Assert.Equal("MOCK_CAPTCHA_TOKEN", result);
    }

    [Fact]
    public async Task GetSolution_WithProxyStr_ShouldReturnMockToken() {
        // Arrange
        var provider = new MockRecaptchaProvider();

        // Act
        var result = await provider.GetSolution("test-key", "https://example.com", "proxy:8080");

        // Assert
        Assert.Equal("MOCK_CAPTCHA_TOKEN", result);
    }

    [Fact]
    public async Task GetSolution_WithNullProxyStr_ShouldReturnMockToken() {
        // Arrange
        var provider = new MockRecaptchaProvider();

        // Act
        var result = await provider.GetSolution("test-key", "https://example.com", null);

        // Assert
        Assert.Equal("MOCK_CAPTCHA_TOKEN", result);
    }
}
