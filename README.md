# PuppeteerSharpToolkit

[![NuGet Badge](https://buildstats.info/nuget/PuppeteerSharpToolkit)](https://www.nuget.org/packages/PuppeteerSharpToolkit)

PuppeteerSharpToolkit is a high-performance, AOT-friendly plugin toolkit for PuppeteerSharp. It’s a modern reimagining of the [PuppeteerExtraSharp](https://github.com/Overmiind/Puppeteer-sharp-extra) pattern with a new entry point (`PluginManager`), focused on trimming- and AOT-safe code, reduced allocations, and startup speed. It is not a drop-in replacement for `PuppeteerExtraSharp`; APIs and behaviors have been redesigned for performance and clarity.

## Why this package exists

- **Updated core**: Bumps PuppeteerSharp dependencies to a more modern stack.
- **.NET 9 support**: Targets .NET 9, unlocking ahead-of-time (AOT) compilation and aggressive trimming for small, fast binaries.
- **Performance-first design**: Rewritten internals to avoid reflection and embedded DLL tricks; optimized for minimal allocations and startup cost.
- **Dropped RestSharp**: No longer depends on RestSharp.
- **Dropped NewtonSoftJson**: No longer uses NewtonSoft - `System.Text.Json` is the new king.

## Quickstart

```csharp
// Initialize the plugin manager
var manager = new PluginManager();

// Register plugins (dependencies first)
manager.Register(new AnonymizeUaPlugin());

// Launch the browser with plugins wired
await using var browser = await manager.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com");
await page.ScreenshotAsync("example.png");
```

## Plugin list

- **Stealth Plugins** – Various evasion techniques to make headless detection harder.
- **Anonymize UA Plugin** – Anonymizes the user-agent on all pages.
- **ReCAPTCHA Plugin** – Solves reCAPTCHAs automatically.
- **Block Resources Plugin** – Blocks images, documents, and other resource types to speed up navigation.

## API

### Stealth Plugins

Stealth related plugins are made for evading detection, usually by injecting scripts at specific times at which websites try to detect bots.

The available evasion plugins are the following: `ChromeAppPlugin`, `ChromeSciPlugin`, `CodecPlugin`, `ContentWindowPlugin`, `EvasionPlugin`, `HardwareConcurrencyPlugin`, `LanguagesPlugin`, `LoadTimesPlugin`, `OutDimensionsPlugin`, `PermissionsPlugin`, `StackTracePlugin`, `UserAgentPlugin`, `VendorPlugin`, `WebDriverPlugin`, `WebGLPlugin`.

In this example we'll use `ChromeSciPlugin` which patches "chrome.app" and some other chrome APIs that are used by bot detection scripts.

```csharp
var manager = new PluginManager();
manager.Register(new ChromeSciPlugin());

// Continue with browser and page
```

To make life a bit easier, `Stealth` is a static class that provides some static methods:

```csharp
PuppeteerPlugin[] GetStandardEvasions(params PuppeteerPlugin[] pluginOverride);
// Which returns an array of most stealth plugins.
// The override allows you to initialize any that you with custom arguments
// Any override that you supply will be used instead of calling the default plugin ctor

Task RegisterUtilsAsync(IPage page);
// This function register `utils.js` on a page, it is required by some scripts
// You could also use it if you implement your own plugin, which injects a script
// ... that can rely on `utils.js`
```

So in essence you could register a suite of stealth plugins in one go:

```csharp
var manager = new PluginManager();
manager.Register(Stealth.GetStandardEvasions());
```

### AnonymizeUaPlugin

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
plugin.UserAgentTransformer = ua => "whatever";
// Initialize browser and page + continue with whatever else
```

### RecaptchaPlugin

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
await using var browser = await manager.LaunchAsync();
await using var page = await browser.NewPageAsync();
// go to page with captcha
await page.GoToAsync("https://patrickhlauke.github.io/recaptcha/");
// now solve captcha at page
await plugin.SolveCaptchaAsync(page); // Also accepts proxyStr and cancellationToken.
```

For [2captcha](https://2captcha.com/ru) simply use the `TwoCaptchaProvider` instead.

### BlockResourcesPlugin

This plugin is used to blocks page resources in Puppeteer requests (img, documents etc.)

```csharp
// Initialize the plugin manager
var manager = new PluginManager();
// Initialize the plugin
var plugin = new BlockResourcesPlugin();
// Register the plugin
manager.Register(plugin);
// Initialize browser and page
await using var browser = await manager.LaunchAsync();
await using var page = await browser.NewPageAsync();
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

## Core API

- Plugin registration: `PluginManager.Register(params PuppeteerPlugin[] plugins)`
- Launch: `await PluginManager.LaunchAsync(LaunchOptions options)`
- Connect: `await PluginManager.ConnectAsync(ConnectOptions options)`

## Caveats / Testing

Some plugin tests for third-party services such as TwoCaptcha and AntiCaptcha require valid API credentials. The test suite is configured to read these credentials from environment variables; if the expected variables are not set, those specific plugin tests are skipped automatically. To include them in a test run, set the appropriate environment variables before executing the test command (e.g., on macOS/Linux: `export KEY_NAME=your_api_key` or on Windows PowerShell: `$env:KEY_NAME = 'your_api_key'`). Refer to the test project source for the exact variable names required.

## Contribution

Patches, performance improvements, and fixes are welcome. Focus is on keeping the core lean and compatible with modern .NET publishing scenarios.
