using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class StackTracePlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(StackTracePlugin);

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.Stacktrace);
    }
}