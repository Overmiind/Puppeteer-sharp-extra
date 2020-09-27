using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public static class Utils
    {
        private static readonly HashSet<int> PreloadedPage = new HashSet<int>();
        private static readonly object Locker = new object();
        public static void EvaluateOnNewPage(Page page, string script)
        {
            lock (Locker)
            {
                if (!PreloadedPage.Contains(page.GetHashCode()))
                {
                    page.EvaluateExpressionOnNewDocumentAsync(Resources.Utils);
                    PreloadedPage.Add(page.GetHashCode());
                }

                page.EvaluateFunctionOnNewDocumentAsync(script);
            }
        }
    }
}
