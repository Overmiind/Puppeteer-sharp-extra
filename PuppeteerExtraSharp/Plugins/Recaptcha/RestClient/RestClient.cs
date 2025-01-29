using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerExtraSharp.Utils;
using RestSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.RestClient
{
    public class RestClient(string url = null)
    {
        private readonly RestSharp.RestClient _client = string.IsNullOrWhiteSpace(url) ? new RestSharp.RestClient() : new RestSharp.RestClient(url);

        public PollingBuilder<T> CreatePollingBuilder<T>(RestRequest request)
        {
            return new PollingBuilder<T>(_client, request);
        }

        public async Task<T> PostWithJsonAsync<T>(string url, object content, CancellationToken token) where T : JToken
        {
            var request = new RestRequest(url);
            request.AddHeader("Content-type", "application/json");
            request.AddJsonBody(JsonConvert.SerializeObject(content));
            request.Method = Method.Post;

            var response = await _client.PostAsync(request, token);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public async Task<T> PostWithQueryAsync<T>(string url, Dictionary<string, string> query, CancellationToken token = default)
        {
            var request = new RestRequest(url) { Method = Method.Post };
            request.AddQueryParameters(query);
            return await _client.PostAsync<T>(request, token);
        }

        private async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken token)
        {
            return await _client.ExecuteAsync<T>(request, token);
        }
    }
}
