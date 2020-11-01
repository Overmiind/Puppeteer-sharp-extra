using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins
{
    public interface IPuppeteerExtraPlugin
    {
        public string GetName();
        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
        public void BeforeLaunch(LaunchOptions options) { }
        public void AfterLaunch(Browser browser) { }
        public void BeforeConnect(ConnectOptions options) { }
        public void AfterConnect(Browser browser) { }
        public void OnBrowser(Browser browser) { }
        public void OnTargetCreated(Target target) { }
        public Task OnPageCreated(Page page) { return Task.CompletedTask; }
        public void OnTargetChanged(Target target) { }
        public void OnTargetDestroyed(Target target) { }
        public void OnDisconnected() { }
        public void OnClose() { }
        public void OnPluginRegistered() { }
    }


    public enum PluginRequirements
    {
        Launch,
        HeadFul,
        DataFromPlugin,
        RunLast
    }


}
