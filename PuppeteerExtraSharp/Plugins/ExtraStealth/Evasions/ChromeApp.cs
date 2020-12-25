using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class ChromeApp : PuppeteerExtraPlugin
    {
        public ChromeApp(): base("stealth-chromeApp")
        { }

        public override Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("ChromeApp.js");
            Utils.EvaluateOnNewPage(page, script);
            return Task.CompletedTask;
        }
    }
}
