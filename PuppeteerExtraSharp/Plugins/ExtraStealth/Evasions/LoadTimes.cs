using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class LoadTimes() : PuppeteerExtraPlugin("stealth-loadTimes")
{
    protected internal override Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("LoadTimes.js");
        return StealthUtils.EvaluateOnNewPage(page, script);
    }
}