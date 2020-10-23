using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider;
using PuppeteerExtraSharp.Utils;
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

        public async Task<RecaptchaResult> SolveCaptchaAsync(Page page)
        {
            return await _recaptcha.Solve(page);
        }

        public async Task OnPageCreated(Page page)
        {
            await page.SetBypassCSPAsync(true);

            //var script = ResourcesReader.ReadFile(this.GetType().Namespace + ".Scripts.FindRecaptcha.js");
            //page.DOMContentLoaded += async (sender, args) =>
            //{
            //    await page.EvaluateExpressionAsync(script);
            //};

            //page.FrameAttached += async (sender, args) =>
            //{
            //    await args.Frame.EvaluateExpressionAsync(script);

            //    foreach (var frameChildFrame in args.Frame.ChildFrames)
            //    {
            //        await frameChildFrame.EvaluateExpressionAsync(script);
            //    }
            //};

            ////await page.EvaluateExpressionOnNewDocumentAsync(script);
        }


        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
