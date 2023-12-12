using Alba.Security;
using TimeProviderExtensions;
using Xunit.Abstractions;

namespace TodoApi.Tests;

public class TodoApiFixture : IAsyncLifetime
{
    private LocalDBAlbaExtension testDb = null!;

    public ITestOutputHelper? OutputHelper { get; set; }

    public IAlbaHost AlbaHost { get; private set; } = null!;

    public ManualTimeProvider TimeProvider { get; } = new();

    public async Task InitializeAsync()
    {
        // Set up Alba's built in auth stub
        // https://jasperfx.github.io/alba/guide/security.html
        var securityStub =
            new AuthenticationStub()
                .WithName("TestUser");

        // Replace the database used by the API with a localdb
        // version that can be reset via Respawn.
        // This can be replaced with something else, e.g. a version
        // that uses TestContainers to spin up a SQL server in
        // docker container instead.
        testDb = new LocalDBAlbaExtension();

        // Override the default TimeProvider.System used by the API
        // with the ManualTimeProvider to enable tests to control
        // the passage of time.
        var timeProviderOverride = new TimeProviderAlbaExtension(TimeProvider);

        // Since tests are not running in parallel it is possible to
        // safely capture ILogger messages from the API and pass them to
        // the OutputHelper (ITestOutputHelper). This Alba extensions
        // enables this.
        var captureLoggingAlbaExtension = new CaptureLoggingAlbaExtension(() => OutputHelper);

        AlbaHost = await Alba.AlbaHost.For<Program>(
            securityStub,
            testDb,
            timeProviderOverride,
            captureLoggingAlbaExtension);
    }

    public async Task DisposeAsync()
        => await AlbaHost.DisposeAsync();

    public async Task ResetDatabase()
        => await testDb.ResetDatabase();
}
