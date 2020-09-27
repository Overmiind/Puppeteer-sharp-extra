using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class Languages : IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-language";
        }

        public void OnPageCreated(Page page)
        {
            page.EvaluateFunctionOnNewDocumentAsync(
                @"() => {Object.defineProperty(Object.getPrototypeOf(navigator), 'languages', {
        get: () => ['en-US', 'en']
      })}");
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
