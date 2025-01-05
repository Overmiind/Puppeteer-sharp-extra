using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class PluginEvasion : PuppeteerExtraPlugin
{
    public PluginEvasion() : base("stealth-pluginEvasion")
    {
    }

    public override async Task OnPageCreated(IPage page)
    {
        var scipt = Utils.GetScript("Plugin.js");
        await Utils.EvaluateOnNewPage(page, scipt);
    }
}
