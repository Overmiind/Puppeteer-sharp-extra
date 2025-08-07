namespace PuppeteerExtraSharpLite.Plugins;

public abstract class PuppeteerPlugin {
    protected PuppeteerPlugin() {
        // Requirements ??= [];
    }

    public abstract string Name { get; }

    protected virtual string[] RequiredPlugins => [];

    public ReadOnlyCollection<string> GetDependencies => RequiredPlugins.AsReadOnly();
}
