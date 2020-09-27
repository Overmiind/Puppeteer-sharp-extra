using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class ChromeSci: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth_sci";
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }

        public void OnPageCreated(Page page)
        {
            Utils.EvaluateOnNewPage(page, Resources.SCI);
        }
    }
}
