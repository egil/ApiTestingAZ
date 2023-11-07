using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;
using System.Data.Common;

namespace TodoApi.AblaExtensions;

public sealed class UseLocalTestDb : IAlbaExtension
{
    private const string LocalDbTestConnectionString = "Server=(localdb)\\mssqllocaldb;Trusted_Connection=True;MultipleActiveResultSets=true";
    private TodoDb dbContext = null!;
    private DbConnection dbConnection = null!;
    private Respawner respawner = null!;
    private IServiceScope serviceScope = null!;

    public string TestDatabaseName { get; init; } = "TestDb";

    public bool DeleteDatabaseAfterRun { get; init; }

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
