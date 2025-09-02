using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Helpers;

public static class ScriptHelper
{
    private static Dictionary<IPage, List<string>> Scripts { get; } = new();

    public static async Task EnsureEvaluateFunctionAsync(this IPage page,
        string scriptName,
        string script,
        params object[] args)
    {
        if (Scripts.ContainsKey(page) && Scripts[page].Contains(scriptName)) return;

        if (!Scripts.ContainsKey(page))
        {
            Scripts.Add(page, new List<string>());
        }

        await page.EvaluateFunctionAsync(script, args);
        Scripts[page].Add(scriptName);
    }
}