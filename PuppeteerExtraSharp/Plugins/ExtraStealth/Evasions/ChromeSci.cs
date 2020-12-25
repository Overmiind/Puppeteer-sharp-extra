using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class ChromeSci: PuppeteerExtraPlugin
    {
        public ChromeSci(): base("stealth_sci")
        {
            
        }

        public override Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("SCI.js");
            Utils.EvaluateOnNewPage(page, script);
            return Task.CompletedTask;
        }
    }
}
