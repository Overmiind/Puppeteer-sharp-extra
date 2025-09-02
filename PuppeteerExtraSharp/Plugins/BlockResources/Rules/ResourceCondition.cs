using System.Linq;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources.Rules;

internal class ResourceCondition(params ResourceType[] resourceTypes) : ICondition
{
    public bool IsApplies(IRequest request)
    {
        return resourceTypes.Contains(request.ResourceType);
    }
}