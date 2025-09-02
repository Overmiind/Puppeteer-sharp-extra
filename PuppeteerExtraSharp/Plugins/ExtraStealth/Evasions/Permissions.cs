using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class Permissions() : PuppeteerExtraPlugin("stealth-permissions")
{
    protected internal override Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("Permissions.js");
        return StealthUtils.EvaluateOnNewPage(page, script);
    }
}