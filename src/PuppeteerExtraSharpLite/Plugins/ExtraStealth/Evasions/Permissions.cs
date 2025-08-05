using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Permissions : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(Permissions);

    public Permissions() : base() { }

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.Permissions);
    }
}