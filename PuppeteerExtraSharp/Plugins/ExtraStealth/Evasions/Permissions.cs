using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Permissions: PuppeteerExtraPlugin
    {
        public Permissions() : base("stealth-permissions")
        {
        }
 
        public override Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Permissions.js");
            Utils.EvaluateOnNewPage(page, script);
            return Task.CompletedTask;
        }
    }
}
