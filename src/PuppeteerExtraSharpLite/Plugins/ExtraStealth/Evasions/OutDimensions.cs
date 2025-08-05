using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class OutDimensions : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(OutDimensions);

    public OutDimensions() : base() { }

    public Task OnPageCreated(IPage page) {
        var script = Scripts.Outdimensions.WithSourceUrl("Outdimensions.js");
        return page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}