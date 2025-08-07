using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class WebDriverPlugin : PuppeteerPlugin, IOnPageCreatedPlugin, IBeforeLaunchPlugin {
    public override string Name => nameof(WebDriverPlugin);

    public WebDriverPlugin() : base() { }

    public Task OnPageCreated(IPage page) {
        return page.EvaluateExpressionOnNewDocumentAsync(Scripts.WebDriver);
    }

    public void BeforeLaunch(LaunchOptions options) {
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
