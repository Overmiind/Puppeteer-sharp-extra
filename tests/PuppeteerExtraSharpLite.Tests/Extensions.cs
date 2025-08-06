using System.Text.Json;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests;

public static class Extensions {
    public static async Task<IBrowser> LaunchAsync(this PluginManager manager) {
        await EnsureBrowserDownloadedAsync();

        var options = new LaunchOptions() {
            Headless = true
        };

        // Check if we should use a system Chrome installation as fallback
        if (!s_browserDownloaded) {
            var systemChromePath = GetSystemChromePath();
            if (!string.IsNullOrEmpty(systemChromePath) && File.Exists(systemChromePath)) {
                options.ExecutablePath = systemChromePath;
            }
        }

        return await manager.LaunchAsync(options);
    }

    private static readonly SemaphoreSlim BrowserFetchSemaphore = new(1, 1);
    private static bool s_browserDownloaded = false;

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

    private static async Task EnsureBrowserDownloadedAsync() {
        if (s_browserDownloaded) return;

        await BrowserFetchSemaphore.WaitAsync();
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
            BrowserFetchSemaphore.Release();
        }
    }

    /// <summary>
    /// https://antoinevastel.com/bots/
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static async Task<JsonElement> GetFingerPrint(this IPage page) {
        await page.EvaluateExpressionAsync(Constants.FpCollect);

        var fingerPrint =
            await page.EvaluateFunctionAsync<JsonElement>("async () => await fpCollect().generateFingerprint()");

        return fingerPrint;
    }
}