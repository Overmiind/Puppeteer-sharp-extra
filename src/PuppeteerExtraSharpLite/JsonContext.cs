using System.Text.Json;
using System.Text.Json.Serialization;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha.Models;
using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.TwoCaptcha;

namespace PuppeteerExtraSharpLite;

[JsonSerializable(typeof(AntiCaptchaRequest))]
[JsonSerializable(typeof(AntiCaptchaTaskResult))]
[JsonSerializable(typeof(TwoCaptchaResponse))]
[JsonSerializable(typeof(RequestForResultTask))]
[JsonSerializable(typeof(TaskResultModel))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(JsonElement))]
internal partial class JsonContext : JsonSerializerContext { }
