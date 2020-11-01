using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PuppeteerSharp;
[assembly: InternalsVisibleTo("Extra.Tests")]
namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class ChromeApp : IPuppeteerExtraPlugin
    {
        public string GetName() => "stealth-chromeApp";
        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }

        public Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("ChromeApp.js");
            Utils.EvaluateOnNewPage(page, script);
            return Task.CompletedTask;
        }
    }
}
