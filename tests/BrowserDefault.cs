using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins;
using PuppeteerSharp;

namespace Extra.Tests
{
    public abstract class BrowserDefault : IDisposable
    {
        private readonly List<Browser> _launchedBrowsers = new List<Browser>();
        protected BrowserDefault()
        {
        }

        protected async Task<Browser> LaunchAsync(LaunchOptions options = null)
        {
            //DownloadChromeIfNotExists();
            options ??= CreateDefaultOptions();

            var browser = await Puppeteer.LaunchAsync(options);
            _launchedBrowsers.Add(browser);
            return browser;
        }

        protected async Task<Browser> LaunchWithPluginAsync(IPuppeteerExtraPlugin plugin, LaunchOptions options = null)
        {
            var extra = new PuppeteerExtra().Use(plugin);
            //DownloadChromeIfNotExists();
            options ??= CreateDefaultOptions();

            var browser = await extra.LaunchAsync(options);
            _launchedBrowsers.Add(browser);
            return browser;
        }

        private async void DownloadChromeIfNotExists()
        {
            if (File.Exists(Constants.PathToChrome))
                return;

            await new BrowserFetcher(new BrowserFetcherOptions()
            {
                Path = Constants.PathToChrome
            }).DownloadAsync(BrowserFetcher.DefaultRevision);
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
