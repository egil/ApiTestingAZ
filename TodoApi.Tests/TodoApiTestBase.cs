using TimeProviderExtensions;
using Xunit.Abstractions;

namespace TodoApi.Tests;

/// <summary>
/// A base case for API tests that wants to share a single
/// <see cref="TodoApiFixture"/> and database. It resets the database
/// before each test run to ensure consistent execution of tests.
/// </summary>
[Collection(nameof(ApiTestCollection))]
public abstract partial class TodoApiTestBase : IAsyncLifetime, IDisposable
{
    private readonly TodoApiFixture fixture;
    private readonly ITestOutputHelper testOutputHelper;
    private IServiceScope? serviceScope;
    private bool disposedValue;

    public IAlbaHost Host { get; }

    /// <summary>
    /// Gets the manual <see cref="ManualTimeProvider"/> that is used by the
    /// API being tested.
    /// </summary>
    public ManualTimeProvider TimeProvider { get; }

    /// <summary>
    /// Create a <see cref="IServiceProvider"/> which is scoped to the test
    /// currently running (fact or theory). After a test finish running,
    /// the service provider is disposed along with any scoped services retrieved from it.
    /// </summary>
    public IServiceProvider Services
    {
        get
        {
            serviceScope ??= Host.Services.CreateScope();
            return serviceScope.ServiceProvider;
        }
    }

    protected TodoApiTestBase(TodoApiFixture fixture, ITestOutputHelper testOutputHelper)
    {
        this.fixture = fixture;
        this.testOutputHelper = testOutputHelper;
        Host = fixture.AlbaHost;
        TimeProvider = fixture.TimeProvider;
    }

    /// <summary>
    /// Resets the database before each test run.
    /// </summary>
    /// <remarks>
    /// Called immediately after the class has been created, before it is used.
    /// </remarks>
    /// <returns>A <see cref="Task"/> that is completed when the database has been reset.</returns>
    public virtual Task InitializeAsync()
    {
        // Pass the current tests output helper to the fixture
        // such that logs will be collected in the test output.
        fixture.OutputHelper = testOutputHelper;

        // Reset the database before running tests.
        // This is safer compared to doing it after a test completes,
        // since a test run may exit early/crash leaving
        // the database in an unexpected state.
        return fixture.ResetDatabase();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes of the <see cref="IServiceProvider"/> returned from <see cref="Services"/>.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing && serviceScope is not null)
            {
                serviceScope.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}