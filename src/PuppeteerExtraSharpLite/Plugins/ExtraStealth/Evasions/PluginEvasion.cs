using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class PluginEvasion : PuppeteerExtraPlugin {
    public PluginEvasion() : base("stealth-pluginEvasion") {
    }

    public override async Task OnPageCreated(IPage page) {
        var scipt = Scripts.Plugin.WithSourceUrl("Plugin.js");
        await Utils.EvaluateOnNewPage(page, scipt);
    }
}