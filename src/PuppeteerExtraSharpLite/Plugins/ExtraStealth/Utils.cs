using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth;

internal static class Utils {
    public static Task EvaluateOnNewPage(IPage page, string script, params object[] args) {
        if (!page.IsClosed) {
            return page.EvaluateFunctionOnNewDocumentAsync(script, args);
        }

        return Task.CompletedTask;
    }
}
