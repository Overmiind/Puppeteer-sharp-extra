using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth;

internal static class Utils {
    public static Task EvaluateOnNewPage(IPage page, ReadOnlySpan<char> script, params object[] args) {
        if (!page.IsClosed) {
            return page.EvaluateFunctionOnNewDocumentAsync(script.ToString(), args);
        }

        return Task.CompletedTask;
    }

    public static string WithSourceUrl(this ReadOnlySpan<char> script, string name) {
        return $"{script}\n//# sourceURL={name}";
    }
}
