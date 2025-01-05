using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Utils;
using RestSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.RestClient
{
    public class RestClient
    {
        private readonly RestSharp.RestClient _client;

        public RestClient(string url = null)
        {
            _client = string.IsNullOrWhiteSpace(url)
                ? new RestSharp.RestClient()
                : new RestSharp.RestClient(url);
        }

        public PollingBuilder<T> CreatePollingBuilder<T>(RestRequest request)
        {
            return new PollingBuilder<T>(_client, request);
        }

        public async Task<T> PostWithJsonAsync<T>(
            string url,
            object content,
            CancellationToken token)
        {
            var request = new RestRequest(url);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(content);
            request.Method = Method.Post;
            return await _client.PostAsync<T>(request, token);
        }

        public async Task<T> PostWithQueryAsync<T>(
            string url,
            Dictionary<string, string> query,
            CancellationToken token = default)
        {
            var request = new RestRequest(url) { Method = Method.Post };
            ExtendQueryParameters(request, query);
            return await _client.PostAsync<T>(request, token);
        }

        private static void ExtendQueryParameters(
            RestRequest request,
            Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                request.AddQueryParameter(parameter.Key, parameter.Value);
            }
        }
    }
}
