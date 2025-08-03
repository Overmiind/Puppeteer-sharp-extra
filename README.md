# PuppeteerExtraSharpLite

[![NuGet Badge](https://buildstats.info/nuget/PuppeteerExtraSharp)](https://www.nuget.org/packages/PuppeteerExtraSharpLite)

PuppeteerExtraSharpLite is a high-performance, modern rewrite and lighter drop-in successor to PuppeteerExtraSharp. Its primary purpose is to upgrade the underlying PuppeteerSharp dependencies, target .NET 9, and enable AOT compilation, trimming, and significantly better runtime performance. Key changes include removing RestSharp, eliminating reliance on reflection and DLL embedding, and numerous internal rewrites to maximize throughput and startup time.

## Why this package exists

- **Updated core**: Bumps PuppeteerSharp dependencies to a more modern stack.
- **.NET 9 support**: Targets .NET 9, unlocking ahead-of-time (AOT) compilation and aggressive trimming for small, fast binaries.
- **Performance-first design**: Rewritten internals to avoid reflection and embedded DLL tricks; optimized for minimal allocations and startup cost.
- **Dropped RestSharp**: No longer depends on RestSharp; any HTTP client logic uses lighter/custom wrappers or `HttpClient` directly.
- **Trimming & AOT friendly**: Designed to work well with publish-time trimming and native-compilation scenarios.

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
