using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider.AntiCaptcha
{
    public class AntiCaptcha : IRecaptchaProvider
    {
        private readonly string _userKey;

        public AntiCaptcha(string userKey)
        {
            _userKey = userKey;
        }
        public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = null)
        {
            var api = new AntiCaptchaApi(_userKey);
            var task = await api.CreateTaskAsync(pageUrl, key);
            await System.Threading.Tasks.Task.Delay(20000);
            var result = await api.PendingForResult(task.taskId);

            if (result.status != "ready" || result.solution is null || result.errorId != 0)
                throw new HttpRequestException($"AntiCaptcha request ends with error - {result.errorId}");

            return result.solution.gRecaptchaResponse;
        }
    }
}
