using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources.Rules;

internal interface ICondition
{
    public bool IsApplies(IRequest request);
}