using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Utils;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth;

internal static class StealthUtils
{
    private static readonly ConcurrentDictionary<string, string> _scriptCache = new();
    private static readonly string _scriptsNamespace = $"{typeof(StealthUtils).Namespace}.Scripts";
    
    public static Task EvaluateOnNewPage(IPage page, string script, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentException.ThrowIfNullOrWhiteSpace(script);

        return page.IsClosed
            ? Task.CompletedTask
            : page.EvaluateFunctionOnNewDocumentAsync(script, args);
    }

    public static string GetScript(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _scriptCache.GetOrAdd(name, static n =>
        {
            var resourceName = BuildResourceName(n);
            return ResourcesReader.ReadFile(resourceName);
        });
    }

    private static string BuildResourceName(string name) => $"{_scriptsNamespace}.{name}";
}