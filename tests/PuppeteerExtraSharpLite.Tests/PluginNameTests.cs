using System.Reflection;

using PuppeteerExtraSharpLite.Plugins;

namespace PuppeteerExtraSharpLite.Tests;

public class PluginNameTests {
	[Fact]
	public void PluginName_Should_MatchTypeName() {
		// Use a well-known type to locate the assembly that contains all plugins
		var baseType = typeof(PuppeteerPlugin);
		var assembly = baseType.Assembly;

		var pluginTypes = assembly
			.GetTypes()
			.Where(t => baseType.IsAssignableFrom(t)
						&& t.IsClass
						&& !t.IsAbstract)
			.ToArray();

		Assert.NotEmpty(pluginTypes);

		foreach (var t in pluginTypes) {
			// Require a public parameterless ctor per project convention
			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var ctor = t.GetConstructor(flags, Type.EmptyTypes);
			Assert.True(ctor != null, $"{t.FullName} must have a public parameterless constructor.");

			var instance = ctor.Invoke(null) as PuppeteerPlugin;
			Assert.NotNull(instance);
			Assert.Equal(t.Name, instance!.Name);
		}
    }
}
