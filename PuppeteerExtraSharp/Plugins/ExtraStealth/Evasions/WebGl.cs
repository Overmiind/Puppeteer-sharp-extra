using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class WebGl : PuppeteerExtraPlugin
{
    private readonly StealthWebGLOptions _options;

    public WebGl(StealthWebGLOptions options) : base("stealth-webGl")
    {
        _options = options ?? new StealthWebGLOptions("Intel Inc.", "Intel Iris OpenGL Engine");
    }

    public override async Task OnPageCreated(IPage page)
    {
        var script = Utils.GetScript("WebGL.js");
        await page.EvaluateFunctionOnNewDocumentAsync(script, _options.Vendor, _options.Renderer);
    }
}

public class StealthWebGLOptions : IPuppeteerExtraPluginOptions
{
    public string Vendor { get; }
    public string Renderer { get; }

    public StealthWebGLOptions(string vendor, string renderer)
    {
        Vendor = vendor;
        Renderer = renderer;
    }
}
