using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class OutDimensions() : PuppeteerExtraPlugin("stealth-dimensions")
{
    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("Outdimensions.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}