## CAPTCHA Plugin

Solve CAPTCHA challenges programmatically with a single call. Supports reCAPTCHA v2, v3, and invisible (button/callback).

### Features
- Single-call solve: detect the widget, request a token via the provider API, and inject it into the page.
- Supports:
    - reCAPTCHA v2 (checkbox)
    - reCAPTCHA v2 Invisible (button/callback-triggered)
    - reCAPTCHA v3 (score-based)
- Options for viewport-only solving, inactive/lazy widgets, debugging, timeouts, and v3 score threshold.
- Pluggable provider design. Currently supported: [2Captcha](https://2captcha.com/?from=1937404).

## Quick start

```csharp
// Initialize the 2Captcha provider with your API key
var twoCaptchaProvider = new TwoCaptcha("<YOUR_2CAPTCHA_API_KEY>");
var recaptchaPlugin = new RecaptchaPlugin(twoCaptchaProvider);

var puppeteerExtra = new PuppeteerExtra();

// Launch browser with the reCAPTCHA plugin enabled
var browser = await puppeteerExtra.Use(recaptchaPlugin).LaunchAsync();

var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.google.com/recaptcha/api2/demo");

// Single call to detect the widget, request a solution via the API, and inject the token
await recaptchaPlugin.SolveCaptchaAsync(page);

// Submit the form on the page
var submitButton = await page.QuerySelectorAsync("#recaptcha-demo-submit");
await submitButton.ClickAsync();
```

#### Advanced configuration

```csharp
// Configure the 2Captcha provider (timeouts and polling behavior)
var twoCaptchaProviderOptions = new CaptchaProviderOptions
{
    ApiTimeout = TimeSpan.FromMinutes(2),       // Maximum total time to wait for a solution
    MaxPollingAttempts = 5,                     // Max number of status checks before giving up
    StartTimeout = TimeSpan.FromSeconds(50),    // Initial delay before polling (provider-side job start)
};

var twoCaptchaProvider = new TwoCaptcha("<YOUR_2CAPTCHA_API_KEY>", twoCaptchaProviderOptions);

// Configure how the plugin detects and solves challenges
var pluginOptions = new RecaptchaSolveOptions
{
    ThrowOnError = true,                        // Throw exceptions on failure (otherwise return a soft failure)
    SolveInViewportOnly = true,                 // Only solve widgets visible in the viewport
    SolveScoreBased = true,                    // Enable handling of reCAPTCHA v3 (score-based)
    SolveInactiveChallenges = true,            // Attempt to solve lazy/inactive widgets
    CaptchaWaitTimeout = TimeSpan.FromSeconds(10), // Wait time for a widget/challenge to appear
    Debug = false,                             // Verbose debug logging
    MinV3RecaptchaScore = 0.3,                 // Minimum acceptable v3 score
    SolveInvisibleChallenges = true,           // Handle invisible/triggered reCAPTCHA
};

var recaptchaPlugin = new RecaptchaPlugin(twoCaptchaProvider, pluginOptions);
```

#### Notes
- reCAPTCHA v3 is score-based; adjust MinV3RecaptchaScore to match your tolerance.
- For slow pages or delayed widgets, consider increasing CaptchaWaitTimeout and provider timeouts.
- Set Debug = true to enable verbose diagnostics.

#### Legal and ethical use
Use this library only on properties you own or where you have explicit permission to automate. Respect target site terms of service. “reCAPTCHA” is a trademark of Google.