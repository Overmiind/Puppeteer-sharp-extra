using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    internal static class Utils
    {
        private static readonly HashSet<int> PreloadedPage = new HashSet<int>();
        private static readonly object Locker = new object();
        public static Task EvaluateOnNewPageWithUtilsScript(Page page, string script, params object[] args)
        {
            lock (Locker)
            {
                var tasks = new List<Task>();

                if (!PreloadedPage.Contains(page.GetHashCode()))
                {
                    var utilsScript = Utils.GetScript("Utils.js");
                    PreloadedPage.Add(page.GetHashCode());
                    tasks.Add(page.EvaluateExpressionOnNewDocumentAsync(utilsScript));
                }

                tasks.Add(page.EvaluateFunctionOnNewDocumentAsync(script, args));

                return Task.WhenAll(tasks);
            }
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
}
