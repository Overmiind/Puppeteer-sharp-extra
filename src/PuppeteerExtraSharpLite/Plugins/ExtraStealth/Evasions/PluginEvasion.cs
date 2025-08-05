using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class PluginEvasion : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(PluginEvasion);

    public PluginEvasion() : base() {
    }

    public async Task OnPageCreated(IPage page) {
        var script = Scripts.Plugin.WithSourceUrl("Plugin.js");
        await Utils.EvaluateOnNewPage(page, script);
    }
}