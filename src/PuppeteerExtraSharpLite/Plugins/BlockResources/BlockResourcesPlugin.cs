using System.Linq;
using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.BlockResources;

public class BlockResourcesPlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin, IBeforeLaunchPlugin {
    public override string Name => nameof(BlockResourcesPlugin);

    public readonly List<BlockRule> BlockResources = new();

    public BlockResourcesPlugin(IEnumerable<ResourceType>? blockResources = null) : base() {
        if (blockResources != null)
            AddRule(builder => builder.BlockedResources(blockResources.ToArray()));
    }

    public BlockRule AddRule(Action<ResourcesBlockBuilder> builderAction) {
        var builder = new ResourcesBlockBuilder();
        builderAction(builder);

        var rule = builder.Build();
        BlockResources.Add(builder.Build());

        return rule;
    }

    public BlockResourcesPlugin RemoveRule(BlockRule rule) {
        BlockResources.Remove(rule);
        return this;
    }


    public async Task OnPageCreated(IPage page) {
        await page.SetRequestInterceptionAsync(true);
        page.Request += (sender, args) => OnPageRequest(page, args);
    }


    private async void OnPageRequest(IPage sender, RequestEventArgs e) {
        if (BlockResources.Any(rule => rule.IsRequestBlocked(sender, e.Request))) {
            await e.Request.AbortAsync();
            return;
        }

        await e.Request.ContinueAsync();
    }


    public void BeforeLaunch(LaunchOptions options) {
        options.Args = options.Args.Append("--site-per-process").Append("--disable-features=IsolateOrigins").ToArray();
    }
}
