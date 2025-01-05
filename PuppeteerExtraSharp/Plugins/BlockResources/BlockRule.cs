using System.Collections.Generic;
using System.Text.RegularExpressions;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources;

public class BlockRule
{
    public string SitePattern;
    public IPage IPage;
    public HashSet<ResourceType> ResourceType = [];

    internal BlockRule()
    {
    }

    public bool IsRequestBlocked(IPage fromPage, IRequest request)
    {
        if (!IsResourcesBlocked(request.ResourceType))
            return false;

        return IsSiteBlocked(request.Url) || IsPageBlocked(fromPage);
    }


    public bool IsPageBlocked(IPage page)
    {
        return IPage != null && page.Equals(IPage);
    }

    public bool IsSiteBlocked(string siteUrl)
    {
        return !string.IsNullOrWhiteSpace(SitePattern) && Regex.IsMatch(siteUrl, SitePattern);
    }

    public bool IsResourcesBlocked(ResourceType resource)
    {
        return ResourceType.Contains(resource);
    }
}
