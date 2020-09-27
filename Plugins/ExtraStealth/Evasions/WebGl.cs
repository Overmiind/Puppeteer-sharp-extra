using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class WebGl: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-webGl";
        }

        public void OnPageCreated(Page page)
        {
            page.EvaluateFunctionOnNewDocumentAsync(@"() => {const getParameter = WebGLRenderingContext.getParameter;
WebGLRenderingContext.prototype.getParameter = function(parameter) {
  // UNMASKED_VENDOR_WEBGL
  if (parameter === 37445) {
    return 'Intel Open Source Technology Center';
  }
  // UNMASKED_RENDERER_WEBGL
  if (parameter === 37446) {
    return 'Mesa DRI Intel(R) Ivybridge Mobile ';
  }

  return getParameter(parameter);
};}");
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
