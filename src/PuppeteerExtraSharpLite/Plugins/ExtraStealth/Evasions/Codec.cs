using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Codec : PuppeteerExtraPlugin {
    public Codec() : base("stealth-codec") { }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.Codec.WithSourceUrl("Codec.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}