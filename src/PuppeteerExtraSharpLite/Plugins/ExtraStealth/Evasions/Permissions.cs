using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Permissions : PuppeteerExtraPlugin {
    public override string Name => nameof(Permissions);

    public Permissions() : base() { }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.Permissions.WithSourceUrl("Permissions.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}