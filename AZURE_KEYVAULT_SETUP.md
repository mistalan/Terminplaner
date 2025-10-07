# Azure Key Vault Setup and GitHub Actions Integration

This guide explains how to set up Azure Key Vault for the Terminplaner application and grant GitHub Actions access.

## Part 1: Setting Up Azure Key Vault

### 1. Store Cosmos DB Connection String in Key Vault

**Using Azure Portal:**

1. Navigate to your Key Vault: https://terminplaner.vault.azure.net/
2. Click **Secrets** in the left menu
3. Click **+ Generate/Import**
4. Configure the secret:
   - **Upload options**: Manual
   - **Name**: `CosmosDbConnectionString` (no special characters, use CamelCase)
   - **Value**: Your Cosmos DB connection string
   - **Content type**: (optional) `connection-string`
   - **Enabled**: Yes
   - Click **Create**

**Using Azure CLI:**

```bash
# Set your Cosmos DB connection string
COSMOS_CONNECTION_STRING="AccountEndpoint=https://terninplaner.documents.azure.com:443/;AccountKey=YOUR_KEY_HERE"

# Store in Key Vault
az keyvault secret set \
  --vault-name terminplaner \
  --name CosmosDbConnectionString \
  --value "$COSMOS_CONNECTION_STRING"
```

### 2. Store Additional Secrets (Optional)

```bash
az keyvault secret set --vault-name terminplaner --name CosmosDbDatabaseId --value "db_1"
az keyvault secret set --vault-name terminplaner --name CosmosDbContainerId --value "container_1"
```

### 3. Verify Secrets

**Azure Portal:**
- Navigate to Secrets and verify `CosmosDbConnectionString` is listed

**Azure CLI:**
```bash
# List all secrets
az keyvault secret list --vault-name terminplaner --output table

# Show secret value (be careful with this command)
az keyvault secret show --vault-name terminplaner --name CosmosDbConnectionString --query value -o tsv
```

## Part 2: Granting GitHub Actions Access to Key Vault

### Option 1: Using Federated Identity (Recommended - No Secrets Needed)

This method uses OpenID Connect (OIDC) for passwordless authentication.

#### Step 1: Create a Microsoft Entra ID App Registration

```bash
# Create app registration
APP_NAME="Terminplaner-GitHub-Actions"
APP_ID=$(az ad app create --display-name "$APP_NAME" --query appId -o tsv)
echo "App ID: $APP_ID"

# Create service principal
SP_ID=$(az ad sp create --id $APP_ID --query id -o tsv)
echo "Service Principal ID: $SP_ID"
```

#### Step 2: Configure Federated Credentials for GitHub

```bash
# Set your GitHub repository details
GITHUB_ORG="mistalan"
GITHUB_REPO="Terminplaner"

# Create federated credential for main branch
az ad app federated-credential create \
  --id $APP_ID \
  --parameters '{
    "name": "GitHubActionsMain",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:'"$GITHUB_ORG"'/'"$GITHUB_REPO"':ref:refs/heads/main",
    "audiences": ["api://AzureADTokenExchange"]
  }'

# Create federated credential for pull requests
az ad app federated-credential create \
  --id $APP_ID \
  --parameters '{
    "name": "GitHubActionsPR",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:'"$GITHUB_ORG"'/'"$GITHUB_REPO"':pull_request",
    "audiences": ["api://AzureADTokenExchange"]
  }'

# Create federated credential for all branches (optional)
az ad app federated-credential create \
  --id $APP_ID \
  --parameters '{
    "name": "GitHubActionsAllBranches",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:'"$GITHUB_ORG"'/'"$GITHUB_REPO"':ref:refs/heads/*",
    "audiences": ["api://AzureADTokenExchange"]
  }'
```

#### Step 3: Grant Key Vault Access

