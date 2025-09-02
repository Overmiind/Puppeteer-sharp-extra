using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins;

public abstract class PuppeteerExtraPlugin(string pluginName)
{
    public string Name { get; } = pluginName;

    public virtual List<PluginRequirements> Requirements { get; } = new();

    protected internal virtual ICollection<PuppeteerExtraPlugin> GetDependencies()
    {
        return null;
    }

    protected internal virtual Task BeforeLaunchAsync(LaunchOptions options)
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task AfterLaunchAsync(IBrowser browser)
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task BeforeConnectAsync(ConnectOptions options)
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task AfterConnectAsync(IBrowser browser)
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task OnBrowserAsync(IBrowser browser)
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task OnTargetCreatedAsync(Target target)
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task OnPageCreatedAsync(IPage page)
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task OnTargetChangedAsync(Target target)
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task OnTargetDestroyedAsync(Target target)
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task OnDisconnectedAsync()
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task OnCloseAsync()
    {
        return Task.CompletedTask;
    }

    protected internal virtual Task OnPluginRegisteredAsync()
    {
        return Task.CompletedTask;
    }
}

public enum PluginRequirements
{
    Launch,
    HeadFul,
    RunLast
}