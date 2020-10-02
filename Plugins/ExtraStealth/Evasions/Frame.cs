using System.Collections.Generic;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Frame: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-iframe";
        }

        public Frame()
        {
            Requirements = new List<PluginRequirements>()
            {
                PluginRequirements.RunLast
            };
        }


        public void OnPageCreated(Page page)
        {
            page.EvaluateFunctionOnNewDocumentAsync(Resources.Frame);
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
