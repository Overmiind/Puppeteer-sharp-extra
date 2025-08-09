using System.Text.Json;
using System.Text.Json.Serialization;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha;
using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.TwoCaptcha;

namespace PuppeteerExtraSharpLite;

[JsonSerializable(typeof(AntiCaptchaRequest))]
[JsonSerializable(typeof(AntiCaptchaTaskResult))]
[JsonSerializable(typeof(AntiCaptchaTaskResultModel))]
[JsonSerializable(typeof(TwoCaptchaResponse))]
[JsonSerializable(typeof(AntiCaptchaRequestForResultTask))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(JsonElement))]
internal partial class JsonContext : JsonSerializerContext;
