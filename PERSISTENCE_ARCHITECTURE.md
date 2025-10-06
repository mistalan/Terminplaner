# Persistence Architecture Documentation

This document describes the persistence architecture implemented for the Terminplaner application.

## Overview

The application uses the **Repository Pattern** with abstraction to support multiple storage backends. This design allows switching between in-memory storage (for development/testing) and Azure Cosmos DB (for production) without changing the business logic.

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

## Configuration

The repository type is configured in `appsettings.json`:

```json
{
  "RepositoryType": "InMemory",  // or "CosmosDb"
  "CosmosDb": {
    "ConnectionString": "AccountEndpoint=https://...;AccountKey=...",
    "DatabaseId": "db_1",
    "ContainerId": "container_1"
  }
}
```

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
else
{
    // Use in-memory repository (default)
    builder.Services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();
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

### Connection String

The connection string is stored in `appsettings.json` (production) and should be stored in Azure Key Vault or environment variables for security in deployed environments.

**⚠️ Security Note**: Never commit production connection strings to source control. Use user secrets for local development:

```bash
dotnet user-secrets set "CosmosDb:ConnectionString" "AccountEndpoint=...;AccountKey=..."
```

## NuGet Packages

The following packages were added to support Cosmos DB:

- **Microsoft.Azure.Cosmos** (v3.54.0) - Azure Cosmos DB SDK
- **Newtonsoft.Json** (v13.0.4) - JSON serialization (required by Cosmos SDK)

## Testing

All tests have been updated to work with the new repository pattern:

- **Unit Tests** (`AppointmentRepositoryTests.cs`): Test the `InMemoryAppointmentRepository` directly
- **Integration Tests** (`AppointmentApiIntegrationTests.cs`): Test the HTTP API endpoints

### Test Results

✅ All 42 tests passing:
- 23 unit tests
- 19 integration tests

## Migration Path

To switch from in-memory to Cosmos DB:

1. Ensure Azure Cosmos DB is set up with the correct database and container
2. Update `appsettings.json`:
   ```json
   {
     "RepositoryType": "CosmosDb"
   }
   ```
3. Restart the application

## MAUI App Updates

The MAUI app has been updated to work with string IDs:

- **Model**: `TerminplanerMaui/Models/Appointment.cs` - ID changed to `string`
- **Service**: `TerminplanerMaui/Services/AppointmentApiService.cs` - Methods updated to accept `string` IDs
- **ViewModels**: Updated to use `Dictionary<string, int>` for priority updates

## Benefits of This Architecture

1. **Flexibility**: Easy to switch between storage implementations
2. **Testability**: Can test with in-memory repository without external dependencies
3. **Clean Architecture**: Business logic is decoupled from data access
4. **Future-Proof**: Can add new repository implementations (e.g., SQL Server, MongoDB) without changing existing code
5. **Production-Ready**: Azure Cosmos DB provides scalable, globally distributed database

## Future Enhancements

Potential improvements to consider:

- Add caching layer (Redis) for frequently accessed data
- Implement repository pattern for other entities (if added)
- Add bulk operations support
- Implement optimistic concurrency control using ETags
- Add retry policies for transient failures
- Implement health checks for Cosmos DB connectivity

## References

- [Azure Cosmos DB Documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Microsoft.Azure.Cosmos SDK](https://github.com/Azure/azure-cosmos-dotnet-v3)
