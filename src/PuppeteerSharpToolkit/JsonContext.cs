using System.Text.Json.Serialization;

using PuppeteerSharpToolkit.Plugins.Recaptcha.AntiCaptcha;
using PuppeteerSharpToolkit.Plugins.Recaptcha.TwoCaptcha;

namespace PuppeteerSharpToolkit;

[JsonSerializable(typeof(AntiCaptchaRequest))]
[JsonSerializable(typeof(AntiCaptchaTaskResult))]
[JsonSerializable(typeof(AntiCaptchaTaskResultModel))]
[JsonSerializable(typeof(TwoCaptchaResponse))]
[JsonSerializable(typeof(AntiCaptchaRequestForResultTask))]
internal partial class JsonContext : JsonSerializerContext;