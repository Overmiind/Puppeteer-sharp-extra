using System.Collections.Generic;
using System.Text.RegularExpressions;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources
{
    public class BlockRule
    {
        public string SitePattern;
        public Page Page;
        public HashSet<ResourceType> ResourceType = new HashSet<ResourceType>();

        internal BlockRule()
        {

        }

        public bool IsRequestBlocked(Page fromPage, Request request)
        {
            if (!IsResourcesBlocked(request.ResourceType))
                return false;

            return IsSiteBlocked(request.Url) || IsPageBlocked(fromPage);
        }


        public bool IsPageBlocked(Page page)
        {
            return Page != null && page.Equals(Page);
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
}
