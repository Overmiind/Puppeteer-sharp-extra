using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeSci : PuppeteerExtraPlugin {
    public ChromeSci() : base("stealth_sci") { }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.SCI.WithSourceUrl("SCI.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}