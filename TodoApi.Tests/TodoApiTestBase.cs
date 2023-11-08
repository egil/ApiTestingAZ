using TimeProviderExtensions;

namespace TodoApi.Tests;

/// <summary>
/// A base case for API tests that wants to share a single
/// <see cref="TodoApiFixture"/> and database. It resets the database
/// before each test run to ensure consistent execution of tests.
/// </summary>
[Collection(nameof(ApiTestCollection))]
public abstract class TodoApiTestBase : IAsyncLifetime
{
    private readonly TodoApiFixture fixture;
    private IServiceScope? serviceScope;

    public IAlbaHost Host { get; }

    public ManualTimeProvider TimeProvider { get; }

    public IServiceProvider Services
    {
        get
        {
            serviceScope ??= Host.Services.CreateScope();
            return serviceScope.ServiceProvider;
        }
    }

    protected TodoApiTestBase(TodoApiFixture fixture)
    {
        this.fixture = fixture;
        Host = fixture.AlbaHost;
        TimeProvider = fixture.TimeProvider;
    }

    public Task InitializeAsync()
    {
        // Reset the database before running tests.
        // This is safer compared to doing it after a test completes,
        // since a test run may exit early/crash leaving
        // the database in an unexpected state.
        return fixture.ResetDatabase();
    }

    public Task DisposeAsync()
    {
        serviceScope?.Dispose();
        return Task.CompletedTask;
    }
}