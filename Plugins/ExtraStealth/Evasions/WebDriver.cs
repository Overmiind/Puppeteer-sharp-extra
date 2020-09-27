using System.Collections.Generic;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class WebDriver: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-webDriver";
        }
        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }

        public void OnPageCreated(Page page)
        {
            page.EvaluateFunctionOnNewDocumentAsync(@"
            () => {
            delete Object.getPrototypeOf(navigator).webdriver
            }");
        }
    }
}
