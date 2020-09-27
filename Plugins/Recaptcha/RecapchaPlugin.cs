using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha
{
    public class RecaptchaPlugin : IPuppeteerExtraPlugin
    {
        private readonly Recaptcha _recaptcha;

        public RecaptchaPlugin(IRecaptchaProvider provider, CaptchaOptions opt = null)
        {
            _recaptcha = new Recaptcha(provider, opt ?? new CaptchaOptions());
        }

        public string GetName()
        {
            return "recaptcha";
        }

        public async Task SolveCaptchaAsync(Page page)
        {
            await _recaptcha.Solve(page);
        }

        public void OnPageCreated(Page page)
        {
            page.EvaluateExpressionOnNewDocumentAsync(Resources.FindRecaptchaScript);
        }


        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
