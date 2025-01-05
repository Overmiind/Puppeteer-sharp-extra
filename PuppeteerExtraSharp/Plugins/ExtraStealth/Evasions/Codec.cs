using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class Codec : PuppeteerExtraPlugin
{
    public Codec() : base("stealth-codec")
    {
    }

    public override Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("Codec.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}
