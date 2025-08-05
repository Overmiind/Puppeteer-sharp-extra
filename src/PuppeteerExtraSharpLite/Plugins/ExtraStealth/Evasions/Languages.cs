using System.Linq;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Languages : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(Languages);

    public StealthLanguagesOptions Options { get; }

    public Languages(StealthLanguagesOptions? options = null) : base() {
        Options = options ?? new StealthLanguagesOptions("en-US", "en");
    }

    public Task OnPageCreated(IPage page) {
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