using System.Linq;
using System.Threading.Tasks;
using Extra.Tests.Utils;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using Xunit;

namespace Extra.Tests.StealthPluginTests.EvasionsTests;

public class ContentWindowTest : BrowserDefault
{
    [Fact]
    public async Task IFrameShouldBeObject()
    {
        var plugin = new StealthPlugin();

        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var finger = await new FingerPrint().GetFingerPrint(page);

        Assert.Equal("object", finger["iframeChrome"].GetValue<string>());
    }

    [Fact]
    public async Task ShouldNotBreakIFrames()
    {
        var plugin = new StealthPlugin();

        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        const string testFuncReturnValue = "TESTSTRING";

        await page.EvaluateFunctionAsync(@"(testFuncReturnValue) => {
                        const { document } = window // eslint-disable-line
                            const body = document.querySelector('body')
                            const iframe = document.createElement('iframe')
                            iframe.srcdoc = 'foobar'
                            iframe.contentWindow.mySuperFunction = () => testFuncReturnValue
                            body.appendChild(iframe)
                        }",
            testFuncReturnValue);

        var result =
            await page.EvaluateExpressionAsync<string>(
                "document.querySelector('iframe').contentWindow.mySuperFunction()");

        Assert.Equal(testFuncReturnValue, result);
    }

    [Fact]
    public async Task ShouldCoverAllFrames()
    {
        var plugin = new StealthPlugin();

        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");


        var basicFrame = await page.EvaluateFunctionAsync<string>(@"() => {
                                    const el = document.createElement('iframe')
                                    document.body.appendChild(el)
                                    return typeof(el.contentWindow.chrome)
                                   }");

        var sandboxSOIFrame = await page.EvaluateFunctionAsync<string>(@"() => {
                                    const el = document.createElement('iframe')
                                    el.setAttribute('sandbox', 'allow-same-origin')
                                    document.body.appendChild(el)
                                    return typeof(el.contentWindow.chrome)
                                   }");

        var sandboxSOASIFrame = await page.EvaluateFunctionAsync<string>(@"() => {
                                    const el = document.createElement('iframe')
                                    el.setAttribute('sandbox', 'allow-same-origin allow-scripts')
                                    document.body.appendChild(el)
                                    return typeof(el.contentWindow.chrome)
                                   }");

        var srcdocIFrame = await page.EvaluateFunctionAsync<string>(@"() => {
                                    const el = document.createElement('iframe')
                                    el.srcdoc = 'blank page, boys.'
                                    document.body.appendChild(el)
                                    return typeof(el.contentWindow.chrome)
                                   }");

        Assert.Equal("object", basicFrame);
        Assert.Equal("object", sandboxSOIFrame);
        Assert.Equal("object", sandboxSOASIFrame);
        Assert.Equal("object", srcdocIFrame);
    }


    [Fact]
    public async Task ShouldEmulateFeatures()
    {
        var plugin = new StealthPlugin();

        var page = await LaunchAndGetPage(plugin);
        await page.GoToAsync("https://google.com");

        var results = await page.EvaluateFunctionAsync(@"() => {
                                const results = {}
                                
                                    const iframe = document.createElement('iframe')
                                    iframe.srcdoc = 'page intentionally left blank' // Note: srcdoc
                                    document.body.appendChild(iframe)
                                
                                    const basicIframe = document.createElement('iframe')
                                    basicIframe.src = 'data:text/plain;charset=utf-8,foobar'
                                    document.body.appendChild(iframe)
                                
                                    results.descriptorsOK = (() => {
                                      // Verify iframe prototype isn't touched
                                      const descriptors = Object.getOwnPropertyDescriptors(
                                        HTMLIFrameElement.prototype
                                      )
                                      const str = descriptors.contentWindow.get.toString()
                                      return str === `function get contentWindow() { [native code] }`
                                    })()
                                
                                    results.noProxySignature = (() => {
                                      return iframe.srcdoc.toString.hasOwnProperty('[[IsRevoked]]') // eslint-disable-line
                                    })()
                                
                                    results.doesExist = (() => {
                                      // Verify iframe isn't remapped to main window
                                      return !!iframe.contentWindow
                                    })()
                                
                                    results.isNotAClone = (() => {
                                      // Verify iframe isn't remapped to main window
                                      return iframe.contentWindow !== window
                                    })()
                                
                                    results.hasSameNumberOfPlugins = (() => {
                                      return (
                                        window.navigator.plugins.length ===
                                        iframe.contentWindow.navigator.plugins.length
                                      )
                                    })()
                                
                                    results.SelfIsNotWindow = (() => {
                                      return iframe.contentWindow.self !== window
                                    })()
                                
                                    results.SelfIsNotWindowTop = (() => {
                                      return iframe.contentWindow.self !== window.top
                                    })()
                                
                                    results.TopIsNotSame = (() => {
                                      return iframe.contentWindow.top !== iframe.contentWindow
                                    })()
                                
                                    results.FrameElementMatches = (() => {
                                      return iframe.contentWindow.frameElement === iframe
                                    })()
                                
                                    results.StackTraces = (() => {
                                      try {
                                        // eslint-disable-next-line
                                        document['createElement'](0)
                                      } catch (e) {
                                        return e.stack
                                      }
                                      return false
                                      })()

                                      return results
                                }");


        Assert.NotNull(results);
        Assert.True(results.Value.GetProperty("descriptorsOK").GetBoolean());
        Assert.True(results.Value.GetProperty("doesExist").GetBoolean());
        Assert.True(results.Value.GetProperty("isNotAClone").GetBoolean());
        Assert.True(results.Value.GetProperty("hasSameNumberOfPlugins").GetBoolean());
        Assert.True(results.Value.GetProperty("SelfIsNotWindow").GetBoolean());
        Assert.True(results.Value.GetProperty("SelfIsNotWindowTop").GetBoolean());
        Assert.True(results.Value.GetProperty("TopIsNotSame").GetBoolean());

        Assert.DoesNotContain("at Object.apply", results.Value.GetProperty("StackTraces").GetString());
    }
}
