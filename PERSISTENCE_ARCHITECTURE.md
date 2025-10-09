# Persistence Architecture Documentation

This document describes the persistence architecture implemented for the Terminplaner application.

## Overview

The application uses the **Repository Pattern** with abstraction to support multiple storage backends. This design allows switching between different storage implementations without changing the business logic:

- **InMemory**: For development and testing
- **SQLite**: For local persistence with offline support
- **Cosmos DB**: For cloud-based production storage
- **Hybrid**: Combines SQLite (local) with Cosmos DB (cloud) synchronization

## Architecture Components

### 1. Repository Interface (`IAppointmentRepository`)

Located in: `TerminplanerApi/Repositories/IAppointmentRepository.cs`

Defines the contract for appointment data access:

```csharp
public interface IAppointmentRepository
{
    Task<Appointment> CreateAsync(Appointment appointment);
    Task<Appointment?> GetByIdAsync(string id);
    Task<List<Appointment>> GetAllAsync();
    Task<Appointment?> UpdateAsync(string id, Appointment appointment);
    Task<bool> DeleteAsync(string id);
    Task UpdatePrioritiesAsync(Dictionary<string, int> priorities);
}
```

### 2. In-Memory Implementation (`InMemoryAppointmentRepository`)

Located in: `TerminplanerApi/Repositories/InMemoryAppointmentRepository.cs`

- Default implementation for development and testing
- Stores appointments in a `List<Appointment>`
- Includes sample data on initialization
- Sequential string IDs (converted from integer counter)

### 3. Azure Cosmos DB Implementation (`CosmosAppointmentRepository`)

Located in: `TerminplanerApi/Repositories/CosmosAppointmentRepository.cs`

- Production-ready implementation for Azure Cosmos DB
- Uses the Microsoft.Azure.Cosmos SDK (v3.54.0)
- Implements all CRUD operations asynchronously
- Uses document ID as partition key for optimal performance

### 4. SQLite Implementation (`SqliteAppointmentRepository`)

Located in: `TerminplanerApi/Repositories/SqliteAppointmentRepository.cs`

- Local file-based persistence using SQLite
- Provides offline-first capability
- Creates database and table automatically on initialization
- Stores appointments in a local `appointments.db` file
- Uses Microsoft.Data.Sqlite (v9.0.0)
- Generates GUIDs for appointment IDs
- Preserves existing IDs when provided (for synchronization)

### 5. Hybrid Implementation (`HybridAppointmentRepository`)

Located in: `TerminplanerApi/Repositories/HybridAppointmentRepository.cs`

- Combines local SQLite storage with cloud Cosmos DB synchronization
- Provides offline capability with cloud backup
- Implements automatic synchronization on application startup
- Falls back to local-only mode when remote is unavailable

**Synchronization Strategy:**

On application startup (when internet is available):
1. Load all appointments from both local and remote repositories
2. Apply delta synchronization:
   - Appointments only in remote → Add to local
   - Appointments only in local → Add to remote
   - Appointments in both but different → Update local (Cosmos wins)

On application startup (no internet):
- Uses local SQLite repository only
- Logs warning and continues without sync

**Known Limitations:**
- Deletions are not synced (deleted local items may be recreated from remote)
- Sync only occurs on startup (not on reconnect)
- No unique constraints beyond ID (duplicates possible with same properties)

## Configuration

The repository type is configured in `appsettings.json`:

```json
{
  "RepositoryType": "InMemory",  // Options: "InMemory", "Sqlite", "CosmosDb", "Hybrid"
  "Sqlite": {
    "ConnectionString": "Data Source=appointments.db"
  },
  "CosmosDb": {
    "ConnectionString": "",  // Set via environment variables or user secrets - NEVER commit actual keys
    "DatabaseId": "db_1",
    "ContainerId": "container_1"
  }
}
```

### Repository Types

- **InMemory**: Default for development/testing. No persistence, includes sample data.
- **Sqlite**: Local file-based persistence. Good for offline-first scenarios.
- **CosmosDb**: Cloud-based production storage with global distribution.
- **Hybrid**: Combines SQLite (local) with Cosmos DB (cloud) for offline capability with cloud backup.

**⚠️ Security Warning**: The `CosmosDb:ConnectionString` should be empty in committed files. Configure it using environment variables or user secrets. See [SECURITY_CONFIGURATION.md](SECURITY_CONFIGURATION.md) for details.

### Environment-Specific Configuration

