using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class ChromeSci: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth_sci";
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }

        public Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("SCI.js");
            Utils.EvaluateOnNewPage(page, script);
            return Task.CompletedTask;
        }
    }
}
