using System.Text.Json.Serialization;

using PuppeteerExtraSharpLite.Plugins.Recaptcha.Provider.AntiCaptcha.Models;

namespace PuppeteerExtraSharpLite.Plugins.Recaptcha.RestClient;

[JsonSerializable(typeof(AntiCaptchaTaskResult))]
internal partial class JsonContext : JsonSerializerContext { }