- **Development** (`appsettings.Development.json`): Uses `InMemory` repository
- **Production** (`appsettings.json`): Can be configured to use `CosmosDb`

## Data Model Changes

### Appointment Model

The `Appointment` model was updated to support Cosmos DB:

**Before:**
```csharp
public class Appointment
{
    public int Id { get; set; }
    // ...
}
```

**After:**
```csharp
public class Appointment
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    // ...
}
```

**Key Changes:**
- ID changed from `int` to `string` for Cosmos DB compatibility
- Added `[JsonPropertyName("id")]` attribute for proper JSON serialization
- Both API and MAUI models updated to maintain consistency

## Dependency Injection Setup

In `Program.cs`:

```csharp
var repositoryType = builder.Configuration.GetValue<string>("RepositoryType") ?? "InMemory";

if (repositoryType == "CosmosDb")
{
    // Configure Cosmos DB
    var cosmosConnectionString = builder.Configuration.GetValue<string>("CosmosDb:ConnectionString");
    var databaseId = builder.Configuration.GetValue<string>("CosmosDb:DatabaseId");
    var containerId = builder.Configuration.GetValue<string>("CosmosDb:ContainerId");

    builder.Services.AddSingleton<CosmosClient>(sp => new CosmosClient(cosmosConnectionString));
    builder.Services.AddSingleton<IAppointmentRepository>(sp =>
    {
        var cosmosClient = sp.GetRequiredService<CosmosClient>();
        return new CosmosAppointmentRepository(cosmosClient, databaseId, containerId);
    });
}
else if (repositoryType == "Sqlite")
{
    // Configure SQLite
    var sqliteConnectionString = builder.Configuration.GetValue<string>("Sqlite:ConnectionString") 
        ?? "Data Source=appointments.db";
    builder.Services.AddSingleton<IAppointmentRepository>(sp =>
        new SqliteAppointmentRepository(sqliteConnectionString));
}
else if (repositoryType == "Hybrid")
{
    // Configure Hybrid (SQLite + Cosmos DB with sync)
    var sqliteConnectionString = builder.Configuration.GetValue<string>("Sqlite:ConnectionString") 
        ?? "Data Source=appointments.db";
    var cosmosConnectionString = builder.Configuration.GetValue<string>("CosmosDb:ConnectionString");
    var databaseId = builder.Configuration.GetValue<string>("CosmosDb:DatabaseId");
    var containerId = builder.Configuration.GetValue<string>("CosmosDb:ContainerId");

    builder.Services.AddSingleton<IAppointmentRepository>(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<HybridAppointmentRepository>>();
        var localRepository = new SqliteAppointmentRepository(sqliteConnectionString);
        
        IAppointmentRepository? remoteRepository = null;
        if (!string.IsNullOrEmpty(cosmosConnectionString) && 
            !string.IsNullOrEmpty(databaseId) && 
            !string.IsNullOrEmpty(containerId))
        {
            var cosmosClient = new CosmosClient(cosmosConnectionString);
            remoteRepository = new CosmosAppointmentRepository(cosmosClient, databaseId, containerId);
        }

        return new HybridAppointmentRepository(localRepository, remoteRepository, logger);
    });
}
else
{
    // Use in-memory repository (default)
    builder.Services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();
}

// Build the app
var app = builder.Build();

// Perform initial sync for Hybrid repository
if (repositoryType == "Hybrid")
{
    var repository = app.Services.GetRequiredService<IAppointmentRepository>();
    if (repository is HybridAppointmentRepository hybridRepository)
    {
        await hybridRepository.SyncAsync();
    }
}
```

## Azure Cosmos DB Setup

### Database Configuration

- **Database Name**: `terminplaner`
- **Subscription ID**: `3a71b41d-97e5-4220-bd00-dc421cd97bb3`
- **URI**: `https://terninplaner.documents.azure.com:443/`
- **Database ID**: `db_1`
- **Container ID**: `container_1`
- **Partition Key**: `/id`

### Connection String Security

**⚠️ CRITICAL SECURITY REQUIREMENT**: The connection string contains sensitive credentials and must NEVER be committed to source control.

For detailed instructions on how to securely configure the connection string, see [SECURITY_CONFIGURATION.md](SECURITY_CONFIGURATION.md).

**Quick Setup:**

