using System.Collections.Generic;
namespace PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;

public class EnterCaptchaSolutionsResult
{
    public ICollection<CaptchaSolved> Solved { get; set; } = new List<CaptchaSolved>();
    public ICollection<FilteredCaptcha> Filtered { get; set; } = new List<FilteredCaptcha>();
    public string Error { get; set; }
}
