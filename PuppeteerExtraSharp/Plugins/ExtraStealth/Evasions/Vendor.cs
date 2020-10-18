using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Vendor : IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-vendor";
        }

        public async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Vendor.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
