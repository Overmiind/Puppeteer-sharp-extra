using System.Text.RegularExpressions;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources.Rules;

internal class UrlPatternCondition(string urlPattern) : ICondition
{
    public bool IsApplies(IRequest request)
    {
        return Regex.IsMatch(request.Url, urlPattern);
    }
}