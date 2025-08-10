using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Plugins;

/// <summary>
/// Implement to modify launch options prior to launching a browser instance.
/// </summary>
public interface IBeforeLaunchPlugin {
    /// <summary>
    /// Called before the browser is launched, allowing mutation of <paramref name="options"/>.
    /// </summary>
    /// <param name="options">Launch options that will be used to start the browser.</param>
    Task BeforeLaunch(LaunchOptions options);
}

/// <summary>
/// Implement to perform actions immediately after a browser instance has launched.
/// </summary>
public interface IAfterLaunchPlugin {
    /// <summary>
    /// Called after the browser has launched.
    /// </summary>
    /// <param name="browser">The launched browser instance.</param>
    Task AfterLaunch(IBrowser browser);
}

/// <summary>
/// Implement to modify connect options prior to connecting to an existing browser instance.
/// </summary>
public interface IBeforeConnectPlugin {
    /// <summary>
    /// Called before connecting to a browser, allowing mutation of <paramref name="options"/>.
    /// </summary>
    /// <param name="options">Connection options to be used for establishing the connection.</param>
    Task BeforeConnect(ConnectOptions options);
}

/// <summary>
/// Implement to perform actions immediately after establishing a connection to a browser instance.
/// </summary>
public interface IAfterConnectPlugin {
    /// <summary>
    /// Called after connecting to a browser instance.
    /// </summary>
    /// <param name="browser">The connected browser instance.</param>
    Task AfterConnect(IBrowser browser);
}

/// <summary>
/// Implement to receive a callback with the browser instance as soon as it is available.
/// </summary>
public interface IOnBrowserPlugin {
    /// <summary>
    /// Called when a browser instance is available to the plugin.
    /// </summary>
    /// <param name="browser">The browser instance.</param>
    Task OnBrowser(IBrowser browser);
}

/// <summary>
/// Implement to receive notifications when new targets are created.
/// </summary>
public interface IOnTargetCreatedPlugin {
    /// <summary>
    /// Called when a new target has been created.
    /// </summary>
    /// <param name="target">The newly created target.</param>
    Task OnTargetCreated(Target target);
}

/// <summary>
/// Implement to act when targets change.
/// </summary>
public interface IOnTargetChangedPlugin {
    /// <summary>
    /// Called when a target has changed.
    /// </summary>
    /// <param name="target">The changed target.</param>
    Task OnTargetChanged(Target target);
}

/// <summary>
/// Implement to act when targets are destroyed.
/// </summary>
public interface IOnTargetDestroyedPlugin {
    /// <summary>
    /// Called when a target has been destroyed.
    /// </summary>
    /// <param name="target">The destroyed target.</param>
    Task OnTargetDestroyed(Target target);
}

/// <summary>
/// Implement to act when the browser disconnects.
/// </summary>
public interface IOnDisconnectedPlugin {
    /// <summary>
    /// Called when the browser has disconnected.
    /// </summary>
    Task OnDisconnected();
}

/// <summary>
/// Implement to act when the browser closes.
/// </summary>
public interface IOnClosePlugin {
    /// <summary>
    /// Called when the browser has closed.
    /// </summary>
    Task OnClose();
}
