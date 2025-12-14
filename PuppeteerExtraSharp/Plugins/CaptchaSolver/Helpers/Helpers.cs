using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Helpers;

public static class Helpers
{
    private static ConcurrentDictionary<IPage, List<string>> Scripts { get; } = new();

    public static async Task EnsureEvaluateFunctionAsync(
        this IPage page,
        string scriptName,
        params object[] args)
    {
        var script = ResourcesReader.ReadFile(scriptName);

        var pageScripts = Scripts.GetOrAdd(page, _ => new List<string>());

        lock (pageScripts)
        {
            if (pageScripts.Contains(scriptName)) return;
        }

        await page.EvaluateFunctionAsync(script, args);

        lock (pageScripts)
        {
            if (!pageScripts.Contains(scriptName))
            {
                pageScripts.Add(scriptName);
            }
        }
    }

    public static async Task EnsureEvaluateExpressionOnNewDocumentAsync(this IPage page, string scriptName)
    {
        var script = ResourcesReader.ReadFile(scriptName);

        var pageScripts = Scripts.GetOrAdd(page, _ => new List<string>());

        lock (pageScripts)
        {
            if (pageScripts.Contains(scriptName)) return;
        }

        await page.EvaluateExpressionOnNewDocumentAsync(script);

        lock (pageScripts)
        {
            if (!pageScripts.Contains(scriptName))
            {
                pageScripts.Add(scriptName);
            }
        }
    }
}