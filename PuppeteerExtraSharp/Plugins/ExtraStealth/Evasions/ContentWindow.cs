using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class ContentWindow : PuppeteerExtraPlugin
{
    public ContentWindow() : base("Iframe.ContentWindow")
    {
    }

    public override IReadOnlyCollection<PluginRequirements> Requirements { get; } =
    [
        PluginRequirements.RunLast
    ];

    public override Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("ContentWindow.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}
