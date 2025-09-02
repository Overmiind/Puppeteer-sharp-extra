using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class ChromeRuntime() : PuppeteerExtraPlugin("stealth-runtime")
{
    protected internal override Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("Runtime.js");
        return StealthUtils.EvaluateOnNewPage(page, script);
    }
}