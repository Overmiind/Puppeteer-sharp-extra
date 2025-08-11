namespace PuppeteerSharpToolkit.Plugins.Recaptcha;

/// <summary>
/// Abstraction for services that solve Google reCAPTCHA and return the solution token.
/// </summary>
public interface IRecaptchaProvider {
    /// <summary>
    /// Requests a solution token for the specified site key and page URL.
    /// </summary>
    /// <param name="pageUrl">URL of the page hosting the challenge.</param>
    /// <param name="proxyStr">Optional proxy description string required by some providers.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Solution token as a string.</returns>
    Task<string> GetSolutionAsync(string pageUrl, string proxyStr = "", CancellationToken token = default);
}

/// <summary>
/// Fallback provider used only for internal tests; always throws if invoked.
/// </summary>
internal sealed class InvalidRecaptchaProvider : IRecaptchaProvider {
    /// <inheritdoc />
    public Task<string> GetSolutionAsync(string pageUrl, string proxyStr = "", CancellationToken token = default) {
        throw new NotImplementedException();
    }
}
