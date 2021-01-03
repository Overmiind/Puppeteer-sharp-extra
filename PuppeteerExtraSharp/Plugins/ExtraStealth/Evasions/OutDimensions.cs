using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class OutDimensions : PuppeteerExtraPlugin
    {
        public OutDimensions() : base("stealth-dimensions") { }

        public override async Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Outdimensions.js");
            await page.EvaluateFunctionOnNewDocumentAsync(script);
        }

        public override void BeforeLaunch(LaunchOptions options)
        {
            options.DefaultViewport = null;
        }
    }
}
