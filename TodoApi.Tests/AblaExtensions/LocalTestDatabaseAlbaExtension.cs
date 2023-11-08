using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Respawn;
using System.Data.Common;

namespace TodoApi.Tests.AblaExtensions;

public sealed class LocalTestDatabaseAlbaExtension : IAlbaExtension
{
    private const string LocalDbTestConnectionString = "Server=(localdb)\\mssqllocaldb;Trusted_Connection=True;MultipleActiveResultSets=true";
    private TodoDb dbContext = null!;
    private DbConnection dbConnection = null!;
    private Respawner respawner = null!;
    private IServiceScope serviceScope = null!;

    /// <summary>
    /// Gets the name of test database to create in on the LocalDB Sql Server instance.
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

    IHostBuilder IAlbaExtension.Configure(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, c) =>
        {
            // Replace the default connection string with one pointing to test database
            // to avoid overriding a default localdb.
            c.AddInMemoryCollection([
                new KeyValuePair<string, string?>(
                    "ConnectionStrings:DefaultConnection",
                    $"{LocalDbTestConnectionString};Database={TestDatabaseName}")]);
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

        // Initialize respawner.        
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

    public async ValueTask DisposeAsync()
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
