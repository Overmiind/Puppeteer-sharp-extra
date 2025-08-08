using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins;

/// <summary>
/// Implement to modify launch options prior to launching a browser instance.
/// </summary>
public interface IBeforeLaunchPlugin {
    /// <summary>
    /// Called before the browser is launched, allowing mutation of <paramref name="options"/>.
    /// </summary>
    /// <param name="options">Launch options that will be used to start the browser.</param>
    void BeforeLaunch(LaunchOptions options);
}

/// <summary>
/// Implement to perform actions immediately after a browser instance has launched.
/// </summary>
public interface IAfterLaunchPlugin {
    /// <summary>
    /// Called after the browser has launched.
    /// </summary>
    /// <param name="browser">The launched browser instance.</param>
    void AfterLaunch(IBrowser browser);
}

/// <summary>
/// Implement to modify connect options prior to connecting to an existing browser instance.
/// </summary>
public interface IBeforeConnectPlugin {
    /// <summary>
    /// Called before connecting to a browser, allowing mutation of <paramref name="options"/>.
    /// </summary>
    /// <param name="options">Connection options to be used for establishing the connection.</param>
    void BeforeConnect(ConnectOptions options);
}

/// <summary>
/// Implement to perform actions immediately after establishing a connection to a browser instance.
/// </summary>
public interface IAfterConnectPlugin {
    /// <summary>
    /// Called after connecting to a browser instance.
    /// </summary>
    /// <param name="browser">The connected browser instance.</param>
    void AfterConnect(IBrowser browser);
}

/// <summary>
/// Implement to receive a callback with the browser instance as soon as it is available.
/// </summary>
public interface IOnBrowserPlugin {
    /// <summary>
    /// Called when a browser instance is available to the plugin.
    /// </summary>
    /// <param name="browser">The browser instance.</param>
    void OnBrowser(IBrowser browser);
}

/// <summary>
/// Implement to receive notifications when new targets are created.
/// </summary>
public interface IOnTargetCreatedPlugin {
    /// <summary>
    /// Called when a new target has been created.
    /// </summary>
    /// <param name="target">The newly created target.</param>
    void OnTargetCreated(Target target);
}

/// <summary>
/// Implement to run code when a new page is created.
/// </summary>
public interface IOnPageCreatedPlugin {
    /// <summary>
    /// Called when a new page has been created.
    /// </summary>
    /// <param name="page">The newly created page.</param>
    Task OnPageCreated(IPage page);
}

/// <summary>
/// Implement to act when targets change.
/// </summary>
public interface IOnTargetChangedPlugin {
    /// <summary>
    /// Called when a target has changed.
    /// </summary>
    /// <param name="target">The changed target.</param>
    void OnTargetChanged(Target target);
}

/// <summary>
/// Implement to act when targets are destroyed.
/// </summary>
public interface IOnTargetDestroyedPlugin {
    /// <summary>
    /// Called when a target has been destroyed.
    /// </summary>
    /// <param name="target">The destroyed target.</param>
    void OnTargetDestroyed(Target target);
}

/// <summary>
/// Implement to act when the browser disconnects.
/// </summary>
public interface IOnDisconnectedPlugin {
    /// <summary>
    /// Called when the browser has disconnected.
    /// </summary>
    void OnDisconnected();
}

/// <summary>
/// Implement to act when the browser closes.
/// </summary>
public interface IOnClosePlugin {
    /// <summary>
    /// Called when the browser has closed.
    /// </summary>
    void OnClose();
}
