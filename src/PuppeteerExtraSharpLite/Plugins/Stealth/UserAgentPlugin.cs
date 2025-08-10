using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Overrides user agent and related UA-CH metadata to remove headless markers and better match real devices.
/// </summary>
public partial class UserAgentPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(UserAgentPlugin);

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            var ua = await page.Browser.GetUserAgentAsync().ConfigureAwait(false);
            ua = ua.Replace("HeadlessChrome/", "Chrome/");
            var uaVersion = ua.Contains("Chrome/")
                ? ChromeRegex().Match(ua).Groups[1].Value
                : BrowserVersionRegex().Match(await page.Browser.GetVersionAsync()).Groups[1].Value;

            var platform = GetPlatform(ua);
            var brand = GetBrands(uaVersion);

            var isMobile = GetIsMobile(ua);
            var platformVersion = GetPlatformVersion(ua);
            var platformArch = GetPlatformArch(isMobile);
            var platformModel = GetPlatformModel(isMobile, ua);

            var overrideObject = new OverrideUserAgent() {
                UserAgent = ua,
                Platform = platform,
                AcceptLanguage = "en-US, en",
                UserAgentMetadata = new UserAgentMetadata() {
                    Brands = brand,
                    FullVersion = uaVersion,
                    Platform = platform,
                    PlatformVersion = platformVersion,
                    Architecture = platformArch,
                    Model = platformModel,
                    Mobile = isMobile
                }
            };

            await page.Client.SendAsync("Network.setUserAgentOverride", overrideObject).ConfigureAwait(false);
        }
    }

    private static string GetPlatform(string ua) {
        if (ua.Contains("Mac OS X")) {
            return "Mac OS X";
        }

        if (ua.Contains("Android")) {
            return "Android";
        }

        if (ua.Contains("Linux")) {
            return "Linux";
        }

        return "Windows";
    }

    private static string GetPlatformVersion(string ua) {
        if (ua.Contains("Mac OS X ")) {
            return MacOsxVersionRegex().Match(ua).Groups[1].Value;
        }

        if (ua.Contains("Android ")) {
            return AndroidVersionRegex().Match(ua).Groups[1].Value;
        }

        if (ua.Contains("Windows ")) {
            return WindowsVersionRegex().Match(ua).Groups[1].Value;
        }

        return string.Empty;
    }

    private static string GetPlatformArch(bool isMobile) {
        return isMobile ? string.Empty : "x86";
    }

    private static string GetPlatformModel(bool isMobile, string ua) {
        return isMobile ? PlatformModelRegex().Match(ua).Groups[1].Value : string.Empty;
    }

    private static bool GetIsMobile(string ua) => ua.Contains("Android");

    private static readonly int[][] UserAgentOrders =
    [
        [0, 1, 2],
        [0, 2, 1],
        [1, 0, 2],
        [1, 2, 0],
        [2, 0, 1],
        [2, 1, 0]
    ];

    private static UserAgentBrand[] GetBrands(string uaVersion) {
        var seed = int.Parse(uaVersion.Split('.')[0]);

        var order = UserAgentOrders[seed % 6];

        string[] escapedChars = [" ", " ", ";"];

        var greasyBrand = $"{escapedChars[order[0]]}Not{escapedChars[order[1]]}A{escapedChars[order[2]]}Brand";

        var brands = new UserAgentBrand[3];
        brands[order[0]] = new UserAgentBrand {
            Brand = greasyBrand,
            Version = "99"
        };
        brands[order[1]] = new UserAgentBrand {
            Brand = "Chromium",
            Version = seed.ToString()
        };
        brands[order[2]] = new UserAgentBrand {
            Brand = "Google Chrome",
            Version = seed.ToString()
        };
        return brands;
    }

    private class OverrideUserAgent {
        public string UserAgent { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string AcceptLanguage { get; set; } = string.Empty;
        public UserAgentMetadata UserAgentMetadata { get; set; } = new();
    }

    private class UserAgentMetadata {
        public UserAgentBrand[] Brands { get; set; } = [];
        public string FullVersion { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string PlatformVersion { get; set; } = string.Empty;
        public string Architecture { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public bool Mobile { get; set; }
    }

    private class UserAgentBrand {
        public string Brand { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }

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