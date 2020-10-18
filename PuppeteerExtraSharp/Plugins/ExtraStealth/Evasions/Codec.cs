using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Codec: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-codec";
        }

        public Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Codec.js");
            Utils.EvaluateOnNewPage(page, script);
            return Task.CompletedTask;
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
