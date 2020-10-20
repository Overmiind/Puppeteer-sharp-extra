using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider
{
    public class RestClient
    {
        private readonly RestSharp.RestClient _client;

        public RestClient(string url)
        {
            _client = new RestSharp.RestClient(url);
        }

        public async Task<IRestResponse<T>> PendingWhileAsync<T>(RestRequest request, Func<IRestResponse<T>, bool> whileAction, int everySeconds = 5, int triesLimit = 5, CancellationToken token = default)
        {
            var result = await ExecuteAsync<T>(request, token);

            if (triesLimit == 1 || whileAction(result))
                return result;

            await System.Threading.Tasks.Task.Delay(everySeconds * 1000, token);
            return await PendingWhileAsync(request, whileAction, everySeconds, triesLimit - 1, token);
        }

        public async Task<T> PostWithJsonAsync<T>(string url, object content, CancellationToken token)
        {
            var request = new RestRequest(url);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(content);
            request.Method = Method.POST;
            return await _client.PostAsync<T>(request, token);
        }

        public async Task<T> PostWithQueryAsync<T>(string url, Dictionary<string,string> query, CancellationToken token = default)
        {
            var request = new RestRequest(url) {Method = Method.POST};

            foreach (var (key, value) in query)
            {
                request.AddQueryParameter(key, value);
            }
            var test = await _client.ExecuteAsync(request);
            return await _client.PostAsync<T>(request, token);
        }

        private async Task<IRestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken token)
        {
            return await _client.ExecuteAsync<T>(request, token);
        }
    }
}
