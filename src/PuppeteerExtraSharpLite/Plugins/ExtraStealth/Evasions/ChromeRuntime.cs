using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeRuntime : PuppeteerExtraPlugin {
    public ChromeRuntime() : base("stealth-runtime") {}

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.Runtime.WithSourceUrl("Runtime.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}