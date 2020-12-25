using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Frame: PuppeteerExtraPlugin
    {
        public Frame(): base("stealth-iframe")
        {
        }

        public override async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Frame.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }

        public override List<PluginRequirements> Requirements { get; set; } = new List<PluginRequirements>()
        {
            PluginRequirements.RunLast
        };
    }
}
