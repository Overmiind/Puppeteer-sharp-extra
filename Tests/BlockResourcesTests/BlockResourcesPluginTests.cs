using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using PuppeteerExtraSharp.Plugins.BlockResources;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests.BlockResourcesTests
{
    public class BlockResourcesPluginTests : BrowserDefault
    {
        public BlockResourcesPluginTests()
        {

        }

        [Fact]
        public void RuleForResources()
        {
            var plugin = new BlockResourcesPlugin();
            var rule = plugin.AddRule(builder => builder.BlockedResources(ResourceType.Document));
            Assert.NotEmpty(plugin.BlockResources);
            Assert.Contains(ResourceType.Document, plugin.BlockResources[0].ResourceType);
        }


        [Fact]
        public async Task RuleForPage()
        {

            var plugin = new BlockResourcesPlugin();
            var browser = await base.LaunchWithPluginAsync(plugin);
            var actualPage = (await browser.PagesAsync())[0];
            var otherPage = await browser.NewPageAsync();
            var rule = plugin.AddRule(builder => builder.BlockedResources(ResourceType.Document).OnlyForPage(actualPage));
            
            Assert.True(rule.IsPageBlocked(actualPage));
            Assert.False(rule.IsPageBlocked(otherPage));
        }

        [Fact]
        public void RuleForUrl()
        {
            var plugin = new BlockResourcesPlugin();
            var rule = plugin.AddRule(builder => builder.ForUrl("google"));

            Assert.True(rule.IsSiteBlocked("http://google.com"));
            Assert.True(rule.IsSiteBlocked("http://google.kz"));
            Assert.True(rule.IsSiteBlocked("https://googleeee.com"));
        }
    }
}
