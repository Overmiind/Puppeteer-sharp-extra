using System;
using System.Threading.Tasks;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests.SourceUrl;

public class SourceUrlTest : BrowserDefault
{
    private readonly string _pageUrl = Environment.CurrentDirectory +
                                       "\\StealthPluginTests\\EvasionsTests\\SourceUrl\\fixtures\\Test.html";

    [Fact]
    public async Task ShouldWork()
    {
        var plugin = new PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions.SourceUrl();

        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync(_pageUrl, WaitUntilNavigation.Load);

        await page.EvaluateExpressionAsync("document.querySelector('title')");
        var result =
            await page.EvaluateExpressionAsync<string>(
                "document.querySelector('#result').innerText");

        var result2 = await page.EvaluateFunctionAsync<string>(@"() => {
                                try {
                                   Function.prototype.toString.apply({})
                                } catch (err) {
                                   return err.stack
                                }}");

        Assert.Equal("PASS", result);
        Assert.DoesNotContain("__puppeteer_evaluation_script", result2);
    }
}
