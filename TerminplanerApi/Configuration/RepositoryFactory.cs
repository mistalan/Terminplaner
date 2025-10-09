using Microsoft.Azure.Cosmos;
using System.Diagnostics.CodeAnalysis;
using TerminplanerApi.Repositories;

namespace TerminplanerApi.Configuration;

/// <summary>
/// Factory for creating appointment repository instances based on configuration.
/// </summary>
public static class RepositoryFactory
{
    /// <summary>
    /// Configures and registers the appropriate IAppointmentRepository implementation
    /// based on the RepositoryType configuration value.
    /// </summary>
    public static void AddAppointmentRepository(this IServiceCollection services, IConfiguration configuration)
    {
        var repositoryType = configuration.GetValue<string>("RepositoryType") ?? "InMemory";

        switch (repositoryType)
        {
            case "CosmosDb":
                AddCosmosDbRepository(services, configuration);
                break;

            case "Sqlite":
                AddSqliteRepository(services, configuration);
                break;

            case "Hybrid":
                AddHybridRepository(services, configuration);
                break;

            default:
                AddInMemoryRepository(services);
                break;
        }
    }

    private static void AddInMemoryRepository(IServiceCollection services)
    {
        services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();
    }

    private static void AddSqliteRepository(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("Sqlite:ConnectionString") 
            ?? "Data Source=appointments.db";
        
        services.AddSingleton<IAppointmentRepository>(sp =>
            new SqliteAppointmentRepository(connectionString));
    }

    [ExcludeFromCodeCoverage(Justification = "CosmosDb registration requires actual CosmosClient which needs real credentials. Integration tested through application startup.")]
    private static void AddCosmosDbRepository(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("CosmosDb:ConnectionString");
        var databaseId = configuration.GetValue<string>("CosmosDb:DatabaseId");
        var containerId = configuration.GetValue<string>("CosmosDb:ContainerId");

        ValidateCosmosDbConfiguration(connectionString, databaseId, containerId);

        services.AddSingleton<CosmosClient>(sp => new CosmosClient(connectionString));
        services.AddSingleton<IAppointmentRepository>(sp =>
        {
            var cosmosClient = sp.GetRequiredService<CosmosClient>();
            return new CosmosAppointmentRepository(cosmosClient, databaseId!, containerId!);
        });
    }

    private static void AddHybridRepository(IServiceCollection services, IConfiguration configuration)
    {
        var sqliteConnectionString = configuration.GetValue<string>("Sqlite:ConnectionString") 
            ?? "Data Source=appointments.db";
        var cosmosConnectionString = configuration.GetValue<string>("CosmosDb:ConnectionString");
        var databaseId = configuration.GetValue<string>("CosmosDb:DatabaseId");
        var containerId = configuration.GetValue<string>("CosmosDb:ContainerId");

        services.AddSingleton<IAppointmentRepository>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<HybridAppointmentRepository>>();
            var localRepository = new SqliteAppointmentRepository(sqliteConnectionString);

            IAppointmentRepository? remoteRepository = null;
            if (IsCosmosDbConfigured(cosmosConnectionString, databaseId, containerId))
            {
                // Excluded from coverage: CosmosClient instantiation requires real credentials
                [ExcludeFromCodeCoverage]
                static IAppointmentRepository CreateCosmosRepository(string connStr, string dbId, string cId)
                {
                    var cosmosClient = new CosmosClient(connStr);
                    return new CosmosAppointmentRepository(cosmosClient, dbId, cId);
                }
                remoteRepository = CreateCosmosRepository(cosmosConnectionString!, databaseId!, containerId!);
            }

            return new HybridAppointmentRepository(localRepository, remoteRepository, logger);
        });
    }

    private static bool IsCosmosDbConfigured(string? connectionString, string? databaseId, string? containerId)
    {
        return !string.IsNullOrEmpty(connectionString) 
            && !string.IsNullOrEmpty(databaseId) 
            && !string.IsNullOrEmpty(containerId);
    }

    private static void ValidateCosmosDbConfiguration(string? connectionString, string? databaseId, string? containerId)
    {
        if (!IsCosmosDbConfigured(connectionString, databaseId, containerId))
        {
            throw new InvalidOperationException("CosmosDb configuration is missing in appsettings.json");
        }
    }

    /// <summary>
    /// Performs initial synchronization for Hybrid repository if configured.
    /// Should be called after the application is built.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Integration-tested through application startup. Requires WebApplication instance which is complex to mock in unit tests.")]
    public static async Task InitializeRepositoryAsync(this WebApplication app, IConfiguration configuration)
    {
        var repositoryType = configuration.GetValue<string>("RepositoryType") ?? "InMemory";

        if (repositoryType == "Hybrid")
        {
            var repository = app.Services.GetRequiredService<IAppointmentRepository>();
            if (repository is HybridAppointmentRepository hybridRepository)
            {
                await hybridRepository.SyncAsync();
            }
        }
    }
}
