using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class UserAgent : IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-userAgent";
        }


        public async void OnPageCreated(Page page)
        {
            var ua = await page.Browser.GetUserAgentAsync();
            ua = ua.Replace("HeadlessChrome/", "Chrome/");
            await page.SetUserAgentAsync(ua);
            
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
