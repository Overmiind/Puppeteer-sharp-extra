using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class UserAgent(StealthUserAgentOptions options = null) : PuppeteerExtraPlugin("stealth-userAgent")
{
    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        var ua = options?.UserAgent ?? (await page.Browser.GetUserAgentAsync()).Replace("HeadlessChrome/", "Chrome/")
            .Replace("HeadlessChrome/", "Chrome/");

        var uaVersion = ua.Contains("Chrome/")
            ? Regex.Match(ua, @"Chrome\/([\d|.]+)").Groups[1].Value
            : Regex.Match(await page.Browser.GetVersionAsync(), @"\/([\d|.]+)").Groups[1].Value;

        var platform = GetPlatform(ua);
        var brand = GetBrands(uaVersion);

        var isMobile = GetIsMobile(ua);
        var platformVersion = GetPlatformVersion(ua);
        var platformArch = GetPlatformArch(isMobile);
        var platformModel = GetPlatformModel(isMobile, ua);

        var overrideObject = new OverrideUserAgent
        {
            UserAgent = ua,
            Platform = platform,
            AcceptLanguage = "en-US, en",
            UserAgentMetadata = new UserAgentMetadata
            {
                Brands = brand,
                FullVersion = uaVersion,
                Platform = platform,
                PlatformVersion = platformVersion,
                Architecture = platformArch,
                Model = platformModel,
                Mobile = isMobile
            }
        };

        await page.Client.SendAsync("Network.setUserAgentOverride", overrideObject);
    }

    private string GetPlatform(string ua)
    {
        if (ua.Contains("Mac OS X")) return "Mac OS X";

        if (ua.Contains("Android")) return "Android";

        if (ua.Contains("Linux")) return "Linux";

        return "Windows";
    }

    public string GetPlatformVersion(string ua)
    {
        if (ua.Contains("Mac OS X ")) return Regex.Match(ua, "Mac OS X ([^)]+)").Groups[1].Value;

        if (ua.Contains("Android ")) return Regex.Match(ua, "Android ([^;]+)").Groups[1].Value;

        if (ua.Contains("Windows ")) return Regex.Match(ua, @"Windows .*?([\d|.]+);").Groups[1].Value;

        return string.Empty;
    }

    public string GetPlatformArch(bool isMobile)
    {
        return isMobile ? string.Empty : "x86";
    }

    public string GetPlatformModel(bool isMobile, string ua)
    {
        return isMobile ? Regex.Match(ua, @"Android.*?;\s([^)]+)").Groups[1].Value : string.Empty;
    }

    public bool GetIsMobile(string ua)
    {
        return ua.Contains("Android");
    }

    private List<UserAgentBrand> GetBrands(string uaVersion)
    {
        var seed = int.Parse(uaVersion.Split('.')[0]);

        var order = new List<List<int>>
        {
            new()
            {
                0, 1, 2
            },
            new()
            {
                0, 2, 1
            },
            new()
            {
                1, 0, 2
            },
            new()
            {
                1, 2, 0
            },
            new()
            {
                2, 0, 1
            },
            new()
            {
                2, 1, 0
            }
        }[seed % 6];

        var greaseyBrand = $"Not;A=Brand";
        var greasedBrandVersionList = new Dictionary<int, UserAgentBrand>();

        greasedBrandVersionList.Add(order[0], new UserAgentBrand
        {
            Brand = greaseyBrand,
            Version = "99"
        });
        greasedBrandVersionList.Add(order[1], new UserAgentBrand
        {
            Brand = "Chromium",
            Version = "139",//seed.ToString()
        });
        greasedBrandVersionList.Add(order[2], new UserAgentBrand
        {
            Brand = "Google Chrome",
            Version = "139"//seed.ToString()
        });

        return greasedBrandVersionList.OrderBy(e => e.Key).Select(e => e.Value).ToList();
    }

    private class OverrideUserAgent
    {
        public string UserAgent { get; set; }
        public string Platform { get; set; }
        public string AcceptLanguage { get; set; }
        public UserAgentMetadata UserAgentMetadata { get; set; }
    }

    private class UserAgentMetadata
    {
        public List<UserAgentBrand> Brands { get; set; }
        public string FullVersion { get; set; }
        public string Platform { get; set; }
        public string PlatformVersion { get; set; }
        public string Architecture { get; set; }
        public string Model { get; set; }
        public bool Mobile { get; set; }
    }

    private class UserAgentBrand
    {
        public string Brand { get; set; }
        public string Version { get; set; }
    }
}

public class StealthUserAgentOptions(string ua) : IPuppeteerExtraPluginOptions
{
    public string UserAgent { get; } = ua;
}