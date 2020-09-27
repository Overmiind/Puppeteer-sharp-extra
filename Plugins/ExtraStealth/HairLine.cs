using System;
using System.Collections.Generic;
using System.Text;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth
{
    public class HairLine: IPuppeteerExtraPlugin
    {
        public string GetName()
        {
            return "stealth-hairLine";
        }

        public void OnPageCreated(Page page)
        {

            page.EvaluateFunctionAsync(@" () => {
const elementDescriptor = Object.getOwnPropertyDescriptor(HTMLElement.prototype, 'offsetHeight');

// redefine the property with a patched descriptor
Object.defineProperty(HTMLDivElement.prototype, 'offsetHeight', {
  ...elementDescriptor,
  get: function() {
    if (this.id === 'modernizr') {
        return 1;
    }
    return elementDescriptor.get.apply(this);
  },
});}");
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
