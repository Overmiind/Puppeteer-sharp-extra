using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources;

public class ResourcesBlockBuilder
{
    private BlockRule Rule { get; set; } = new();

    public ResourcesBlockBuilder BlockedResources(params ResourceType[] resources)
    {
        foreach (var resourceType in resources)
        {
            Rule.ResourceType.Add(resourceType);
        }

        return this;
    }

    public ResourcesBlockBuilder OnlyForPage(IPage page)
    {
        Rule.IPage = page;
        return this;
    }

    public ResourcesBlockBuilder ForUrl(string pattern)
    {
        Rule.SitePattern = pattern;
        return this;
    }

    internal BlockRule Build()
    {
        return Rule;
    }
}
