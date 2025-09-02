using System;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources.Rules;

internal class PredicateBlockRule(Func<IRequest, bool> predicate) : IBlockRule
{
    public bool ShouldBlock(IRequest request)
    {
        return predicate(request);
    }
}