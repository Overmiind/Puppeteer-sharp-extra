using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    public class ContentWindow : PuppeteerExtraPlugin
    {
        public ContentWindow() : base("Iframe.ContentWindow") { }

        public override List<PluginRequirements> Requirements { get; set; } = new()
        {
            PluginRequirements.RunLast
        };

        public override Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("ContentWindow.js");
            return Utils.EvaluateOnNewPage(page, script);
        }
    }
}