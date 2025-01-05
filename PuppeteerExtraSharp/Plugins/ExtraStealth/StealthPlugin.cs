using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth;

public class StealthPlugin : PuppeteerExtraPlugin
{
    private readonly IPuppeteerExtraPluginOptions[] _options;
    private readonly List<PuppeteerExtraPlugin> _standardEvasions;

    public StealthPlugin(params IPuppeteerExtraPluginOptions[] options) : base("stealth")
    {
        _options = options;
        _standardEvasions = GetStandardEvasions();
    }

    private List<PuppeteerExtraPlugin> GetStandardEvasions()
    {
        return
        [
            new WebDriver(),
            new ChromeApp(),
            new ChromeSci(),
            new Codec(),
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
        ];
    }

    public override ICollection<PuppeteerExtraPlugin> GetDependencies() => _standardEvasions;

    private T GetOptionByType<T>()
        where T : IPuppeteerExtraPluginOptions
        => _options.OfType<T>().FirstOrDefault();

    public void RemoveEvasionByType<T>()
        where T : PuppeteerExtraPlugin
        => _standardEvasions.RemoveAll(ev => ev is T);
}
