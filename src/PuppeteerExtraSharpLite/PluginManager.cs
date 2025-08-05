using PuppeteerExtraSharpLite.Plugins;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite;

public class PluginManager {
    private readonly List<PuppeteerExtraPlugin> _plugins = [];
    private readonly HashSet<string> _registeredPlugins = [];

    public PluginManager Register(PuppeteerExtraPlugin plugin) => Register([plugin]);

    public PluginManager Register(PuppeteerExtraPlugin[] plugins) {
        foreach (var plugin in plugins) {
            string name = plugin.Name;

            if (_registeredPlugins.Contains(name)) {
                continue;
            }

            ReadOnlyCollection<string> dependencies = plugin.GetDependencies;

            if (dependencies.Count > 0) {
                foreach (var dependency in dependencies) {
                    if (!_registeredPlugins.Contains(dependency)) {
                        throw new InvalidOperationException($"{name} requires {dependency} to be registered first.");
                    }
                }
            }

            _registeredPlugins.Add(name);
            _plugins.Add(plugin);
        }
        return this;
    }

    public async Task<IBrowser> LaunchAsync(LaunchOptions options) {
        foreach (var plugin in _plugins) {
            if (plugin is IBeforeLaunchPlugin p) {
                p.BeforeLaunch(options);
            }
        }
        var browser = await Puppeteer.LaunchAsync(options);

        foreach (var plugin in _plugins) {
            if (plugin is IAfterLaunchPlugin p) {
                p.AfterLaunch(browser);
            }
        }

        RegisterPluginEvents(browser);

        return browser;
    }

    public async Task<IBrowser> ConnectAsync(ConnectOptions options) {
        foreach (var plugin in _plugins) {
            if (plugin is IBeforeConnectPlugin p) {
                p.BeforeConnect(options);
            }
        }

        var browser = await Puppeteer.ConnectAsync(options);

        RegisterPluginEvents(browser);

        return browser;
    }

    internal void RegisterPluginEvents(IBrowser browser) {
        foreach (var plugin in _plugins) {
            if (plugin is IAfterConnectPlugin p) {
                p.AfterConnect(browser);
            }
        }

        // TODO: check plugin limitations
        // TODO: Add page created calls (maybe TargetCreating with Type=Page is not enough?)

        foreach (var plugin in _plugins) {
            switch (plugin) {
                case IOnTargetCreatedPlugin p: {
                        browser.TargetCreated += async (sender, args) => {
                            p.OnTargetCreated(args.Target);

                            if (args.Target.Type == TargetType.Page) {
                                var page = await args.Target.PageAsync();

                                foreach (var relatedPlugin in _plugins) {
                                    if (relatedPlugin.Name != plugin.Name && relatedPlugin is IOnPageCreatedPlugin pp) {
                                        await pp.OnPageCreated(page);
                                    }
                                }
                            }
                        };
                        break;
                    }
                case IOnTargetChangedPlugin p: {
                        browser.TargetChanged += (sender, args) => p.OnTargetChanged(args.Target);
                        break;
                    }
                case IOnTargetDestroyedPlugin p: {
                        browser.TargetDestroyed += (sender, args) => p.OnTargetDestroyed(args.Target);
                        break;
                    }
                case IOnDisconnectedPlugin p: {
                        browser.Disconnected += (sender, args) => p.OnDisconnected();
                        break;
                    }
                case IOnClosePlugin p: {
                        browser.Closed += (sender, args) => p.OnClose();
                        break;
                    }
            }
        }
    }

    // private void CheckPluginRequirements(BrowserStartContext context) {
    //     foreach (var puppeteerExtraPlugin in _plugins) {
    //         if (puppeteerExtraPlugin.Requirements is null)
    //             continue;
    //         foreach (var requirement in puppeteerExtraPlugin.Requirements) {
    //             switch (context.StartType) {
    //                 case StartType.Launch when requirement == PluginRequirements.HeadFul && context.IsHeadless:
    //                     throw new NotSupportedException(
    //                         $"Plugin - {puppeteerExtraPlugin.Name} is not supported in headless mode");
    //                 case StartType.Connect when requirement == PluginRequirements.Launch:
    //                     throw new NotSupportedException(
    //                         $"Plugin - {puppeteerExtraPlugin.Name} doesn't support connect");
    //             }
    //         }
    //     }
    // }
}