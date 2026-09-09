using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins;
using PuppeteerSharp;
using Xunit;

namespace Extra.Tests;

public class PageInitializationTests
{
    [Fact]
    public async Task ConcurrentWaitersRunPluginsOnceAndInOrder()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        var nextRan = false;
        var extra = new PuppeteerExtra()
            .Use(new PagePlugin(async _ =>
            {
                Interlocked.Increment(ref calls);
                started.SetResult();
                await release.Task;
            }))
            .Use(new PagePlugin(_ => { nextRan = true; return Task.CompletedTask; }));
        var page = DispatchProxy.Create<IPage, UnusedPage>();

        var waits = Enumerable.Range(0, 8).Select(_ => Task.Run(() => extra.PreparePageAsync(page))).ToArray();
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.False(nextRan);
        Assert.All(waits, wait => Assert.False(wait.IsCompleted));
        release.SetResult();
        await Task.WhenAll(waits).WaitAsync(TimeSpan.FromSeconds(5));
        await extra.PreparePageAsync(page);

        Assert.Equal(1, calls);
        Assert.True(nextRan);
    }

    [Fact]
    public async Task FailedPreparationRemainsObservableWithoutRunningPluginsAgain()
    {
        var expected = new InvalidOperationException("plugin failed");
        var calls = 0;
        var extra = new PuppeteerExtra().Use(new PagePlugin(_ =>
        {
            calls++;
            throw expected;
        }));
        var page = DispatchProxy.Create<IPage, UnusedPage>();

        var first = await Assert.ThrowsAsync<InvalidOperationException>(() => extra.PreparePageAsync(page));
        var second = await Assert.ThrowsAsync<InvalidOperationException>(() => extra.PreparePageAsync(page));

        Assert.Same(expected, first);
        Assert.Same(expected, second);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task CancellingOneWaiterDoesNotCancelOtherWaiters()
    {
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var extra = new PuppeteerExtra().Use(new PagePlugin(_ => release.Task));
        var page = DispatchProxy.Create<IPage, UnusedPage>();
        using var cancellation = new CancellationTokenSource();

        var cancelled = extra.PreparePageAsync(page, cancellation.Token);
        var other = extra.PreparePageAsync(page);
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => cancelled);
        Assert.False(other.IsCompleted);
        release.SetResult();
        await other.WaitAsync(TimeSpan.FromSeconds(5));
    }

    public class UnusedPage : DispatchProxy
    {
        protected override object Invoke(MethodInfo targetMethod, object[] args)
            => throw new NotSupportedException(targetMethod.Name);
    }

    private sealed class PagePlugin(Func<IPage, Task> initialize) : PuppeteerExtraPlugin("test-page")
    {
        protected override Task OnPageCreatedAsync(IPage page) => initialize(page);
    }
}
