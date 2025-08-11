# PuppeteerSharpToolkit

PuppeteerSharpToolkit is a high-performance, AOT-friendly plugin toolkit for PuppeteerSharp. It’s a modern reimagining of the [PuppeteerExtraSharp](https://github.com/Overmiind/Puppeteer-sharp-extra) pattern with a new entry point ([PluginManager](src/PuppeteerSharpToolkit/PluginManager.cs)), focused on trimming- and AOT-safe code, reduced allocations, and startup speed. It is not a drop-in replacement for `PuppeteerExtraSharp`; APIs and behaviors have been redesigned for performance and clarity.

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
- **[Anonymize UA Plugin](src/PuppeteerSharpToolkit/Plugins/AnonymizeUaPlugin.cs)** – Anonymizes the user-agent on all pages.
- **[ReCAPTCHA Plugin](src/PuppeteerSharpToolkit/Plugins/Recaptcha/RecapchaPlugin.cs)** – Solves reCAPTCHAs automatically.
- **[Block Resources Plugin](src/PuppeteerSharpToolkit/Plugins/BlockResourcesPlugin.cs)** – Blocks images, documents, and other resource types to speed up navigation.

## API

### Stealth Plugins

Stealth related plugins are made for evading detection, usually by injecting scripts at specific times at which websites try to detect bots.

The available evasion plugins are the following: [ChromeAppPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/ChromeAppPlugin.cs), [ChromeSciPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/ChromeSciPlugin.cs), [CodecPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/CodecPlugin.cs), [ContentWindowPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/ContentWindowPlugin.cs), [EvasionPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/EvasionPlugin.cs), [HardwareConcurrencyPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/HardwareConcurrencyPlugin.cs), [LanguagesPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/LanguagesPlugin.cs), [LoadTimesPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/LoadTimesPlugin.cs), [OutDimensionsPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/OutDimensionsPlugin.cs), [PermissionsPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/PermissionsPlugin.cs), [StackTracePlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/StackTracePlugin.cs), [UserAgentPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/UserAgentPlugin.cs), [VendorPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/VendorPlugin.cs), [WebDriverPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/WebDriverPlugin.cs), [WebGLPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/WebGlPlugin.cs).

In this example we'll use [ChromeSciPlugin](src/PuppeteerSharpToolkit/Plugins/Stealth/ChromeSciPlugin.cs) which patches "chrome.app" and some other chrome APIs that are used by bot detection scripts.

```csharp
var manager = new PluginManager();
manager.Register(new ChromeSciPlugin());

// Continue with browser and page
```

To make life a bit easier, [Stealth](src/PuppeteerSharpToolkit/Plugins/Stealth/Stealth.cs) is a static class that provides some static methods:

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

### [AnonymizeUaPlugin](src/PuppeteerSharpToolkit/Plugins/AnonymizeUaPlugin.cs)

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

### [RecaptchaPlugin](src/PuppeteerSharpToolkit/Plugins/Recaptcha/RecapchaPlugin.cs)

RecaptchaPlugin is used in conjunction with Recaptcha solving providers, at this state [AntiCaptchaProvider](src/PuppeteerSharpToolkit/Plugins/Recaptcha/AntiCaptcha/AntiCaptchaProvider.cs) and [TwoCaptchaProvider](src/PuppeteerSharpToolkit/Plugins/Recaptcha/Provider/TwoCaptchaProvider.cs) are supported, but this is extensible so you add your own.

The following is an example with `AntiCaptcha`

- `Anti-Captcha` provider is disabled pending better solution for their requirement to get recaptcha key.

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

For [2captcha](https://2captcha.com/ru) simply use the [TwoCaptchaProvider](src/PuppeteerSharpToolkit/Plugins/Recaptcha/TwoCaptcha/TwoCaptchaProvider.cs) instead.

### [BlockResourcesPlugin](src/PuppeteerSharpToolkit/Plugins/BlockResourcesPlugin.cs)

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

- Plugin registration: [PluginManager](src/PuppeteerSharpToolkit/PluginManager.cs).Register(params [PuppeteerPlugin](src/PuppeteerSharpToolkit/Plugins/PuppeteerPlugin.cs)[] plugins)
- Launch: await [PluginManager](src/PuppeteerSharpToolkit/PluginManager.cs).LaunchAsync(LaunchOptions options)
- Connect: await [PluginManager](src/PuppeteerSharpToolkit/PluginManager.cs).ConnectAsync(ConnectOptions options)

## Caveats / Testing

[RecaptchaPlugin](src/PuppeteerSharpToolkit/Plugins/Recaptcha/RecapchaPlugin.cs) is a clean abstraction, and the tests are mainly testing the specific providers, these tests require having API keys for said providers.

To set the API keys, `dotnet` user secrets is used, it must be configured when inside the test project directory:

```bash
dotnet user-secrets set [secretName] [secretValue]
```

- For [AntiCaptchaProvider](src/PuppeteerSharpToolkit/Plugins/Recaptcha/AntiCaptcha/AntiCaptchaProvider.cs): `secretName = "AntiCaptchaKey"`
- For [TwoCaptchaProvider](src/PuppeteerSharpToolkit/Plugins/Recaptcha/TwoCaptcha/TwoCaptchaProvider.cs): `secretName = "TwoCaptchaKey"`

All Recaptcha tests are marked as `Explicit` and will not execute when running the full test suite (unless specifically asked for), this is to avoid burning API usage.

If you do want to run them use:

```bash
dotnet run --explicit on/only
```

Also, each provider test will be skipped automatically if the required API key wasn't found in user secrets, so if for example you only have an API key for `AntiCaptcha` - only the tests related to this provider will run.

## Contribution

Patches, performance improvements, and fixes are welcome. Focus is on keeping the core lean and compatible with modern .NET publishing scenarios.
