using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.BlockResources
{
    public class BlockResourcesPlugin : IPuppeteerExtraPlugin
    {
        public readonly List<BlockRule> BlockResources = new List<BlockRule>();
        public string GetName() => "block-resources";

        public BlockResourcesPlugin(IEnumerable<ResourceType> blockResources = null)
        {
            if (blockResources != null)
                AddRule(builder => builder.BlockedResources(blockResources.ToArray()));
        }

        public BlockRule AddRule(Action<ResourcesBlockBuilder> builderAction)
        {
            var builder = new ResourcesBlockBuilder();
            builderAction(builder);

            var rule = builder.Build();
            this.BlockResources.Add(builder.Build());

            return rule;
        }

        public BlockResourcesPlugin RemoveRule(BlockRule rule)
        {
            BlockResources.Remove(rule);
            return this;
        }


        public async Task OnPageCreated(Page page)
        {
            await page.SetRequestInterceptionAsync(true);
            page.Request += (sender, args) => OnPageRequest(page, args);

        }


        private async void OnPageRequest(Page sender, RequestEventArgs e)
        {
            if (BlockResources.Any(rule => rule.IsRequestBlocked(sender, e.Request)))
            {
                await e.Request.AbortAsync();
                return;
            }

            await e.Request.ContinueAsync();
        }


        public void BeforeLaunch(LaunchOptions options)
        {
            options.Args = options.Args.Append("--site-per-process").Append("--disable-features=IsolateOrigins").ToArray();
        }

        public List<PluginRequirements> Requirements { get; set; }
        public ICollection<IPuppeteerExtraPlugin> Dependencies { get; set; }
    }
}
