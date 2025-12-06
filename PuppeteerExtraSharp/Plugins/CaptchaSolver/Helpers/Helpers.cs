using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Helpers;

public static class Helpers
{
    private static Dictionary<IPage, List<string>> Scripts { get; } = new();

    public static async Task EnsureEvaluateFunctionAsync(
        this IPage page,
        string scriptName,
        params object[] args)
    {
        var script = ResourcesReader.ReadFile(scriptName);
        
        if (Scripts.ContainsKey(page) && Scripts[page].Contains(scriptName)) return;

        if (!Scripts.ContainsKey(page))
        {
            Scripts.Add(page, new List<string>());
        }

        await page.EvaluateFunctionAsync(script, args);
        Scripts[page].Add(scriptName);
    }
    
    public static async Task EnsureEvaluateExpressionOnNewDocumentAsync(this IPage page, string scriptName)
    {
        var script = ResourcesReader.ReadFile(scriptName);
        
        if (Scripts.ContainsKey(page) && Scripts[page].Contains(scriptName)) return;

        if (!Scripts.ContainsKey(page))
        {
            Scripts.Add(page, new List<string>());
        }

        await page.EvaluateExpressionOnNewDocumentAsync(script);
        Scripts[page].Add(scriptName);
    }
}