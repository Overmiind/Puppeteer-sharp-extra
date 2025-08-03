using System.Net.Http;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.RestClient;

public class PollingBuilder<T> {
    private readonly HttpClient _client;
    private readonly HttpRequestMessage _request;
    private int _timeout = 5;
    private int _limit = 5;
    public PollingBuilder(HttpClient client, HttpRequestMessage request) {
        _client = client;
        _request = request;
    }

    public PollingBuilder<T> WithTimeoutSeconds(int timeout) {
        _timeout = timeout;
        return this;
    }

    public PollingBuilder<T> TriesLimit(int limit) {
        _limit = limit;
        return this;
    }

    public async Task<HttpResponseMessage> ActivatePollingAsync(Func<HttpResponseMessage, PollingAction> resultDelegate) {
        var response = await _client.SendAsync(_request);

        if (resultDelegate(response) == PollingAction.Break || _limit <= 1) {
            return response;
        }

        await Task.Delay(_timeout * 1000);
        _limit -= 1;

        return await ActivatePollingAsync(resultDelegate);
    }
}

public enum PollingAction {
    ContinuePolling,
    Break
}