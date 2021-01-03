using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class PluginEvasion: PuppeteerExtraPlugin
    {
        public PluginEvasion():base("stealth-pluginEvasion") { }

        public override async Task OnPageCreated(Page page)
        {
            var scipt = Utils.GetScript("Plugin.js");
            await page.EvaluateFunctionOnNewDocumentAsync(scipt);
        }
    }
}
