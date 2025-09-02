# PuppeteerExtraSharp

[![NuGet Downloads](https://img.shields.io/nuget/dt/PuppeteerExtraSharp)](https://img.shields.io/nuget/dt/PuppeteerExtraSharp)
[![NuGet Version](https://img.shields.io/nuget/v/PuppeteerExtraSharp)](https://img.shields.io/nuget/v/PuppeteerExtraSharp)

PuppeteerExtraSharp is a .NET port of the [puppeteer-extra](https://github.com/berstend/puppeteer-extra/tree/master/packages/puppeteer-extra) library for Node.js

## Plugins

🪄 [Puppeteer reCAPTCHA plugin](https://github.com/Overmiind/Puppeteer-sharp-extra/tree/master/PuppeteerExtraSharp/Plugins/Recaptcha)
- Automatically handles reCAPTCHA challenges (v2, invisible, v3).

🏴 [Puppeteer stealth plugin](https://github.com/Overmiind/Puppeteer-sharp-extra/tree/master/PuppeteerExtraSharp/Plugins/ExtraStealth)
- Applies multiple evasions to make headless automation harder to detect.

📃 [Puppeteer block resources plugin](https://github.com/Overmiind/Puppeteer-sharp-extra/tree/master/PuppeteerExtraSharp/Plugins/BlockResources)
- Block unwanted network requests (scripts, images, documents, etc.) using simple, composable rules

## Quick Start

```csharp
// Initialize plugin builder
var extra = new PuppeteerExtra();

// Enable the Stealth plugin
extra.Use(new StealthPlugin());

// Launch the browser with plugins applied
var browser = await extra.LaunchAsync();

// Create a new page
var page = await browser.NewPageAsync();

await page.GoToAsync("https://google.com");
await Task.Delay(2000);

// Take a screenshot
await page.ScreenshotAsync("extra.png");
```

## Notes
- Use the reCAPTCHA plugin only on properties you own or where you have explicit permission to automate.
- Some targets may still detect automation; adjust plugin combinations and browser settings as needed.