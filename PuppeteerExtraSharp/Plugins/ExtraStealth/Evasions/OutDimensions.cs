using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class OutDimensions: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-dimensions";
        }

        public async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Outdimensions.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }


        public void BeforeLaunch(LaunchOptions options)
        {
            options.DefaultViewport = null;
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
