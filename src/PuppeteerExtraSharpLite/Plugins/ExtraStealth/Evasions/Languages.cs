using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;

public class LanguagesPlugin : PuppeteerExtraPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(LanguagesPlugin);

    private readonly string[] _languages;

    public ReadOnlyCollection<string> Languages => _languages.AsReadOnly();

    public LanguagesPlugin() : this("en-US", "en") {}

    public LanguagesPlugin(params string[] languages) {
        _languages = languages;
    }

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public Task OnPageCreated(IPage page) {
        return page.EvaluateFunctionOnNewDocumentAsync(Scripts.Language, [_languages]);
    }
}