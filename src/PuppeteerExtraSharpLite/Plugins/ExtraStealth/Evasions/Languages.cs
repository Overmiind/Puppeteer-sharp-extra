using System.Linq;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class Languages : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(Languages);

    public StealthLanguagesOptions Options { get; }

    public Languages(StealthLanguagesOptions? options = null) : base() {
        Options = options ?? new StealthLanguagesOptions("en-US", "en");
    }

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateFunctionOnNewDocumentAsync(Scripts.Language, [Options.Languages]);
    }
}

public class StealthLanguagesOptions : IPuppeteerExtraPluginOptions {
    public object[] Languages { get; }

    public StealthLanguagesOptions(params string[] languages) {
        Languages = languages.Cast<object>().ToArray();
    }
}