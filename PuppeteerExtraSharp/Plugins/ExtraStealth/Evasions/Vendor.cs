using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Vendor : PuppeteerExtraPlugin
    {
        public Vendor() : base("stealth-vendor") { }

        public override async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Vendor.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }
    }
}
