using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.AnonymizeUa;

public class AnonymizeUaPlugin : PuppeteerExtraPlugin
{
    public AnonymizeUaPlugin() : base("anonymize-ua")
    {
    }

    private Func<string, string> _customAction;

    public void CustomizeUa(Func<string, string> uaAction)
    {
        _customAction = uaAction;
    }

    public override async Task OnPageCreated(IPage page)
    {
        var ua = await page.Browser.GetUserAgentAsync();
        ua = ua.Replace("HeadlessChrome", "Chrome");

        var regex = new Regex(@"/\(([^)]+)\)/");
        ua = regex.Replace(ua, "(Windows NT 10.0; Win64; x64)");

        if (_customAction != null)
            ua = _customAction(ua);

        await page.SetUserAgentAsync(ua);
    }
}
