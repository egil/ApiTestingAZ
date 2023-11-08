using Alba.Security;
using TimeProviderExtensions;

namespace TodoApi.Tests;

/// <summary>
/// A test fixture that uses <see cref="UseLocalTestDb"/>.
/// </summary>
public class TodoApiFixture : IAsyncLifetime
{
    private UseLocalTestDb testDb = null!;

    public IAlbaHost AlbaHost { get; private set; } = null!;

    public ManualTimeProvider TimeProvider { get; } = new();

    public async Task InitializeAsync()
    {
        var securityStub = new AuthenticationStub().WithName("TestUser");
        testDb = new UseLocalTestDb();

        AlbaHost = await Alba.AlbaHost.For<Program>(
            testDb,
            securityStub,
            new UseManualtTimeProvider(TimeProvider));
    }

    public async Task DisposeAsync()
    {
        await testDb.DisposeAsync();
    }

    public async Task ResetDatabase()
        => await testDb.ResetDatabase();
}
