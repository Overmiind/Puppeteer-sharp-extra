using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class Codec : PuppeteerExtraPlugin
    {
        public Codec() : base("stealth-codec") { }

        public override Task OnPageCreated(Page page)
        {
            var script = Utils.GetScript("Codec.js");
            return Utils.EvaluateOnNewPage(page, script);
        }
    }
}
