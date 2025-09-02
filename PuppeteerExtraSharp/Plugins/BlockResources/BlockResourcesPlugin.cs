using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.BlockResources.Rules;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources;

public class BlockResourcesPlugin(
    RequestAbortErrorCode? abortErrorCode = null,
    int? priority = null) : PuppeteerExtraPlugin("block-resources")
{
    private readonly List<IBlockRule> _blockRules = [];

    public IBlockRule AddRule(Action<ResourcesBlockBuilder> builderAction)
    {
        var builder = new ResourcesBlockBuilder();
        builderAction(builder);

        var rule = builder.Build();
        _blockRules.Add(rule);
        return rule;
    }

    public IBlockRule AddRule(IBlockRule rule)
    {
        _blockRules.Add(rule);
        return rule;
    }

    public BlockResourcesPlugin RemoveRule(IBlockRule rule)
    {
        _blockRules.Remove(rule);
        return this;
    }


    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        await page.SetRequestInterceptionAsync(true);
        page.AddRequestInterceptor(OnPageRequestAsync);
    }


    private async Task OnPageRequestAsync(IRequest req)
    {
        if (_blockRules.Any(rule => rule.ShouldBlock(req)))
        {
            await req.AbortAsync(abortErrorCode ?? RequestAbortErrorCode.BlockedByClient, priority);
            return;
        }

        await req.ContinueAsync();
    }


    protected internal override Task BeforeLaunchAsync(LaunchOptions options)
    {
        options.Args = options.Args.Append("--site-per-process").Append("--disable-features=IsolateOrigins")
            .ToArray();
        
        return Task.CompletedTask;
    }
}