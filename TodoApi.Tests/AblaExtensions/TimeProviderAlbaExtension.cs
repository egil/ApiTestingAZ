﻿using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TimeProviderExtensions;

namespace TodoApi.Tests.AblaExtensions;

public sealed class TimeProviderAlbaExtension : IAlbaExtension
{
    public ManualTimeProvider TimeProvider { get; }

    public TimeProviderAlbaExtension(ManualTimeProvider? timeProvider = null)
    {
        TimeProvider = timeProvider ?? new ManualTimeProvider();
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