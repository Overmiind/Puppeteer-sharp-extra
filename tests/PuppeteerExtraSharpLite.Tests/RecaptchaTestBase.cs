using Microsoft.Extensions.DependencyInjection;

using PuppeteerExtraSharpLite.Plugins.Recaptcha;
using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider;
using PuppeteerExtraSharpLite.Tests.Recaptcha;

namespace PuppeteerExtraSharpLite.Tests;

public abstract class RecaptchaTestBase : BrowserDefault {
    protected IServiceProvider ServiceProvider { get; }

    protected RecaptchaTestBase() {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services) {
        services.AddSingleton<IRecaptchaProvider, MockRecaptchaProvider>();
    }

    protected RecaptchaPlugin CreateRecaptchaPlugin() {
        var provider = ServiceProvider.GetRequiredService<IRecaptchaProvider>();
        return new RecaptchaPlugin(provider);
    }
}
