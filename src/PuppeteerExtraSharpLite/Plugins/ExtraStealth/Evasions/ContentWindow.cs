using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ContentWindow : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(ContentWindow);

    public ContentWindow() : base() { }

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.ContentWindow);
    }
}