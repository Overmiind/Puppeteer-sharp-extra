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
        public virtual void AfterLaunch(IBrowser browser) { }
        public virtual void BeforeConnect(ConnectOptions options) { }
        public virtual void AfterConnect(IBrowser browser) { }
        public virtual void OnBrowser(IBrowser browser) { }
        public virtual void OnTargetCreated(ITarget target) { }
        public virtual Task OnPageCreated(IPage page) { return Task.CompletedTask; }
        public virtual void OnTargetChanged(ITarget target) { }
        public virtual void OnTargetDestroyed(ITarget target) { }
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
