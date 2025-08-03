using System.Text.Json.Serialization;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider._2Captcha.Models;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.RestClient;

[JsonSerializable(typeof(AntiCaptchaRequest))]
[JsonSerializable(typeof(AntiCaptchaTaskResult))]
[JsonSerializable(typeof(TwoCaptchaResponse))]
internal partial class JsonContext : JsonSerializerContext { }
