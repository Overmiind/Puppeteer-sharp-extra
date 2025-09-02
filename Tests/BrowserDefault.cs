using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins;
using PuppeteerSharp;

namespace Extra.Tests
{
    public abstract class BrowserDefault : IDisposable
    {
        private readonly List<IBrowser> _launchedBrowsers = new List<IBrowser>();

        protected async Task<IBrowser> LaunchAsync(LaunchOptions options = null)
        {
            //DownloadChromeIfNotExists();
            options ??= CreateDefaultOptions();

            var browser = await Puppeteer.LaunchAsync(options);
            _launchedBrowsers.Add(browser);
            return browser;
        }

        protected async Task<IBrowser> LaunchWithPluginAsync(PuppeteerExtraPlugin plugin, LaunchOptions options = null)
        {
            var extra = new PuppeteerExtra().Use(plugin);
            //DownloadChromeIfNotExists();
            options ??= CreateDefaultOptions();

            var browser = await extra.LaunchAsync(options);
            _launchedBrowsers.Add(browser);
            return browser;
        }

        protected async Task<IPage> LaunchAndGetPageAsync(PuppeteerExtraPlugin plugin = null)
        {
            IBrowser browser = null;
            if (plugin != null)
                browser = await LaunchWithPluginAsync(plugin);
            else
                browser = await LaunchAsync();

            var page = (await browser.PagesAsync())[0];

            return page;
        }


        private async Task DownloadChromeIfNotExistsAsync()
        {
            if (File.Exists(Constants.PathToChrome))
                return;

            await new BrowserFetcher(new BrowserFetcherOptions()
            {
                Path = Constants.PathToChrome
            }).DownloadAsync();
        }

        protected LaunchOptions CreateDefaultOptions()
        {
            return new LaunchOptions()
            {
                ExecutablePath = Constants.PathToChrome,
                Headless = Constants.Headless
            };
        }

        public void Dispose()
        {
            foreach (var launchedBrowser in _launchedBrowsers)
            {
                launchedBrowser.CloseAsync().Wait();
            }
        }
    }
}
