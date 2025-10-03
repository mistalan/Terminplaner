# GitHub Actions Workflows - Quick Reference

This document provides a quick reference for the GitHub Actions workflows in this repository.

## 📋 Available Workflows

### 1. CI - Build and Test (`ci.yml`)

**Status Badge:**
```markdown
[![CI](https://github.com/mistalan/Terminplaner/actions/workflows/ci.yml/badge.svg)](https://github.com/mistalan/Terminplaner/actions/workflows/ci.yml)
```

**Trigger:** Automatic on push/PR to `main`

**Purpose:** Validates code quality on every change

**What it does:**
- ✅ Builds the API project
- ✅ Runs all 42 tests (unit + integration)
- ✅ Checks code formatting
- ✅ Runs on Ubuntu (fastest and cheapest runner)

**Duration:** ~2-3 minutes

**View:** [Actions → CI - Build and Test](https://github.com/mistalan/Terminplaner/actions/workflows/ci.yml)

---

### 2. Deploy API (`deploy-api.yml`)

**Trigger:** 
- Manual: Go to Actions → Deploy API → Run workflow
- Automatic: Push tag `v*.*.*` (e.g., `v1.0.0`)

**Purpose:** Creates deployable API packages

**What it does:**
- ✅ Runs all tests
- ✅ Builds API for Linux x64
- ✅ Builds API for Windows x64
- ✅ Creates tar.gz (Linux) and zip (Windows) packages
- ✅ Uploads artifacts (30-90 day retention)

**Duration:** ~3-5 minutes

**Artifacts:**
- `api-linux-x64` - Extracted files for Linux
- `api-win-x64` - Extracted files for Windows
- `deployment-packages` - Compressed archives ready for download

**How to use:**
```bash
# 1. Trigger workflow manually via GitHub UI or push tag:
git tag v1.0.0
git push origin v1.0.0

# 2. Download artifact from workflow run page

# 3. Deploy on Linux:
tar -xzf terminplaner-api-linux-x64.tar.gz -C /opt/terminplaner
cd /opt/terminplaner && ./TerminplanerApi

# 4. Deploy on Windows:
# Extract ZIP and run TerminplanerApi.exe
```

**View:** [Actions → Deploy API](https://github.com/mistalan/Terminplaner/actions/workflows/deploy-api.yml)

---

### 3. Deploy Android App (`deploy-android.yml`)

**Trigger:**
- Manual: Go to Actions → Deploy Android App → Run workflow
- Automatic: Push tag `android-v*.*.*` (e.g., `android-v1.0.0`)

**Purpose:** Builds Android APK

**What it does:**
- ✅ Installs MAUI Android workload
- ✅ Builds Android app (Debug or Release)
- ✅ Publishes APK
- ✅ Uploads APK artifact (90 day retention)

**Duration:** ~10-15 minutes (first run), ~5-8 minutes (cached)

**Platform:** Windows runner (required for MAUI)

**Artifacts:**
- `terminplaner-android-apk` - APK file for Android installation

**How to use:**
```bash
# 1. Trigger workflow via GitHub UI

# 2. Download APK from workflow run page

# 3. Install on Android:
#    - Copy APK to device
#    - Enable "Unknown Sources" in Settings
#    - Tap APK to install
```

**Future: Signed APK for Play Store**
- Uncomment the signed build job
- Configure secrets: `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEY_ALIAS`, etc.

**View:** [Actions → Deploy Android App](https://github.com/mistalan/Terminplaner/actions/workflows/deploy-android.yml)

---

### 4. Deploy Windows App (`deploy-windows.yml`)

**Trigger:**
- Manual: Go to Actions → Deploy Windows App → Run workflow
- Automatic: Push tag `windows-v*.*.*` (e.g., `windows-v1.0.0`)

**Purpose:** Builds Windows desktop app

**What it does:**
- ✅ Installs MAUI Windows workload
- ✅ Builds Windows app (Debug or Release)
- ✅ Publishes Windows executable/MSIX
- ✅ Uploads artifacts (90 day retention)

**Duration:** ~10-15 minutes (first run), ~5-8 minutes (cached)

**Platform:** Windows runner (required for MAUI)

**Artifacts:**
- `terminplaner-windows` - Windows app files

**How to use:**
```bash
# 1. Trigger workflow via GitHub UI

# 2. Download artifact from workflow run page

# 3. Extract and distribute to Windows users
```

**Future: Signed MSIX for Microsoft Store**
- Uncomment the signed build job
- Configure secrets: `WINDOWS_PFX_BASE64`, `WINDOWS_PFX_PASSWORD`

**View:** [Actions → Deploy Windows App](https://github.com/mistalan/Terminplaner/actions/workflows/deploy-windows.yml)

---

## 🎯 Common Use Cases

### Scenario 1: Regular Development

**Workflow:** CI automatically runs on every push/PR

```bash
git add .
git commit -m "Add new feature"
git push origin main
# CI runs automatically - check Actions tab
```

### Scenario 2: Create a Release

**For API:**
```bash
# 1. Update version in code
# 2. Create tag
git tag v1.0.0
git push origin v1.0.0
# 3. Deploy API workflow runs automatically
# 4. Download artifacts from Actions tab
```

**For Android:**
```bash
# 1. Update version in TerminplanerMaui.csproj
# 2. Create tag
git tag android-v1.0.0
git push origin android-v1.0.0
# 3. Deploy Android workflow runs automatically
# 4. Download APK from Actions tab
```

**For Windows:**
```bash
# 1. Update version in TerminplanerMaui.csproj
# 2. Create tag
git tag windows-v1.0.0
git push origin windows-v1.0.0
# 3. Deploy Windows workflow runs automatically
# 4. Download app from Actions tab
```

### Scenario 3: Quick Manual Build

**Go to GitHub → Actions → Select workflow → Run workflow**

1. Click "Run workflow" button
2. Select configuration (if applicable)
3. Click "Run workflow"
4. Wait for completion
5. Download artifacts

---

## 🛠️ Workflow Configuration

### Runners

| Workflow | Runner | Why |
|----------|--------|-----|
| CI | `ubuntu-latest` | Fastest, cheapest, sufficient for API tests |
| Deploy API | `ubuntu-latest` | Can build for both Linux and Windows |
| Deploy Android | `windows-latest` | MAUI workloads work best on Windows |
| Deploy Windows | `windows-latest` | Required for Windows MAUI builds |

### Artifact Retention

| Artifact Type | Retention |
|---------------|-----------|
| CI builds | Not uploaded (tests only) |
| API packages | 90 days |
| Android APK | 90 days |
| Windows app | 90 days |
| Temporary API artifacts | 30 days |

### Secrets Required (for signed builds)

**Android (optional):**
- `ANDROID_KEYSTORE_BASE64` - Base64-encoded keystore
- `ANDROID_KEY_ALIAS` - Keystore alias
- `ANDROID_KEY_PASSWORD` - Key password
- `ANDROID_KEYSTORE_PASSWORD` - Keystore password

**Windows (optional):**
- `WINDOWS_PFX_BASE64` - Base64-encoded certificate
- `WINDOWS_PFX_PASSWORD` - Certificate password

---

## 📊 Workflow Comparison

| Feature | CI | Deploy API | Deploy Android | Deploy Windows |
|---------|----|-----------:|---------------:|---------------:|
| Auto on push | ✅ | ❌ | ❌ | ❌ |
| Auto on tag | ❌ | ✅ | ✅ | ✅ |
| Manual trigger | ❌ | ✅ | ✅ | ✅ |
| Runs tests | ✅ | ✅ | ❌ | ❌ |
| Creates artifacts | ❌ | ✅ | ✅ | ✅ |
| Duration | 2-3 min | 3-5 min | 10-15 min | 10-15 min |
| Runner cost | Low | Low | Medium | Medium |

---

## 🔍 Troubleshooting Workflows

### CI Workflow Fails

**Tests fail:**
- Check test output in workflow logs
- Run tests locally: `dotnet test`
- Fix failing tests

**Build fails:**
- Check for syntax errors
- Verify .NET 9.0 compatibility
- Check dependencies

**Format check fails:**
- Run `dotnet format` locally
- Commit formatting changes

### Deploy API Workflow Fails

**Restore fails:**
- Check internet connectivity in runner
- Verify NuGet sources

**Publish fails:**
- Check target runtime compatibility
- Verify output paths

### Deploy Android/Windows Fails

**MAUI workload installation fails:**
- Check runner has sufficient disk space
- Verify .NET version compatibility

**Build fails:**
- Check for platform-specific issues
- Verify project configuration
- Check Android SDK version (Android)
- Check Windows SDK version (Windows)

---

## 📈 Metrics & Monitoring

**View workflow runs:**
- Go to: https://github.com/mistalan/Terminplaner/actions

**Add status badges to README:**
```markdown
[![CI](https://github.com/mistalan/Terminplaner/actions/workflows/ci.yml/badge.svg)](https://github.com/mistalan/Terminplaner/actions/workflows/ci.yml)
```

**Monitor workflow usage:**
- Settings → Billing → Actions usage
- Free tier: 2,000 minutes/month (public repos have unlimited)

---

## 📝 Maintenance

### Update Workflows

1. Edit workflow files in `.github/workflows/`
2. Test changes in a feature branch
3. Monitor workflow runs
4. Merge to main when validated

### Update .NET Version

When .NET 10 becomes available:
1. Update `dotnet-version: 10.0.x` in all workflows
2. Update projects to target `net10.0`
3. Test locally first
4. Update CI workflow
5. Update deployment workflows

### Update Actions Versions

Check for updates to GitHub Actions:
- `actions/checkout` - Currently v4
- `actions/setup-dotnet` - Currently v4
- `actions/upload-artifact` - Currently v4

---

## 🔗 Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET CLI Documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [MAUI Deployment Guide](DEPLOYMENT.md)
- [Test Documentation](TEST_CASES.md)

---

**Last Updated:** 2024-10-03
