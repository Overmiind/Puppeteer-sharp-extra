using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Configures navigator.languages to a specified set of language codes.
/// </summary>
public class LanguagesPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(LanguagesPlugin);

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    private readonly string[] _languages;

    /// <summary>
    /// Gets the languages configured for this plugin.
    /// </summary>
    public ReadOnlyCollection<string> Languages => _languages.AsReadOnly();

    public LanguagesPlugin() : this("en-US", "en") { }

    public LanguagesPlugin(params string[] languages) {
        _languages = languages;
    }

   /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await page.EvaluateFunctionOnNewDocumentAsync(Scripts.Language, _languages).ConfigureAwait(false);
        }
    }
}
