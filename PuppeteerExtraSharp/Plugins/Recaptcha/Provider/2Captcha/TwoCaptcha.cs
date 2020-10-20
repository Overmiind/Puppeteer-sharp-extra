using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Provider._2Captcha
{
    public class TwoCaptcha: IRecaptchaProvider
    {
        private readonly TwoCaptchaApi _api;

        public TwoCaptcha(string key)
        {
            _api =  new TwoCaptchaApi(key);
        }

        public async Task<string> GetSolution(string key, string pageUrl, string proxyStr = null)
        {
            var id = await _api.CreateTaskAsync(key, pageUrl);

            await Task.Delay(20 * 1000);

            var result = await _api.GetSolution(id);

            if(result.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException($"Two captcha request ends with error [{result.StatusCode}] {result.StatusDescription}");

            return result.Data;
        }
    }
}
