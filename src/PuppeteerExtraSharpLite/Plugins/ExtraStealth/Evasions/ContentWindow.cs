using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ContentWindow : PuppeteerExtraPlugin {
    public override string Name => nameof(ContentWindow);

    public ContentWindow() : base() { }

    public override List<PluginRequirements> Requirements { get; set; } = new()
    {
            PluginRequirements.RunLast
        };

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.ContentWindow.WithSourceUrl("ContentWindow.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}