# Git History Cleanup - Removing Accidentally Committed Secrets

## ⚠️ CRITICAL: First Steps

**IMMEDIATELY rotate the compromised credentials:**

```bash
# 1. Regenerate Cosmos DB keys in Azure Portal
#    Go to: Azure Portal → Cosmos DB Account → Keys → Regenerate Primary Key

# 2. Or use Azure CLI:
az cosmosdb keys regenerate \
  --name terminplaner \
  --resource-group YOUR_RESOURCE_GROUP \
  --key-kind primary
```

**Why rotate first?**
- Once credentials are committed to GitHub, assume they are compromised
- Cleaning git history doesn't remove data from GitHub's cache immediately
- Rotating keys makes the old credentials useless

## Understanding the Problem

When secrets are committed to git:
1. They exist in commit history
2. They're stored in GitHub's cache
3. They may be in forks, clones, or Pull Request discussions
4. Simply deleting the file in a new commit doesn't remove them

## Solution Options

### Option 1: GitHub Secret Scanning (Automatic - Already Done)

GitHub automatically scans for secrets and notifies you. If detected:
- You'll receive a security alert
- GitHub may revoke certain types of tokens automatically
- Check: `https://github.com/mistalan/Terminplaner/security/secret-scanning`

**Action Required:**
- Review alerts
- Rotate any exposed credentials
- Close alerts after rotation

### Option 2: Rewrite Git History (Complete Removal)

This permanently removes secrets from git history. **Use with caution!**

#### Using BFG Repo-Cleaner (Recommended - Easier)

**Install BFG:**

```bash
# macOS
brew install bfg

# Linux (download JAR)
wget https://repo1.maven.org/maven2/com/madgag/bfg/1.14.0/bfg-1.14.0.jar
alias bfg='java -jar bfg-1.14.0.jar'

# Windows (download from https://rtyley.github.io/bfg-repo-cleaner/)
```

**Clean the repository:**

```bash
# 1. Clone a fresh copy
cd /tmp
git clone --mirror https://github.com/mistalan/Terminplaner.git

# 2. Create a file with text patterns to remove
cat > secrets.txt << 'EOF'
AccountKey=****
bQborSCArDOwv9PqhrT3Q0mlcGuEd2vygZSEyGX1LwtmAsHqcoki2Sh5BBmbwmSZaJBYt1nC2348ACDbHLx7oQ==
AccountEndpoint=https://terninplaner.documents.azure.com:443/
EOF

# 3. Run BFG to remove secrets
cd Terminplaner.git
bfg --replace-text ../secrets.txt

# 4. Clean up
git reflog expire --expire=now --all
git gc --prune=now --aggressive

# 5. Force push (⚠️ This rewrites history!)
git push --force

# 6. Clean up
cd ..
rm -rf Terminplaner.git secrets.txt
```

#### Using git-filter-repo (More Control)

**Install git-filter-repo:**

```bash
# macOS
brew install git-filter-repo

# Linux
pip3 install git-filter-repo

# Windows
pip install git-filter-repo
```

**Clean specific files:**

```bash
# 1. Clone repository
cd /tmp
git clone https://github.com/mistalan/Terminplaner.git
cd Terminplaner

# 2. Remove specific file from all history
git filter-repo --invert-paths --path TerminplanerApi/appsettings.json

# Or remove specific text from all files
cat > expressions.txt << 'EOF'
regex:AccountKey=[A-Za-z0-9+/=]+
regex:AccountEndpoint=https://[a-z0-9.-]+
EOF

git filter-repo --replace-text expressions.txt

# 3. Force push
git push --force --all
git push --force --tags

# 4. Clean up
cd ..
rm -rf Terminplaner
```

#### Using git filter-branch (Built-in but Slower)

