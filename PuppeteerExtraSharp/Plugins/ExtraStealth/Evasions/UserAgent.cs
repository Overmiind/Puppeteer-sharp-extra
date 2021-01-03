using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class UserAgent : PuppeteerExtraPlugin
    {
        public UserAgent(): base("stealth-userAgent") { }

        public override async Task OnPageCreated(Page page)
        {
            var ua = await page.Browser.GetUserAgentAsync();
            ua = ua.Replace("HeadlessChrome/", "Chrome/");
            await page.SetUserAgentAsync(ua);
            
        }
    }
}
