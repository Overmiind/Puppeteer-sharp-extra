using System.Buffers;
using System.Text.RegularExpressions;

using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Plugins;

/// <summary>
/// A plugin that blocks page resources
/// </summary>
public class BlockResourcesPlugin : PuppeteerPlugin, IBeforeLaunchPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(BlockResourcesPlugin);

    private readonly List<BlockRule> _blockResources;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Returns a readonly collection of the registered rules
    /// </summary>
    public ReadOnlyCollection<BlockRule> Rules => _blockResources.AsReadOnly();

    /// <summary>
    /// Initializes the plugin with no rules
    /// </summary>
    public BlockResourcesPlugin() : this(ReadOnlySpan<BlockRule>.Empty) { }

    /// <summary>
    /// Initializes the plugin with <paramref name="blockRules"/>
    /// </summary>
    /// <param name="blockRules"></param>
    public BlockResourcesPlugin(params ReadOnlySpan<BlockRule> blockRules) {
        _blockResources = [];
        foreach (var rule in blockRules) {
            AddRule(rule);
        }
    }

    /// <summary>
    /// Add a rule to the registered rules
    /// </summary>
    /// <param name="rule"></param>
    /// <returns></returns>
    public BlockRule AddRule(BlockRule rule) {
        try {
            _semaphore.Wait();
            _blockResources.Add(rule);
        } finally {
            _semaphore.Release();
        }
        return rule;
    }

    /// <summary>
    /// Removes rules from the registered rules
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public BlockResourcesPlugin RemoveRules(Func<BlockRule, bool> predicate) {
        try {
            _semaphore.Wait();
            for (int i = _blockResources.Count - 1; i >= 0; i--) {
                if (predicate(_blockResources[i])) {
                    _blockResources.RemoveAt(i);
                }
            }
        } finally {
            _semaphore.Release();
        }
        return this;
    }

    /// <inheritdoc />
    public Task BeforeLaunch(LaunchOptions options) {
        options.Args = [.. options.Args, "--site-per-process", "--disable-features=IsolateOrigins"];
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await page.SetRequestInterceptionAsync(true).ConfigureAwait(false);
            page.Request += async (sender, args) => {
                if (sender is not IPage p) {
                    await args.Request.ContinueAsync().ConfigureAwait(false);
                    return;
                }

                int length = 0;
                BlockRule[] array = [];
                try {
                    await _semaphore.WaitAsync();
                    length = _blockResources.Count;
                    array = ArrayPool<BlockRule>.Shared.Rent(length);
                    _blockResources.CopyTo(array, 0);
                } finally {
                    _semaphore.Release();
                }
                try {
                    if (array.Length >= length) {
                        for (int i = 0; i < length; i++) {
                            var rule = array[i];
                            if (rule.IsRequestBlocked(p, args.Request)) {
                                await args.Request.AbortAsync().ConfigureAwait(false);
                                return;
                            }
                        }
                    }
                } finally {
                    ArrayPool<BlockRule>.Shared.Return(array, true);
                }

                await args.Request.ContinueAsync().ConfigureAwait(false);
            };
        }
    }
}

/// <summary>
/// Describes a rule to block a resource of specific pages and site patterns
/// </summary>
public partial class BlockRule {
    /// <summary>
    /// The regex to match the site url.
    /// </summary>
    public Regex SitePattern { get; set; } = EmptyRegex();

    /// <summary>
    /// The <see cref="IPage"/> instance to apply to.
    /// </summary>
    public IPage? Page { get; set; }

    /// <summary>
    /// The <see cref="ResourceType"/> to block.
    /// </summary>
    public ResourceType ResourceType { get; set; } = ResourceType.Unknown;

    /// <summary>
    /// Returns true if this request is blocked according to this rule
    /// </summary>
    /// <param name="fromPage"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public bool IsRequestBlocked(IPage fromPage, IRequest request) {
        if (!IsResourcesBlocked(request.ResourceType))
            return false;

        return IsSiteBlocked(request.Url) || IsPageBlocked(fromPage);
    }

    /// <summary>
    /// Returns true if the rule applies to this <see cref="IPage"/> instance.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public bool IsPageBlocked(IPage page) {
        return Page != null && page.Equals(Page);
    }

    /// <summary>
    /// Returns true if the regex pattern of this rule applies to <paramref name="siteUrl"/>
    /// </summary>
    /// <param name="siteUrl"></param>
    /// <returns></returns>
    public bool IsSiteBlocked(string siteUrl) => SitePattern.IsMatch(siteUrl);

    /// <summary>
    /// Returns true if this rule blocks <paramref name="resource"/>
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    public bool IsResourcesBlocked(ResourceType resource) => ResourceType == resource;

    [GeneratedRegex("")]
    private static partial Regex EmptyRegex();
}
