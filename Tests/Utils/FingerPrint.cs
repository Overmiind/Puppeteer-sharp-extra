using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace Extra.Tests.Utils
{
    public class FingerPrint
    {
        /// <summary>
        /// https://antoinevastel.com/bots/
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<JObject> GetFingerPrint(IPage page)
        {
            var script = ResourcesReader.ReadFile("Extra.Tests.StealthPluginTests.Script.fpCollect.js", Assembly.GetExecutingAssembly());
            await page.EvaluateExpressionAsync(script);

            var fingerPrint =
                await page.EvaluateFunctionAsync<JObject>("async () => await fpCollect().generateFingerprint()");

            return fingerPrint;
        }
    }
}
