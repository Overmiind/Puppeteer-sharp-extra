using PuppeteerExtraSharp.Plugins.CaptchaSolver.Enums;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;

public class CaptchaSolution
{
    public string Id { get; set; }
    public string Vendor { get; set; }
    public string Payload { get; set; }
}
