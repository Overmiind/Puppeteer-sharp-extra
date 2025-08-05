using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Codec : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(Codec);

    public Codec() : base() { }

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.Codec);
    }
}