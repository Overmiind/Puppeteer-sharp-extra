using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class StealthPlugin: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth";
        }

        public StealthPlugin()
        {
            Dependencies = new List<IPuppeteerExtraPlugin>()
            {
                new WebDriver(),
                new ChromeApp(),
                new ChromeSci(),
                new ChromeRuntime(),
                new Frame(),
                new Codec(),
                new Languages(),
                new OutDimensions(),
                new Permissions(),
                new UserAgent(),
                new Vendor(),
                new WebGl(),
                new PluginEvasion()
            };
        }
        

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
