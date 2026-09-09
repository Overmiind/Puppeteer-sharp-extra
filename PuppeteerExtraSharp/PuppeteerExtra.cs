using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins;
using PuppeteerSharp;
using ErrorEventArgs = System.IO.ErrorEventArgs;


namespace PuppeteerExtraSharp;

public class PuppeteerExtra
{
    private List<PuppeteerExtraPlugin> _plugins = [];
    private readonly ConditionalWeakTable<IPage, Lazy<Task>> _pageInitializations = new();

    /// <summary>Reports failures in automatic target and page initialization.</summary>
    public event EventHandler<ErrorEventArgs> PluginError;

    /// <summary>
    /// Waits for this page's plugins before navigation. Repeated calls share the same initialization.
    /// Cancellation stops waiting; close the page or its context to interrupt browser work.
    /// </summary>
    public Task PreparePageAsync(IPage page, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(page);
        cancellationToken.ThrowIfCancellationRequested();
        return _pageInitializations.GetValue(page, value => new Lazy<Task>(() => InitializePageAsync(value)))
            .Value.WaitAsync(cancellationToken);
    }

    private async Task InitializePageAsync(IPage page)
    {
        foreach (var plugin in _plugins)
            await plugin.OnPageCreatedAsync(page);
    }

    public PuppeteerExtra Use(PuppeteerExtraPlugin plugin)
    {
        _plugins.Add(plugin);
        ResolveDependencies(plugin);
        plugin.OnPluginRegisteredAsync();
        return this;
    }

    public Task<IBrowser> LaunchAsync()
    {
        return LaunchAsync(new LaunchOptions());
    }

    public async Task<IBrowser> LaunchAsync(LaunchOptions options)
    {
        _plugins.ForEach(e => e.BeforeLaunchAsync(options));
        var browser = await Puppeteer.LaunchAsync(options);
        _plugins.ForEach(e => e.AfterLaunchAsync(browser));
        await OnStartAsync(new BrowserStartContext
        {
            StartType = StartType.Launch,
            IsHeadless = options.Headless
        }, browser);
        return browser;
    }

    public async Task<IBrowser> ConnectAsync(ConnectOptions options)
    {
        _plugins.ForEach(e => e.BeforeConnectAsync(options));
        var browser = await Puppeteer.ConnectAsync(options);
        _plugins.ForEach(e => e.AfterConnectAsync(browser));
        await OnStartAsync(new BrowserStartContext
        {
            StartType = StartType.Connect
        }, browser);
        return browser;
    }

    public T GetPlugin<T>() where T : PuppeteerExtraPlugin
    {
        return (T)_plugins.FirstOrDefault(e => e.GetType() == typeof(T));
    }

    private async Task OnStartAsync(BrowserStartContext context, IBrowser browser)
    {
        OrderPlugins();
        CheckPluginRequirements(context);
        await RegisterAsync(browser);
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

            if (plugDependencies != null && plugDependencies.Any())
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
            if (puppeteerExtraPlugin.Requirements is null)
                continue;

            foreach (var requirement in puppeteerExtraPlugin.Requirements)
                switch (context.StartType)
                {
                    case StartType.Launch when requirement == PluginRequirements.HeadFul && context.IsHeadless:
                        throw new NotSupportedException(
                            $"Plugin - {puppeteerExtraPlugin.Name} is not supported in headless mode");
                    case StartType.Connect when requirement == PluginRequirements.Launch:
                        throw new NotSupportedException(
                            $"Plugin - {puppeteerExtraPlugin.Name} doesn't support connect");
                }
        }
    }

    private async Task RegisterAsync(IBrowser browser)
    {
        var pages = await browser.PagesAsync();

        browser.TargetCreated += (_, args) => _ = HandleTargetCreatedAsync(args.Target);

        foreach (var puppeteerExtraPlugin in _plugins)
        {
            browser.TargetChanged += (sender, args) => puppeteerExtraPlugin.OnTargetChangedAsync(args.Target);
            browser.TargetDestroyed += (sender, args) => puppeteerExtraPlugin.OnTargetDestroyedAsync(args.Target);
            browser.Disconnected += (sender, args) => puppeteerExtraPlugin.OnDisconnectedAsync();
            browser.Closed += (sender, args) => puppeteerExtraPlugin.OnCloseAsync();
        }

        foreach (var page in pages) await PreparePageAsync(page);
    }

    private async Task HandleTargetCreatedAsync(Target target)
    {
        try
        {
            foreach (var plugin in _plugins) await plugin.OnTargetCreatedAsync(target);
            if (target.Type == TargetType.Page)
            {
                var page = await target.PageAsync();
                if (page != null) await PreparePageAsync(page);
            }
        }
        catch (TargetClosedException)
        {
            // A page can close while its plugins are still initializing.
        }
        catch (Exception exception)
        {
            PluginError?.Invoke(this, new ErrorEventArgs(exception));
        }
    }
}
