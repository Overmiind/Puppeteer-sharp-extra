using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class ChromeRuntime: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-runtime";
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }


        public Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Runtime.js");
            Utils.EvaluateOnNewPage(page, script);
            return Task.CompletedTask;
        }
    }
}
