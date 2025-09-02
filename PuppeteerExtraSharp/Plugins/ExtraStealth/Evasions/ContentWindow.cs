using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class ContentWindow() : PuppeteerExtraPlugin("Iframe.ContentWindow")
{
    public override List<PluginRequirements> Requirements { get; } = [PluginRequirements.RunLast];

    protected internal override Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("ContentWindow.js");
        return StealthUtils.EvaluateOnNewPage(page, script);
    }
}