using System.Reflection;

namespace PuppeteerExtraSharpLite.Tests.Utils;

public static class Helper {
	public static string GetScriptContent(string filename) {
		var testAssemblyLocation = Assembly.GetExecutingAssembly().Location;
		var testAssemblyDir = Path.GetDirectoryName(testAssemblyLocation)!;

		// Navigate up to find the repository root (where we can find src/ folder)
		var repoRoot = FindRepositoryRoot(testAssemblyDir);
		var projectPath = Path.Combine(repoRoot, "src", "PuppeteerExtraSharpLite");
		var filePath = Path.Combine(projectPath, "EmbeddedScripts", "JS", filename);

		if (!File.Exists(filePath)) {
			throw new FileNotFoundException($"File {filePath} does not exist.");
		}

		return File.ReadAllText(filePath);

	}

	public static string FindRepositoryRoot(string startPath) {
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

	public static bool TryGetEnvironmentVariable(string name, out string value) {
		value = Environment.GetEnvironmentVariable(name) ?? string.Empty;
		return value.Length > 0;
	}
}