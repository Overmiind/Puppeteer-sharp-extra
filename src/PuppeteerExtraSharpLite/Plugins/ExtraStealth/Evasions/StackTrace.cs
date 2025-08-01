using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class StackTrace : PuppeteerExtraPlugin {
    public StackTrace() : base("stealth-stackTrace") { }

    public override Task OnPageCreated(IPage page) {
        var script = Utils.GetScript("Stacktrace.js");
        return page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}