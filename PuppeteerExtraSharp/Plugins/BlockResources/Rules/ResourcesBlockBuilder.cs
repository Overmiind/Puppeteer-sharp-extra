using System;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources.Rules;

public class ResourcesBlockBuilder
{
    private readonly AggregateCondition _aggregateCondition = new();

    internal ResourcesBlockBuilder()
    {
    }

    public ResourcesBlockBuilder Resources(params ResourceType[] resources)
    {
        _aggregateCondition.Add(new ResourceCondition(resources));
        return this;
    }

    public ResourcesBlockBuilder Page(IPage page)
    {
        _aggregateCondition.Add(new PageCondition(page));
        return this;
    }

    public ResourcesBlockBuilder Url(string pattern)
    {
        _aggregateCondition.Add(new UrlPatternCondition(pattern));
        return this;
    }

    public ResourcesBlockBuilder Custom(Func<IRequest, bool> customCondition)
    {
        _aggregateCondition.Add(new PredicateCondition(customCondition));
        return this;
    }

    internal IBlockRule Build()
    {
        return new PredicateBlockRule(e => _aggregateCondition.IsApplies(e));
    }
}