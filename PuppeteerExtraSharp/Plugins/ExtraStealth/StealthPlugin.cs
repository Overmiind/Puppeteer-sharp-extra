using System.Collections.Generic;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class StealthPlugin: PuppeteerExtraPlugin
    {
        public StealthPlugin() : base("stealth")
        {
        }
        
        public override ICollection<PuppeteerExtraPlugin> Dependencies { get; set; } = new List<PuppeteerExtraPlugin>()
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
            new PluginEvasion(),
            new StackTrace(),
            new LoadTimes()
        };
    }
}
