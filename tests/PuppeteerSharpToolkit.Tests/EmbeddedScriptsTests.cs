using System.Text.RegularExpressions;

namespace PuppeteerSharpToolkit.Tests;

public partial class EmbeddedScriptsTests {
	public static readonly TheoryData<string, string> ScriptNames = new() {
		{ $"{nameof(Scripts.ChromeApp)}.js", Scripts.ChromeApp },
		{ $"{nameof(Scripts.Codec)}.js", Scripts.Codec },
		{ $"{nameof(Scripts.ContentWindow)}.js", Scripts.ContentWindow },
		{ $"{nameof(Scripts.EnterRecaptchaCallBack)}.js", Scripts.EnterRecaptchaCallBack },
		{ $"{nameof(Scripts.HardwareConcurrency)}.js", Scripts.HardwareConcurrency },
		{ $"{nameof(Scripts.Language)}.js", Scripts.Language },
		{ $"{nameof(Scripts.LoadTimes)}.js", Scripts.LoadTimes },
		{ $"{nameof(Scripts.Outdimensions)}.js", Scripts.Outdimensions },
		{ $"{nameof(Scripts.Permissions)}.js", Scripts.Permissions },
		{ $"{nameof(Scripts.Evasion)}.js", Scripts.Evasion },
		{ $"{nameof(Scripts.Runtime)}.js", Scripts.Runtime },
		{ $"{nameof(Scripts.SCI)}.js", Scripts.SCI },
		{ $"{nameof(Scripts.Stacktrace)}.js", Scripts.Stacktrace },
		{ $"{nameof(Scripts.Utils)}.js", Scripts.Utils },
		{ $"{nameof(Scripts.Vendor)}.js", Scripts.Vendor },
		{ $"{nameof(Scripts.WebDriver)}.js", Scripts.WebDriver },
		{ $"{nameof(Scripts.WebGL)}.js", Scripts.WebGL }
	};

	[Theory]
	[MemberData(nameof(ScriptNames))]
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
