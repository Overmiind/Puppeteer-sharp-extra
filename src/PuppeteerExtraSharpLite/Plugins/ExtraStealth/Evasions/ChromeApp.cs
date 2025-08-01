using System.Runtime.CompilerServices;

using PuppeteerSharp;

[assembly: InternalsVisibleTo("Extra.Tests")]
namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class ChromeApp : PuppeteerExtraPlugin {
    public ChromeApp() : base("stealth-chromeApp") { }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.ChromeApp.WithSourceUrl("ChromeApp.js");
        return Utils.EvaluateOnNewPage(page, script);
    }
}