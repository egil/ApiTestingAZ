using TodoApi.AblaExtensions;

namespace TodoApi.Tests;

//[assembly: CollectionBehavior(DisableTestParallelization = true)]

/// <summary>
/// A test fixture that uses <see cref="UseLocalTestDb"/>.
/// </summary>
public class TodoApiFixture : IAsyncLifetime
{
    private UseLocalTestDb testDb = null!;

    public IAlbaHost AlbaHost { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        testDb = new UseLocalTestDb();
        AlbaHost = await Alba.AlbaHost.For<global::Program>(testDb);
    }

    public async Task DisposeAsync()
    {
        await testDb.DisposeAsync();
    }

    public async Task ResetDatabase()
        => await testDb.ResetDatabase();
}

[CollectionDefinition(nameof(ApiTestCollection))]
public sealed class ApiTestCollection : ICollectionFixture<TodoApiFixture>
{
}

/// <summary>
/// A base case for API tests that wants to share a single
/// <see cref="TodoApiFixture"/> and database. It resets the database
/// before each test run to ensure consistent execution of tests.
/// </summary>
[Collection(nameof(ApiTestCollection))]
public abstract class TodoApiTestBase : IAsyncLifetime
{
    private readonly TodoApiFixture fixture;

    public IAlbaHost Host { get; }

    protected TodoApiTestBase(TodoApiFixture fixture)
    {
        this.fixture = fixture;
        Host = fixture.AlbaHost;
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
        return Task.CompletedTask;
    }
}