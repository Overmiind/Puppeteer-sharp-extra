using System.Collections.Generic;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Permissions: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-permissions";
        }

        public void OnPageCreated(Page page)
        {
            Utils.EvaluateOnNewPage(page, Resources.Permissions);
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
