# CaptchaSolver Plugin

A unified CAPTCHA solving plugin for PuppeteerExtraSharp that supports multiple CAPTCHA vendors and solving services.

## Features

- **Multi-vendor support**: Detect and solve CAPTCHAs from multiple vendors in a single plugin
- **Multiple solving providers**: Support for CapSolver and 2Captcha services
- **Automatic detection**: Automatically detect CAPTCHAs on page load and navigation
- **Smart filtering**: Fine-grained control over which CAPTCHAs to solve
- **Extensible architecture**: Easy to add new vendors and providers

## Supported CAPTCHA Vendors

| Vendor | Status                 | Types Supported |
|--------|------------------------|----------------|
| **Google reCAPTCHA** | ✅ Active               | v2 Checkbox, v2 Invisible, v3, Enterprise |
| **GeeTest** | ✅ Active               | v3, v4 |
| **Cloudflare Turnstile** | ✅ Active               | Managed, Non-interactive, Invisible |
| **hCaptcha** | 🔍 Detection only      | - |
| **DataDome** | 🚧 Not yet implemented | Requires proxy support |

> **Notes**:
> - **hCaptcha**: Due to hCaptcha's aggressive anti-automation measures and legal actions against solving services, solving is intentionally disabled. Detection still works, but automated solving is not attempted.
> - **DataDome**: Not yet implemented. DataDome CAPTCHAs require proxy support for proper handling and solving.

## Supported Solving Providers

