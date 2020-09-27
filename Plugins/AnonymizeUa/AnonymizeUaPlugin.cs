using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.AnonymizeUa
{
    public class AnonymizeUaPlugin: IPuppeteerExtraPlugin
    {

        private Func<string, string> _customAction;
        public void CustomizeUa(Func<string, string> uaAction)
        {
            _customAction = uaAction;
        }
        public string GetName()
        {
            return "anonymize-ua";
        }

        public async void OnPageCreated(Page page)
        {
            var ua = await page.Browser.GetUserAgentAsync();
            ua = ua.Replace("HeadlessChrome", "Chrome");

            var regex = new Regex(@"/\(([^)]+)\)/");
            ua = regex.Replace(ua, "(Windows NT 10.0; Win64; x64)");

            if (_customAction != null)
                ua = _customAction(ua);

            await page.SetUserAgentAsync(ua);
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
