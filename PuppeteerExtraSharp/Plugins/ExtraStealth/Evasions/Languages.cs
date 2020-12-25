using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Languages : PuppeteerExtraPlugin
    {
        public Languages() : base("stealth-language")
        {
        }

        public override async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Language.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }
    }
}
