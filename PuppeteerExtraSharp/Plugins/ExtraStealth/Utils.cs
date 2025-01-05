using System.Text;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth;

internal static class Utils
{
    public static Task EvaluateOnNewPage(IPage page, string script, params object[] args)
    {
        if (!page.IsClosed)
            return page.EvaluateFunctionOnNewDocumentAsync(script, args);

        return Task.CompletedTask;
    }

    public static string GetScript(string name)
    {
        var builder = new StringBuilder(typeof(Utils).Namespace);
        builder.Append(".Scripts");
        builder.Append("." + name);

        var file = ResourcesReader.ReadFile(builder.ToString());
        return file;
    }
}
