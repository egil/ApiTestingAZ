using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Respawn;
using System.Data.Common;
using Testcontainers.MsSql;

namespace TodoApi.Tests.AblaExtensions;

public sealed class TestContainersDatabaseAlbaExtension : IAlbaExtension
{
    private const int SQLPORT = 1433;
    private const string SQLPASSWORD = "!Q@W3e4r5t6y7u";
    private readonly MsSqlContainer mssqlContainer;

    private TodoDb dbContext = null!;
    private DbConnection dbConnection = null!;
    private Respawner respawner = null!;
    private IServiceScope serviceScope = null!;

    /// <summary>
    /// Gets the name of test database to create in on the TestContainer DB Sql Server instance.
    /// </summary>
    /// <remarks>
    /// Defaults to "TestDb".
    /// </remarks>
    public string TestDatabaseName { get; init; } = "TestDb";

    /// <summary>
    /// Gets whether or not to delete the test database on dispose. Default is <see langword="false"/>.
    /// </summary>
    public bool DeleteDatabaseAfterRun { get; init; }

    /// <summary>
    /// Gets whether to reseed database keys when the database is reset. Default is <see langword="true"/>.
    /// </summary>
    public bool ReseedDatabase { get; init; } = true;

    public TestContainersDatabaseAlbaExtension()
    {
        mssqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword(SQLPASSWORD)
            .WithCleanUp(cleanUp: true)
            .WithPortBinding(port: SQLPORT, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(SQLPORT))
            .Build();

        // For CosmosDB:
        // var cosmosDbContainer = new CosmosDbBuilder()
        //     .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
        //     .Build();
    }

    /// <summary>
    /// Resets the test database.
    /// </summary>
    /// <returns></returns>
    public async Task ResetDatabase()
    {
        if (dbConnection.State != System.Data.ConnectionState.Open)
        {
            await dbConnection.OpenAsync();
        }

        await respawner.ResetAsync(dbConnection);
    }

    public string GetSqlServerConnectionString()
    {
        var host = mssqlContainer.Hostname;
        var port = mssqlContainer.GetMappedPublicPort(SQLPORT);
        return $"Server={host},{port};Database={TestDatabaseName};User Id=sa;Password={SQLPASSWORD};TrustServerCertificate=True";
    }

    IHostBuilder IAlbaExtension.Configure(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, c) =>
        {
            // Replace the default connection string with one pointing to test database.
            c.AddInMemoryCollection([
                new KeyValuePair<string, string?>(
                    key: "ConnectionStrings:DefaultConnection", 
                    value: GetSqlServerConnectionString())]);
        });

        return builder;
    }

    async Task IAlbaExtension.Start(IAlbaHost host)
    {
        serviceScope = host.Services.CreateScope();
        dbContext = serviceScope.ServiceProvider.GetRequiredService<TodoDb>();
        dbConnection = dbContext.Database.GetDbConnection();

        // Creates database and apply EF migrations, if needed
        await dbContext.Database.EnsureCreatedAsync();

        // Initialize respawn.
        await dbConnection.OpenAsync();
        respawner = await Respawner.CreateAsync(
            dbConnection,
            new RespawnerOptions()
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude = ["dbo"],
                TablesToIgnore = ["__EFMigrationsHistory"],
                WithReseed = ReseedDatabase,
            });
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (DeleteDatabaseAfterRun)
        {
            await dbContext.Database.EnsureDeletedAsync();
        }

        await dbContext.DisposeAsync();
        serviceScope.Dispose();
    }

    void IDisposable.Dispose()
    {
    }
}
