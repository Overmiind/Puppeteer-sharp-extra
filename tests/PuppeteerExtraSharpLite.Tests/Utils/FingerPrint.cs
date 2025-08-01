using System.Reflection;
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
        var testAssemblyLocation = Assembly.GetExecutingAssembly().Location;
        var testAssemblyDir = Path.GetDirectoryName(testAssemblyLocation)!;

        // Navigate up to find the repository root (where we can find src/ folder)
        var repoRoot = Helper.FindRepositoryRoot(testAssemblyDir);
        var projectPath = Path.Combine(repoRoot, "tests", "PuppeteerExtraSharpLite.Tests");
        var filePath = Path.Combine(projectPath, "StealthPluginTests", "Script", "fpCollect.js");

        if (!File.Exists(filePath)) {
            throw new FileNotFoundException($"Script file not found: {filePath}");
        }

        var script = await File.ReadAllTextAsync(filePath);
        await page.EvaluateExpressionAsync(script);

        var fingerPrint =
            await page.EvaluateFunctionAsync<JsonElement>("async () => await fpCollect().generateFingerprint()");

        return fingerPrint;
    }
}