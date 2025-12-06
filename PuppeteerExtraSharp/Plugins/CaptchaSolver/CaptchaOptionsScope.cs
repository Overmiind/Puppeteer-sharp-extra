using System;
using System.Threading;
using PuppeteerExtraSharp.Plugins.CaptchaSolver.Models;

namespace PuppeteerExtraSharp.Plugins.CaptchaSolver;

public class CaptchaOptionsScope(CaptchaOptions defaultOptions)
{
    private readonly AsyncLocal<CaptchaOptions> _currentScope = new();
    private readonly CaptchaOptions _defaultOptions = defaultOptions ?? new CaptchaOptions();

    public CaptchaOptions Current => _currentScope.Value ?? _defaultOptions;

    public IDisposable CreateScope(CaptchaOptions overrides)
    {
        var parent = _currentScope.Value;
        _currentScope.Value = overrides;
        return new ScopeDisposer(this, parent);
    }

    public IDisposable CreateScope(Func<CaptchaOptions, CaptchaOptions> configure)
    {
        var current = Current;
        var newOptions = configure(current);
        return CreateScope(newOptions);
    }

    private class ScopeDisposer(CaptchaOptionsScope provider, CaptchaOptions parentSettings)
        : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            provider._currentScope.Value = parentSettings;
            _disposed = true;
        }
    }
}