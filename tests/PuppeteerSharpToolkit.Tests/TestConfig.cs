using Microsoft.Extensions.Configuration;

namespace PuppeteerSharpToolkit.Tests;

public static class TestConfig {
	public static IConfigurationRoot Config {
		get {
			if (field is null) {
				var builder = new ConfigurationBuilder()
								  .AddUserSecrets(typeof(TestConfig).Assembly, true);
				field = builder.Build();
			}
			return field;
		}
	}
}
