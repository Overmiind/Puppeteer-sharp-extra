using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins;

public class BlockResourcesPlugin : PuppeteerPlugin, IOnPageCreatedPlugin, IBeforeLaunchPlugin {
    public override string Name => nameof(BlockResourcesPlugin);

    private readonly List<BlockRule> _blockResources;

    public ReadOnlyCollection<BlockRule> Rules => _blockResources.AsReadOnly();

    public BlockResourcesPlugin() : this(ReadOnlySpan<BlockRule>.Empty) { }

    public BlockResourcesPlugin(params ReadOnlySpan<BlockRule> blockRules) {
        _blockResources = [];
        foreach (var rule in blockRules) {
            AddRule(rule);
        }
    }

    public BlockRule AddRule(BlockRule rule) {
        _blockResources.Add(rule);
        return rule;
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public BlockResourcesPlugin RemoveRules(Func<BlockRule, bool> predicate) {
        int i = 0;
        while (i < _blockResources.Count) {
            if (predicate(_blockResources[i])) {
                _blockResources.RemoveAt(i);
            }

            i++;
        }
        return this;
    }


    public async Task OnPageCreated(IPage page) {
        await page.SetRequestInterceptionAsync(true).ConfigureAwait(false);
        page.Request += async (sender, args) => {
            if (sender is not IPage p) {
                await args.Request.ContinueAsync().ConfigureAwait(false);
                return;
            }

            foreach (var rule in _blockResources) {
                if (rule.IsRequestBlocked(p, args.Request)) {
                    await args.Request.AbortAsync().ConfigureAwait(false);
                    return;
                }
            }

            await args.Request.ContinueAsync().ConfigureAwait(false);
        };
    }

    public void BeforeLaunch(LaunchOptions options) {
        options.Args = [.. options.Args, "--site-per-process", "--disable-features=IsolateOrigins"];
    }
}

public partial class BlockRule {
    public Regex SitePattern { get; set; } = EmptyRegex();
    public IPage? Page { get; set; }
    public ResourceType ResourceType { get; set; } = ResourceType.Unknown;

    public bool IsRequestBlocked(IPage fromPage, IRequest request) {
        if (!IsResourcesBlocked(request.ResourceType))
            return false;

        return IsSiteBlocked(request.Url) || IsPageBlocked(fromPage);
    }

    public bool IsPageBlocked(IPage page) {
        return Page != null && page.Equals(Page);
    }

    public bool IsSiteBlocked(string siteUrl) => SitePattern.IsMatch(siteUrl);

    public bool IsResourcesBlocked(ResourceType resource) => ResourceType == resource;

    [GeneratedRegex("")]
    private static partial Regex EmptyRegex();
}
