using System;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider;

public class CaptchaProviderOptions
{
    public static readonly int DefaultMaxPollingAttempts = 30;
    public static readonly TimeSpan DefaultStartTimeout = TimeSpan.FromSeconds(30);
    public static readonly TimeSpan DefaultApiTimeout = TimeSpan.FromSeconds(120);

    private int _maxPollingAttempts = DefaultMaxPollingAttempts;
    private TimeSpan _startTimeout = DefaultStartTimeout;
    private TimeSpan _apiTimeout = DefaultApiTimeout;

    /// <summary>
    /// Maximum number of polling attempts to remote service before stopping.
    /// </summary>
    public int MaxPollingAttempts
    {
        get => _maxPollingAttempts;
        set => _maxPollingAttempts = value > 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be greater than 0.");
    }

    /// <summary>
    /// Timeout before requesting a captcha solution.
    /// </summary>
    public TimeSpan StartTimeout
    {
        get => _startTimeout;
        set => _startTimeout = value > TimeSpan.Zero
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be greater than 0.");
    }

    /// <summary>
    /// General timeout for provider API operations.
    /// </summary>
    public TimeSpan ApiTimeout
    {
        get => _apiTimeout;
        set => _apiTimeout = value > TimeSpan.Zero
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be greater than 0.");
    }
}