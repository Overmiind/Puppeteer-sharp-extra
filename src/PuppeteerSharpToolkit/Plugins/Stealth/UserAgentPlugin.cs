using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Plugins;

/// <summary>
/// Overrides user agent and UA-CH metadata to remove headless markers and match real devices.
/// Leaner, fewer allocations, and clearer control flow.
/// </summary>
public partial class UserAgentPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(UserAgentPlugin);

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type != TargetType.Page) {
            return;
        }

        var page = await target.PageAsync().ConfigureAwait(false);
        var browser = page.Browser;

        // Normalize UA (remove headless marker) with Ordinal comparison to avoid culture cost
        var ua = (await browser.GetUserAgentAsync().ConfigureAwait(false))
            .Replace("HeadlessChrome/", "Chrome/", StringComparison.Ordinal);

        // Try to get Chrome version from the UA itself; if absent, fall back to browser version
        var uaVersion = ExtractChromeVersion(ua);
        if (string.IsNullOrEmpty(uaVersion)) {
            // Example: "HeadlessChrome/124.0.6367.207" or "Chrome/124.0.6367.207"
            // Browser.GetVersionAsync() returns e.g. "Chrome/124.0.6367.207" – capture the numeric part
            uaVersion = BrowserVersionRegex().Match(await browser.GetVersionAsync().ConfigureAwait(false)).Groups[1].Value;
        }

        var (platform, platformVersion, isMobile, model, arch) = ParsePlatform(ua);
        var brands = BuildBrands(uaVersion);

        // Keep property names PascalCase to match the previous shape PuppeteerSharp expects.
        var payload = new {
            UserAgent = ua,
            Platform = platform,
            AcceptLanguage = "en-US, en",
            UserAgentMetadata = new {
                Brands = brands,
                FullVersion = uaVersion,
                Platform = platform,
                PlatformVersion = platformVersion,
                Architecture = arch,
                Model = model,
                Mobile = isMobile
            }
        };

        await page.Client.SendAsync("Network.setUserAgentOverride", payload).ConfigureAwait(false);
    }

    private static string ExtractChromeVersion(string ua) {
        var m = ChromeRegex().Match(ua);
        return m.Success ? m.Groups[1].Value : string.Empty;
    }

    private static (string Platform, string PlatformVersion, bool IsMobile, string Model, string Arch) ParsePlatform(string ua) {
        var isAndroid = ua.IndexOf("Android", StringComparison.Ordinal) >= 0;
        var isMac = ua.IndexOf("Mac OS X", StringComparison.Ordinal) >= 0;
        var isLinux = !isAndroid && ua.IndexOf("Linux", StringComparison.Ordinal) >= 0; // Android implies Linux in UA

        string platform = isAndroid ? "Android"
                         : isMac ? "Mac OS X"
                         : isLinux ? "Linux"
                         : "Windows";

        string platformVersion = string.Empty;
        if (isAndroid) {
            var m = AndroidVersionRegex().Match(ua);
            if (m.Success) {
                platformVersion = m.Groups[1].Value;
            }
        } else if (isMac) {
            var m = MacOsxVersionRegex().Match(ua);
            if (m.Success) {
                platformVersion = m.Groups[1].Value;
            }
        } else if (!isLinux) // treat anything not Linux/Mac/Android as Windows
          {
            var m = WindowsVersionRegex().Match(ua);
            if (m.Success) {
                platformVersion = m.Groups[1].Value;
            }
        }

        // Mobile-only fields
        var model = isAndroid ? PlatformModelRegex().Match(ua).Groups[1].Value : string.Empty;
        var arch = isAndroid ? string.Empty : "x86"; // preserve previous behavior

        return (platform, platformVersion, isAndroid, model, arch);
    }

    private static UserAgentBrand[] BuildBrands(string uaVersion) {
        // Be robust to malformed versions
        int major;

        if (!Version.TryParse(uaVersion, out var v)) {
            // fallback: take digits before first dot
            var dot = uaVersion.IndexOf('.');
            var head = dot >= 0 ? uaVersion.AsSpan(0, dot) : uaVersion.AsSpan();
            _ = int.TryParse(head, out major);
        } else {
            major = v.Major;
        }

        var order = UserAgentOrders[major % 6];
        Span<string> escaped = [" ", " ", ";"]; // intentionally odd spacing to mimic real-world grease
        var greasyBrand = $"{escaped[order[0]]}Not{escaped[order[1]]}A{escaped[order[2]]}Brand";

        var brands = new UserAgentBrand[3];
        brands[order[0]] = new UserAgentBrand { Brand = greasyBrand, Version = "99" };
        brands[order[1]] = new UserAgentBrand { Brand = "Chromium", Version = major.ToString() };
        brands[order[2]] = new UserAgentBrand { Brand = "Google Chrome", Version = major.ToString() };
        return brands;
    }

    private sealed class UserAgentBrand {
        public string Brand { get; init; } = string.Empty;
        public string Version { get; init; } = string.Empty;
    }

    private static readonly int[][] UserAgentOrders =
    [
        [0, 1, 2],
        [0, 2, 1],
        [1, 0, 2],
        [1, 2, 0],
        [2, 0, 1],
        [2, 1, 0]
    ];

    [GeneratedRegex("Mac OS X ([^)]+)")]
    private static partial Regex MacOsxVersionRegex();

    [GeneratedRegex("Android ([^;]+)")]
    private static partial Regex AndroidVersionRegex();

    [GeneratedRegex(@"Windows .*?([\d|.]+);")]
    private static partial Regex WindowsVersionRegex();

    [GeneratedRegex(@"Android.*?;\s([^)]+)")]
    private static partial Regex PlatformModelRegex();

    [GeneratedRegex(@"Chrome\/([\d|.]+)")]
    private static partial Regex ChromeRegex();

    [GeneratedRegex(@"\/([\d|.]+)")]
    private static partial Regex BrowserVersionRegex();
}
