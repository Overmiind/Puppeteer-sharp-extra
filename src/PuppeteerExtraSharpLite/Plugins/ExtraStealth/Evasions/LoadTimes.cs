using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class LoadTimes : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(LoadTimes);

    public LoadTimes() : base() { }

    public Task OnPageCreated(IPage page) {
        return Utils.EvaluateOnNewPage(page, Scripts.LoadTimes);
    }
}