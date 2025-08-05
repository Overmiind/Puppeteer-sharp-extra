using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins;

public abstract class PuppeteerExtraPlugin {
    protected PuppeteerExtraPlugin() {
        // Requirements ??= [];
    }

    public abstract string Name { get; }

    protected virtual string[] RequiredPlugins => [];

    public ReadOnlyCollection<string> GetDeps => RequiredPlugins.AsReadOnly();

    // public virtual List<PluginRequirements> Requirements { get; set; }

    // public virtual ICollection<PuppeteerExtraPlugin> GetDependencies() {
    //     return Array.Empty<PuppeteerExtraPlugin>();
    // }

    // public virtual void BeforeLaunch(LaunchOptions options) { }
    // public virtual void AfterLaunch(IBrowser browser) { }
    // public virtual void BeforeConnect(ConnectOptions options) { }
    // public virtual void AfterConnect(IBrowser browser) { }
    // public virtual void OnBrowser(IBrowser browser) { }
    // public virtual void OnTargetCreated(Target target) { }
    // public virtual Task OnPageCreated(IPage page) { return Task.CompletedTask; }
    // public virtual void OnTargetChanged(Target target) { }
    // public virtual void OnTargetDestroyed(Target target) { }
    // public virtual void OnDisconnected() { }
    // public virtual void OnClose() { }
    public virtual void OnPluginRegistered() { }
}
