using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class PluginEvasion() : PuppeteerExtraPlugin("stealth-pluginEvasion")
{
    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        var scipt = StealthUtils.GetScript("Plugin.js");
        await StealthUtils.EvaluateOnNewPage(page, scipt);
    }
}