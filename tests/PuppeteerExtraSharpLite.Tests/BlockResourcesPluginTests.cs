using PuppeteerExtraSharpLite.Plugins;
using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests;

public class BlockResourcesPluginTests {
    [Fact]
    public void BlockResources_Plugin_Should_AddToListOfRules() {
        var plugin = new BlockResourcesPlugin();
        _ = plugin.AddRule(new BlockRule() {
            ResourceType = ResourceType.Document
        });
        Assert.NotEmpty(plugin.Rules);
        Assert.Equal(ResourceType.Document, plugin.Rules[0].ResourceType);
    }

    [Fact]
    public void BlockResources_Plugin_RuleForResource() {
        var plugin = new BlockResourcesPlugin();
        var rule = plugin.AddRule(new BlockRule() {
            ResourceType = ResourceType.Document
        });
        Assert.True(rule.IsResourcesBlocked(ResourceType.Document));
    }


    [Fact]
    public async Task BlockResources_Plugin_RuleForPage() {
        var plugin = new BlockResourcesPlugin();

        var pluginManager = new PluginManager();
        pluginManager.Register(plugin);

        await using var browser = await pluginManager.LaunchAsync();
        using var actualPage = await browser.NewPageAsync();
        using var otherPage = await browser.NewPageAsync();

        var rule = plugin.AddRule(new BlockRule() {
            ResourceType = ResourceType.Document,
            IPage = actualPage
        });

        Assert.True(rule.IsPageBlocked(actualPage));
        Assert.False(rule.IsPageBlocked(otherPage));
    }

    [InlineData("http://google.com")]
    [InlineData("http://google.kz")]
    [InlineData("https://googleeee.com")]
    [Theory]
    public void BlockedResources_Plugin_RuleForUrl(string site) {
        var plugin = new BlockResourcesPlugin();
        var rule = plugin.AddRule(new BlockRule() {
            SitePattern = new Regex("google")
        });

        Assert.True(rule.IsSiteBlocked(site));
    }

    [Fact]
    public void BlockedResources_Plugin_Should_RemoveRule() {
        var plugin = new BlockResourcesPlugin();

        var actualRule = plugin.AddRule(new BlockRule() {
            ResourceType = ResourceType.Font
        });
        var otherRule = plugin.AddRule(new BlockRule() {
            ResourceType = ResourceType.Document
        });

        plugin.RemoveRules(r => r.ResourceType == ResourceType.Font);

        Assert.DoesNotContain(actualRule, plugin.Rules);
        Assert.Contains(otherRule, plugin.Rules);
    }
}