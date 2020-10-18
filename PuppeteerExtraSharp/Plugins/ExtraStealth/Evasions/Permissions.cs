using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Permissions: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-permissions";
        }

        public Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Permissions.js");
            Utils.EvaluateOnNewPage(page, script);
            return Task.CompletedTask;
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
