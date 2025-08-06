namespace PuppeteerExtraSharpLite.Tests.Recaptcha;

public partial class RecaptchaPluginTests {
	private readonly string _twoCaptchaKey;
	private const string TwoCaptchaReason = "TwoCaptchaKey environment variable is not set";

	private readonly string _antiCaptchaKey;
	private const string AntiCaptchaReason = "AntiCaptchaKey environment variable is not set";

	public RecaptchaPluginTests() {
		Helper.TryGetEnvironmentVariable("TwoCaptchaKey", out _twoCaptchaKey);
		Helper.TryGetEnvironmentVariable("AntiCaptchaKey", out _antiCaptchaKey);
	}
}