using System.Text.Json.Serialization;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CaptchaType
{
    invisible,
    checkbox,
    score,
}