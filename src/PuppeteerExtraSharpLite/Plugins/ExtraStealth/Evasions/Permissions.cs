using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Permissions : PuppeteerExtraPlugin {
    public Permissions() : base("stealth-permissions") { }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.Permissions.WithSourceUrl("Permissions.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}