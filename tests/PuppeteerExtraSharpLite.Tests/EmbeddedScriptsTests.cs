using System.Reflection;
using System.Text.RegularExpressions;

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
		// Get the directory where the test assembly is located
		var testAssemblyLocation = Assembly.GetExecutingAssembly().Location;
		var testAssemblyDir = Path.GetDirectoryName(testAssemblyLocation)!;

		// Navigate up to find the repository root (where we can find src/ folder)
		var repoRoot = FindRepositoryRoot(testAssemblyDir);
		var projectPath = Path.Combine(repoRoot, "src", "PuppeteerExtraSharpLite");
		var filePath = Path.Combine(projectPath, "Plugins", "EmbeddedScripts", "JS", fileNameActual);

		if (!File.Exists(filePath)) {
			throw new FileNotFoundException($"File {filePath} does not exist.");
		}

		var actual = File.ReadAllText(filePath);

		// Normalize both strings to compare logic only, ignoring formatting differences
		var normalizedActual = NormalizeScriptContent(actual);
		var normalizedExpected = NormalizeScriptContent(expected);

		Assert.Equal(normalizedExpected, normalizedActual);
	}

	private static string FindRepositoryRoot(string startPath) {
		var directory = new DirectoryInfo(startPath);
		while (directory != null) {
			// Look for src directory as indicator of repo root
			if (directory.GetDirectories("src").Length > 0) {
				return directory.FullName;
			}
			directory = directory.Parent;
		}
		throw new DirectoryNotFoundException($"Could not find repository root starting from {startPath}.");
	}

	[GeneratedRegex(@"\s+")]
	private static partial Regex WhiteSpaceRegex();

	private static string NormalizeScriptContent(string scriptContent) {
		return WhiteSpaceRegex().Replace(scriptContent, "");
	}
}
