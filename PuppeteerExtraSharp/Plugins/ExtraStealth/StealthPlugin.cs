using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using PuppeteerSharp;

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
            // new ChromeApp(),
            new ChromeSci(),
            new ChromeRuntime(),
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
            new ContentWindow(),
            new SourceUrl()
        };

        public override async Task OnPageCreated(Page page)
        {
            var utilsScript = Utils.GetScript("Utils.js");
            await page.EvaluateExpressionOnNewDocumentAsync(utilsScript);
        }

        private T GetOptionByType<T>() where T : IPuppeteerExtraPluginOptions
        {
            return _options.OfType<T>().FirstOrDefault();
        }
    }
}