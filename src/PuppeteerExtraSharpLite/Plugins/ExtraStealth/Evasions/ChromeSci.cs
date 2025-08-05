using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeSci : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(ChromeSci);

    public ChromeSci() : base() { }

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.SCI);
    }
}