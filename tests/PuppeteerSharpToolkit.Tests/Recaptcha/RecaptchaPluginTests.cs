namespace PuppeteerSharpToolkit.Tests.Recaptcha;

public partial class RecaptchaPluginTests {
	private readonly string _twoCaptchaKey;
	private const string TwoCaptchaReason = "TwoCaptchaKey environment variable is not set";

	private readonly string _antiCaptchaKey;
	private const string AntiCaptchaReason = "AntiCaptchaKey environment variable is not set";

	public RecaptchaPluginTests() {
		_twoCaptchaKey = Environment.GetEnvironmentVariable("TwoCaptchaKey") ?? string.Empty;
		_antiCaptchaKey = Environment.GetEnvironmentVariable("AntiCaptchaKey") ?? string.Empty;
	}
}
