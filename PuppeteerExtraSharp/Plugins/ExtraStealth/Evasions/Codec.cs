using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class Codec() : PuppeteerExtraPlugin("stealth-codec")
{
    protected internal override Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("Codec.js");
        return StealthUtils.EvaluateOnNewPage(page, script);
    }
}