```bash
# Get your Azure subscription ID
SUBSCRIPTION_ID=$(az account show --query id -o tsv)

# Grant the service principal access to Key Vault secrets
az keyvault set-policy \
  --name terminplaner \
  --object-id $SP_ID \
  --secret-permissions get list

# Alternative: Use Azure RBAC (recommended for new Key Vaults)
az role assignment create \
  --role "Key Vault Secrets User" \
  --assignee $APP_ID \
  --scope "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/YOUR_RESOURCE_GROUP/providers/Microsoft.KeyVault/vaults/terminplaner"
```

#### Step 4: Configure GitHub Secrets

Add these secrets to your GitHub repository:

1. Go to: `https://github.com/mistalan/Terminplaner/settings/secrets/actions`
2. Add the following secrets:

| Secret Name | Value | How to Get |
|-------------|-------|------------|
| `AZURE_CLIENT_ID` | Application (client) ID | `echo $APP_ID` |
| `AZURE_TENANT_ID` | Directory (tenant) ID | `az account show --query tenantId -o tsv` |
| `AZURE_SUBSCRIPTION_ID` | Subscription ID | `az account show --query id -o tsv` |

**No client secret needed!** OIDC authentication is passwordless.

### Option 2: Using Service Principal with Secret (Legacy Method)

If OIDC is not available:

#### Step 1: Create Service Principal with Secret

```bash
# Create service principal
SP_NAME="Terminplaner-GitHub-Actions"
SP_SECRET=$(az ad sp create-for-rbac --name $SP_NAME --query password -o tsv)
SP_APP_ID=$(az ad sp list --display-name $SP_NAME --query [0].appId -o tsv)
SP_OBJECT_ID=$(az ad sp list --display-name $SP_NAME --query [0].id -o tsv)

echo "Client ID: $SP_APP_ID"
echo "Client Secret: $SP_SECRET"  # Save this securely!
echo "Object ID: $SP_OBJECT_ID"
```

#### Step 2: Grant Key Vault Access

```bash
az keyvault set-policy \
  --name terminplaner \
  --object-id $SP_OBJECT_ID \
  --secret-permissions get list
```

#### Step 3: Configure GitHub Secrets

Add these secrets to your GitHub repository:

| Secret Name | Value |
|-------------|-------|
| `AZURE_CLIENT_ID` | Service principal app ID |
| `AZURE_CLIENT_SECRET` | Service principal password |
| `AZURE_TENANT_ID` | Your tenant ID |
| `AZURE_SUBSCRIPTION_ID` | Your subscription ID |

## Part 3: Using Key Vault in GitHub Actions

### Example Workflow with OIDC (Recommended)

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [main]

permissions:
  id-token: write
  contents: read

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Azure Login (OIDC)
        uses: azure/login@v1
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      
      - name: Get Secrets from Key Vault
        id: keyvault
        run: |
          COSMOS_CONNECTION=$(az keyvault secret show \
            --vault-name terminplaner \
            --name CosmosDbConnectionString \
            --query value -o tsv)
          echo "::add-mask::$COSMOS_CONNECTION"
          echo "COSMOS_CONNECTION=$COSMOS_CONNECTION" >> $GITHUB_OUTPUT
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      
      - name: Build and Test
        env:
          CosmosDb__ConnectionString: ${{ steps.keyvault.outputs.COSMOS_CONNECTION }}
          CosmosDb__DatabaseId: "db_1"
          CosmosDb__ContainerId: "container_1"
          RepositoryType: "CosmosDb"
        run: |
          cd TerminplanerApi
          dotnet restore
          dotnet build --configuration Release
          dotnet test --configuration Release
```

### Example Workflow with Client Secret

```yaml
name: Deploy to Azure

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: |
            {
              "clientId": "${{ secrets.AZURE_CLIENT_ID }}",
              "clientSecret": "${{ secrets.AZURE_CLIENT_SECRET }}",
              "subscriptionId": "${{ secrets.AZURE_SUBSCRIPTION_ID }}",
              "tenantId": "${{ secrets.AZURE_TENANT_ID }}"
            }
      
      - name: Get Secrets from Key Vault
        uses: azure/get-keyvault-secrets@v1
        with:
          keyvault: "terminplaner"
          secrets: |
            CosmosDbConnectionString
            CosmosDbDatabaseId
            CosmosDbContainerId
        id: keyvault
      
      - name: Build and Test
        env:
          CosmosDb__ConnectionString: ${{ steps.keyvault.outputs.CosmosDbConnectionString }}
          CosmosDb__DatabaseId: ${{ steps.keyvault.outputs.CosmosDbDatabaseId }}
          CosmosDb__ContainerId: ${{ steps.keyvault.outputs.CosmosDbContainerId }}
          RepositoryType: "CosmosDb"
        run: |
          cd TerminplanerApi
          dotnet test --configuration Release
