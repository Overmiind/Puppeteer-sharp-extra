using System.Web;

using PuppeteerSharp;

namespace PuppeteerSharpToolkit.Plugins.Recaptcha;

/// <summary>
/// Puppeteer plugin that solves reCAPTCHA challenges by using a configured
/// <see cref="IRecaptchaProvider"/> and injecting the received token into the page.
/// </summary>
public class RecaptchaPlugin : PuppeteerPlugin, IOnTargetCreatedPlugin {
    /// <summary>
    /// Gets the unique plugin name.
    /// </summary>
    public override string Name => nameof(RecaptchaPlugin);

    private readonly IRecaptchaProvider _provider;

    /// <summary>
    /// Used for testing purposes only
    /// </summary>
    internal RecaptchaPlugin() : this(new InvalidRecaptchaProvider()) { }

    /// <summary>
    /// Creates a new <see cref="RecaptchaPlugin"/> with a specific captcha provider.
    /// </summary>
    /// <param name="provider">Provider used to request captcha solutions.</param>
    public RecaptchaPlugin(IRecaptchaProvider provider) {
        _provider = provider;
    }

    /// <summary>
    /// Solves a reCAPTCHA on the current <paramref name="page"/>, writing the solution token
    /// to the <c>g-recaptcha-response</c> field and invoking callbacks when possible.
    /// </summary>
    /// <param name="page">Puppeteer page containing the reCAPTCHA widget.</param>
    /// <param name="proxyStr">Optional proxy description used by some providers.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Operation result. On success, <see cref="RecaptchaResult.IsSuccess"/> is true.</returns>
    public async Task<RecaptchaResult> SolveCaptchaAsync(IPage page, string proxyStr = "", CancellationToken token = default) {
        // var recaptchaKeyResult = await GetKeyAsync(page).ConfigureAwait(false);

        // if (!recaptchaKeyResult.IsSuccess) {
        //     return recaptchaKeyResult;
        // }

        // var key = recaptchaKeyResult.Value; // for success - value=key
        var url = page.Url;
        var solution = await _provider.GetSolutionAsync(url, proxyStr, token).ConfigureAwait(false);
        await WriteToInput(page, solution).ConfigureAwait(false);

        return new RecaptchaResult {
            IsSuccess = true
        };
    }

    /// <summary>
    /// Attempts to extract the site key (<c>k</c>) from the reCAPTCHA anchor iframe on the page.
    /// </summary>
    /// <param name="page">Puppeteer page containing the reCAPTCHA widget.</param>
    /// <returns>Result with <see cref="RecaptchaResult.Value"/> set to the site key when found.</returns>
    public static async Task<RecaptchaResult> GetKeyAsync(IPage page) {
        var element =
            await page.QuerySelectorAsync("iframe[src^='https://www.google.com/recaptcha/api2/anchor'][name^=\"a-\"]");

        if (element is null) {
            return new RecaptchaResult {
                IsSuccess = false,
                Value = page.Url,
                Exception = "Recaptcha key not found!"
            };
        }

        var src = await element.GetPropertyAsync("src");

        if (src is null) {
            return new RecaptchaResult {
                IsSuccess = false,
                Value = page.Url,
                Exception = "Recaptcha key not found!"
            };
        }

        var key = HttpUtility.ParseQueryString(src!.ToString()!).Get("k")!;
        return new RecaptchaResult {
            Value = key
        };
    }

    /// <summary>
    /// Injects the provided token into the hidden <c>g-recaptcha-response</c> element and
    /// tries to invoke the page callback, if available.
    /// </summary>
    /// <param name="page">Target page.</param>
    /// <param name="value">Solution token to inject.</param>
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

    /// <inheritdoc />
    public async Task OnTargetCreated(Target target) {
        if (target.Type == TargetType.Page) {
            var page = await target.PageAsync().ConfigureAwait(false);
            await page.SetBypassCSPAsync(true).ConfigureAwait(false);
        }
    }
}
