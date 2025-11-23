using System;
using System.Collections.Generic;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.Google;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;

public class CaptchaSolverOptions
{
    private static readonly TimeSpan DefaultWaitTimeout = TimeSpan.FromSeconds(10);
    private const double DefaultMinScore = 0.5;
    private double _minScore = DefaultMinScore;

    private static readonly Dictionary<CaptchaVendor, ICaptchaSolveOptions?> DefaultEnabledVendors = new Dictionary<CaptchaVendor, ICaptchaSolveOptions?>
    {
        {
            CaptchaVendor.Google, new GoogleOptions()
        },
        {
            CaptchaVendor.HCaptcha, null
        },
        {
            CaptchaVendor.DataDome, null
        },
        {
            CaptchaVendor.Cloudflare, null
        }
    };

    public Dictionary<CaptchaVendor, ICaptchaSolveOptions?> EnabledVendors { get; set; } = DefaultEnabledVendors;

    /// <summary>
    /// Maximum time to wait for captcha to appear/solve. Default: 10 seconds.
    /// </summary>
    public TimeSpan CaptchaWaitTimeout { get; set; } = DefaultWaitTimeout;

    /// <summary>
    /// Throw on errors instead of returning them in the error property.
    /// </summary>
    public bool ThrowOnError { get; set; } = false;

    /// <summary>
    /// Minimal acceptable score for captcha based on score (range 0..1). Default: 0.5.
    /// </summary>
    public double MinScore
    {
        get => _minScore;
        set
        {
            if (value < 0 || value > 1)
                throw new ArgumentOutOfRangeException(nameof(MinScore), "Value must be in range [0, 1].");
            _minScore = value;
        }
    }

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

    /// <summary>
    /// Enable verbose debug logging.
    /// </summary>
    public bool Debug { get; set; } = false;
}