```

## Part 4: Update Application to Use Key Vault

### Option 1: Use Key Vault Directly in Application

Install NuGet packages:

```bash
cd TerminplanerApi
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

Update `Program.cs`:

```csharp
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault configuration
if (!builder.Environment.IsDevelopment())
{
    var keyVaultEndpoint = new Uri("https://terminplaner.vault.azure.net/");
    var credential = new DefaultAzureCredential();
    
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, credential);
}

// Rest of your configuration...
```

Update `appsettings.json`:

```json
{
  "RepositoryType": "CosmosDb",
  "CosmosDb": {
    "ConnectionString": "",  // Will be loaded from Key Vault
    "DatabaseId": "db_1",
    "ContainerId": "container_1"
  }
}
```

The app will automatically load `CosmosDbConnectionString` from Key Vault and map it to `CosmosDb:ConnectionString`.

### Option 2: Use Environment Variables (Simpler)

Keep the current code and let GitHub Actions set environment variables from Key Vault secrets (as shown in workflow examples above).

## Verification

### Test Key Vault Access Locally

```bash
# Login to Azure
az login

# Test getting the secret
az keyvault secret show \
  --vault-name terminplaner \
  --name CosmosDbConnectionString \
  --query value -o tsv
```

### Test GitHub Actions Access

Push a test workflow to verify access:

```yaml
name: Test Key Vault Access

on: workflow_dispatch

permissions:
  id-token: write
  contents: read

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - name: Azure Login
        uses: azure/login@v1
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      
      - name: Test Key Vault Access
        run: |
          SECRET=$(az keyvault secret show \
            --vault-name terminplaner \
            --name CosmosDbConnectionString \
            --query value -o tsv)
          if [ -n "$SECRET" ]; then
            echo "✅ Successfully retrieved secret from Key Vault"
          else
            echo "❌ Failed to retrieve secret"
            exit 1
          fi
```

## Troubleshooting

**Error: "Caller is not authorized to perform action"**
- Verify the service principal has proper permissions
- Check `az keyvault set-policy` command was successful
- Wait a few minutes for permissions to propagate

**Error: "AADSTS700016: Application not found"**
- Verify AZURE_CLIENT_ID is correct
- Ensure federated credentials are configured for your repository

**Error: "Secret not found"**
- Verify secret name matches exactly (case-sensitive)
- Check secret is enabled in Key Vault

**OIDC authentication fails**
- Verify `permissions: id-token: write` is set in workflow
- Check federated credential subject matches your repository

## Best Practices

1. ✅ Use OIDC/federated identity (no secrets to manage)
2. ✅ Grant minimal permissions (only `get` and `list` for secrets)
3. ✅ Use `::add-mask::` to hide secrets in logs
4. ✅ Rotate service principal secrets regularly (if using secrets)
5. ✅ Use separate Key Vaults for dev/staging/production
6. ✅ Enable Key Vault audit logging
7. ❌ Don't print secret values in logs
8. ❌ Don't grant write permissions unless absolutely necessary

## References

- [Azure Key Vault Documentation](https://learn.microsoft.com/en-us/azure/key-vault/)
- [GitHub OIDC with Azure](https://docs.github.com/en/actions/deployment/security-hardening-your-deployments/configuring-openid-connect-in-azure)
- [Azure Login Action](https://github.com/Azure/login)
- [Get Key Vault Secrets Action](https://github.com/Azure/get-keyvault-secrets)
