using System;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources.Rules;

public class PredicateCondition(Func<IRequest, bool> condition) : ICondition
{
    public bool IsApplies(IRequest request)
    {
        return condition(request);
    }
}