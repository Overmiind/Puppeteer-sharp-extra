namespace PuppeteerSharpToolkit.Plugins.Recaptcha.Provider;

/// <summary>
/// Common options for captcha providers controlling polling attempts and initial wait.
/// </summary>
/// <param name="PendingCount">Maximum number of polling attempts for a result.</param>
/// <param name="StartTimeoutSeconds">Initial delay before starting to poll, in seconds.</param>
public record ProviderOptions(int PendingCount, int StartTimeoutSeconds) {
    /// <summary>
    /// Default provider options: 30 attempts and 30 seconds initial delay.
    /// </summary>
    public static readonly ProviderOptions Default = new(30, 30);
}