- **[CapSolver](https://www.capsolver.com/)** - Fast and reliable CAPTCHA solving service
- **[2Captcha](https://2captcha.com/)** - Popular CAPTCHA solving service with broad support

## Installation

The plugin is included in PuppeteerExtraSharp. Simply add it to your project and configure it with a solving provider.

## Quick Start

### Basic Usage

```csharp
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.CaptchaSolver;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.CapSolver;

// Create plugin with CapSolver provider
var captchaSolver = new CaptchaSolverPlugin(
    new CapSolver("YOUR_CAPSOLVER_API_KEY")
);

// Add plugin to PuppeteerExtra
var extra = new PuppeteerExtra();
extra.Use(captchaSolver);

// Launch browser
var browser = await extra.LaunchAsync(new LaunchOptions
{
    Headless = false
});

var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com/captcha-page");

// Solve CAPTCHAs on the page
var result = await captchaSolver.SolveCaptchaAsync(page);

if (string.IsNullOrEmpty(result.Error))
{
    Console.WriteLine($"Successfully solved {result.Solved.Count} CAPTCHA(s)");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

### Using 2Captcha Provider

```csharp
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.TwoCaptcha;

var captchaSolver = new CaptchaSolverPlugin(
    new TwoCaptcha("YOUR_2CAPTCHA_API_KEY")
);
```

## Configuration

### CaptchaSolverOptions

Configure the plugin behavior with `CaptchaSolverOptions`:

```csharp
var options = new CaptchaSolverOptions
{
    // Maximum time to wait for CAPTCHA to appear (default: 5 seconds)
    CaptchaWaitTimeout = TimeSpan.FromSeconds(10),

    // Throw exceptions on errors instead of returning error messages
    ThrowOnError = false,

    // Minimum acceptable score for score-based CAPTCHAs (0.0 - 1.0, default: 0.5)
    MinScore = 0.7,

    // Only solve CAPTCHAs visible in the viewport
    SolveInViewportOnly = false,

    // Solve score-based CAPTCHAs (like reCAPTCHA v3)
    SolveScoreBased = true,

    // Solve invisible CAPTCHAs with no active challenge
    SolveInactiveChallenges = true,

    // Solve invisible CAPTCHA challenges
    SolveInvisibleChallenges = true,

    // Enable debug logging
    Debug = false,

    // Control which vendors to handle
    EnabledVendors = new Dictionary<CaptchaVendor, ICaptchaSolveOptions?>
    {
        { CaptchaVendor.Google, new GoogleOptions() },
        { CaptchaVendor.GeeTest, null },
        { CaptchaVendor.Cloudflare, null },
        { CaptchaVendor.HCaptcha, null },
        { CaptchaVendor.DataDome, null }
    }
};

var captchaSolver = new CaptchaSolverPlugin(
    new CapSolver("YOUR_API_KEY"),
    options
);
```

### Provider Options

Configure the solving provider's behavior:

```csharp
var providerOptions = new CaptchaProviderOptions
{
    // Timeout for the entire solving operation (default: 5 minutes)
    Timeout = TimeSpan.FromMinutes(3),

    // Initial delay before checking for solution (default: 5 seconds)
    StartTimeout = TimeSpan.FromSeconds(3),

    // Polling interval to check for solution (default: 2 seconds)
    PollingInterval = TimeSpan.FromSeconds(2)
};

var provider = new CapSolver("YOUR_API_KEY", providerOptions);
var captchaSolver = new CaptchaSolverPlugin(provider);
```

### Vendor-Specific Options

Configure options for specific CAPTCHA vendors:

```csharp
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.Google;

var options = new CaptchaSolverOptions
{
    EnabledVendors = new Dictionary<CaptchaVendor, ICaptchaSolveOptions?>
    {
        {
            CaptchaVendor.Google,
            new GoogleOptions
            {
                // Google-specific options
            }
        }
    }
};
```

## Advanced Usage

### Manual CAPTCHA Solving with Custom Options

```csharp
// Create plugin with default options
var captchaSolver = new CaptchaSolverPlugin(
    new CapSolver("YOUR_API_KEY")
);

extra.Use(captchaSolver);
var browser = await extra.LaunchAsync(new LaunchOptions { Headless = false });
var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com/captcha");

// Override options for this specific solve
var customOptions = new CaptchaSolverOptions
{
    SolveInViewportOnly = true,
    ThrowOnError = true
};

var result = await captchaSolver.SolveCaptchaAsync(page, customOptions);
```

### Handling Results

```csharp
var result = await captchaSolver.SolveCaptchaAsync(page);

// Check for errors
if (!string.IsNullOrEmpty(result.Error))
{
    Console.WriteLine($"Error: {result.Error}");
    return;
}

// Check solved CAPTCHAs
Console.WriteLine($"Solved {result.Solved.Count} CAPTCHA(s):");
foreach (var solution in result.Solved)
{
    Console.WriteLine($"  - {solution.Vendor} CAPTCHA (ID: {solution.Id})");
}

// Check filtered CAPTCHAs
if (result.Filtered.Count > 0)
{
    Console.WriteLine($"Filtered {result.Filtered.Count} CAPTCHA(s):");
    foreach (var filtered in result.Filtered)
    {
        Console.WriteLine($"  - Reason: {filtered.FilteredReason}");
    }
}
```

### Automatic CAPTCHA Detection

The plugin automatically hooks into page creation and monitors for CAPTCHAs. To enable automatic solving on page load:

```csharp
// The plugin automatically detects CAPTCHAs when pages are created
// You can manually trigger solving at any time
var result = await captchaSolver.SolveCaptchaAsync(page);
```

## Architecture

### Components

The plugin follows a modular architecture:

```
CaptchaSolverPlugin
├── Providers (ICaptchaSolverProvider)
│   ├── CapSolver
│   └── TwoCaptcha
├── Vendors (ICaptchaVendorHandler)
│   ├── Google (GoogleHandler)
│   ├── GeeTest (GeeTestHandler)
│   ├── Cloudflare (CloudflareHandler)
│   ├── HCaptcha (HCaptchaHandler)
│   └── DataDome (DataDomeHandler)
└── Handler (ICaptchaSolverHandler)
    └── CaptchaSolverHandler (coordinates vendors)
```

### How It Works

1. **Detection**: When a page is created, vendor handlers inject detection scripts
2. **Monitoring**: Handlers monitor page responses for CAPTCHA challenges
3. **Discovery**: When `SolveCaptchaAsync` is called, the plugin scans for CAPTCHAs
4. **Filtering**: CAPTCHAs are filtered based on options (viewport, type, etc.)
5. **Solving**: Filtered CAPTCHAs are sent to the provider for solving
6. **Injection**: Solutions are injected back into the page

### Extending the Plugin

#### Adding a New Provider

Implement the `ICaptchaSolverProvider` interface:

```csharp
public class MyCustomProvider : ICaptchaSolverProvider
{
    public async Task<string> GetSolutionAsync(GetCaptchaSolutionRequest request)
    {
        // Call your solving service API
        // Return the solution token
    }
}
```

#### Adding a New Vendor Handler

1. Implement `ICaptchaVendorHandler` interface
2. Add new vendor to `CaptchaVendor` enum
3. Register handler in `Helpers.CreateHandler()`
4. Add detection/injection scripts

See existing vendor handlers in `Vendors/` directory for examples.

## Troubleshooting

### CAPTCHAs Not Detected

- Ensure the CAPTCHA vendor is enabled in `EnabledVendors`
- Check if the page has loaded completely
- Increase `CaptchaWaitTimeout`
- Enable `Debug = true` for detailed logging

### Solutions Not Working

- Verify your API key is valid and has credits
- Check if the CAPTCHA is being filtered (inspect `result.Filtered`)
- Ensure you're not solving invisible CAPTCHAs when `SolveInvisibleChallenges = false`
- Try different `MinScore` values for score-based CAPTCHAs

### Provider Timeouts

- Increase `CaptchaProviderOptions.Timeout`
- Adjust `PollingInterval` and `StartTimeout`
- Check your provider service status

## Comparison with RecaptchaPlugin

The `CaptchaSolverPlugin` is the successor to `RecaptchaPlugin`:

| Feature | RecaptchaPlugin | CaptchaSolverPlugin |
|---------|----------------|---------------------|
| Vendors | Google reCAPTCHA only | Multiple vendors |
| Providers | CapSolver, 2Captcha, extensible | CapSolver, 2Captcha, extensible |
| Status | Legacy | Active development |

**Recommendation**: Use `CaptchaSolverPlugin` for new projects. `RecaptchaPlugin` is maintained for backwards compatibility.

## Examples

See the `Tests/` directory for comprehensive examples:

- **GoogleTests.cs**: Google reCAPTCHA (v2, v3, Enterprise) solving examples
- **GeeTestTests.cs**: GeeTest v3 and v4 solving examples
- **CloudflareTests.cs**: Cloudflare Turnstile solving examples

## Legal and Ethical Use

Use this library only on properties you own or where you have explicit permission to automate. Respect target site terms of service.

- "reCAPTCHA" is a trademark of Google LLC
- "hCaptcha" is a trademark of Intuition Machines, Inc.
- "Cloudflare Turnstile" is a trademark of Cloudflare, Inc.

## License

Part of PuppeteerExtraSharp. See repository LICENSE for details.

## Credits

Inspired by [puppeteer-extra-plugin-recaptcha](https://github.com/berstend/puppeteer-extra/tree/master/packages/puppeteer-extra-plugin-recaptcha) and adapted for .NET.