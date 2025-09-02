using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
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
        public async Task<JsonElement> GetFingerPrint(IPage page)
        {
            var script = ResourcesReader.ReadFile("Extra.Tests.StealthPluginTests.Script.fpCollect.js", Assembly.GetExecutingAssembly());
            await page.EvaluateExpressionAsync(script);

            var fingerPrint =
                await page.EvaluateFunctionAsync<JsonElement>("async () => await fpCollect().generateFingerprint()");

            return fingerPrint;
        }
    }
}
