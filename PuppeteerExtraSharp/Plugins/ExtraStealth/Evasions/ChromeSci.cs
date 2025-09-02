using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class ChromeSci() : PuppeteerExtraPlugin("stealth_sci")
{
    protected internal override Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("SCI.js");
        return StealthUtils.EvaluateOnNewPage(page, script);
    }
}