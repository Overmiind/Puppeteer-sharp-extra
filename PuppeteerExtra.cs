using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins;
using PuppeteerSharp;

namespace PuppeteerExtraSharp
{
    public class PuppeteerExtra
    {
        private Browser _browser;
        private List<IPuppeteerExtraPlugin> _plugins = new List<IPuppeteerExtraPlugin>();

        public PuppeteerExtra Use(IPuppeteerExtraPlugin plugin)
        {
            ResolveDependencies(plugin);
            _plugins.Add(plugin);
            plugin.OnPluginRegistered();
            return this;
        }

        public async Task<Browser> LaunchAsync(LaunchOptions options)
        {
            _plugins.ForEach(e=>e.BeforeLaunch(options));
            _browser = await Puppeteer.LaunchAsync(options);
            _plugins.ForEach(e=>e.AfterLaunch(_browser));
            OnStart(new BrowserStartContext()
            {
                StartType = StartType.Launch,
                IsHeadless = options.Headless
            });
            return _browser;
        }

        public async Task<Browser> ConnectAsync(ConnectOptions options)
        {
            _plugins.ForEach(e=>e.BeforeConnect(options));
            _browser = await Puppeteer.ConnectAsync(options);
            _plugins.ForEach(e=>e.AfterConnect(_browser));
            OnStart(new BrowserStartContext()
            {
                StartType = StartType.Connect 
            });
            return _browser;
        }

        public T GetPlugin<T>() where T: IPuppeteerExtraPlugin
        {
            return (T)_plugins.FirstOrDefault(e => e.GetType() == typeof(T));
        }

        private void OnStart(BrowserStartContext context)
        {
            OrderPlugins();
            CheckPluginRequirements(context);
            Register(_browser);
        }

        private void ResolveDependencies(IPuppeteerExtraPlugin plugin)
        {
            if (plugin.Dependencies is null || !plugin.Dependencies.Any())
                return;
            var dependencies = plugin.Dependencies.ToList();
            foreach (var puppeteerExtraPlugin in dependencies)
            {
                Use(puppeteerExtraPlugin);

                var plugDependencies = puppeteerExtraPlugin.Dependencies?.ToList();

                if(plugDependencies != null && plugDependencies.Any())
                    plugDependencies.ForEach(ResolveDependencies);
            }
        }

        private void OrderPlugins()
        {
            _plugins = _plugins.OrderBy(e => e.Requirements?.Contains(PluginRequirements.RunLast)).ToList();
        }

        private void CheckPluginRequirements(BrowserStartContext context)
        {
            foreach (var puppeteerExtraPlugin in _plugins)
            {
                if(puppeteerExtraPlugin.Requirements is null)
                    continue;
                foreach (var requirement in puppeteerExtraPlugin.Requirements)
                {
                    if (context.StartType == StartType.Launch && requirement == PluginRequirements.HeadFul &&
                        context.IsHeadless)
                    {
                        throw new NotSupportedException($"Plugin - {puppeteerExtraPlugin.GetName()} is not supported in headless mode");
                    }

                    if (context.StartType == StartType.Connect && requirement == PluginRequirements.Launch)
                    {
                        throw new NotSupportedException($"Plugin - {puppeteerExtraPlugin.GetName()} doesn't support connect");
                    }
                }
            }
        }

        private void Register(Browser browser)
        {
            foreach (var puppeteerExtraPlugin in _plugins)
            {
                browser.TargetChanged += (sender, args) => puppeteerExtraPlugin.OnTargetChanged(args.Target);
                browser.TargetCreated += async (sender, args) =>
                {
                    puppeteerExtraPlugin.OnTargetCreated(args.Target);
                    if (args.Target.Type == TargetType.Page)
                    {
                        var page = await args.Target.PageAsync();
                        puppeteerExtraPlugin.OnPageCreated(page);
                    }
                };
                browser.TargetDestroyed += (sender, args) => puppeteerExtraPlugin.OnTargetDestroyed(args.Target);
                browser.Disconnected += (sender, args) => puppeteerExtraPlugin.OnDisconnected();
                browser.Closed += (sender, args) => puppeteerExtraPlugin.OnClose();
            }
        }
    }
}
