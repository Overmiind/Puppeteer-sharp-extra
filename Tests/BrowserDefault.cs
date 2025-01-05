using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins;
using PuppeteerSharp;

namespace Extra.Tests;

public abstract class BrowserDefault : IDisposable
{
    private readonly List<IBrowser> _launchedBrowsers = [];

    protected async Task<IBrowser> LaunchAsync(LaunchOptions options = null)
    {
        options ??= CreateDefaultOptions();

        var browser = await Puppeteer.LaunchAsync(options);
        _launchedBrowsers.Add(browser);
        return browser;
    }

    protected async Task<IBrowser> LaunchWithPluginAsync(
        PuppeteerExtraPlugin plugin,
        LaunchOptions options = null)
    {
        var extra = new PuppeteerExtra().Use(plugin);
        options ??= CreateDefaultOptions();

        var browser = await extra.LaunchAsync(options);
        _launchedBrowsers.Add(browser);
        return browser;
    }

    protected async Task<IPage> LaunchAndGetPage(
        PuppeteerExtraPlugin plugin = null,
        LaunchOptions options = null)
    {
        IBrowser browser = null;
        if (plugin != null)
            browser = await LaunchWithPluginAsync(plugin, options);
        else
            browser = await LaunchAsync(options);

        var page = (await browser.PagesAsync())[0];

        return page;
    }

    protected LaunchOptions CreateDefaultOptions()
    {
        return new LaunchOptions()
        {
            ExecutablePath = Constants.PathToChrome,
            Headless = Constants.Headless
        };
    }

    public void Dispose()
    {
        foreach (var launchedBrowser in _launchedBrowsers)
        {
            launchedBrowser.CloseAsync().Wait();
        }
    }
}
