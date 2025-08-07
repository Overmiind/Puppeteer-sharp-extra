using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class OutDimensionsPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(OutDimensionsPlugin);

    public OutDimensionsPlugin() : base() { }

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.Outdimensions);
    }
}