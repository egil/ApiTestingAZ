using Alba.Security;
using TimeProviderExtensions;

namespace TodoApi.Tests;

/// <summary>
/// A test fixture that uses <see cref="LocalTestDatabaseAlbaExtension"/>.
/// </summary>
public class TodoApiFixture : IAsyncLifetime
{
    private LocalTestDatabaseAlbaExtension testDb = null!;

    public IAlbaHost AlbaHost { get; private set; } = null!;

    public ManualTimeProvider TimeProvider { get; } = new();

    public async Task InitializeAsync()
    {
        // Set up Alba's built in auth stub
        // https://jasperfx.github.io/alba/guide/security.html
        var securityStub = new AuthenticationStub().WithName("TestUser");

        testDb = new LocalTestDatabaseAlbaExtension();

        AlbaHost = await Alba.AlbaHost.For<Program>(
            testDb,
            securityStub,
            new TimeProviderAlbaExtension(TimeProvider));
    }

    public async Task DisposeAsync() 
        => await testDb.DisposeAsync();

    public async Task ResetDatabase()
        => await testDb.ResetDatabase();
}
