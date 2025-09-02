using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources.Rules;

public interface IBlockRule
{
    public bool ShouldBlock(IRequest request);
}