namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;

public record ProviderOptions(int PendingCount, int StartTimeoutSeconds) {
    public static readonly ProviderOptions Default = new(30, 30);
}
