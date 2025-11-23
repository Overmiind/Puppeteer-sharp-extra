using System;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.Google;

public class GoogleOptions : ICaptchaSolveOptions
{
    // Defaults
    private const double DefaultMinV3Score = 0.5;
    private static readonly TimeSpan DefaultWaitTimeout = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Visualize reCAPTCHAs based on their state.
    /// TODO: NOT IMPLEMENTED
    /// </summary>
    public bool VisualFeedback { get; set; } = false;

    /// <summary>
    /// Throw on errors instead of returning them in the error property.
    /// </summary>
    public bool ThrowOnError { get; set; } = false;

    /// <summary>
    /// Only solve captchas and challenges visible in the viewport.
    /// </summary>
    public bool SolveInViewportOnly { get; set; } = false;

    /// <summary>
    /// Solve invisible captchas used to acquire a score and not present a challenge (e.g. reCAPTCHA v3).
    /// </summary>
    public bool SolveScoreBased { get; set; } = true;

    /// <summary>
    /// Solve invisible captchas that have no active challenge.
    /// </summary>
    public bool SolveInactiveChallenges { get; set; } = true;

    /// <summary>
    /// Solve invisible challenges (checkbox not shown) when present.
    /// </summary>
    public bool SolveInvisibleChallenges { get; set; } = true;

    private double _minV3RecaptchaScore = DefaultMinV3Score;

    /// <summary>
    /// Minimal acceptable score for reCAPTCHA v3 (range 0..1). Default: 0.5.
    /// </summary>
    public double MinV3RecaptchaScore
    {
        get => _minV3RecaptchaScore;
        set
        {
            if (value < 0 || value > 1)
                throw new ArgumentOutOfRangeException(nameof(MinV3RecaptchaScore), "Value must be in range [0, 1].");
            _minV3RecaptchaScore = value;
        }
    }

    /// <summary>
    /// Enable verbose debug logging.
    /// </summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Maximum time to wait for captcha to appear/solve. Default: 10 seconds.
    /// </summary>
    public TimeSpan CaptchaWaitTimeout { get; set; } = DefaultWaitTimeout;
}
