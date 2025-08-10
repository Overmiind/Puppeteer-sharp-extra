using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Disables the AutomationControlled blink feature and patches navigator.webdriver.
/// </summary>
public class WebDriverPlugin : PuppeteerPlugin, IOnPageCreatedPlugin, IBeforeLaunchPlugin {
    /// <inheritdoc />
    public override string Name => nameof(WebDriverPlugin);

    /// <inheritdoc />
    public async Task OnPageCreated(IPage page) {
        await page.EvaluateExpressionOnNewDocumentAsync(Scripts.WebDriver).ConfigureAwait(false);
    }

    /// <inheritdoc />
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
