using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class WebDriver() : PuppeteerExtraPlugin("stealth-webDriver")
{
    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("WebDriver.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script);
    }

    protected internal override Task BeforeLaunchAsync(LaunchOptions options)
    {
        var args = options.Args.ToList();
        var idx = args.FindIndex(e => e.StartsWith("--disable-blink-features="));
        if (idx != -1)
        {
            var arg = args[idx];
            args[idx] = $"{arg}, AutomationControlled";
            return Task.CompletedTask;
        }

        args.Add("--disable-blink-features=AutomationControlled");

        options.Args = args.ToArray();
        
        return Task.CompletedTask;
    }
}