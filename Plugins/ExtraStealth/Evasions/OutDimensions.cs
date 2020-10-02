using System.Collections.Generic;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    internal class OutDimensions: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-dimensions";
        }

        public void OnPageCreated(Page page)
        {
            page.EvaluateFunctionOnNewDocumentAsync(@"() => {
      try {
        if (window.outerWidth && window.outerHeight) {
          return // nothing to do here
        }
        const windowFrame = 85 // probably OS and WM dependent
        window.outerWidth = window.innerWidth
        window.outerHeight = window.innerHeight + windowFrame
      } catch (err) {}
    }");
        }


        public void BeforeLaunch(LaunchOptions options)
        {
            options.DefaultViewport = null;
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
