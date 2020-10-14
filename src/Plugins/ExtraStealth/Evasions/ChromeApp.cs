using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    internal class ChromeApp: IPuppeteerExtraPlugin
    {
        public string GetName() => "stealth-chromeApp";
        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }

        public void OnPageCreated(Page page)
        {
            var str = $@"() => {Resources.ChromeApp}";
            Utils.EvaluateOnNewPage(page, str);
        }
    }
}
