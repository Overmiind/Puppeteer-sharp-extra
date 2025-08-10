namespace PuppeteerSharpToolkit.Plugins;

/// <summary>
/// Base type for all Puppeteer Extra plugins.
/// Provides a common contract for naming and declaring soft dependencies on other plugins.
/// </summary>
public abstract class PuppeteerPlugin {
    /// <summary>
    /// Initializes a new instance of the plugin.
    /// </summary>
    protected PuppeteerPlugin() { }

    /// <summary>
    /// Gets the unique name of the plugin. Used for registration and dependency resolution.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets an optional list of plugin names that must be registered before this plugin.
    /// </summary>
    protected virtual string[] RequiredPlugins => [];

    /// <summary>
    /// Gets a read-only view of the plugin dependencies declared in <see cref="RequiredPlugins"/>.
    /// </summary>
    public ReadOnlyCollection<string> GetDependencies => RequiredPlugins.AsReadOnly();
}
