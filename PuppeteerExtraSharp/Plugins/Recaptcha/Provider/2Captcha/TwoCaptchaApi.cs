using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha
{
    internal class TwoCaptchaApi
    {
        private readonly RestClient _client = new RestClient("https://rucaptcha.com");
        private readonly string _userKey;

        public TwoCaptchaApi(string userKey)
        {
            _userKey = userKey;
        }

        public async Task<string> CreateTaskAsync(string key, string pageUrl)
        {
            var result = await _client.PostWithQueryAsync<string>("in.php", new Dictionary<string, string>()
            {
                ["key"] = _userKey,
                ["googlekey"] = key,
                ["pageurl"] = pageUrl
            });

            return result;
        }


        public async Task<IRestResponse<string>> GetSolution(string id)
        {
            var request = new RestRequest("res.php");
            request.Method = Method.POST;

            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("key", _userKey, ParameterType.UrlSegment);
            request.AddParameter("action", "get", ParameterType.UrlSegment);

            var result = await _client.PendingWhileAsync<string>(request,
                s => s.StatusDescription != "CAPCHA_NOT_READY", everySeconds: 5, triesLimit: 10);

            return result;
        }
    }
}
