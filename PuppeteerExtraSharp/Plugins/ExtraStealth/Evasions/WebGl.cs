using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class WebGl : IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-webGl";
        }

        public async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("WebGL.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
