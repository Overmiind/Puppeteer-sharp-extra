using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class Codec: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-codec";
        }

        public void OnPageCreated(Page page)
        {
            Utils.EvaluateOnNewPage(page, Resources.Codec);
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
