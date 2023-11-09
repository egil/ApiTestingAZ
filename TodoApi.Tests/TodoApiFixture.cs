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

        testDb = new LocalTestDatabaseAlbaExtension();

        var timeProviderOverride = new TimeProviderAlbaExtension(TimeProvider);

        AlbaHost = await Alba.AlbaHost.For<Program>(
            testDb,
            securityStub,
            timeProviderOverride);
    }

    public async Task DisposeAsync() 
        => await AlbaHost.DisposeAsync();

    public async Task ResetDatabase()
        => await testDb.ResetDatabase();
}
