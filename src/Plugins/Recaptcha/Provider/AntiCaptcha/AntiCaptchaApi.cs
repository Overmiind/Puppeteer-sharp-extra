using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        public Task<TaskResult> CreateTaskAsync(string pageUrl, string key, CancellationToken token = default)
        {
            var content = new TaskModel()
            {
                clientKey = _userKey,
                task = new Task()
                {
                    type = "NoCaptchaTaskProxyless",
                    websiteURL = pageUrl,
                    websiteKey = key
                }
            };

         

            var result = _client.PostAsync<TaskResult>("createTask",content,token);
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
            return await _client.PendingWhileAsync<TaskResultModel>(request,
                model => model.status == "ready" || model.errorId != 0, everySeconds: 10, triesLimit: 30, token);
        }

    }
}
