using System.Reflection;
using System.Text.RegularExpressions;

using PuppeteerExtraSharpLite.Tests.Utils;

using Scripts = PuppeteerExtraSharpLite.Plugins.EmbeddedScripts.CS.Scripts;

namespace PuppeteerExtraSharpLite.Tests;

public partial class EmbeddedScriptsTests {
	public static readonly TheoryData<string, string> EnterRecaptchaCallBack = new() {
		{ $"{nameof(Scripts.ChromeApp)}.js", Scripts.ChromeApp.ToString() },
		{ $"{nameof(Scripts.Codec)}.js", Scripts.Codec.ToString() },
		{ $"{nameof(Scripts.ContentWindow)}.js", Scripts.ContentWindow.ToString() },
		{ $"{nameof(Scripts.EnterRecaptchaCallBack)}.js", Scripts.EnterRecaptchaCallBack.ToString() },
		{ $"{nameof(Scripts.HardwareConcurrency)}.js", Scripts.HardwareConcurrency.ToString() },
		{ $"{nameof(Scripts.Language)}.js", Scripts.Language.ToString() },
		{ $"{nameof(Scripts.LoadTimes)}.js", Scripts.LoadTimes.ToString() },
		{ $"{nameof(Scripts.Outdimensions)}.js", Scripts.Outdimensions.ToString() },
		{ $"{nameof(Scripts.Permissions)}.js", Scripts.Permissions.ToString() },
		{ $"{nameof(Scripts.Plugin)}.js", Scripts.Plugin.ToString() },
		{ $"{nameof(Scripts.Runtime)}.js", Scripts.Runtime.ToString() },
		{ $"{nameof(Scripts.SCI)}.js", Scripts.SCI.ToString() },
		{ $"{nameof(Scripts.Stacktrace)}.js", Scripts.Stacktrace.ToString() },
		{ $"{nameof(Scripts.Utils)}.js", Scripts.Utils.ToString() },
		{ $"{nameof(Scripts.Vendor)}.js", Scripts.Vendor.ToString() },
		{ $"{nameof(Scripts.WebDriver)}.js", Scripts.WebDriver.ToString() },
		{ $"{nameof(Scripts.WebGL)}.js", Scripts.WebGL.ToString() }
	};

	[Theory]
	[MemberData(nameof(EnterRecaptchaCallBack))]
	public void EnsureScriptConsistency(string fileNameActual, string expected) {
		var actual = Helper.GetScriptContent(fileNameActual);

		// Normalize both strings to compare logic only, ignoring formatting differences
		var normalizedActual = NormalizeScriptContent(actual);
		var normalizedExpected = NormalizeScriptContent(expected);

		Assert.Equal(normalizedExpected, normalizedActual);
	}

	[GeneratedRegex(@"\s+")]
	private static partial Regex WhiteSpaceRegex();

	private static string NormalizeScriptContent(string scriptContent) {
		return WhiteSpaceRegex().Replace(scriptContent, "");
	}
}
