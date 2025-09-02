using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class Languages(StealthLanguagesOptions options = null) : PuppeteerExtraPlugin("stealth-language")
{
    public StealthLanguagesOptions Options { get; } = options ?? new StealthLanguagesOptions("en-US", "en");

    protected internal override Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("Language.js");
        return StealthUtils.EvaluateOnNewPage(page, script, Options.Languages);
    }
}

public class StealthLanguagesOptions(params string[] languages) : IPuppeteerExtraPluginOptions
{
    public object[] Languages { get; } = languages.Cast<object>().ToArray();
}