namespace PuppeteerExtraSharpLite.Tests.Recaptcha;

public class MockRecaptchaProviderTests {
    [Fact]
    public async Task GetSolution_ShouldReturnMockToken() {
        // Arrange
        var provider = new MockRecaptchaProvider();

        // Act
        var result = await provider.GetSolutionAsync("test-key", "https://example.com", token: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("MOCK_CAPTCHA_TOKEN", result);
    }

    [Fact]
    public async Task GetSolution_WithProxyStr_ShouldReturnMockToken() {
        // Arrange
        var provider = new MockRecaptchaProvider();

        // Act
        var result = await provider.GetSolutionAsync("test-key", "https://example.com", "proxy:8080", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("MOCK_CAPTCHA_TOKEN", result);
    }

    [Fact]
    public async Task GetSolution_WithNullProxyStr_ShouldReturnMockToken() {
        // Arrange
        var provider = new MockRecaptchaProvider();

        // Act
        var result = await provider.GetSolutionAsync("test-key", "https://example.com", null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("MOCK_CAPTCHA_TOKEN", result);
    }
}
