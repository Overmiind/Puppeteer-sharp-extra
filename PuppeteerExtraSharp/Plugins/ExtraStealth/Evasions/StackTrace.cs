using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class StackTrace: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-stackTrace";
        }

        public async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Stacktrace.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
