using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    public class Languages(StealthLanguagesOptions options = null) : PuppeteerExtraPlugin("stealth-language")
    {
        public StealthLanguagesOptions Options { get; } = options ?? new StealthLanguagesOptions("en-US", "en");

        public override async Task OnPageCreated(IPage page)
        {
            if (Options.Languages.Length > 0)
            {
                await page.SetExtraHttpHeadersAsync(new()
                {
                    ["Accept-Language"] = string.Join(',', Options.Languages),
                });

                List<string> langs = Options.Languages.Select(l => "\"" + l.ToString() + "\"").ToList();
                await page.EvaluateExpressionOnNewDocumentAsync("Object.defineProperty(Object.getPrototypeOf(navigator), 'languages', { get: function () { '[native code]'; return '[" + string.Join(",", langs) + "]'; } });");
            }

            var script = Utils.GetScript("Language.js");
            await Utils.EvaluateOnNewPage(page, script, Options.Languages);
        }
    }

    public class StealthLanguagesOptions(params string[] languages) : IPuppeteerExtraPluginOptions
    {
        public object[] Languages { get; } = languages.Cast<object>().ToArray();
    }
}
