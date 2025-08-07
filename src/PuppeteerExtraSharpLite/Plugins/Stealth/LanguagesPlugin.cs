using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class LanguagesPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
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