using System.Web;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha;

public class RecaptchaPlugin : PuppeteerPlugin, IOnPageCreatedPlugin {
    public override string Name => nameof(RecaptchaPlugin);
    private readonly IRecaptchaProvider _provider;

    /// <summary>
    /// Used for testing purposes only
    /// </summary>
    internal RecaptchaPlugin() : this(new InvalidRecaptchaProvider()) { }

    public RecaptchaPlugin(IRecaptchaProvider provider) {
        _provider = provider;
    }

    public async Task<RecaptchaResult> SolveCaptchaAsync(IPage page, string proxyStr = "", CancellationToken token = default) {
        var recaptchaKeyResult = await GetKeyAsync(page);

        if (!recaptchaKeyResult.IsSuccess) {
            return recaptchaKeyResult;
        }

        var key = recaptchaKeyResult.Value; // for success - value=key
        var url = page.Url;
        var solution = await _provider.GetSolutionAsync(key, url, proxyStr, token);
        await WriteToInput(page, solution);

        return new RecaptchaResult() {
            IsSuccess = true
        };
    }

    public async Task OnPageCreated(IPage page) {
        await page.SetBypassCSPAsync(true);
    }

    public static async Task<RecaptchaResult> GetKeyAsync(IPage page) {
        var element =
            await page.QuerySelectorAsync("iframe[src^='https://www.google.com/recaptcha/api2/anchor'][name^=\"a-\"]");

        if (element is null) {
            return new() {
                IsSuccess = false,
                Value = page.Url,
                Exception = "Recaptcha key not found!"
            };
        }

        var src = await element.GetPropertyAsync("src");

        if (src is null) {
            return new() {
                IsSuccess = false,
                Value = page.Url,
                Exception = "Recaptcha key not found!"
            };
        }

        var key = HttpUtility.ParseQueryString(src!.ToString()!).Get("k")!;
        return new() {
            Value = key
        };
    }

    public static async Task WriteToInput(IPage page, string value) {
        await page.EvaluateFunctionAsync(
              $"() => {{document.getElementById('g-recaptcha-response').innerHTML='{value}'}}");

        var script = Scripts.EnterRecaptchaCallBack;

        try {
            await page.EvaluateFunctionAsync(
                $$"""
                (value) => {
                    {{script}}
                }
                """, value);
        } catch {
            // ignored
        }
    }
}