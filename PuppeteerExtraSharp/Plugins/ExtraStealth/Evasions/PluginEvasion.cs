using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class PluginEvasion: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-pluginEvasion";
        }

        public async Task OnPageCreated(Page page)
        {
            var scipt = Utils.GetScript("Plugin.js");
            await page.EvaluateFunctionOnNewDocumentAsync(scipt);
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
