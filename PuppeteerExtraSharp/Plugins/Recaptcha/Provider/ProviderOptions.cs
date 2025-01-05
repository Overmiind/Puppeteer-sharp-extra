namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider;

public class ProviderOptions
{
    public int PendingCount { get; set; }
    public int StartTimeoutSeconds { get; set; }

    public static ProviderOptions CreateDefaultOptions()
    {
        return new ProviderOptions
        {
            PendingCount = 30,
            StartTimeoutSeconds = 30
        };
    }
}
