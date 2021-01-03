using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class WebDriver : PuppeteerExtraPlugin
    {
        public WebDriver() : base("stealth-webDriver") { }

        public override async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("WebDriver.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }
    }
}
