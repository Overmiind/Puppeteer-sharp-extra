using System;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests;

public class PageInitializationBrowserTests
{
    private static readonly TimeSpan Deadline = TimeSpan.FromSeconds(10);

    [ChromeFact]
    public async Task StealthIsReadyBeforeTheFirstPageScriptRuns()
    {
        var extra = new PuppeteerExtra().Use(new StealthPlugin());
        await using var owned = await OwnedBrowser.StartAsync(extra);
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var context = await owned.Browser.CreateBrowserContextAsync();
            try
            {
                var page = await context.NewPageAsync();
                await extra.PreparePageAsync(page).WaitAsync(Deadline);
                await page.GoToAsync("data:text/html,<script>window.firstScript={webdriver:navigator.webdriver,cores:navigator.hardwareConcurrency}</script>");

                Assert.False(await page.EvaluateExpressionAsync<bool>("window.firstScript.webdriver"));
                Assert.Equal(4, await page.EvaluateExpressionAsync<int>("window.firstScript.cores"));
            }
            finally { await context.CloseAsync().WaitAsync(Deadline); }
        }
    }

    [ChromeTheory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ClosingAPageDuringSetupDoesNotCrashAndTheNextPageWorks(bool connect)
    {
        var plugin = new DeferredPlugin();
        var extra = new PuppeteerExtra().Use(plugin);
        await using var owned = await OwnedBrowser.StartAsync(extra, connect);
        for (var attempt = 0; attempt < 10; attempt++)
        {
            plugin.Arm();
            var context = await owned.Browser.CreateBrowserContextAsync();
            try
            {
                var page = await context.NewPageAsync();
                await plugin.Started.Task.WaitAsync(Deadline);
                using var cancellation = new CancellationTokenSource();
                var preparation = extra.PreparePageAsync(page, cancellation.Token);
                cancellation.Cancel();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => preparation);
                await page.CloseAsync().WaitAsync(Deadline);
                plugin.Release.TrySetResult();
                await Assert.ThrowsAsync<TargetClosedException>(() => extra.PreparePageAsync(page).WaitAsync(Deadline));
                Assert.Equal(1, plugin.Calls);

                plugin.Disarm();
                var next = await context.NewPageAsync();
                await extra.PreparePageAsync(next).WaitAsync(Deadline);
                await next.GoToAsync("data:text/html,<script>document.title='next-page-ok'</script>");
                Assert.Equal("next-page-ok", await next.GetTitleAsync());
            }
            finally
            {
                plugin.Release.TrySetResult();
                await context.CloseAsync().WaitAsync(Deadline);
            }
        }
    }

    [ChromeFact]
    public async Task AutomaticSetupReportsUnexpectedFailuresAndAnExplicitWaiterCanObserveThem()
    {
        var plugin = new DeferredPlugin();
        var extra = new PuppeteerExtra().Use(plugin);
        await using var owned = await OwnedBrowser.StartAsync(extra);
        var reported = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
        extra.PluginError += (_, error) => reported.TrySetResult(error.GetException());
        var failure = new InvalidOperationException("test plugin failure");
        plugin.Arm(failure);
        var context = await owned.Browser.CreateBrowserContextAsync();
        try
        {
            var page = await context.NewPageAsync();
            await plugin.Started.Task.WaitAsync(Deadline);
            plugin.Release.TrySetResult();

            Assert.Same(failure, await reported.Task.WaitAsync(Deadline));
            Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => extra.PreparePageAsync(page)));
            Assert.Equal(1, plugin.Calls);
        }
        finally
        {
            plugin.Release.TrySetResult();
            await context.CloseAsync().WaitAsync(Deadline);
        }
    }

    private sealed class DeferredPlugin() : PuppeteerExtraPlugin("deferred-test")
    {
        public TaskCompletionSource Started { get; private set; }
        public TaskCompletionSource Release { get; private set; }
        public int Calls { get; private set; }
        private bool _armed;
        private Exception _failure;

        public void Arm(Exception failure = null)
        {
            Started = new(TaskCreationOptions.RunContinuationsAsynchronously);
            Release = new(TaskCreationOptions.RunContinuationsAsynchronously);
            Calls = 0;
            _failure = failure;
            _armed = true;
        }

        public void Disarm() => _armed = false;

        protected override async Task OnPageCreatedAsync(IPage page)
        {
            if (!_armed) return;
            Calls++;
            Started.TrySetResult();
            await Release.Task;
            if (_failure != null) throw _failure;
            await page.EvaluateExpressionOnNewDocumentAsync("window.pluginReady = true");
        }
    }

    private sealed class OwnedBrowser(IBrowser browser, IBrowser owner) : IAsyncDisposable
    {
        public IBrowser Browser { get; } = browser;

        public static async Task<OwnedBrowser> StartAsync(PuppeteerExtra extra, bool connect = false)
        {
            var options = new LaunchOptions
            {
                ExecutablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH"),
                Headless = true,
                Timeout = 15000,
                ProtocolTimeout = 10000,
                Args = ["--no-sandbox"]
            };
            if (!connect)
            {
                var browser = await extra.LaunchAsync(options).WaitAsync(TimeSpan.FromSeconds(20));
                return new(browser, browser);
            }

            var owner = await Puppeteer.LaunchAsync(options);
            try
            {
                var browser = await extra.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = owner.WebSocketEndpoint });
                return new(browser, owner);
            }
            catch
            {
                await owner.CloseAsync();
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            var process = owner.Process;
            try { await owner.CloseAsync().WaitAsync(Deadline); }
            finally
            {
                Browser.Disconnect();
                owner.Disconnect();
                if (process is { HasExited: false })
                {
                    process.Kill(entireProcessTree: true);
                    await process.WaitForExitAsync().WaitAsync(Deadline);
                }
                process?.Dispose();
            }
        }
    }

    public sealed class ChromeFactAttribute : FactAttribute
    {
        public ChromeFactAttribute()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH")))
                Skip = "Set PUPPETEER_EXECUTABLE_PATH to run the local Chrome tests.";
        }
    }

    public sealed class ChromeTheoryAttribute : TheoryAttribute
    {
        public ChromeTheoryAttribute()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH")))
                Skip = "Set PUPPETEER_EXECUTABLE_PATH to run the local Chrome tests.";
        }
    }
}
