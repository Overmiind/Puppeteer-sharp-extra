using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Configures navigator.languages to a specified set of language codes.
/// </summary>
public class LanguagesPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(LanguagesPlugin);

    private readonly string[] _languages;

    /// <summary>
    /// Gets the languages configured for this plugin.
    /// </summary>
    public ReadOnlyCollection<string> Languages => _languages.AsReadOnly();

    public LanguagesPlugin() : this("en-US", "en") {}

    public LanguagesPlugin(params string[] languages) {
        _languages = languages;
    }

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <inheritdoc />
    public async Task OnPageCreated(IPage page) {
        await page.EvaluateFunctionOnNewDocumentAsync(Scripts.Language, _languages).ConfigureAwait(false);
    }
}
