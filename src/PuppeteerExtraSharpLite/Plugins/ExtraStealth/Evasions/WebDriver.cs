using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class WebDriver : PuppeteerExtraPlugin {
    public WebDriver() : base("stealth-webDriver") { }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.WebDriver.WithSourceUrl("WebDriver.js");
        return page.EvaluateFunctionOnNewDocumentAsync(script);
    }

    public override void BeforeLaunch(LaunchOptions options) {
        var args = options.Args;
        var idx = -1;
        for (var i = 0; i < args.Length; i++) {
            if (args[i].StartsWith("--disable-blink-features=")) {
                idx = i;
                break;
            }
        }
        if (idx != -1) {
            var arg = args[idx];
            args[idx] = $"{arg}, AutomationControlled";
            return;
        }

        string[] temp = new string[args.Length + 1];
        Array.Copy(args, temp, args.Length);
        temp[^1] = "--disable-blink-features=AutomationControlled";

        options.Args = temp;
    }
}
