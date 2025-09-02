using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class StackTrace() : PuppeteerExtraPlugin("stealth-stackTrace")
{
    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("Stacktrace.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}