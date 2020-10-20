using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha.Models;
using RestSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha
{
    public class AntiCaptchaApi
    {
        private readonly string _userKey;
        private readonly RestClient _client = new RestClient("http://api.anti-captcha.com");
        public AntiCaptchaApi(string userKey)
        {
            _userKey = userKey;
        }

        public Task<AntiCaptchaTaskResult> CreateTaskAsync(string pageUrl, string key, CancellationToken token = default)
        {
            var content = new AntiCaptchaRequest()
            {
                clientKey = _userKey,
                task = new AntiCaptchaTask()
                {
                    type = "NoCaptchaTaskProxyless",
                    websiteURL = pageUrl,
                    websiteKey = key
                }
            };



            var result = _client.PostWithJsonAsync<AntiCaptchaTaskResult>("createTask", content, token);
            return result;
        }


        public async Task<TaskResultModel> PendingForResult(int taskId, CancellationToken token = default)
        {
            var content = new RequestForResultTask()
            {
                clientKey = _userKey,
                taskId = taskId
            };


            var request = new RestRequest("getTaskResult");
            request.AddJsonBody(content);
            request.Method = Method.POST;
            var result = await _client.PendingWhileAsync<TaskResultModel>(request,
                model => model.Data.status == "ready" || model.Data.errorId != 0, everySeconds: 10, triesLimit: 30, token);

            return result.Data;
        }

    }
}
