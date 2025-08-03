using PuppeteerExtraSharpLite.Plugins;

using PuppeteerSharp;


namespace PuppeteerExtraSharpLite;

public class PuppeteerExtra {
    private readonly List<PuppeteerExtraPlugin> _plugins = new();

    public PuppeteerExtra Use(PuppeteerExtraPlugin plugin) {
        _plugins.Add(plugin);
        ResolveDependencies(plugin);
        plugin.OnPluginRegistered();
        return this;
    }

    public async Task<IBrowser> LaunchAsync(LaunchOptions options) {
        _plugins.ForEach(e => e.BeforeLaunch(options));
        var browser = await Puppeteer.LaunchAsync(options);
        _plugins.ForEach(e => e.AfterLaunch(browser));
        await OnStart(new BrowserStartContext() {
            StartType = StartType.Launch,
            IsHeadless = options.Headless
        }, browser);
        return browser;
    }

public async Task<IBrowser> ConnectAsync(ConnectOptions options) {
    foreach (var plugin in _plugins) {
        plugin.BeforeConnect(options);
    }

    var browser = await Puppeteer.ConnectAsync(options);

    foreach (var plugin in _plugins) {
        plugin.AfterConnect(browser);
    }

    await OnStart(new BrowserStartContext {
        StartType = StartType.Connect
    }, browser);

    return browser;
}

    public T? GetPlugin<T>() where T : PuppeteerExtraPlugin {
        foreach (var plugin in _plugins) {
            if (plugin is T tPlugin) {
                return tPlugin;
            }
        }
        return default;
    }

    private async Task OnStart(BrowserStartContext context, IBrowser browser) {
        OrderPlugins();
        CheckPluginRequirements(context);
        await Register(browser);
    }

    private void ResolveDependencies(PuppeteerExtraPlugin plugin) {
        var dependencies = plugin.GetDependencies();
        if (dependencies.Count == 0) {
            return;
        }

        foreach (var puppeteerExtraPlugin in dependencies) {
            Use(puppeteerExtraPlugin);

            var pluginDependencies = puppeteerExtraPlugin.GetDependencies();

            foreach (var dependency in pluginDependencies) {
                ResolveDependencies(dependency);
            }
        }
    }

    private void OrderPlugins() {
        _plugins.Sort(static (a, b) => {
            var aHasRunLast = a.Requirements?.Contains(PluginRequirements.RunLast) == true;
            var bHasRunLast = b.Requirements?.Contains(PluginRequirements.RunLast) == true;
            return aHasRunLast.CompareTo(bHasRunLast);
        });
    }

    private void CheckPluginRequirements(BrowserStartContext context) {
        foreach (var puppeteerExtraPlugin in _plugins) {
            if (puppeteerExtraPlugin.Requirements is null)
                continue;
            foreach (var requirement in puppeteerExtraPlugin.Requirements) {
                switch (context.StartType) {
                    case StartType.Launch when requirement == PluginRequirements.HeadFul && context.IsHeadless:
                        throw new NotSupportedException(
                            $"Plugin - {puppeteerExtraPlugin.Name} is not supported in headless mode");
                    case StartType.Connect when requirement == PluginRequirements.Launch:
                        throw new NotSupportedException(
                            $"Plugin - {puppeteerExtraPlugin.Name} doesn't support connect");
                }
            }
        }
    }

    private async Task Register(IBrowser browser) {
        var pages = await browser.PagesAsync();

        browser.TargetCreated += async (sender, args) => await UpdatePluginsOnTargetCreated(args);

        async Task UpdatePluginsOnTargetCreated(TargetChangedArgs args) {
            foreach (var plugin in _plugins) {
                plugin.OnTargetCreated(args.Target);
            }
            if (args.Target.Type == TargetType.Page) {
                var page = await args.Target.PageAsync();
                foreach (var plugin in _plugins) {
                    await plugin.OnPageCreated(page);
                }
            }
        }

        foreach (var puppeteerExtraPlugin in _plugins) {
            browser.TargetChanged += (sender, args) => puppeteerExtraPlugin.OnTargetChanged(args.Target);
            browser.TargetDestroyed += (sender, args) => puppeteerExtraPlugin.OnTargetDestroyed(args.Target);
            browser.Disconnected += (sender, args) => puppeteerExtraPlugin.OnDisconnected();
            browser.Closed += (sender, args) => puppeteerExtraPlugin.OnClose();
            foreach (var page in pages) {
                await puppeteerExtraPlugin.OnPageCreated(page);
            }
        }
    }
}