using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources.Rules;

internal class PageCondition(IPage page) : ICondition
{
    public bool IsApplies(IRequest request)
    {
        return page == request.Frame.Page;
    }
}