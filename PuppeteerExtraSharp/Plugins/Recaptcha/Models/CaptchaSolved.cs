using System;

namespace PuppeteerExtraSharp.Plugins.Recaptcha.Models;

public class CaptchaSolved
{
    public string Vendor { get; set; }
    public string Id { get; set; }
    public bool? ResponseElement { get; set; }
    public bool? ResponseCallback { get; set; }
    public DateTime? SolvedAt { get; set; }
    public string Error { get; set; }
    public bool? IsSolved { get; set; }
}