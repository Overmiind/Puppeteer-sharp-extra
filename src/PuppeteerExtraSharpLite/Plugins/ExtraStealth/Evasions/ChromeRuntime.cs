using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeRuntime : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(ChromeRuntime);

    public ChromeRuntime() : base() { }

    public Task OnPageCreated(IPage page) {
        return Utils.EvaluateOnNewPage(page, Scripts.Runtime);
    }
}