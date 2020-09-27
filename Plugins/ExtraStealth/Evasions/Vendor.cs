using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class Vendor : IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-vendor";
        }

        public void OnPageCreated(Page page)
        {
            page.EvaluateFunctionOnNewDocumentAsync(@"() => {
      // Overwrite the `vendor` property to use a custom getter.
      Object.defineProperty(Object.getPrototypeOf(navigator), 'vendor', {
        get: () => 'Intel inc.'
      })
    }");
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
