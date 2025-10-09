# Local Persistence Implementation Guide

This document describes the local persistence feature added to the Terminplaner application.

## Overview

The application now supports local SQLite database storage with optional cloud synchronization. This provides:

- **Offline-first capability**: Work without internet connection
- **Data persistence**: Appointments survive app crashes and restarts
- **Cloud backup**: Automatic synchronization with Azure Cosmos DB when online
- **Simple setup**: No complex configuration required

## Repository Types

### 1. SQLite (Local Only)

**Use Case**: Single-device usage with local-only storage

**Configuration** (`appsettings.json`):
```json
{
  "RepositoryType": "Sqlite",
  "Sqlite": {
    "ConnectionString": "Data Source=appointments.db"
  }
}
```

**Features:**
- ✅ Offline support
- ✅ Data persists across restarts
- ✅ No internet required
- ❌ No cloud backup
- ❌ No multi-device sync

**Database Location:**
- File is created in the application's working directory
- Default: `appointments.db`
- Customizable via connection string

### 2. Hybrid (SQLite + Cosmos DB)

**Use Case**: Offline-capable with cloud backup and basic synchronization

**Configuration** (`appsettings.json`):
```json
{
  "RepositoryType": "Hybrid",
  "Sqlite": {
    "ConnectionString": "Data Source=appointments.db"
  },
  "CosmosDb": {
    "ConnectionString": "",  // Configure via user secrets or environment variables
    "DatabaseId": "db_1",
    "ContainerId": "container_1"
  }
}
```

**Features:**
- ✅ Offline support
- ✅ Data persists locally
- ✅ Cloud backup when online
- ✅ Automatic sync on startup
- ✅ Falls back to local-only when offline
- ⚠️ Simple conflict resolution (cloud wins)

## Synchronization Strategy

### When Does Sync Occur?

**Hybrid mode performs sync:**
- ✅ Once at application startup (if internet available)
- ❌ Not on reconnection (future enhancement)
- ❌ Not on background schedule (future enhancement)

### How Sync Works

When the application starts in Hybrid mode:

1. **Load Data**
   - Fetch all appointments from local SQLite
   - Fetch all appointments from remote Cosmos DB

2. **Delta Comparison**
   - Compare appointments by ID
   - Identify differences

3. **Apply Changes**
   - **Remote only** → Add to local
   - **Local only** → Add to remote
   - **Both, but different** → Update local (Cosmos wins)

4. **Continue Operations**
   - All subsequent CRUD operations update both local and remote
   - If remote fails, local succeeds and warning is logged

### Sync Flow Diagram

```
Application Startup (Hybrid Mode)
         |
         v
    Internet Available?
    /              \
  YES               NO
   |                 |
   v                 v
Load Both       Use Local Only
   |                 |
   v                 |
Apply Sync           |
   |                 |
   v                 v
    Ready for Use
```

## Known Limitations

The current implementation has some known limitations that are **acceptable for the current use case**:

### 1. Deletion Not Synced

**Issue**: If you delete an appointment locally while offline, it will be recreated when you go online (if it still exists in Cosmos DB).

**Why**: Tracking deletions requires additional state management.

**Workaround**: Delete appointments only when online, or accept that they may reappear.

**Future Fix**: Implement a "deleted items" table to track removals.

### 2. No Unique Constraints

**Issue**: It's possible to create duplicate appointments with the same properties but different IDs.

**Why**: Only ID is used as the unique key currently.

**Workaround**: Be careful not to create duplicates.

**Future Fix**: Define composite unique keys in both Cosmos DB and SQLite.

### 3. Sync Only on Startup

**Issue**: If data changes remotely while the app is running, you won't see the changes until restart.

**Why**: Continuous sync adds complexity and resource usage.

**Workaround**: Restart the app to pull latest changes.

**Future Fix**: Add background sync or manual refresh button.

### 4. Simple Conflict Resolution

**Issue**: When the same appointment exists in both local and remote with different values, Cosmos DB always wins.

**Why**: More sophisticated conflict resolution (timestamps, user choice) is complex.

**Workaround**: Be aware that cloud changes override local changes on sync.

**Future Fix**: Add last-modified timestamps and merge logic.

## Configuration Examples

### Development (Local Only)

```json
{
  "RepositoryType": "Sqlite",
  "Sqlite": {
    "ConnectionString": "Data Source=dev_appointments.db"
  }
}
```

### Production (Hybrid with Secrets)

