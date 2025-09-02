using System.Collections.Generic;
using System.Linq;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources.Rules;

internal class AggregateCondition : ICondition
{
    private readonly IList<ICondition> _conditions = new List<ICondition>();

    public AggregateCondition()
    {
    }

    public AggregateCondition(IList<ICondition> conditions)
    {
        _conditions = conditions;
    }

    public void Add(ICondition condition)
    {
        _conditions.Add(condition);
    }

    public bool IsApplies(IRequest request)
    {
        return _conditions.All(c => c.IsApplies(request));
    }
}