using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins
{
    public abstract class PuppeteerExtraPlugin
    {
        protected PuppeteerExtraPlugin(string pluginName)
        {
            Name = pluginName;
        }

        public string Name { get; }

        public virtual List<PluginRequirements> Requirements { get; set; }

        public virtual ICollection<PuppeteerExtraPlugin> GetDependencies()
        {
            return null;
        }

        public virtual void BeforeLaunch(LaunchOptions options) { }
        public virtual void AfterLaunch(Browser browser) { }
        public virtual void BeforeConnect(ConnectOptions options) { }
        public virtual void AfterConnect(Browser browser) { }
        public virtual void OnBrowser(Browser browser) { }
        public virtual void OnTargetCreated(Target target) { }
        public virtual Task OnPageCreated(Page page) { return Task.CompletedTask; }
        public virtual void OnTargetChanged(Target target) { }
        public virtual void OnTargetDestroyed(Target target) { }
        public virtual void OnDisconnected() { }
        public virtual void OnClose() { }
        public virtual void OnPluginRegistered() { }
    }


    public enum PluginRequirements
    {
        Launch,
        HeadFul,
        DataFromPlugin,
        RunLast
    }
}
