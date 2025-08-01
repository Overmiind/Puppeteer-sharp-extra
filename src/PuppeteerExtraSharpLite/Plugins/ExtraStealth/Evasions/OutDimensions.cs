using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class OutDimensions : PuppeteerExtraPlugin {
    public OutDimensions() : base("stealth-dimensions") { }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.Outdimensions.WithSourceUrl("Outdimensions.js");
        return page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}