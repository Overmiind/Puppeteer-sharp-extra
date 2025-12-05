using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Interfaces;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;
using PuppeteerSharp;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Helpers;

public static class Helpers
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

    public static ICaptchaVendorHandler? CreateHandler(CaptchaVendor vendor, ICaptchaSolverProvider provider, CaptchaSolverOptions options, IPage page)
    {
        var assemmbly = typeof(CaptchaSolverPlugin).Assembly;
        var typeName = $"PuppeteerExtraSharp.Plugins.CaptchaSolver.Vendors.{vendor}.{vendor}Handler";
        var handlerType = assemmbly.GetType(typeName);
        if (handlerType is null)
            return null;

        if (!typeof(ICaptchaVendorHandler).IsAssignableFrom(handlerType))
            return null;

        return (ICaptchaVendorHandler?)Activator.CreateInstance(handlerType, new object[]
        {
            provider, options, page
        });
    }
}
