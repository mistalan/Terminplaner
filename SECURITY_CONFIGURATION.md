# Security Configuration Guide

This document explains how to securely configure the Terminplaner API with Azure Cosmos DB credentials.

## ⚠️ Security Warning

**NEVER commit sensitive credentials (connection strings, API keys, passwords) to source control.**

The `appsettings.json` file is tracked by git and should only contain non-sensitive configuration. All sensitive configuration should be stored using one of the secure methods described below.

## Configuration Methods

### 1. Environment Variables (Recommended for Production)

Set the Cosmos DB connection string as an environment variable:

**Linux/macOS:**
```bash
export CosmosDb__ConnectionString="AccountEndpoint=https://...;AccountKey=..."
export RepositoryType="CosmosDb"
```

**Windows (PowerShell):**
```powershell
$env:CosmosDb__ConnectionString="AccountEndpoint=https://...;AccountKey=..."
$env:RepositoryType="CosmosDb"
```

**Windows (Command Prompt):**
```cmd
set CosmosDb__ConnectionString=AccountEndpoint=https://...;AccountKey=...
set RepositoryType=CosmosDb
```

**Docker/Kubernetes:**
```yaml
environment:
  - CosmosDb__ConnectionString=AccountEndpoint=https://...;AccountKey=...
  - RepositoryType=CosmosDb
```

**Azure App Service:**
Navigate to Configuration → Application Settings and add:
- Name: `CosmosDb__ConnectionString`
- Value: `AccountEndpoint=https://...;AccountKey=...`

### 2. User Secrets (Recommended for Local Development)

User secrets are stored outside the project directory and are never committed to source control.

**Initialize user secrets:**
```bash
cd TerminplanerApi
dotnet user-secrets init
```

**Set the connection string:**
```bash
dotnet user-secrets set "CosmosDb:ConnectionString" "AccountEndpoint=https://terninplaner.documents.azure.com:443/;AccountKey=YOUR_KEY_HERE"
dotnet user-secrets set "RepositoryType" "CosmosDb"
```

**List all secrets:**
```bash
dotnet user-secrets list
```

**Remove a secret:**
```bash
dotnet user-secrets remove "CosmosDb:ConnectionString"
```

**Clear all secrets:**
```bash
dotnet user-secrets clear
```

### 3. Azure Key Vault (Recommended for Production)

For production deployments on Azure, use Azure Key Vault to store secrets.

**Install the NuGet package:**
```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

**Update Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault
if (!builder.Environment.IsDevelopment())
{
    var keyVaultEndpoint = builder.Configuration["KeyVaultEndpoint"];
    if (!string.IsNullOrEmpty(keyVaultEndpoint))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultEndpoint),
            new DefaultAzureCredential());
    }
}
```

**Store the connection string in Key Vault:**
```bash
az keyvault secret set --vault-name <your-vault-name> \
  --name CosmosDb--ConnectionString \
  --value "AccountEndpoint=https://...;AccountKey=..."
```

### 4. appsettings.Production.json (NOT Recommended)

If you must use a configuration file for production:

1. Copy `appsettings.Production.json.template` to `appsettings.Production.json`
2. Add your connection string to `appsettings.Production.json`
3. **IMPORTANT**: `appsettings.Production.json` is listed in `.gitignore` and will NOT be committed

```bash
cp TerminplanerApi/appsettings.Production.json.template TerminplanerApi/appsettings.Production.json
# Edit appsettings.Production.json with your connection string
```

**⚠️ Warning:** This method is less secure as the file can be accidentally committed if `.gitignore` is modified.

## Configuration Priority

ASP.NET Core loads configuration in the following order (later sources override earlier ones):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User secrets (Development environment only)
4. Environment variables
5. Command-line arguments

This means environment variables will override settings in `appsettings.json`.

## Verifying Configuration

To verify your configuration is loaded correctly without exposing secrets:

**Check configuration at startup:**
```csharp
// In Program.cs (for debugging only, remove in production)
var cosmosConnectionString = builder.Configuration.GetValue<string>("CosmosDb:ConnectionString");
var hasConnectionString = !string.IsNullOrEmpty(cosmosConnectionString);
Console.WriteLine($"Cosmos DB connection string configured: {hasConnectionString}");
```

## Azure Cosmos DB Configuration

The application requires the following configuration values:

| Key | Description | Example |
|-----|-------------|---------|
| `CosmosDb:ConnectionString` | Azure Cosmos DB connection string | `AccountEndpoint=https://...;AccountKey=...` |
| `CosmosDb:DatabaseId` | Database identifier | `db_1` |
| `CosmosDb:ContainerId` | Container identifier | `container_1` |
| `RepositoryType` | Storage type (`InMemory` or `CosmosDb`) | `CosmosDb` |

## Getting the Connection String

**Azure Portal:**
1. Navigate to your Azure Cosmos DB account
2. Go to "Keys" section
3. Copy the "PRIMARY CONNECTION STRING"

**Azure CLI:**
```bash
az cosmosdb keys list --name terminplaner --resource-group <your-resource-group> --type connection-strings
```

## Security Best Practices

1. ✅ **DO** use environment variables or Azure Key Vault for production
2. ✅ **DO** use user secrets for local development
3. ✅ **DO** rotate your Cosmos DB keys regularly
4. ✅ **DO** use Managed Identities when deploying to Azure
5. ✅ **DO** restrict access to your Cosmos DB using firewall rules
6. ❌ **DON'T** commit connection strings to source control
7. ❌ **DON'T** share connection strings in chat, email, or documentation
8. ❌ **DON'T** use the same keys for development and production
9. ❌ **DON'T** store secrets in plain text files

## Troubleshooting

**Error: "CosmosDb configuration is missing in appsettings.json"**
- Ensure you've set the connection string using one of the methods above
- Verify the configuration key names match exactly (case-sensitive)
- Check that `RepositoryType` is set to `CosmosDb`

**Connection string not loading:**
- Check environment variable names use double underscores: `CosmosDb__ConnectionString`
- In user secrets, use colons: `CosmosDb:ConnectionString`
- Verify the environment is correct (Development/Production/Staging)

**Still having issues:**
- Enable detailed logging: Set `"Logging:LogLevel:Default": "Debug"` in appsettings.json
- Check the application logs for configuration-related errors
- Verify Azure Cosmos DB firewall allows connections from your IP/Azure service

## References

- [Safe storage of app secrets in development in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Use Azure Key Vault configuration provider in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/key-vault-configuration)
- [Azure Cosmos DB security](https://learn.microsoft.com/en-us/azure/cosmos-db/database-security)
