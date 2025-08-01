using PuppeteerExtraSharpLite.Plugins;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests;

public abstract class BrowserDefault : IDisposable {
    private readonly List<IBrowser> _launchedBrowsers = new List<IBrowser>();
    private static readonly SemaphoreSlim _browserFetchSemaphore = new(1, 1);
    private static bool s_browserDownloaded = false;

    protected BrowserDefault() {
    }

    protected Task<IBrowser> LaunchAsync() => LaunchAsync(CreateDefaultOptions());

    protected async Task<IBrowser> LaunchAsync(LaunchOptions options) {
        await EnsureBrowserDownloadedAsync();
        var browser = await Puppeteer.LaunchAsync(options);
        _launchedBrowsers.Add(browser);
        return browser;
    }

    protected Task<IBrowser> LaunchWithPluginAsync(PuppeteerExtraPlugin plugin)
        => LaunchWithPluginAsync(plugin, CreateDefaultOptions());

    protected async Task<IBrowser> LaunchWithPluginAsync(PuppeteerExtraPlugin plugin, LaunchOptions options) {
        await EnsureBrowserDownloadedAsync();
        var extra = new PuppeteerExtra().Use(plugin);

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


    private static async Task EnsureBrowserDownloadedAsync() {
        if (s_browserDownloaded) return;

        await _browserFetchSemaphore.WaitAsync();
        try {
            if (s_browserDownloaded) return;

            var browserFetcher = new BrowserFetcher();

            // Try downloading with retries for network issues
            const int maxRetries = 3;
            Exception? lastException = null;

            for (int attempt = 1; attempt <= maxRetries; attempt++) {
                try {
                    await browserFetcher.DownloadAsync();
                    s_browserDownloaded = true;
                    return;
                } catch (Exception ex) when (attempt < maxRetries) {
                    lastException = ex;
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // Exponential backoff
                    Console.WriteLine($"Browser download attempt {attempt} failed: {ex.Message}. Retrying in {delay.TotalSeconds} seconds...");
                    await Task.Delay(delay);
                }
            }

            throw new InvalidOperationException($"Failed to download browser after {maxRetries} attempts. This appears to be a network connectivity issue. You may need to manually install Chrome or run the tests when network conditions are better.", lastException);
        } finally {
            _browserFetchSemaphore.Release();
        }
    }

    protected static LaunchOptions CreateDefaultOptions() {
        var options = new LaunchOptions() {
            Headless = Constants.Headless
        };

        // Check if we should use a system Chrome installation as fallback
        if (!s_browserDownloaded) {
            var systemChromePath = GetSystemChromePath();
            if (!string.IsNullOrEmpty(systemChromePath) && File.Exists(systemChromePath)) {
                options.ExecutablePath = systemChromePath;
            }
        }

        return options;
    }

    private static string? GetSystemChromePath() {
        // macOS Chrome paths
        string[] macChromePaths =
        [
            "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
            "/Applications/Chromium.app/Contents/MacOS/Chromium"
        ];

        foreach (var path in macChromePaths) {
            if (File.Exists(path)) {
                return path;
            }
        }

        return null;
    }

    public void Dispose() {
        foreach (var launchedBrowser in _launchedBrowsers) {
            launchedBrowser.CloseAsync().Wait();
        }
    }
}