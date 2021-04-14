using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    internal static class Utils
    {
        private static readonly HashSet<int> PreloadedPage = new();
        private static readonly SemaphoreSlim Semaphore = new(1, 1);
        private static readonly string Script;

        static Utils()
        {
            Script = GetScript("Utils.js");
        }

        public static async Task EvaluateOnNewPageWithUtilsScript(Page page, string script, params object[] args)
        {
            await Semaphore.WaitAsync();
            if (!PreloadedPage.Contains(page.GetHashCode()))
            {
                PreloadedPage.Add(page.GetHashCode());
                await page.EvaluateExpressionOnNewDocumentAsync(Script);
            }
            Semaphore.Release();
            await page.EvaluateFunctionOnNewDocumentAsync(script, args);
         
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