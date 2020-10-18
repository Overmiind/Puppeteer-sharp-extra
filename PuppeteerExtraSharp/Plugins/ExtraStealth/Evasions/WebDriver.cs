using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class WebDriver: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-webDriver";
        }
        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }

        public async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("WebDriver.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }
    }
}
