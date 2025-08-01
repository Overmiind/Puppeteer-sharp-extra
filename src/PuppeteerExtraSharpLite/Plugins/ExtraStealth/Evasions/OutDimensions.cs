using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class OutDimensions : PuppeteerExtraPlugin {
    public OutDimensions() : base("stealth-dimensions") { }

    public override Task OnPageCreated(IPage page) {
        var script = Utils.GetScript("Outdimensions.js");
        return page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}