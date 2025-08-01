using System.Text.Json;

using PuppeteerSharp;

namespace PuppeteerExtraSharpLite.Tests.Utils;

public class FingerPrint {
    /// <summary>
    /// https://antoinevastel.com/bots/
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static async Task<JsonElement> GetFingerPrint(IPage page) {
        var path = Path.Join("PuppeteerExtraSharpLite.Tests", "StealthPluginTests", "Script", "fpCollect.js");

        if (!File.Exists(path)) {
            throw new FileNotFoundException($"Script file not found: {path}");
        }

        var script = await File.ReadAllTextAsync(path);
        await page.EvaluateExpressionAsync(script);

        var fingerPrint =
            await page.EvaluateFunctionAsync<JsonElement>("async () => await fpCollect().generateFingerprint()");

        return fingerPrint;
    }
}