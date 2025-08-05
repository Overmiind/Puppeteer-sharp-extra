using System.Text.Json;

using PuppeteerExtraSharpLite.Plugins.ExtraStealth.Evasions;
using PuppeteerExtraSharpLite.Tests.Utils;

namespace PuppeteerExtraSharpLite.Tests.StealthPluginTests.EvasionsTests;

public class ContentWindowTest {
    [Fact]
    public async Task IFrameShouldBeObject() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new ContentWindow());

        await using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var finger = await FingerPrint.GetFingerPrint(page);

        Assert.Equal("object", finger.GetProperty("iframeChrome").GetString());
    }

    [Fact]
    public async Task ShouldNotBreakIFrames() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new ContentWindow());

        using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        const string testFuncReturnValue = "TESTSTRING";

        await page.EvaluateFunctionAsync(
            """
            async (testFuncReturnValue) => {
                const { document } = window
                const body = document.querySelector('body')
                const iframe = document.createElement('iframe')
                iframe.srcdoc = 'foobar'
                body.appendChild(iframe)

                // Wait for contentWindow to be set
                await new Promise(resolve => {
                    const check = () => {
                        if (iframe.contentWindow) return resolve()
                        requestAnimationFrame(check)
                    }
                    check()
                })

            iframe.contentWindow.mySuperFunction = () => testFuncReturnValue
            }
            """, testFuncReturnValue);

        var result =
            await page.EvaluateExpressionAsync(
                "document.querySelector('iframe').contentWindow.mySuperFunction()");

        Assert.Equal(testFuncReturnValue, result.ToString());
    }

    [Fact]
    public async Task ShouldCoverAllFrames() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new ContentWindow());

        using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        var basicFrame = await page.EvaluateFunctionAsync(@"() => {
                                    const el = document.createElement('iframe')
                                    document.body.appendChild(el)
                                    return typeof(el.contentWindow.chrome)
                                   }");

        var sandboxSOIFrame = await page.EvaluateFunctionAsync(@"() => {
                                    const el = document.createElement('iframe')
                                    el.setAttribute('sandbox', 'allow-same-origin')
                                    document.body.appendChild(el)
                                    return typeof(el.contentWindow.chrome)
                                   }");

        var sandboxSOASIFrame = await page.EvaluateFunctionAsync(@"() => {
                                    const el = document.createElement('iframe')
                                    el.setAttribute('sandbox', 'allow-same-origin allow-scripts')
                                    document.body.appendChild(el)
                                    return typeof(el.contentWindow.chrome)
                                   }");

        var srcdocIFrame = await page.EvaluateFunctionAsync(@"() => {
                                    const el = document.createElement('iframe')
                                    el.srcdoc = 'blank page, boys.'
                                    document.body.appendChild(el)
                                    return typeof(el.contentWindow.chrome)
                                   }");

        Assert.Equal("object", basicFrame.ToString());
        Assert.Equal("object", sandboxSOIFrame.ToString());
        Assert.Equal("object", sandboxSOASIFrame.ToString());
        Assert.Equal("object", srcdocIFrame.ToString());
    }


    [Fact]
    public async Task ShouldEmulateFeatures() {
        var pluginManager = new PluginManager();
        pluginManager.Register(new ContentWindow());

        using var browser = await pluginManager.LaunchAsync();
        using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://google.com");

        JsonElement results = await page.EvaluateFunctionAsync(@"() => {
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
                                }")
                                ?? new JsonElement();

        Assert.True(results.GetProperty("descriptorsOK").GetBoolean());
        Assert.True(results.GetProperty("doesExist").GetBoolean());
        Assert.True(results.GetProperty("isNotAClone").GetBoolean());
        Assert.True(results.GetProperty("hasSameNumberOfPlugins").GetBoolean());
        Assert.True(results.GetProperty("SelfIsNotWindow").GetBoolean());
        Assert.True(results.GetProperty("SelfIsNotWindowTop").GetBoolean());
        Assert.True(results.GetProperty("TopIsNotSame").GetBoolean());

        Assert.DoesNotContain("at Object.apply", results.GetProperty("StackTraces").GetString());
    }
}