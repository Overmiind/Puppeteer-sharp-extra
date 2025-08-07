using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins;

public interface IBeforeLaunchPlugin {
    void BeforeLaunch(LaunchOptions options);
}

public interface IAfterLaunchPlugin {
    void AfterLaunch(IBrowser browser);
}

public interface IBeforeConnectPlugin {
    void BeforeConnect(ConnectOptions options);
}

public interface IAfterConnectPlugin {
    void AfterConnect(IBrowser browser);
}

public interface IOnBrowserPlugin {
    void OnBrowser(IBrowser browser);
}

public interface IOnTargetCreatedPlugin {
    void OnTargetCreated(Target target);
}

public interface IOnPageCreatedPlugin {
    Task OnPageCreated(IPage page);
}

public interface IOnTargetChangedPlugin {
    void OnTargetChanged(Target target);
}

public interface IOnTargetDestroyedPlugin {
    void OnTargetDestroyed(Target target);
}

public interface IOnDisconnectedPlugin {
    void OnDisconnected();
}

public interface IOnClosePlugin {
    void OnClose();
}