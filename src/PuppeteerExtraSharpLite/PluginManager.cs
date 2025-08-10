using PuppeteerExtraSharpLite.Plugins;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite;

public class PluginManager {
    private readonly List<PuppeteerPlugin> _plugins = [];
    private readonly HashSet<string> _registeredPlugins = [];

    public PluginManager Register(PuppeteerPlugin plugin) => Register([plugin]);

    public PluginManager Register(PuppeteerPlugin[] plugins) {
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
                await p.BeforeLaunch(options).ConfigureAwait(false);
            }
        }
        var browser = await Puppeteer.LaunchAsync(options).ConfigureAwait(false);

        foreach (var plugin in _plugins) {
            if (plugin is IAfterLaunchPlugin p) {
                await p.AfterLaunch(browser).ConfigureAwait(false);
            }
        }

        RegisterPluginEvents(browser);

        return browser;
    }

    public async Task<IBrowser> ConnectAsync(ConnectOptions options) {
        foreach (var plugin in _plugins) {
            if (plugin is IBeforeConnectPlugin p) {
                await p.BeforeConnect(options).ConfigureAwait(false);
            }
        }

        var browser = await Puppeteer.ConnectAsync(options).ConfigureAwait(false);

        foreach (var plugin in _plugins) {
            if (plugin is IAfterConnectPlugin p) {
                await p.AfterConnect(browser).ConfigureAwait(false);
            }
        }

        RegisterPluginEvents(browser);

        return browser;
    }

    internal void RegisterPluginEvents(IBrowser browser) {
        foreach (var plugin in _plugins) {
            // if (plugin is IOnPageCreatedPlugin onPageCreated) {
            //     browser.TargetCreated += async (sender, args) => {
            //         if (args.Target.Type == TargetType.Page) {
            //             var page = await args.Target.PageAsync().ConfigureAwait(false);
            //             await onPageCreated.OnPageCreated(page).ConfigureAwait(false);
            //         }
            //     };
            if (plugin is IOnTargetCreatedPlugin onTargetCreated) {
                browser.TargetCreated += async (sender, args) => await onTargetCreated.OnTargetCreated(args.Target);
            }
            if (plugin is IOnTargetChangedPlugin onTargetChanged) {
                browser.TargetChanged += async (sender, args) => await onTargetChanged.OnTargetChanged(args.Target);
            }
            if (plugin is IOnTargetDestroyedPlugin onTargetDestroyed) {
                browser.TargetDestroyed += async (sender, args) => await onTargetDestroyed.OnTargetDestroyed(args.Target);
            }
            if (plugin is IOnDisconnectedPlugin onDisconnected) {
                browser.Disconnected += async (sender, args) => await onDisconnected.OnDisconnected();
            }
            if (plugin is IOnClosePlugin onClose) {
                browser.Closed += async (sender, args) => await onClose.OnClose();
            }
        }
    }
}