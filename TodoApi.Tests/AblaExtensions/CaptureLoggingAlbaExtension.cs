using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace TodoApi.Tests.AblaExtensions;

internal class CaptureLoggingAlbaExtension : IAlbaExtension
{
    private readonly Func<ITestOutputHelper?> helperAccessor;
    private readonly XUnitLoggerOptions loggerOptions;

    public CaptureLoggingAlbaExtension(Func<ITestOutputHelper?> helperAccessor, XUnitLoggerOptions? loggerOptions = null)
    {
        this.helperAccessor = helperAccessor;
        this.loggerOptions = loggerOptions ?? new XUnitLoggerOptions
        {
            UseUtcTimestamp = true,
            IncludeScopes = false,
            IncludeCategory = true,
            IncludeLogLevel = true,
            TimestampFormat = "s",
        };
    }

    IHostBuilder IAlbaExtension.Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddLogging(options =>
            {
                options.AddProvider(
                    new XUnitLoggerProvider(
                        new ProxyTestOutputHelper(helperAccessor),
                        loggerOptions));
            });
        });
        return builder;
    }

    void IDisposable.Dispose()
    { }

    ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;

    Task IAlbaExtension.Start(IAlbaHost host) => Task.CompletedTask;

    private sealed class ProxyTestOutputHelper : ITestOutputHelper
    {
        private readonly Func<ITestOutputHelper?> helperAccessor;

        public ProxyTestOutputHelper(Func<ITestOutputHelper?> helperAccessor)
        {
            this.helperAccessor = helperAccessor;
        }

        public void WriteLine(string message)
        {
            helperAccessor()?.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            helperAccessor()?.WriteLine(format, args);
        }
    }
}
