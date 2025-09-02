using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerExtraSharp.Plugins.BlockResources;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests.BlockResourcesTests
{
    public class BlockResourcesPluginTests : BrowserDefault
    {
        private const string TestUrl = "https://adblock.turtlecute.org/";
        private const string BlockedByClientError = "ERR_BLOCKED_BY_CLIENT";
        private const string AbortedError = "ERR_ABORTED";

        [Fact]
        public async Task ShouldBlockPage()
        {
            var plugin = new BlockResourcesPlugin();
            await using var actualPage = await LaunchAndGetPageAsync(plugin);
            var newPage = await actualPage.Browser.NewPageAsync();

            plugin.AddRule(e => e.Page(actualPage).Url("ads.js"));

            await actualPage.GoToAsync(TestUrl);
            await newPage.GoToAsync(TestUrl);

            var actualPageClasses = await GetClassNames(actualPage, "#sfa_1");
            var newPageClasses = await GetClassNames(newPage, "#sfa_1");

            Assert.Contains("_bg-green", actualPageClasses);
            Assert.Contains("_bg-red", newPageClasses);
        }

        [Fact]
        public async Task ShouldBlockResources()
        {
            var plugin = new BlockResourcesPlugin();
            await using var page = await LaunchAndGetPageAsync(plugin);

            plugin.AddRule(e => e.Resources(ResourceType.Script));

            var requests = await ReadRequests(page, TestUrl);

            var scriptRequests = requests.Where(e => e.ResourceType == ResourceType.Script).ToList();
            var otherRequests = requests.Except(scriptRequests).ToList();

            Assert.NotEmpty(scriptRequests);
            Assert.All(scriptRequests,
                request => Assert.Contains(BlockedByClientError, request.FailureText));

            Assert.All(otherRequests, request => Assert.Null(request.FailureText));
        }

        [Fact]
        public async Task ShouldBlockUrl()
        {
            var plugin = new BlockResourcesPlugin();
            await using var page = await LaunchAndGetPageAsync(plugin);

            plugin.AddRule(e => e.Url("ads.js"));

            var requests = await ReadRequests(page, TestUrl);

            var scriptRequests = requests.Where(e => e.Url.Contains("ads.js")).ToList();
            var otherRequests = requests.Except(scriptRequests).ToList();

            Assert.NotEmpty(scriptRequests);
            Assert.All(scriptRequests,
                request => Assert.Contains(BlockedByClientError, request.FailureText));

            Assert.All(otherRequests, request => Assert.DoesNotContain(BlockedByClientError, request.FailureText));
        }

        [Fact]
        public async Task ShouldAddCustomBlockReason()
        {
            var plugin = new BlockResourcesPlugin(RequestAbortErrorCode.Aborted);
            await using var page = await LaunchAndGetPageAsync(plugin);

            plugin.AddRule(e => e.Resources(ResourceType.Script));

            var requests = await ReadRequests(page, TestUrl);

            var scriptRequests = requests.Where(e => e.ResourceType == ResourceType.Script).ToList();

            Assert.NotEmpty(scriptRequests);
            Assert.All(scriptRequests,
                request => Assert.Contains(AbortedError, request.FailureText));
        }

        [Fact]
        public async Task ShouldWorkWithCustomCondition()
        {
            var plugin = new BlockResourcesPlugin(RequestAbortErrorCode.Aborted);
            await using var page = await LaunchAndGetPageAsync(plugin);

            plugin.AddRule(e =>
                e.Custom(request => !request.IsNavigationRequest));

            var requests = await ReadRequests(page, TestUrl);

            var navigationRequests = requests.Where(request => request.IsNavigationRequest).ToList();
            var suspendedRequests = requests.Except(navigationRequests).ToList();


            Assert.All(navigationRequests, request => Assert.Null(request.FailureText));

            Assert.NotEmpty(suspendedRequests);
            Assert.All(suspendedRequests,
                request => Assert.Contains(AbortedError, request.FailureText));
        }

        [Fact]
        public async Task ShouldWorkWithMultipleConditions()
        {
            var plugin = new BlockResourcesPlugin();
            await using var actualPage = await LaunchAndGetPageAsync(plugin);
            var newPage = await actualPage.Browser.NewPageAsync();

            plugin.AddRule(e => e
                .Page(actualPage)
                .Resources(ResourceType.Script)
                .Url("ads.js"));

            var actualRequests = await ReadRequests(actualPage, TestUrl);
            var newPageRequests = await ReadRequests(newPage, TestUrl);

            var actualBlocked = actualRequests
                .Where(r => r.ResourceType == ResourceType.Script && r.Url.Contains("ads.js"))
                .ToList();
            var actualOthers = actualRequests.Except(actualBlocked).ToList();

            Assert.NotEmpty(actualBlocked);
            Assert.All(actualBlocked, r => Assert.Contains(BlockedByClientError, r.FailureText));
            Assert.All(actualOthers, r => Assert.DoesNotContain(BlockedByClientError, r.FailureText));

            Assert.All(newPageRequests, r => Assert.DoesNotContain(BlockedByClientError, r.FailureText));
        }

        private async Task<IReadOnlyList<IRequest>> ReadRequests(IPage page, string url)
        {
            var requests = new List<IRequest>();

            void OnRequest(object _, RequestEventArgs e) => requests.Add(e.Request);

            try
            {
                page.Request += OnRequest;
                await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);
            }
            finally
            {
                page.Request -= OnRequest;
            }

            return requests;
        }

        private async Task<string> GetClassNames(IPage page, string elementId)
        {
            var selector = await page.QuerySelectorAsync(elementId);
            return await selector.EvaluateFunctionAsync<string>("el => el.className");
        }
    }
}