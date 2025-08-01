using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeRuntime : PuppeteerExtraPlugin {
    public ChromeRuntime() : base("stealth-runtime") {}
    public override Task OnPageCreated(IPage page) {
        var script = Utils.GetScript("Runtime.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}