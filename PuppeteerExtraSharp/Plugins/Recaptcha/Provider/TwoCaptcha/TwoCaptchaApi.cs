using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.ApiClient;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider.TwoCaptcha.Models;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.TwoCaptcha;

internal class TwoCaptchaApi(string userKey, CaptchaProviderOptions options)
{
    private readonly ApiClient.ApiClient _client = new("https://api.2captcha.com");

    public async Task<TwoCaptchaCreateTaskResponse> CreateTaskAsync(GetRecaptchaSolutionRequest request)
    {
        var json = new Dictionary<string, object>
        {
            ["clientKey"] = userKey,
            ["task"] = new Dictionary<string, string>()
            {
                ["websiteKey"] = request.SiteKey,
                ["websiteURL"] = request.PageUrl,
                ["type"] = request.Version switch
                {
                    CaptchaVersion.V2 => "RecaptchaV2TaskProxyless ",
                    CaptchaVersion.V3 => "RecaptchaV3TaskProxyless",
                    CaptchaVersion.HCaptcha => throw new NotSupportedException("HCaptcha is not supported"),
                    _ => throw new ArgumentOutOfRangeException()
                },
                ["isInvisible"] = request.IsInvisible.ToString(),
                ["recaptchaDataSValue"] = request.DataS,
                ["minScore"] = request.MinV3RecaptchaScore.ToString()
            }
        };

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