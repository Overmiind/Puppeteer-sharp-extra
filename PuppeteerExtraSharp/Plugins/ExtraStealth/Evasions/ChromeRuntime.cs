using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class ChromeRuntime: PuppeteerExtraPlugin
    {
        public ChromeRuntime(): base("stealth-runtime") { }
        
        public override Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Runtime.js");
            return Utils.EvaluateOnNewPageWithUtilsScript(page, script);
        }
    }
}
