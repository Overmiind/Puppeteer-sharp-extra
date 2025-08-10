using System.Text.Json;
using System.Text.Json.Serialization;

using PuppeteerSharpToolkit.Plugins.Recaptcha.Provider.AntiCaptcha;
using PuppeteerSharpToolkit.Plugins.Recaptcha.Provider.TwoCaptcha;

namespace PuppeteerSharpToolkit;

[JsonSerializable(typeof(AntiCaptchaRequest))]
[JsonSerializable(typeof(AntiCaptchaTaskResult))]
[JsonSerializable(typeof(AntiCaptchaTaskResultModel))]
[JsonSerializable(typeof(TwoCaptchaResponse))]
[JsonSerializable(typeof(AntiCaptchaRequestForResultTask))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(JsonElement))]
internal partial class JsonContext : JsonSerializerContext;
