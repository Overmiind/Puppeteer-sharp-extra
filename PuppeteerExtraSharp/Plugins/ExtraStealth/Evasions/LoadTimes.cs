using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class LoadTimes : PuppeteerExtraPlugin
{
    public LoadTimes() : base("stealth-loadTimes")
    {
    }

    public override Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("LoadTimes.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}
