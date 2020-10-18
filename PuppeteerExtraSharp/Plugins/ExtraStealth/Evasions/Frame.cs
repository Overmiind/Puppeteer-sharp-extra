using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Frame: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-iframe";
        }

        public Frame()
        {
            Requirements = new List<PluginRequirements>()
            {
                PluginRequirements.RunLast
            };
        }


        public async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Frame.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
