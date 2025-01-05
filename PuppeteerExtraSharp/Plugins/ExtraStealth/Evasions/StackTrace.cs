using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class StackTrace : PuppeteerExtraPlugin
{
    public StackTrace() : base("stealth-stackTrace")
    {
    }

    public override async Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("Stacktrace.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}
