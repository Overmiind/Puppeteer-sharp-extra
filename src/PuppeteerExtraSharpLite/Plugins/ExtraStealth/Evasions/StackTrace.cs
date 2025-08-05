using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class StackTrace : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(StackTrace);

    public StackTrace() : base() { }

    public Task OnPageCreated(IPage page) {
        var script = Scripts.Stacktrace.WithSourceUrl("Stacktrace.js");
        return page.EvaluateFunctionOnNewDocumentAsync(script);
    }
}