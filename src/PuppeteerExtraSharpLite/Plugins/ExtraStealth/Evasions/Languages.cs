using System.Linq;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Languages : PuppeteerExtraPlugin {
    public StealthLanguagesOptions Options { get; }

    public Languages(StealthLanguagesOptions options = null) : base("stealth-language") {
        Options = options ?? new StealthLanguagesOptions("en-US", "en");
    }

    public override Task OnPageCreated(IPage page) {
        var script = Scripts.Language.WithSourceUrl("Language.js");
        return Utils.EvaluateOnNewPage(page, script, Options.Languages);
    }
}

public class StealthLanguagesOptions : IPuppeteerExtraPluginOptions {
    public object[] Languages { get; }

    public StealthLanguagesOptions(params string[] languages) {
        Languages = languages.Cast<object>().ToArray();
    }
}