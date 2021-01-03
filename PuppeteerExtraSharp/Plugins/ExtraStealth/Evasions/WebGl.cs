using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class WebGl : PuppeteerExtraPlugin
    {
        public WebGl() : base("stealth-webGl") { }

        public override async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("WebGL.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }
    }
}
