using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.BlockResources;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests.BlockResourcesTests;

public class BlockResourcesPluginTests : BrowserDefault
{
    [Fact]
    public void ShouldAddsToListOfRules()
    {
        var plugin = new BlockResourcesPlugin();
        var rule = plugin.AddRule(builder => builder.BlockedResources(ResourceType.Document));

        Assert.NotEmpty(plugin.BlockResources);
        Assert.Contains(ResourceType.Document, plugin.BlockResources[0].ResourceType);
    }

    [Fact]
    public void RuleForResource()
    {
        var plugin = new BlockResourcesPlugin();
        var rule = plugin.AddRule(builder => builder.BlockedResources(ResourceType.Document));
        Assert.True(rule.IsResourcesBlocked(ResourceType.Document));
    }

    [Fact]
    public async Task RuleForPage()
    {
        var plugin = new BlockResourcesPlugin();
        var browser = await LaunchWithPluginAsync(plugin);
        var actualPage = (await browser.PagesAsync())[0];
        var otherPage = await browser.NewPageAsync();
        var rule = plugin.AddRule(builder
            => builder.BlockedResources(ResourceType.Document).OnlyForPage(actualPage));

        Assert.True(rule.IsPageBlocked(actualPage));
        Assert.False(rule.IsPageBlocked(otherPage));
    }

    [InlineData("http://google.com")]
    [InlineData("http://google.kz")]
    [InlineData("https://googleeee.com")]
    [Theory]
    public void RuleForUrl(string site)
    {
        var plugin = new BlockResourcesPlugin();
        var rule = plugin.AddRule(builder => builder.ForUrl("google"));

        Assert.True(rule.IsSiteBlocked(site));
    }

    [Fact]
    public void ShouldRemoveRule()
    {
        var plugin = new BlockResourcesPlugin();

        var actualRule = plugin.AddRule(builder => builder.BlockedResources(ResourceType.Font));
        var otherRule = plugin.AddRule(builder => builder.BlockedResources(ResourceType.Document));

        plugin.RemoveRule(actualRule);

        Assert.DoesNotContain(actualRule, plugin.BlockResources);
        Assert.Contains(otherRule, plugin.BlockResources);
    }
}
