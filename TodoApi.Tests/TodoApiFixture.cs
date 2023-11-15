using Alba.Security;
using TimeProviderExtensions;

namespace TodoApi.Tests;

public class TodoApiFixture : IAsyncLifetime
{
    private LocalTestDatabaseAlbaExtension testDb = null!;

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
        testDb = new LocalTestDatabaseAlbaExtension();

        // Override the default TimeProvider.System used by the API
        // with the ManualTimeProvider to enable tests to control
        // the passage of time.
        var timeProviderOverride = new TimeProviderAlbaExtension(TimeProvider);

        AlbaHost = await Alba.AlbaHost.For<Program>(
            securityStub,
            testDb,
            timeProviderOverride);
    }

    public async Task DisposeAsync() 
        => await AlbaHost.DisposeAsync();

    public async Task ResetDatabase()
        => await testDb.ResetDatabase();
}
