using System;
using System.Collections.Generic;
using Extra.Tests.Properties;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.CapSolver;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.TwoCaptcha;

namespace Extra.Tests.CaptchaSolverTests;

public abstract class CaptchaSolverTestsBase : BrowserDefault
{
    protected static CaptchaProviderOptions Options = new()
    {
        StartTimeout = TimeSpan.FromSeconds(10),
        MaxPollingAttempts = 30,
        ApiTimeout = TimeSpan.FromMinutes(3),
    };

    public static IEnumerable<object[]> Providers => GetDefaultProviders();

    private static IEnumerable<object[]> GetDefaultProviders()
    {
        return new List<ICaptchaSolverProvider[]>()
        {
            new ICaptchaSolverProvider[]
            {
                new TwoCaptcha(Resources.TwoCaptchaKey, Options),
            },
            new ICaptchaSolverProvider[]
            {
                new CapSolver(Resources.CapSolverKey, Options),
            }
        };
    }
}