## Block Resources Plugin

Block unwanted network requests (scripts, images, documents, etc.) using simple, composable rules. Useful for speeding up crawls, reducing bandwidth, and cutting out ads/trackers.

#### Features
- Rule-based blocking by:
    - Page instance
    - URL substring
    - Resource type (PuppeteerSharp ResourceType)
    - Custom predicate (IRequest => bool)
- Works with PuppeteerExtra’s plugin pipeline
- Configurable abort error code

#### Quick start

```csharp
// Create the plugin and register it
var blockResourcesPlugin = new BlockResourcesPlugin();
var puppeteerExtra = new PuppeteerExtra();

var browser = await puppeteerExtra.Use(blockResourcesPlugin).LaunchAsync();
var page = await browser.NewPageAsync();

// Add some rules BEFORE navigation for best effect

// 1) Block scripts for a specific page and URL pattern
blockResourcesPlugin.AddRule(s => 
    s.Page(page)
     .Url("ads.js")
     .Resources(ResourceType.Script));

// 2) Block non-navigation requests via a custom predicate
blockResourcesPlugin.AddRule(s => 
    s.Custom(request => !request.IsNavigationRequest));

// 3) Block all scripts on all pages
blockResourcesPlugin.AddRule(s => 
    s.Resources(ResourceType.Script));

// 4) Block all images on a specific page
blockResourcesPlugin.AddRule(s => 
    s.Page(page)
     .Resources(ResourceType.Image));

await page.GoToAsync("https://adblock.turtlecute.org/"); // many/most requests will be blocked
```

#### Rule builder reference
Use AddRule with a builder to combine conditions:

- Page(Page page)
    - Apply the rule only to requests originating from a specific PuppeteerSharp Page.
- Url(string substring)
    - Apply the rule to requests whose URL contains the given substring.
- Resources(ResourceType type)
    - Apply the rule to requests of a given resource type (e.g., Script, Image, Document, etc.).
- Custom(Func<IRequest, bool> predicate)
    - Apply the rule to requests that satisfy a custom predicate.

Notes:
- Chain multiple conditions in a single rule to narrow targeting (e.g., Page + Url + Resources).
- Add multiple rules to cover different scenarios.

#### Customize abort error code
By default, blocked requests are aborted with RequestAbortErrorCode.BlockedByClient. You can override this:

```csharp
var blockResourcesPlugin = new BlockResourcesPlugin(RequestAbortErrorCode.BlockedByClient);
// ... register and use as usual
```

Pick the error code that best fits your use case (see PuppeteerSharp’s RequestAbortErrorCode).

#### Custom rule class
If you need full control, implement IBlockRule:

```csharp
public class MyRule : IBlockRule
{
    public bool ShouldBlock(IRequest request)
    {
        return request.IsNavigationRequest && request.Url.Contains("ads.js");
    }
}

// Register your custom rule
var blockResourcesPlugin = new BlockResourcesPlugin();
blockResourcesPlugin.AddRule(new MyRule());
```

#### Tips
- Add rules before navigating to ensure they apply from the first request.
- Be careful when blocking navigation requests; it can prevent pages from loading.