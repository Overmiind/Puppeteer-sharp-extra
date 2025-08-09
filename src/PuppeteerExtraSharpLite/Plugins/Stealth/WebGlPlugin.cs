using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

/// <summary>
/// Mocks WebGL vendor and renderer strings to align with typical hardware configurations.
/// </summary>
public class WebGlPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    /// <inheritdoc />
    public override string Name => nameof(WebGlPlugin);

    private readonly StealthWebGLOptions _options;

    public WebGlPlugin() : this(new StealthWebGLOptions("Intel Inc.", "Intel Iris OpenGL Engine")) { }

    public WebGlPlugin(StealthWebGLOptions options) {
        _options = options;
    }

    // StealthPlugin injects utils.js
    /// <inheritdoc />
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    /// <inheritdoc />
    public async Task OnPageCreated(IPage page) {
        await page.EvaluateFunctionOnNewDocumentAsync(Scripts.WebGL, _options.Vendor, _options.Renderer);
    }
}

/// <summary>
/// Options controlling the WebGL vendor and renderer values used by <see cref="WebGlPlugin"/>.
/// </summary>
public record StealthWebGLOptions(string Vendor, string Renderer);
