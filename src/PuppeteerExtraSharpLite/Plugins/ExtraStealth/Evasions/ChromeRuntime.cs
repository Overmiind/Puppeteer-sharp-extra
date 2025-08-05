using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeRuntime : PuppeteerExtraPlugin {
    public override string Name => nameof(ChromeRuntime);

    public ChromeRuntime() : base() { }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.Runtime.WithSourceUrl("Runtime.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}