# PuppeteerExtraSharpLite

[![NuGet Badge](https://buildstats.info/nuget/PuppeteerExtraSharp)](https://www.nuget.org/packages/PuppeteerExtraSharpLite)

PuppeteerExtraSharpLite is a high-performance, modern rewrite and lighter drop-in successor to PuppeteerExtraSharp. Its primary purpose is to upgrade the underlying PuppeteerSharp dependencies, target .NET 9, and enable AOT compilation, trimming, and significantly better runtime performance. Key changes include removing RestSharp, eliminating reliance on reflection and DLL embedding, and numerous internal rewrites to maximize throughput and startup time.

## Why this package exists

- **Updated core**: Bumps PuppeteerSharp dependencies to a more modern stack.
- **.NET 9 support**: Targets .NET 9, unlocking ahead-of-time (AOT) compilation and aggressive trimming for small, fast binaries.
- **Performance-first design**: Rewritten internals to avoid reflection and embedded DLL tricks; optimized for minimal allocations and startup cost.
- **Dropped RestSharp**: No longer depends on RestSharp.
- **Dropped NewtonSoftJson**: No longer uses NewtonSoft - `System.Text.Json` is the new king.

## Quickstart

Usage is intentionally very similar to PuppeteerExtraSharp and meant to serve as a drop-in replacement in many cases. Note: some test suites or integrations rely on specific browser/engine APIs, so depending on your environment not all tests may run out of the box.

```csharp
// Initialize the plugin pipeline (same idiom as before)
var extra = new PuppeteerExtra();

// Use stealth plugin
extra.Use(new StealthPlugin());

// Launch the browser with plugins
var browser = await extra.LaunchAsync(new LaunchOptions()
{
    Headless = false
});

// Create a new page
var page = await browser.NewPageAsync();

await page.GoToAsync("http://google.com");

// Wait 2 seconds
await page.WaitForTimeoutAsync(2000);

// Take a screenshot
await page.ScreenshotAsync("extra.png");
```

## Major Highlights / Differences from PuppeteerExtraSharp

- Drop-in similar API surface but internally reworked for performance.
- Native .NET 9 targeting: AOT, trimming, reduced binary size.
- No RestSharp dependency anymore.
- Removed reflection-based plumbing and embedded DLL complexity.
- Internal HTTP clients use `HttpClient` or minimal wrappers, not heavy third-party HTTP libraries.

## Plugin list

- **Stealth Plugin** – Applies various evasion techniques to make headless detection harder.
- **Anonymize UA Plugin** – Anonymizes the user-agent on all pages.
- **ReCAPTCHA Plugin** – Solves reCAPTCHAs automatically.
- **Block Resources Plugin** – Blocks images, documents, and other resource types to speed up navigation.

## API

//TODO: Add specific plugin readmes

### `StealthPlugin`



### `AnonymizeUaPlugin`

AnonymizeUaPlugin is used to transform the user agent on any page as soon as it launches.

```csharp
// Initialize the plugin manager
var manager = new PluginManager();
// Initialize the plugin
var plugin = new AnonymizeUaPlugin();
// Register the plugin
manager.Register(plugin);
// by default it will replace headless with regular chrome
// and set platform to windows
// you can also add your own custom transformation at any time by changing the function
plugin.UserAgentTransformer = ua => "whatever"
// Initialize browser and page + continue with whatever else
```

### `RecaptchaPlugin`

RecaptchaPlugin is used in conjunction with Recaptcha solving providers, at this state [AntiCaptcha](https://anti-captcha.com/mainpage) and [2captcha](https://2captcha.com/ru) are supported, but this is extensible so you add your own.

The following is an example with `AntiCaptcha`

```csharp
// have HttpClient instance ready (it is used to send the requests)
using var client = new HttpClient(); // initialization as example
                                     // best practices will use a factory/singleton
// Initialize the plugin manager
var manager = new PluginManager();
// Initialize the provider (HttpClient, userKey, ProviderOptions)
var provider = new AntiCaptchaProvider(client, "sampleKey", ProviderOptions.Default);
// Default provider options can be used or inferred if omitted.
// Initialize plugin
var plugin = new RecaptchaPlugin(provider);
// Register the plugin
manager.Register(plugin);
// Initialize browser and page
await using var browser = manager.LaunchAsync();
using var page = browser.NewPageAsync();
// go to page with captcha
await page.GoToAsync("https://patrickhlauke.github.io/recaptcha/");
// now solve captcha at page
await plugin.SolveCaptchaAsync(page); // Also accepts proxyStr and cancellationToken.
```

For [2captcha](https://2captcha.com/ru) simply use the `TwoCaptchaProvider` instead.

### `BlockResourcesPlugin`

This plugin is used to blocks page resources in Puppeteer requests (img, documents etc.)

```csharp
// Initialize the plugin manager
var manager = new PluginManager();
// Initialize the plugin
var plugin = new BlockResourcesPlugin();
// Register the plugin
manager.Register(plugin);
// Initialize browser and page
await using var browser = manager.LaunchAsync();
using var page = browser.NewPageAsync();
// Create a rule
var blockGoogle = new BlockRule() {
    SitePattern = new Regex("google"), // SitePattern applies only to specific urls by pattern
                                       // You can also point it to a GeneratedRegex
    IPage = page, // it will affect this instance of the page
    ResourceType = ResourceType.Scripts // Block scripts
};
// Add rule
plugin.AddRule(blockGoogle);
```

You can also inspect rules on the plugin, as well as add or remove rules at any point. removing rules uses a `Func<BlockRule, bool>` predicate to select the rules to remove.

### Use(IPuppeteerExtraPlugin plugin)

Adds a plugin to the pipeline. Returns the same `PuppeteerExtra` instance for chaining.

```csharp
var puppeteerExtra = new PuppeteerExtra()
    .Use(new AnonymizeUaPlugin())
    .Use(new StealthPlugin());
```

### LaunchAsync(LaunchOptions options)

Launches a new browser with the configured plugins.

```csharp
var browser = await new PuppeteerExtra().LaunchAsync(new LaunchOptions());
```

### ConnectAsync(ConnectOptions options)

Connects to an existing browser instance.

```csharp
var browser = await new PuppeteerExtra().ConnectAsync(new ConnectOptions());
```

### GetPlugin<T>()

Retrieves a registered plugin by type.

```csharp
var stealthPlugin = puppeteerExtra.GetPlugin<StealthPlugin>();
```

## Caveats / Testing

Some plugin tests for third-party services such as TwoCaptcha and AntiCaptcha require valid API credentials. The test suite is configured to read these credentials from environment variables; if the expected variables are not set, those specific plugin tests are skipped automatically. To include them in a test run, set the appropriate environment variables before executing the test command (e.g., on macOS/Linux: `export KEY_NAME=your_api_key` or on Windows PowerShell: `$env:KEY_NAME = 'your_api_key'`). Refer to the test project source for the exact variable names required.

## Contribution

Patches, performance improvements, and fixes are welcome. Focus is on keeping the core lean and compatible with modern .NET publishing scenarios.
