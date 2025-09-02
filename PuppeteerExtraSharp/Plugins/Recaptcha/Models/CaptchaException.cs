using System;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Models;

public class CaptchaException(string url, string error) : Exception($"Error while solving reCAPTCHA on {url}: {error}");