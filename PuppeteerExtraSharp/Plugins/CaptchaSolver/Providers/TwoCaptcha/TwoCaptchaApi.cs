using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.ApiClient;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.TwoCaptcha.Models;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.TwoCaptcha;

internal class TwoCaptchaApi(string userKey, CaptchaProviderOptions options)
{
    private readonly ApiClient.ApiClient _client = new("https://api.2captcha.com");

    public async Task<TwoCaptchaCreateTaskResponse> CreateTaskAsync(GetCaptchaSolutionRequest request)
    {
        Dictionary<string, object>? json = null;
        switch (request.Vendor)
        {
            case CaptchaVendor.Google:
                json = GetGoogleJson(request);
                break;
            case CaptchaVendor.HCaptcha:
                json = GetHCaptchaJson(request);
                break;
            case CaptchaVendor.Cloudflare:
                json = GetCloudflareTurnstileJson(request);
                break;
            case CaptchaVendor.GeeTest:
                json = GetGeeTestJson(request);
                break;
        }

        if (json == null) throw new NotSupportedException($"Vendor [{request.Vendor}] is not supported");

        var cancellationToken = GetCancellationToken();
        var result = await _client.PostAsync<TwoCaptchaCreateTaskResponse>("createTask", json, cancellationToken);

        ThrowErrorIfBadStatus(result.ErrorId);
        return result;
    }


    public async Task<TwoCaptchaGetTaskResult> GetSolution(ulong id)
    {
        var query = new Dictionary<string, string>()
        {
            ["clientKey"] = userKey,
            ["taskId"] = id.ToString(),
        };

        var cancellationToken = GetCancellationToken();
        var result = await _client.CreatePostPollingRequest<TwoCaptchaGetTaskResult>("getTaskResult", query)
            .TriesLimit(options.MaxPollingAttempts)
            .ActivatePollingAsync(response =>
                    response.Data.Status == "processing" && response.Data.ErrorId == 0
                        ? PollingAction.ContinuePolling
                        : PollingAction.Break,
                cancellationToken);

        ThrowErrorIfBadStatus(result.Data.ErrorId, result.Data.ErrorDescription);
        return result.Data;
    }

    private Dictionary<string, object> GetGoogleJson(GetCaptchaSolutionRequest request)
    {
        return new Dictionary<string, object>
        {
            ["clientKey"] = userKey,
            ["task"] = new Dictionary<string, object>
            {
                ["websiteKey"] = request.SiteKey,
                ["websiteURL"] = request.PageUrl,
                ["type"] = request.Version switch
                {
                    CaptchaVersion.RecaptchaV2 => "ReCaptchaV2TaskProxyless",
                    CaptchaVersion.RecaptchaV3 => "ReCaptchaV3TaskProxyless",
                    CaptchaVersion.HCaptcha => throw new NotSupportedException("HCaptcha is not yet supported"),
                    _ => throw new ArgumentOutOfRangeException()
                },
                ["isInvisible"] = request.IsInvisible,
                ["recaptchaDataSValue"] = request.DataS,
            }
        };
    }

    private Dictionary<string, object> GetHCaptchaJson(GetCaptchaSolutionRequest request)
    {
        return new Dictionary<string, object>
        {
            ["clientKey"] = userKey,
            ["task"] = new Dictionary<string, object>
            {
                ["websiteKey"] = request.SiteKey,
                ["websiteURL"] = request.PageUrl,
                ["type"] = "HCaptchaTaskProxyless",
                ["isInvisible"] = request.IsInvisible
            }
        };
    }

    private Dictionary<string, object> GetGeeTestJson(GetCaptchaSolutionRequest request)
    {
        return new Dictionary<string, object>
        {
            ["clientKey"] = userKey,
            ["task"] = new Dictionary<string, object>
            {
                ["websiteURL"] = request.PageUrl,
                ["type"] = "GeeTestTaskProxyless",
                ["gt"] = request.Gt,
                ["challenge"] = request.Challenge
            }
        };
    }

    private Dictionary<string, object> GetCloudflareTurnstileJson(GetCaptchaSolutionRequest request)
    {
        return new Dictionary<string, object>
        {
            ["clientKey"] = userKey,
            ["task"] = new Dictionary<string, object>
            {
                ["websiteKey"] = request.SiteKey,
                ["websiteURL"] = request.PageUrl,
                ["type"] = "AntiTurnstileTaskProxyLess"
            }
        };
    }

    private void ThrowErrorIfBadStatus(int errorId, string? errorDescription = null)
    {
        if (errorId != 0)
            throw new HttpRequestException(
                $"Two captcha request ends with error id [{errorId} {errorDescription ?? string.Empty}]");
    }

    private CancellationToken GetCancellationToken()
    {
        var source = new CancellationTokenSource();
        source.CancelAfter(options.ApiTimeout);

        return source.Token;
    }
}
