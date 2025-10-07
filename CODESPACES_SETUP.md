# GitHub Codespaces Configuration Guide

This guide explains how to set up environment variables and secrets for the Terminplaner API in GitHub Codespaces.

## Overview

GitHub Codespaces provides a cloud-based development environment. For security, sensitive credentials like Azure Cosmos DB connection strings should be stored as Codespaces secrets rather than in configuration files.

## Setting Up Codespaces Secrets

### Option 1: Repository Secrets (Recommended)

Repository secrets are available to all Codespaces created from your repository.

1. **Navigate to Repository Settings:**
   - Go to your repository: `https://github.com/mistalan/Terminplaner`
   - Click on **Settings** tab
   - In the left sidebar, click **Secrets and variables** → **Codespaces**

2. **Add New Secret:**
   - Click **New repository secret**
   - Name: `COSMOSDB_CONNECTION_STRING`
   - Value: `AccountEndpoint=https://terninplaner.documents.azure.com:443/;AccountKey=YOUR_KEY_HERE`
   - Click **Add secret**

3. **Add Additional Secrets (Optional):**
   - Name: `COSMOSDB_DATABASE_ID`, Value: `db_1`
   - Name: `COSMOSDB_CONTAINER_ID`, Value: `container_1`
   - Name: `REPOSITORY_TYPE`, Value: `CosmosDb`

### Option 2: User Secrets (Personal)

User secrets are available only to Codespaces created by you, across all repositories.

1. **Navigate to User Settings:**
   - Click your profile picture → **Settings**
   - In the left sidebar, click **Codespaces**
   - Scroll to **Codespaces secrets**

2. **Add New Secret:**
   - Click **New secret**
   - Name: `COSMOSDB_CONNECTION_STRING`
   - Value: `AccountEndpoint=https://terninplaner.documents.azure.com:443/;AccountKey=YOUR_KEY_HERE`
   - Select repositories that can access this secret
   - Click **Add secret**

## Using Secrets in Codespaces

### Method 1: Environment Variables in devcontainer.json

Create or update `.devcontainer/devcontainer.json`:

```json
{
  "name": "Terminplaner Dev Environment",
  "image": "mcr.microsoft.com/dotnet/sdk:9.0",
  "containerEnv": {
    "CosmosDb__ConnectionString": "${localEnv:COSMOSDB_CONNECTION_STRING}",
    "CosmosDb__DatabaseId": "${localEnv:COSMOSDB_DATABASE_ID:db_1}",
    "CosmosDb__ContainerId": "${localEnv:COSMOSDB_CONTAINER_ID:container_1}",
    "RepositoryType": "${localEnv:REPOSITORY_TYPE:InMemory}"
  },
  "postCreateCommand": "dotnet restore"
}
```

### Method 2: Manual Environment Variables

If you don't use a devcontainer, set environment variables in your Codespace terminal:

```bash
# Add to ~/.bashrc or ~/.zshrc for persistence
echo 'export CosmosDb__ConnectionString="${COSMOSDB_CONNECTION_STRING}"' >> ~/.bashrc
echo 'export CosmosDb__DatabaseId="db_1"' >> ~/.bashrc
echo 'export CosmosDb__ContainerId="container_1"' >> ~/.bashrc
echo 'export RepositoryType="CosmosDb"' >> ~/.bashrc

# Reload shell configuration
source ~/.bashrc
```

### Method 3: User Secrets (Recommended for .NET)

Even in Codespaces, you can use .NET user secrets:

```bash
cd TerminplanerApi
dotnet user-secrets init
dotnet user-secrets set "CosmosDb:ConnectionString" "${COSMOSDB_CONNECTION_STRING}"
dotnet user-secrets set "CosmosDb:DatabaseId" "db_1"
dotnet user-secrets set "CosmosDb:ContainerId" "container_1"
dotnet user-secrets set "RepositoryType" "CosmosDb"
```

## Accessing Secrets in Your Application

The ASP.NET Core configuration system automatically loads environment variables. No code changes are needed if you use the naming convention `CosmosDb__ConnectionString` (double underscore).

**Verification:**

```bash
# Check if environment variables are set
echo $CosmosDb__ConnectionString

# Or check user secrets
cd TerminplanerApi
dotnet user-secrets list
```

## Running the Application

Once secrets are configured:

```bash
cd TerminplanerApi
dotnet run
```

The application will automatically use the connection string from:
1. User secrets (if set)
2. Environment variables (if set)
3. appsettings.json (fallback - should be empty for security)

## Troubleshooting

**Secret not available in Codespace:**
- Verify the secret is created in repository or user settings
- Restart the Codespace after adding new secrets
- Check secret permissions for the repository

**Application can't connect to Cosmos DB:**
- Verify the connection string format is correct
- Check that the Cosmos DB firewall allows connections from GitHub Codespaces IPs
- Enable "Allow access from Azure Portal" in Cosmos DB firewall settings

**Environment variables not persisting:**
- Add export commands to `~/.bashrc` or `~/.zshrc`
- Or use the devcontainer.json method for automatic setup

## Best Practices

1. ✅ Use repository secrets for team collaboration
2. ✅ Use user secrets for personal development
3. ✅ Never commit actual secrets to `.devcontainer/devcontainer.json`
4. ✅ Use `${localEnv:VAR_NAME:default}` syntax for optional secrets
5. ✅ Document required secrets in repository README
6. ❌ Don't share secret values in chat, issues, or commits

## Example devcontainer.json

Create `.devcontainer/devcontainer.json` in your repository:

```json
{
  "name": "Terminplaner Development",
  "image": "mcr.microsoft.com/dotnet/sdk:9.0",
  "features": {
    "ghcr.io/devcontainers/features/azure-cli:1": {}
  },
  "containerEnv": {
    "CosmosDb__ConnectionString": "${localEnv:COSMOSDB_CONNECTION_STRING}",
    "CosmosDb__DatabaseId": "db_1",
    "CosmosDb__ContainerId": "container_1",
    "RepositoryType": "${localEnv:REPOSITORY_TYPE:InMemory}"
  },
  "postCreateCommand": "dotnet restore && dotnet build",
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csharp",
        "ms-azuretools.vscode-cosmosdb"
      ]
    }
  }
}
```

## References

- [GitHub Codespaces Secrets Documentation](https://docs.github.com/en/codespaces/managing-your-codespaces/managing-secrets-for-your-codespaces)
- [Dev Container Environment Variables](https://containers.dev/implementors/json_reference/#variables-in-devcontainerjson)
- [ASP.NET Core Configuration](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
