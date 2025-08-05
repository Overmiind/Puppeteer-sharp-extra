using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class LoadTimes : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(LoadTimes);

    public LoadTimes() : base() { }

    public Task OnPageCreated(IPage page) {
        var script = Scripts.LoadTimes.WithSourceUrl("LoadTimes.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}