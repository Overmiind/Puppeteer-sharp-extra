using System.Web;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

public class Recaptcha {
    private readonly IRecaptchaProvider _provider;

    public Recaptcha(IRecaptchaProvider provider) {
        _provider = provider;
    }

    public async Task<RecaptchaResult> Solve(IPage page) {
        try {
            var key = await GetKeyAsync(page);
            var solution = await GetSolutionAsync(key, page.Url);
            await WriteToInput(page, solution);

            return new RecaptchaResult() {
                IsSuccess = true
            };
        } catch (CaptchaException ex) {
            return new RecaptchaResult() {
                Exception = ex,
                IsSuccess = false
            };
        }

    }

    public static async Task<string> GetKeyAsync(IPage page) {
        var element =
            await page.QuerySelectorAsync("iframe[src^='https://www.google.com/recaptcha/api2/anchor'][name^=\"a-\"]");

        if (element == null) {
            throw new CaptchaException {
                PageUrl = page.Url,
                Content = "Recaptcha key not found!"
            };
        }

        var src = await element.GetPropertyAsync("src");

        if (src is null) {
            throw new CaptchaException {
                PageUrl = page.Url,
                Content = "Recaptcha key not found!"
            };
        }

        var key = HttpUtility.ParseQueryString(src!.ToString()!).Get("k")!;
        return key;
    }

    public async Task<string> GetSolutionAsync(string key, string urlPage) {
        return await _provider.GetSolution(key, urlPage);
    }

    public static async Task WriteToInput(IPage page, string value) {
        await page.EvaluateFunctionAsync(
              $"() => {{document.getElementById('g-recaptcha-response').innerHTML='{value}'}}");

        var script = Scripts.EnterRecaptchaCallBack;

        try {
            await page.EvaluateFunctionAsync($@"(value) => {{{script}}}", value);
        } catch {
            // ignored
        }
    }
}