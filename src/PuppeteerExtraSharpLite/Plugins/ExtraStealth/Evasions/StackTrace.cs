using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class StackTrace : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(StackTrace);

    public StackTrace() : base() { }

    public Task OnPageCreated(IPage page) {
        return page.EvaluateFunctionOnNewDocumentAsync(Scripts.Stacktrace);
    }
}