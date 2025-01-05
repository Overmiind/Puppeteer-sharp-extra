using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources;

public class BlockResourcesPlugin : PuppeteerExtraPlugin
{
    public readonly List<BlockRule> BlockResources = [];

    public BlockResourcesPlugin(IEnumerable<ResourceType> blockResources = null)
        : base("block-resources")
    {
        if (blockResources != null)
            AddRule(builder => builder.BlockedResources(blockResources.ToArray()));
    }

    public BlockRule AddRule(Action<ResourcesBlockBuilder> builderAction)
    {
        var builder = new ResourcesBlockBuilder();
        builderAction(builder);

        var rule = builder.Build();
        BlockResources.Add(builder.Build());

        return rule;
    }

    public BlockResourcesPlugin RemoveRule(BlockRule rule)
    {
        BlockResources.Remove(rule);
        return this;
    }

    public override async Task OnPageCreated(IPage page)
    {
        await page.SetRequestInterceptionAsync(true);
        page.Request += (sender, args) => OnPageRequest(page, args);
    }

    private async void OnPageRequest(IPage sender, RequestEventArgs e)
    {
        if (BlockResources.Any(rule => rule.IsRequestBlocked(sender, e.Request)))
        {
            await e.Request.AbortAsync();
            return;
        }

        await e.Request.ContinueAsync();
    }

    public override void BeforeLaunch(LaunchOptions options)
    {
        options.Args = options.Args
            .Append("--site-per-process")
            .Append("--disable-features=IsolateOrigins")
            .Distinct()
            .ToArray();
    }
}
