using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins;
using PuppeteerSharp;


namespace PuppeteerExtraSharp
{
    public class PuppeteerExtra
    {
        private List<PuppeteerExtraPlugin> _plugins = new List<PuppeteerExtraPlugin>();

        public PuppeteerExtra Use(PuppeteerExtraPlugin plugin)
        {
            ResolveDependencies(plugin);
            _plugins.Add(plugin);
            plugin.OnPluginRegistered();
            return this;
        }

        public async Task<Browser> LaunchAsync(LaunchOptions options)
        {
            _plugins.ForEach(e=>e.BeforeLaunch(options));
            var browser = await Puppeteer.LaunchAsync(options);
            _plugins.ForEach(e=>e.AfterLaunch(browser));
            await OnStart(new BrowserStartContext()
            {
                StartType = StartType.Launch,
                IsHeadless = options.Headless
            }, browser);
            return browser;
        }

        public async Task<Browser> ConnectAsync(ConnectOptions options)
        {
            _plugins.ForEach(e=>e.BeforeConnect(options));
            var browser = await Puppeteer.ConnectAsync(options);
            _plugins.ForEach(e=>e.AfterConnect(browser));
            await OnStart(new BrowserStartContext()
            {
                StartType = StartType.Connect 
            }, browser);
            return browser;
        }

        public T GetPlugin<T>() where T: PuppeteerExtraPlugin
        {
            return (T)_plugins.FirstOrDefault(e => e.GetType() == typeof(T));
        }

        private async Task OnStart(BrowserStartContext context, Browser browser)
        {
            OrderPlugins();
            CheckPluginRequirements(context);
            await Register(browser);
        }

        private void ResolveDependencies(PuppeteerExtraPlugin plugin)
        {
            var dependencies = plugin.GetDependencies()?.ToList();
            if (dependencies is null || !dependencies.Any())
                return;
           
            foreach (var puppeteerExtraPlugin in dependencies)
            {
                Use(puppeteerExtraPlugin);

                var plugDependencies = puppeteerExtraPlugin.GetDependencies()?.ToList();

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
                    switch (context.StartType)
                    {
                        case StartType.Launch when requirement == PluginRequirements.HeadFul && context.IsHeadless:
                            throw new NotSupportedException($"Plugin - {puppeteerExtraPlugin.Name} is not supported in headless mode");
                        case StartType.Connect when requirement == PluginRequirements.Launch:
                            throw new NotSupportedException($"Plugin - {puppeteerExtraPlugin.Name} doesn't support connect");
                    }
                }
            }
        }

        private async Task Register(Browser browser)
        {
            var pages = await browser.PagesAsync();
            foreach (var puppeteerExtraPlugin in _plugins)
            {
                browser.TargetChanged += (sender, args) => puppeteerExtraPlugin.OnTargetChanged(args.Target);
                browser.TargetCreated += async (sender, args) =>
                {
                    puppeteerExtraPlugin.OnTargetCreated(args.Target);
                    if (args.Target.Type == TargetType.Page)
                    {
                        var page = await args.Target.PageAsync();
                        await puppeteerExtraPlugin.OnPageCreated(page);
                    }
                };
                browser.TargetDestroyed += (sender, args) => puppeteerExtraPlugin.OnTargetDestroyed(args.Target);
                browser.Disconnected += (sender, args) => puppeteerExtraPlugin.OnDisconnected();
                browser.Closed += (sender, args) => puppeteerExtraPlugin.OnClose();

                foreach (var page in pages)
                {
                    await puppeteerExtraPlugin.OnPageCreated(page);
                }
            }
        }
    }
}
