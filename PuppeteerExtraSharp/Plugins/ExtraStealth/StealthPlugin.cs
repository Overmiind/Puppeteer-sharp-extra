using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth;

public class StealthPlugin : PuppeteerExtraPlugin
{
    private readonly List<IPuppeteerExtraPluginOptions> _options;
    private readonly List<PuppeteerExtraPlugin> _standardEvasions;

    public StealthPlugin(params IPuppeteerExtraPluginOptions[] options) : base("stealth")
    {
        _options = options.ToList();
        _standardEvasions = GetStandardEvasions();
    }

    private List<PuppeteerExtraPlugin> GetStandardEvasions()
    {
        return
        [
            new WebDriver(),
            new ChromeSci(),
            new ChromeRuntime(),
            new Codec(),
            new Languages(GetOptionsByType<StealthLanguagesOptions>()),
            new OutDimensions(),
            new Permissions(),
            new UserAgent(),
            new Vendor(GetOptionsByType<StealthVendorSettings>()),
            new WebGl(GetOptionsByType<StealthWebGLOptions>()),
            new PluginEvasion(),
            new StackTrace(),
            new HardwareConcurrency(GetOptionsByType<StealthHardwareConcurrencyOptions>()),
            new ContentWindow(),
            new SourceUrl()
        ];
    }

    protected internal override ICollection<PuppeteerExtraPlugin> GetDependencies()
    {
        return _standardEvasions;
    }

    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        var utilsScript = StealthUtils.GetScript("Utils.js");
        await page.EvaluateExpressionOnNewDocumentAsync(utilsScript);
    }

    public void AddOptions<T>(T options)
        where T: IPuppeteerExtraPluginOptions
    {
        if (_options.OfType<T>().Any())
        {
            throw new ArgumentException("Option already exists", nameof(options));
        }
        
        _options.Add(options);
    }

    private T GetOptionsByType<T>() where T : IPuppeteerExtraPluginOptions
    {
        return _options.OfType<T>().SingleOrDefault();
    }

    public void RemoveEvasion<T>() where T : PuppeteerExtraPlugin
    {
        _standardEvasions.RemoveAll(ev => ev is T);
    }

    public void RemoveEvasion(string name)
    {
        _standardEvasions.RemoveAll(ev => ev.Name == name);
    }
}