**Local Development (User Secrets):**
```bash
cd TerminplanerApi
dotnet user-secrets set "CosmosDb:ConnectionString" "AccountEndpoint=https://terninplaner.documents.azure.com:443/;AccountKey=YOUR_KEY_HERE"
dotnet user-secrets set "RepositoryType" "CosmosDb"
```

**Production (Environment Variables):**
```bash
export CosmosDb__ConnectionString="AccountEndpoint=https://terninplaner.documents.azure.com:443/;AccountKey=YOUR_KEY_HERE"
export RepositoryType="CosmosDb"
```

See [SECURITY_CONFIGURATION.md](SECURITY_CONFIGURATION.md) for complete security best practices and configuration options.

## NuGet Packages

The following packages are used for persistence:

- **Microsoft.Azure.Cosmos** (v3.54.0) - Azure Cosmos DB SDK
- **Microsoft.Data.Sqlite** (v9.0.0) - SQLite database engine
- **Newtonsoft.Json** (v13.0.4) - JSON serialization (required by Cosmos SDK)

## Testing

All tests have been updated to work with the new repository pattern:

- **Unit Tests** (`AppointmentRepositoryTests.cs`): Test the `InMemoryAppointmentRepository` directly
- **SQLite Tests** (`SqliteAppointmentRepositoryTests.cs`): Test SQLite repository CRUD operations and persistence
- **Hybrid Tests** (`HybridAppointmentRepositoryTests.cs`): Test synchronization logic and offline scenarios
- **Integration Tests** (`AppointmentApiIntegrationTests.cs`): Test the HTTP API endpoints
- **Cosmos Tests** (`CosmosAppointmentRepositoryTests.cs`): Test Cosmos DB repository with mocked client

### Test Results

✅ All 87 tests passing:
- 23 unit tests (InMemory)
- 13 unit tests (SQLite)
- 12 unit tests (Hybrid)
- 19 integration tests (API)
- 20 unit tests (Cosmos - mocked)

## Migration Path

### To SQLite (Local Persistence Only)

1. Update `appsettings.json`:
   ```json
   {
     "RepositoryType": "Sqlite",
     "Sqlite": {
       "ConnectionString": "Data Source=appointments.db"
     }
   }
   ```
2. Restart the application
3. Database file will be created automatically at the specified path

### To Cosmos DB (Cloud Only)

1. Ensure Azure Cosmos DB is set up with the correct database and container
2. Update `appsettings.json`:
   ```json
   {
     "RepositoryType": "CosmosDb"
   }
   ```
3. Configure connection string via user secrets or environment variables
4. Restart the application

### To Hybrid (Local + Cloud with Sync)

1. Ensure Azure Cosmos DB is set up
2. Update `appsettings.json`:
   ```json
   {
     "RepositoryType": "Hybrid",
     "Sqlite": {
       "ConnectionString": "Data Source=appointments.db"
     }
   }
   ```
3. Configure Cosmos DB connection string via user secrets or environment variables
4. Restart the application
5. Initial sync will occur automatically on startup

## MAUI App Updates

The MAUI app has been updated to work with string IDs:

- **Model**: `TerminplanerMaui/Models/Appointment.cs` - ID changed to `string`
- **Service**: `TerminplanerMaui/Services/AppointmentApiService.cs` - Methods updated to accept `string` IDs
- **ViewModels**: Updated to use `Dictionary<string, int>` for priority updates

## Benefits of This Architecture

1. **Flexibility**: Easy to switch between storage implementations
2. **Testability**: Can test with in-memory repository without external dependencies
3. **Clean Architecture**: Business logic is decoupled from data access
4. **Future-Proof**: Can add new repository implementations without changing existing code
5. **Offline Support**: SQLite and Hybrid modes provide offline-first capability
6. **Cloud Backup**: Hybrid mode automatically syncs with Cosmos DB when online
7. **Production-Ready**: Azure Cosmos DB provides scalable, globally distributed database

## Future Enhancements

Potential improvements to consider:

- Add caching layer (Redis) for frequently accessed data
- Implement repository pattern for other entities (if added)
- Add bulk operations support
- Implement optimistic concurrency control using ETags
- Add retry policies for transient failures
- Implement health checks for Cosmos DB connectivity
- Enhance sync logic:
  - Sync deletions (track deleted items separately)
  - Background sync on reconnection
  - Conflict resolution UI for user input
  - Define unique constraints beyond ID to prevent duplicates

## References

- [Azure Cosmos DB Documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Microsoft.Azure.Cosmos SDK](https://github.com/Azure/azure-cosmos-dotnet-v3)