```bash
# 1. Clone repository
git clone https://github.com/mistalan/Terminplaner.git
cd Terminplaner

# 2. Remove file from all commits
git filter-branch --force --index-filter \
  "git rm --cached --ignore-unmatch TerminplanerApi/appsettings.json" \
  --prune-empty --tag-name-filter cat -- --all

# 3. Clean up
git reflog expire --expire=now --all
git gc --prune=now --aggressive

# 4. Force push
git push origin --force --all
git push origin --force --tags
```

### Option 3: Delete and Recreate Repository (Nuclear Option)

If the repository is new and doesn't have much history:

```bash
# 1. Backup your code locally
cd /tmp
git clone https://github.com/mistalan/Terminplaner.git Terminplaner-backup
cd Terminplaner-backup
git checkout main

# 2. Create fresh repository
rm -rf .git
git init
git add .
git commit -m "Initial commit with secure configuration"

# 3. Create new GitHub repository
# Go to GitHub → New Repository → Create "Terminplaner-new"

# 4. Push to new repository
git remote add origin https://github.com/mistalan/Terminplaner-new.git
git push -u origin main

# 5. Archive old repository and transfer issues/PRs manually
```

## Important Warnings for History Rewriting

### Before You Proceed

- ✅ **Rotate all exposed credentials FIRST**
- ✅ **Notify all collaborators** (they need to re-clone)
- ✅ **Backup the repository** before rewriting
- ✅ **Close all open Pull Requests** (they'll be invalidated)
- ✅ **Note all issue numbers** you want to preserve
- ❌ **Don't do this on active collaborative projects** without coordination

### After Force Pushing

All collaborators must re-clone:

```bash
# Old clone must be discarded
cd ~/projects
rm -rf Terminplaner

# Fresh clone
git clone https://github.com/mistalan/Terminplaner.git
```

If someone tries to pull instead of re-cloning:

```bash
# This will cause conflicts! Must re-clone instead.
# If you must update existing clone:
git fetch origin
git reset --hard origin/main
git clean -fdx
```

## Cleaning Up References

### 1. Close and Delete Pull Requests

```bash
# List all PRs
gh pr list --state all

# Close PRs that reference the old commits
gh pr close PR_NUMBER --delete-branch
```

### 2. Delete Tags with Secrets

```bash
# List tags
git tag

# Delete local tag
git tag -d TAG_NAME

# Delete remote tag
git push origin :refs/tags/TAG_NAME
```

### 3. Clear GitHub Actions Logs

GitHub Actions logs may contain the secrets:

1. Go to: `https://github.com/mistalan/Terminplaner/actions`
2. For each workflow run that might have logged secrets:
   - Click the workflow run
   - Click the three dots (•••) in the top right
   - Select "Delete workflow run"

### 4. Request GitHub Cache Purge

Contact GitHub Support to purge cached data:

1. Go to: `https://support.github.com/`
2. Select "Private Information Removal"
3. Explain you need to purge commits containing credentials
4. Provide commit SHAs and file paths

## Verification

### Verify Secrets Are Removed

```bash
# Search entire history for a pattern
git log -S "AccountKey=" --all --pretty=format:"%H %s"

# If this returns results, secrets still exist
# If empty, they've been removed

# Search for specific text
git grep "bQborSCArDOwv9" $(git rev-list --all)

# Check specific file history
git log --all --full-history -- TerminplanerApi/appsettings.json
```

### Verify Key Rotation

```bash
# Test old connection string (should fail)
az cosmosdb sql database list \
  --account-name terminplaner \
  --key "OLD_KEY_HERE"
# Expected: Authentication failed

# Test new connection string (should work)
az cosmosdb sql database list \
  --account-name terminplaner \
  --key "NEW_KEY_HERE"
# Expected: List of databases
```

## Prevention for the Future

### 1. Use git-secrets

Install and configure pre-commit hooks:

```bash
# Install git-secrets
brew install git-secrets  # macOS
# or
git clone https://github.com/awslabs/git-secrets.git
cd git-secrets && sudo make install

# Setup in repository
cd /path/to/Terminplaner
git secrets --install
git secrets --register-aws
git secrets --add 'AccountKey=[A-Za-z0-9+/=]+'
git secrets --add 'AccountEndpoint=https://[a-z0-9.-]+'

# Scan existing commits
git secrets --scan-history
```

### 2. Use pre-commit Hooks

Create `.pre-commit-config.yaml`:

```yaml
repos:
  - repo: https://github.com/Yelp/detect-secrets
    rev: v1.4.0
    hooks:
      - id: detect-secrets
        args: ['--baseline', '.secrets.baseline']
```

### 3. Use .gitignore Properly

Ensure these are in `.gitignore`:

```gitignore
# Sensitive configuration
appsettings.Production.json
appsettings.*.Local.json
*.secret
*.key
.env
.env.local

# User secrets
**/secrets.json
```

### 4. Use Environment Variables

Update your workflow:

```bash
# Never commit
export COSMOS_KEY="..."

# Always use
export COSMOS_KEY="${COSMOS_KEY}"  # Reference environment variable
```

## Summary Checklist

For the Terminplaner repository:

- [ ] **STEP 1: Rotate Cosmos DB keys immediately** ⚠️ CRITICAL
- [ ] **STEP 2: Update Key Vault with new keys**
- [ ] **STEP 3: Update GitHub Codespaces secrets**
- [ ] **STEP 4: Verify old keys no longer work**
- [ ] **STEP 5: Choose history cleanup method** (BFG recommended)
- [ ] **STEP 6: Backup repository**
- [ ] **STEP 7: Rewrite git history**
- [ ] **STEP 8: Force push changes**
- [ ] **STEP 9: Verify secrets removed from history**
- [ ] **STEP 10: Delete old GitHub Actions logs**
- [ ] **STEP 11: Contact GitHub Support for cache purge**
- [ ] **STEP 12: Install git-secrets for prevention**
- [ ] **STEP 13: Update all local clones**

## Quick Start for Terminplaner

```bash
# 1. FIRST: Rotate the key
az cosmosdb keys regenerate \
  --name terminplaner \
  --resource-group YOUR_RESOURCE_GROUP \
  --key-kind primary

# 2. Get the new key
NEW_KEY=$(az cosmosdb keys list \
  --name terminplaner \
  --resource-group YOUR_RESOURCE_GROUP \
  --query primaryMasterKey -o tsv)

# 3. Update Key Vault
az keyvault secret set \
  --vault-name terminplaner \
  --name CosmosDbConnectionString \
  --value "AccountEndpoint=https://terninplaner.documents.azure.com:443/;AccountKey=$NEW_KEY"

# 4. Update Codespaces secret (via GitHub UI)
# Go to: https://github.com/mistalan/Terminplaner/settings/secrets/codespaces

# 5. Clean git history with BFG
cd /tmp
git clone --mirror https://github.com/mistalan/Terminplaner.git
cd Terminplaner.git
echo "AccountKey=" > patterns.txt
bfg --replace-text patterns.txt
git reflog expire --expire=now --all
git gc --prune=now --aggressive
git push --force

# 6. Verify
cd /tmp
git clone https://github.com/mistalan/Terminplaner.git test-clean
cd test-clean
git log -S "AccountKey=" --all
# Should return nothing
```

## Need Help?

If you encounter issues:
1. Don't panic - the key rotation is the most important step
2. Backup before any history rewriting
3. Test commands on a clone first
4. Ask for help in GitHub Discussions if unsure

## References

- [BFG Repo-Cleaner](https://rtyley.github.io/bfg-repo-cleaner/)
- [git-filter-repo](https://github.com/newren/git-filter-repo)
- [GitHub Secret Scanning](https://docs.github.com/en/code-security/secret-scanning)
- [Removing Sensitive Data from GitHub](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/removing-sensitive-data-from-a-repository)
