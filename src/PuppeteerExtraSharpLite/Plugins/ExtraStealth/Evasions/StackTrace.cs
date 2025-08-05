using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class StackTrace : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(StackTrace);

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.Stacktrace);
    }
}