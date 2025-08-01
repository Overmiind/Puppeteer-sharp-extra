using PuppeteerExtraSharpLite.Plugins;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests;

//TODO: Revisit
public abstract class BrowserDefault : IDisposable {
    private readonly List<IBrowser> _launchedBrowsers = new List<IBrowser>();

    protected BrowserDefault() {
    }

    protected Task<IBrowser> LaunchAsync() => LaunchAsync(CreateDefaultOptions());

    protected async Task<IBrowser> LaunchAsync(LaunchOptions options) {
        var browser = await Puppeteer.LaunchAsync(options);
        _launchedBrowsers.Add(browser);
        return browser;
    }

    protected Task<IBrowser> LaunchWithPluginAsync(PuppeteerExtraPlugin plugin)
        => LaunchWithPluginAsync(plugin, CreateDefaultOptions());

    protected async Task<IBrowser> LaunchWithPluginAsync(PuppeteerExtraPlugin plugin, LaunchOptions options) {
        var extra = new PuppeteerExtra().Use(plugin);
        //DownloadChromeIfNotExists();

        var browser = await extra.LaunchAsync(options);
        _launchedBrowsers.Add(browser);
        return browser;
    }

    protected async Task<IPage> LaunchAndGetPage(PuppeteerExtraPlugin plugin) {
        IBrowser browser = plugin is null
            ? await LaunchAsync()
            : await LaunchWithPluginAsync(plugin);

        var page = (await browser.PagesAsync())[0];

        return page;
    }


    private async void DownloadChromeIfNotExists() {
        if (File.Exists(Constants.PathToChrome)) {
            return;
        }

        await new BrowserFetcher(new BrowserFetcherOptions() {
            Path = Constants.PathToChrome
        }).DownloadAsync(BrowserTag.Stable);
    }

    protected LaunchOptions CreateDefaultOptions() {
        return new LaunchOptions() {
            ExecutablePath = Constants.PathToChrome,
            Headless = Constants.Headless
        };
    }

    public void Dispose() {
        foreach (var launchedBrowser in _launchedBrowsers) {
            launchedBrowser.CloseAsync().Wait();
        }
    }
}