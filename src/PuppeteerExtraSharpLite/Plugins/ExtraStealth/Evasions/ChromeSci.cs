using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeSci : PuppeteerExtraPlugin {
    public override string Name => nameof(ChromeSci);

    public ChromeSci() : base() { }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.SCI.WithSourceUrl("SCI.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}