using System.Text.Json.Serialization;

// using PuppeteerSharpToolkit.Plugins.Recaptcha.AntiCaptcha;
using PuppeteerSharpToolkit.Plugins.Recaptcha.TwoCaptcha;

namespace PuppeteerSharpToolkit;

// [JsonSerializable(typeof(AntiCaptchaTaskResult))]
// [JsonSerializable(typeof(AntiCaptchaTaskResultFull))]
[JsonSerializable(typeof(TwoCaptchaResponse))]
internal partial class JsonContext : JsonSerializerContext;