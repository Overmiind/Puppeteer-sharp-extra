# PuppeteerExtraSharp

[![NuGet Downloads](https://img.shields.io/nuget/dt/PuppeteerExtraSharp)](https://www.nuget.org/packages/PuppeteerExtraSharp)
[![NuGet Version](https://img.shields.io/nuget/v/PuppeteerExtraSharp)](https://www.nuget.org/packages/PuppeteerExtraSharp)

PuppeteerExtraSharp is a .NET port of the [puppeteer-extra](https://github.com/berstend/puppeteer-extra/tree/master/packages/puppeteer-extra) library for Node.js

## Plugins

🪄 [Puppeteer captcha solver plugin](https://github.com/Overmiind/Puppeteer-sharp-extra/tree/master/PuppeteerExtraSharp/Plugins/CaptchaSolver)
- Automatically handles reCAPTCHA challenges (v2, invisible, v3), GeeTest, CloudFlare.

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
await extra.PreparePageAsync(page);

await page.GoToAsync("https://google.com");
await Task.Delay(2000);

// Take a screenshot
await page.ScreenshotAsync("extra.png");
```

## Page initialization

New pages still receive plugins automatically. Await `extra.PreparePageAsync(page)` before
navigation when page scripts must see the configured settings from the start. This also works
with pages created in a browser context and with browsers returned by `ConnectAsync`.
Repeated calls share one initialization task, including its failure.

The optional cancellation token cancels the wait. Close the page or its context to interrupt
browser work, then close the browser when it is no longer needed.

```csharp
extra.PluginError += (_, error) => Console.Error.WriteLine(error.GetException());

var context = await browser.CreateBrowserContextAsync();
try
{
    var page = await context.NewPageAsync();
    await extra.PreparePageAsync(page, cancellationToken);
    await page.GoToAsync(url);
}
finally
{
    await context.CloseAsync();
}
```

`PreparePageAsync` propagates setup errors to its caller. Automatic initialization reports
unexpected errors through `PluginError`. A target closing during automatic initialization is
expected during shutdown and is ignored by the event handler; an explicit waiter still receives
the failure. These guarantees cover the page initialization callbacks, not asynchronous event
handlers that a plugin may register itself.

Run the page initialization regression tests with:

```sh
dotnet test Tests/Extra.Tests.csproj --filter FullyQualifiedName~PageInitialization
```

Set `PUPPETEER_EXECUTABLE_PATH` to a local Chrome or Chromium executable to include the browser
tests. They use local data URLs and do not need credentials or external websites.

## Notes
- Use the captcha solver plugin only on properties you own or where you have explicit permission to automate.
- Some targets may still detect automation; adjust plugin combinations and browser settings as needed.
