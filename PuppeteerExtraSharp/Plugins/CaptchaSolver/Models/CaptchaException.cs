using System;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;

public class CaptchaException(string url, string error) : Exception($"Error while solving CAPTCHA on {url}: {error}");