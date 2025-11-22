using System;
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
        // If we think we've already injected this script for the page, double‑check
        // that the expected globals still exist (the JS context is reset on navigation).
        if (Scripts.ContainsKey(page) && Scripts[page].Contains(scriptName))
        {
            // Heuristic: our Recaptcha script defines window.reScript
            if (scriptName.EndsWith("RecaptchaScript.js", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var hasGlobal = await page.EvaluateExpressionAsync<bool>("typeof window.reScript !== 'undefined'");
                    if (!hasGlobal)
                    {
                        // Context was lost; re-inject the function
                        await page.EvaluateFunctionAsync(script, args);
                    }
                }
                catch
                {
                    // If evaluation fails (e.g., context not ready), fall back to injecting
                    await page.EvaluateFunctionAsync(script, args);
                }
            }

            return;
        }

        if (!Scripts.ContainsKey(page))
        {
            Scripts.Add(page, new List<string>());
        }

        await page.EvaluateFunctionAsync(script, args);
        Scripts[page].Add(scriptName);
    }
}