**appsettings.json:**
```json
{
  "RepositoryType": "Hybrid",
  "Sqlite": {
    "ConnectionString": "Data Source=/var/app/data/appointments.db"
  },
  "CosmosDb": {
    "ConnectionString": "",
    "DatabaseId": "db_1",
    "ContainerId": "container_1"
  }
}
```

**Configure via User Secrets:**
```bash
dotnet user-secrets set "CosmosDb:ConnectionString" "AccountEndpoint=https://...;AccountKey=..."
```

**Or via Environment Variables:**
```bash
export CosmosDb__ConnectionString="AccountEndpoint=https://...;AccountKey=..."
```

## Database Schema

### SQLite Schema

```sql
CREATE TABLE Appointments (
    Id TEXT PRIMARY KEY,
    Text TEXT NOT NULL,
    Category TEXT NOT NULL,
    Color TEXT NOT NULL,
    Priority INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL,
    ScheduledDate TEXT,
    Duration TEXT,
    IsOutOfHome INTEGER NOT NULL
);
```

**Notes:**
- `Id` is a GUID string
- `CreatedAt` and `ScheduledDate` are stored as ISO 8601 strings
- `IsOutOfHome` is stored as 0 or 1 (SQLite has no boolean type)

## Testing

The implementation includes comprehensive tests:

### SQLite Repository Tests (13 tests)

- CRUD operations
- ID generation
- Timestamp handling
- Data persistence across repository instances
- All properties correctly saved and retrieved

### Hybrid Repository Tests (12 tests)

- Sync adds remote-only items to local
- Sync adds local-only items to remote
- Sync resolves conflicts (Cosmos wins)
- Sync only runs once
- Sync works without remote repository
- CRUD operations update both local and remote
- Offline behavior (local continues when remote fails)

**Run tests:**
```bash
dotnet test TerminplanerApi.Tests/TerminplanerApi.Tests.csproj
```

## Troubleshooting

### Database File Not Found

**Problem**: SQLite database file doesn't exist.

**Solution**: The database is created automatically on first run. Ensure the application has write permissions to the directory.

### Sync Fails Silently

**Problem**: Hybrid mode isn't syncing with Cosmos DB.

**Solution**: 
1. Check logs for sync warnings
2. Verify Cosmos DB connection string is configured
3. Verify internet connectivity
4. Verify Cosmos DB credentials are valid

### Data Not Persisting

**Problem**: Data disappears after restart.

**Solution**:
1. Verify `RepositoryType` is set to `Sqlite` or `Hybrid`
2. Check that database file exists and is writable
3. Ensure database file isn't in a temporary directory that gets cleaned

### Duplicate Appointments After Sync

**Problem**: Appointments appear twice after syncing.

**Solution**: This is a known limitation. Delete duplicates manually. Ensure you're not creating the same appointment in multiple places.

## Migration from In-Memory

If you're currently using the in-memory repository and want to migrate to SQLite or Hybrid:

### Step 1: Export Current Data (Optional)

If you have important data in memory, you'll need to manually recreate it or use the API to export/import.

### Step 2: Update Configuration

Change `RepositoryType` in `appsettings.json`:

```json
{
  "RepositoryType": "Sqlite"  // or "Hybrid"
}
```

### Step 3: Restart Application

The application will:
1. Create the SQLite database automatically
2. Start with an empty database
3. (For Hybrid) Sync with Cosmos DB if configured

### Step 4: Import Data

Manually recreate appointments using the UI or API endpoints.

## Performance Considerations

### SQLite

- **Reads**: Very fast (microseconds for single row)
- **Writes**: Fast (milliseconds for single row)
- **Transactions**: Used for bulk operations like priority updates
- **File Size**: Minimal overhead, ~4KB per appointment estimated

### Hybrid

- **Startup Time**: Adds 1-3 seconds for sync (depends on number of appointments)
- **CRUD Operations**: Adds latency of Cosmos DB call (100-500ms typically)
- **Offline Mode**: Same as SQLite (no performance impact)

## Security Considerations

### SQLite Database File

- **Location**: Store in a secure directory with appropriate permissions
- **Encryption**: SQLite database is not encrypted by default
- **Backup**: Consider regular backups of the .db file
- **Sensitive Data**: Be mindful of storing sensitive information

### Connection Strings

- **Never commit** Cosmos DB connection strings to source control
- Use **user secrets** for development
- Use **environment variables** or **Azure Key Vault** for production
- See [SECURITY_CONFIGURATION.md](SECURITY_CONFIGURATION.md) for details

## References

- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [Microsoft.Data.Sqlite Documentation](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [PERSISTENCE_ARCHITECTURE.md](PERSISTENCE_ARCHITECTURE.md) - Complete architecture documentation
