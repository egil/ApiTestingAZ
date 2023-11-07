using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TimeProviderExtensions;

namespace TodoApi.AblaExtensions;

public sealed class UseManualtTimeProvider : IAlbaExtension
{
    public ManualTimeProvider TimeProvider { get; }

    public UseManualtTimeProvider(ManualTimeProvider? timeProvider = null)
    {
        TimeProvider = timeProvider ?? new ManualTimeProvider(); ;
    }

    IHostBuilder IAlbaExtension.Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(TimeProvider);
        });
        return builder;
    }

    void IDisposable.Dispose()
    { }

    ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;

    Task IAlbaExtension.Start(IAlbaHost host) => Task.CompletedTask;
}