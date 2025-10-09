using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TerminplanerApi.Configuration;
using TerminplanerApi.Repositories;

namespace TerminplanerApi.Tests;

public class RepositoryFactoryTests
{
    [Fact]
    public void TC_RF001_AddAppointmentRepository_RegistersInMemoryByDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddAppointmentRepository(configuration);
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IAppointmentRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<InMemoryAppointmentRepository>(repository);
    }

    [Fact]
    public void TC_RF002_AddAppointmentRepository_RegistersInMemoryWhenSpecified()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "RepositoryType", "InMemory" }
            })
            .Build();

        // Act
        services.AddAppointmentRepository(configuration);
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IAppointmentRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<InMemoryAppointmentRepository>(repository);
    }

    [Fact]
    public void TC_RF003_AddAppointmentRepository_RegistersSqlite()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "RepositoryType", "Sqlite" },
                { "Sqlite:ConnectionString", "Data Source=test.db" }
            })
            .Build();

        // Act
        services.AddAppointmentRepository(configuration);
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IAppointmentRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<SqliteAppointmentRepository>(repository);
    }

    [Fact]
    public void TC_RF004_AddAppointmentRepository_SqliteUsesDefaultConnectionString()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "RepositoryType", "Sqlite" }
            })
            .Build();

        // Act
        services.AddAppointmentRepository(configuration);
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IAppointmentRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<SqliteAppointmentRepository>(repository);
    }

    [Fact]
    public void TC_RF005_AddAppointmentRepository_ThrowsWhenCosmosDbConfigurationMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "RepositoryType", "CosmosDb" }
            })
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            services.AddAppointmentRepository(configuration);
        });

        Assert.Contains("CosmosDb configuration is missing", exception.Message);
    }

    [Fact]
    public void TC_RF006_AddAppointmentRepository_ThrowsWhenCosmosDbConnectionStringMissing()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "RepositoryType", "CosmosDb" },
                { "CosmosDb:DatabaseId", "db1" },
                { "CosmosDb:ContainerId", "container1" }
            })
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            services.AddAppointmentRepository(configuration);
        });

        Assert.Contains("CosmosDb configuration is missing", exception.Message);
    }

    [Fact]
    public void TC_RF007_AddAppointmentRepository_RegistersHybridWithoutRemote()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Required for HybridAppointmentRepository
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "RepositoryType", "Hybrid" },
                { "Sqlite:ConnectionString", "Data Source=test_hybrid.db" }
            })
            .Build();

        // Act
        services.AddAppointmentRepository(configuration);
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IAppointmentRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<HybridAppointmentRepository>(repository);
    }

    [Fact]
    public void TC_RF008_AddAppointmentRepository_HybridUsesDefaultSqliteConnectionString()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "RepositoryType", "Hybrid" }
            })
            .Build();

        // Act
        services.AddAppointmentRepository(configuration);
        var provider = services.BuildServiceProvider();
        var repository = provider.GetRequiredService<IAppointmentRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<HybridAppointmentRepository>(repository);
    }
}
