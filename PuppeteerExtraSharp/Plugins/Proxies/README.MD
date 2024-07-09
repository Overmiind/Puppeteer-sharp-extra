# Quickstart
  
Work's with IPv$ and IPv6 proxies!  

```c#
var extra = new PuppeteerExtra();
// initialize proxies plugin
var proxies = new ProxiesPlugin(new ProxyInfo("localhost", "9000"));

var browser = await extra.Use(proxies).LaunchAsync(new LaunchOptions());

var page = await browser.NewPageAsync();

await page.GoToAsync("https://gimly.su/");
```