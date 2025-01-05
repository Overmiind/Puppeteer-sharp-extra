using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class WebDriver : PuppeteerExtraPlugin
{
    public WebDriver() : base("stealth-webDriver")
    {
    }

    public override async Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("WebDriver.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script);
    }

    public override void BeforeLaunch(LaunchOptions options)
    {
        var args = options.Args.ToList();
        var idx = args.FindIndex(e => e.StartsWith("--disable-blink-features="));
        if (idx != -1)
        {
            var arg = args[idx];
            args[idx] = $"{arg}, AutomationControlled";
            return;
        }

        args.Add("--disable-blink-features=AutomationControlled");

        options.Args = args.ToArray();
    }
}
