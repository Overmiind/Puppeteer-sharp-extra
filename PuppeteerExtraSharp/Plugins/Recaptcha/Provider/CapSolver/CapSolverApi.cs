using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider.CapSolver.Models;
using PuppeteerExtraSharp.Plugins.Recaptcha.RestClient;
using RestSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.CapSolver;

public class CapSolverApi
{
    private readonly string _userKey;
    private readonly ProviderOptions _options;
    private readonly RestClient.RestClient _client = new RestClient.RestClient("https://api.capsolver.com");
    public CapSolverApi(string userKey, ProviderOptions options)
    {
        _userKey = userKey;
        _options = options;
    }

    public Task<CapSolverTaskResponse> CreateTaskAsync(string pageUrl, string key, CancellationToken token = default)
    {
        var request = new CapSolverRequest()
        {
            clientKey = key,
            task = new CapSolverTaskRequest()
            {
                type = "ReCaptchaV2TaskProxyless",
                websiteURL = pageUrl,
                websiteKey = key
            }
        };
        
        var result = _client.PostWithJsonAsync<CapSolverTaskResponse>("createTask", request, token);
        return result;
    }

    public async Task<CapSolverTaskResponse> PendingForResult(string taskId, CancellationToken token = default)
    {
        var content = new CapSolverRequest()
        {
            clientKey = _userKey,
            taskId = taskId
        };
        
        var request = new RestRequest("getTaskResult");
        request.AddJsonBody(content);
        request.Method = Method.Post;
        
        var result = await _client.CreatePollingBuilder<CapSolverTaskResponse>(request).TriesLimit(_options.PendingCount)
            .WithTimeoutSeconds(5).ActivatePollingAsync(
                response =>
                {
                    if (response.Data.status == "ready" || response.Data.errorId != 0)
                        return PollingAction.Break;

                    return PollingAction.ContinuePolling;
                });
        return result.Data;
    }
}