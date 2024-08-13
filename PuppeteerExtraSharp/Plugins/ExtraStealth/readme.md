# Quickstart
```c#
var extra = new PuppeteerExtra();
// initialize stealth plugin
var stealth = new StealthPlugin();

var browser = await extra.Use(stealth).LaunchAsync(new LaunchOptions());

var page = await browser.NewPageAsync();

await page.GoToAsync("https://bot.sannysoft.com/");
```

# Plugin options

```c#
var extra = new PuppeteerExtra();
// Initialize hardware concurrency plugin options
var stealthHardwareConcurrencyOptions = new StealthHardwareConcurrencyOptions(12);
// Put it on stealth plugin constructor
var stealth = new StealthPlugin(stealthHardwareConcurrencyOptions);
var browser = await extra.Use(stealth).LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
await page.GoToAsync("https://bot.sannysoft.com/");
```

### Available options: 
#### Hardware concurrency
see (https://arh.antoinevastel.com/reports/stats/osName_hardwareConcurrency_report.html)
```c#
var concurrency = 12; // your number
var stealthHardwareConcurrencyOptions = new StealthHardwareConcurrencyOptions(concurrency);
```
#### Vendor
```c#
var vendor = "Google Inc."; // your custom navigator.vendor
var stealthVendorSettings = new StealthVendorSettings(vendor);
```
### Languages
```c#
var languages = "en-US"; // your custom languages array
var languagesSettings = new StealthLanguagesOptions(languages);
```

### WebGL
```c#
var webGLVendor = "Intel Inc."; // your custom webGL vendor
var render = "Intel Iris OpenGL Engine"; // your custom webGL renderer
var languagesSettings = new StealthWebGLOptions(webGLVendor, render);
```

# Removing evasions:
You can remove an evasion from the plugin by using the RemoveEvasionByType
```c#
var extra = new PuppeteerExtra();
// initialize stealth plugin
var stealth = new StealthPlugin();
stealthPlugin.RemoveEvasionByType<ContentWindow>();
var browser = await extra.Use(stealth).LaunchAsync(new LaunchOptions());
```