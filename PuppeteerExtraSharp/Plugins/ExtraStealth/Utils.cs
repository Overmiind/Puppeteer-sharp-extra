using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    internal static class Utils
    {
        private static readonly HashSet<int> PreloadedPage = new HashSet<int>();
        private static readonly object Locker = new object();
        public static void EvaluateOnNewPage(Page page, string script)
        {
            lock (Locker)
            {
                if (!PreloadedPage.Contains(page.GetHashCode()))
                {
                    var utilsScript = Utils.GetScript("Utils.js");
                    page.EvaluateExpressionOnNewDocumentAsync(utilsScript);
                    PreloadedPage.Add(page.GetHashCode());
                }

                page.EvaluateFunctionOnNewDocumentAsync(script);
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
