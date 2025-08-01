## Block resoures plugin

Blocks page resources in Puppeteer requests (img, documents etc.)

## Usage 

### Add rule
```c#
var plugin = new BlockResourcesPlugin();
var extra = new PuppeteerExtra().Use(plugin);
var browser = await extra.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();

// creates rule for resources blocking
plugin.AddRule(builder => builder.BlockedResources(ResourceType.Image));
```
### Block resources by type 

```c# 
// blocks images for all pages 
plugin.AddRule(builder => builder.BlockedResources(ResourceType.Image));
```

### Block resources by page 

```c#
// blocks images for current page 
plugin.AddRule(builder => builder.BlockedResources(ResourceType.Image).OnlyForPage(page));
```

### Block resources by request url 

```c#
// blocks images when request url equals pattern
plugin.AddRule(builder => builder.BlockedResources(ResourceType.Image).ForUrl("msn"));
```
