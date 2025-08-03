using System.Linq;
using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public partial class UserAgent : PuppeteerExtraPlugin {
    public UserAgent() : base("stealth-userAgent") {
    }

    public override void BeforeLaunch(LaunchOptions options) { }

    public override async Task OnPageCreated(IPage page) {
        var ua = await page.Browser.GetUserAgentAsync();
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
        //
        // if (this._isHeadless)
        // {
        //     var dynamicObject = overrideObject as dynamic;
        //     dynamicObject.AcceptLanguage = "en-US, en";
        //     overrideObject = dynamicObject;
        // }

        await page.Client.SendAsync("Network.setUserAgentOverride", overrideObject);
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

    public static string GetPlatformVersion(string ua) {
        if (ua.Contains("Mac OS X ")) {
            return MacOSXVersionRegex().Match(ua).Groups[1].Value;
        }

        if (ua.Contains("Android ")) {
            return AndroidVersionRegex().Match(ua).Groups[1].Value;
        }

        if (ua.Contains("Windows ")) {
            return WindowsVersionRegex().Match(ua).Groups[1].Value;
        }

        return string.Empty;
    }

    public static string GetPlatformArch(bool isMobile) {
        return isMobile ? string.Empty : "x86";
    }

    public static string GetPlatformModel(bool isMobile, string ua) {
        return isMobile ? PlatformModelRegex().Match(ua).Groups[1].Value : string.Empty;
    }

    public static bool GetIsMobile(string ua) => ua.Contains("Android");

    private static readonly int[][] UserAgentOrders =
    [
        [0, 1, 2],
        [0, 2, 1],
        [1, 0, 2],
        [1, 2, 0],
        [2, 0, 1],
        [2, 1, 0]
    ];

    private static List<UserAgentBrand> GetBrands(string uaVersion) {
        var seed = int.Parse(uaVersion.Split('.')[0]);

        var order = UserAgentOrders[seed % 6];

        string[] escapedChars = [" ", " ", ";"];

        var greasyBrand = $"{escapedChars[order[0]]}Not{escapedChars[order[1]]}A{escapedChars[order[2]]}Brand";

        var greasedBrandVersionList = new Dictionary<int, UserAgentBrand>();

        greasedBrandVersionList.Add(order[0], new UserAgentBrand() {
            Brand = greasyBrand,
            Version = "99"
        });
        greasedBrandVersionList.Add(order[1], new UserAgentBrand() {
            Brand = "Chromium",
            Version = seed.ToString()
        });
        greasedBrandVersionList.Add(order[2], new UserAgentBrand() {
            Brand = "Google Chrome",
            Version = seed.ToString()
        });

        return greasedBrandVersionList.OrderBy(e => e.Key).Select(e => e.Value).ToList();
    }

    private class OverrideUserAgent {
        public string UserAgent { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string AcceptLanguage { get; set; } = string.Empty;
        public UserAgentMetadata UserAgentMetadata { get; set; } = new();
    }

    private class UserAgentMetadata {
        public List<UserAgentBrand> Brands { get; set; } = [];
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
    private static partial Regex MacOSXVersionRegex();

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
