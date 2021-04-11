using System.Collections.Generic;
using System.Linq;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using PuppeteerSharp;
using Frame = PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions.Frame;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class StealthPlugin : PuppeteerExtraPlugin
    {
        private readonly IPuppeteerExtraPluginOptions[] _options;

        public StealthPlugin(params IPuppeteerExtraPluginOptions[] options) : base("stealth")
        {
            _options = options;
        }

        public override ICollection<PuppeteerExtraPlugin> GetDependencies() => new List<PuppeteerExtraPlugin>()
        {
            new WebDriver(),
            new ChromeApp(),
            new ChromeSci(),
            new ChromeRuntime(),
            new Frame(),
            new Codec(),
            new Languages(GetOptionByType<StealthLanguagesOptions>()),
            new OutDimensions(),
            new Permissions(),
            new UserAgent(),
            new Vendor(GetOptionByType<StealthVendorSettings>()),
            new WebGl(GetOptionByType<StealthWebGLOptions>()),
            new PluginEvasion(),
            new StackTrace(),
            new HardwareConcurrency(GetOptionByType<StealthHardwareConcurrencyOptions>()),
            //new ContentWindow(),
            new SourceUrl()
        };
        

        private T GetOptionByType<T>() where T : IPuppeteerExtraPluginOptions
        {
            return _options.OfType<T>().FirstOrDefault();
        }
    }
}