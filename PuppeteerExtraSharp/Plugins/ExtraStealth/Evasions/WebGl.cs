using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class WebGl(StealthWebGLOptions options) : PuppeteerExtraPlugin("stealth-webGl")
{
    private readonly StealthWebGLOptions _options =
        options ?? new StealthWebGLOptions("Intel Inc.", "Intel Iris OpenGL Engine");

    protected internal override async Task OnPageCreatedAsync(IPage page)
    {
        var script = StealthUtils.GetScript("WebGL.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script, _options.Vendor, _options.Renderer);
    }
}

public class StealthWebGLOptions(string vendor, string renderer) : IPuppeteerExtraPluginOptions
{
    public string Vendor { get; } = vendor;
    public string Renderer { get; } = renderer;
}