using System.Collections.Generic;
using HarmonyLib;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;

public class SourceUrl : PuppeteerExtraPlugin
{
    private const string EvaluationScriptUrl = "__puppeteer_evaluation_script__";
    private const string EvaluationScriptSuffix = $"//# sourceURL={EvaluationScriptUrl}";
    private const string EvaluationScriptSuffixReplace = "//# sourceURL=''";

    public SourceUrl() : base("SourceUrl")
    {
    }

    public override void BeforeLaunch(LaunchOptions options)
    {
        var harmony = new Harmony("SourceUrl_Patch");

        var original = AccessTools.Method(typeof(ExecutionContext), "ExecuteEvaluationAsync");
        var prefix = new HarmonyMethod(typeof(SourceUrl), nameof(ExecuteEvaluationPrefix));

        harmony.Patch(original, prefix: prefix);
    }

    private static bool ExecuteEvaluationPrefix(ref string method, ref object args)
    {
        if (args is Dictionary<string, object> dictionary)
        {
            if (dictionary.TryGetValue("expression", out var expression) &&
                expression is string expressionString)
            {
                dictionary["expression"] = expressionString.Replace(
                    EvaluationScriptSuffix,
                    EvaluationScriptSuffixReplace);
            }
        }

        var typeInfo = args.GetType();

        if (typeInfo.Name == "RuntimeCallFunctionOnRequest")
        {
            var functionDeclarationProperty = typeInfo.GetProperty("FunctionDeclaration")!;

            var value = functionDeclarationProperty.GetValue(args) as string;

            var newValue = value!.Replace(EvaluationScriptSuffix, EvaluationScriptSuffixReplace);

            functionDeclarationProperty.SetValue(args, newValue);
        }

        // to next execute, after change args
        return true;
    }
}
