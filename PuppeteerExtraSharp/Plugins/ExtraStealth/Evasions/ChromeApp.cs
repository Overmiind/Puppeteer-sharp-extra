using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PuppeteerSharp;

[assembly: InternalsVisibleTo("Extra.Tests")]

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class ChromeApp() : PuppeteerExtraPlugin("stealth-chromeApp")
{
    protected internal override Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("ChromeApp.js");
        return StealthUtils.EvaluateOnNewPage(page, script);
    }
}