using PuppeteerExtraSharpLite.Utils;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth;

internal static class Utils {
    public static Task EvaluateOnNewPage(IPage page, string script, params object[] args) {
        if (!page.IsClosed)
            return page.EvaluateFunctionOnNewDocumentAsync(script, args);

        return Task.CompletedTask;
    }

    public static string GetScript(string name) {
        var path = $"{typeof(Utils).Namespace}.Scripts.{name}";
        var builder = new StringBuilder(ResourcesReader.ReadFile(path));
        builder.AppendLine();
        builder.Append("//# sourceURL=");
        builder.Append(name);
        return builder.ToString();
    }
}