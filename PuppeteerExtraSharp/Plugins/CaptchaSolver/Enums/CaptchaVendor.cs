using System.Text.Json.Serialization;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CaptchaVendor
{
    Google,
    HCaptcha,
    DataDome,
    Cloudflare,
    GeeTest
}
