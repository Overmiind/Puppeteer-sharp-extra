using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Stealth;

public class WebGlPlugin : PuppeteerPlugin {
    public override string Name => nameof(WebGlPlugin);

    private readonly StealthWebGLOptions _options;

    public WebGlPlugin() : this(new StealthWebGLOptions("Intel Inc.", "Intel Iris OpenGL Engine")) { }

    public WebGlPlugin(StealthWebGLOptions options) {
        _options = options;
    }

    // StealthPlugin injects utils.js
    protected override string[] RequiredPlugins => [nameof(StealthPlugin)];

    public async Task OnPageCreated(IPage page) {
        await page.EvaluateFunctionOnNewDocumentAsync(Scripts.WebGL, _options.Vendor, _options.Renderer);
    }
}

public record StealthWebGLOptions(string Vendor, string Renderer);
