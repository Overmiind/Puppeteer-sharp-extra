using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class PluginEvasion : PuppeteerExtraPlugin {
    public override string Name => nameof(PluginEvasion);

    public PluginEvasion() : base() {
    }

    public override async Task OnPageCreated(IPage page) {
        var scipt = Scripts.Plugin.WithSourceUrl("Plugin.js");
        await Utils.EvaluateOnNewPage(page, scipt);
    }
}