using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    public class LoadTimes: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-loadTimes";
        }

        public Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("LoadTimes.js");
            Utils.EvaluateOnNewPage(page, script);
            return Task.CompletedTask;
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
