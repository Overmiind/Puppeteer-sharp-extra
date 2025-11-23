using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.ApiClient;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.CapSolver.Models;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Providers.CapSolver;

internal class CapSolverApi(string userKey, CaptchaProviderOptions options)
{
    private readonly ApiClient.ApiClient _client = new("https://api.capsolver.com");

    public async Task<CapSolverCreateTaskResponse> CreateTaskAsync(GetCaptchaSolutionRequest request)
    {
        var json = new Dictionary<string, object>
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

        var cancellationToken = GetCancellationToken();
        var result = await _client.PostAsync<CapSolverCreateTaskResponse>("createTask", json, cancellationToken);

        ThrowErrorIfBadStatus(result.ErrorId);
        return result;
    }

    public async Task<CapSolverGetTaskResult> GetSolution(string id)
    {
        var query = new Dictionary<string, string>()
        {
            ["clientKey"] = userKey,
            ["taskId"] = id,
        };

        var cancellationToken = GetCancellationToken();
        var result = await _client.CreatePostPollingRequest<CapSolverGetTaskResult>("getTaskResult", query)
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
                $"CapSolver request ends with error id [{errorId} {errorDescription ?? string.Empty}]");
    }

    private CancellationToken GetCancellationToken()
    {
        var source = new CancellationTokenSource();
        source.CancelAfter(options.ApiTimeout);

        return source.Token;
    }
}
