# 🚀 Deployment Guide - Terminplaner

This guide explains how to deploy the Terminplaner application using the automated CI/CD workflows.

## 📋 Table of Contents

- [Overview](#overview)
- [CI/CD Workflows](#cicd-workflows)
- [Backend API Deployment](#backend-api-deployment)
- [MAUI App Deployment](#maui-app-deployment)
- [Update Strategies](#update-strategies)
- [Security Considerations](#security-considerations)

## Overview

The Terminplaner project uses GitHub Actions for automated builds and deployments. The CI/CD pipeline consists of:

1. **Continuous Integration** - Automated testing on every push/PR
2. **API Deployment** - Backend API packaging for Linux and Windows servers
3. **Android Deployment** - APK generation for Android devices
4. **Windows Deployment** - MSIX/executable for Windows desktops

## CI/CD Workflows

### 🔄 Continuous Integration (ci.yml)

**Trigger:** Automatically runs on every push and pull request to `main` branch

**Purpose:** 
- Build the API project
- Run all unit and integration tests (42 tests)
- Verify code formatting

**What it does:**
- ✅ Checkout code
- ✅ Setup .NET 9.0
- ✅ Restore dependencies
- ✅ Build API in Release mode
- ✅ Run all tests
- ✅ Check code formatting

**No action required** - This runs automatically to ensure code quality.

---

## Backend API Deployment

### 📦 Deploy API Workflow (deploy-api.yml)

**Trigger:** 
- Manual trigger via GitHub Actions UI
- Automatically on version tags (e.g., `v1.0.0`)

**Supported Platforms:**
- Linux x64 (for Ubuntu/Debian servers)
- Windows x64 (for Windows servers)

### How to Deploy the API

#### Option 1: Manual Deployment via GitHub Actions

1. Go to **Actions** tab in GitHub
2. Select **"Deploy API"** workflow
3. Click **"Run workflow"**
4. Select environment (production/staging/development)
5. Click **"Run workflow"** button

The workflow will:
- Build and test the API
- Create deployment packages for Linux and Windows
- Upload artifacts that you can download

#### Option 2: Tag-based Deployment

Create a version tag to trigger automatic deployment:

```bash
git tag v1.0.0
git push origin v1.0.0
```

### Downloading and Installing the API

After the workflow completes:

1. Go to the workflow run page
2. Download the appropriate artifact:
   - **api-linux-x64** - For Linux servers
   - **api-win-x64** - For Windows servers
   - **deployment-packages** - Pre-packaged archives

#### Installing on Linux

```bash
# Extract the package
tar -xzf terminplaner-api-linux-x64.tar.gz -C /opt/terminplaner-api

# Navigate to the directory
cd /opt/terminplaner-api

# Make the executable runnable
chmod +x TerminplanerApi

# Run the API
./TerminplanerApi
```

**Create a systemd service (recommended for production):**

```bash
sudo nano /etc/systemd/system/terminplaner-api.service
```

Add the following content:

```ini
[Unit]
Description=Terminplaner API Service
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/opt/terminplaner-api
ExecStart=/opt/terminplaner-api/TerminplanerApi
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5215

[Install]
WantedBy=multi-user.target
```

Enable and start the service:

```bash
sudo systemctl daemon-reload
sudo systemctl enable terminplaner-api
sudo systemctl start terminplaner-api
sudo systemctl status terminplaner-api
```

#### Installing on Windows

```powershell
# Extract the ZIP file to C:\Program Files\TerminplanerAPI

# Navigate to the directory
cd "C:\Program Files\TerminplanerAPI"

# Run the API
.\TerminplanerApi.exe
```

**Create a Windows Service (optional):**

Use tools like [NSSM (Non-Sucking Service Manager)](https://nssm.cc/) or create a scheduled task to run on startup.

---

## MAUI App Deployment

### 📱 Android Deployment (deploy-android.yml)

**Trigger:**
- Manual trigger via GitHub Actions UI
- Automatically on Android version tags (e.g., `android-v1.0.0`)

#### How to Build Android APK

1. Go to **Actions** tab in GitHub
2. Select **"Deploy Android App"** workflow
3. Click **"Run workflow"**
4. Select configuration (Release or Debug)
5. Click **"Run workflow"** button

After completion:
1. Download the **terminplaner-android-apk** artifact
2. Extract the APK file
3. Install on Android devices:
   - Copy APK to device
   - Enable "Install from Unknown Sources" in Settings
   - Tap the APK to install

**For Play Store deployment:**
Uncomment the signed build job in `deploy-android.yml` and configure these secrets:
- `ANDROID_KEYSTORE_BASE64` - Base64-encoded keystore file
- `ANDROID_KEY_ALIAS` - Key alias
- `ANDROID_KEY_PASSWORD` - Key password
- `ANDROID_KEYSTORE_PASSWORD` - Keystore password

### 🖥️ Windows Desktop Deployment (deploy-windows.yml)

**Trigger:**
- Manual trigger via GitHub Actions UI
- Automatically on Windows version tags (e.g., `windows-v1.0.0`)

#### How to Build Windows App

1. Go to **Actions** tab in GitHub
2. Select **"Deploy Windows App"** workflow
3. Click **"Run workflow"**
4. Select configuration (Release or Debug)
5. Click **"Run workflow"** button

After completion:
1. Download the **terminplaner-windows** artifact
2. Extract the files
3. Distribute to Windows users

**For Microsoft Store deployment:**
Uncomment the signed build job in `deploy-windows.yml` and configure these secrets:
- `WINDOWS_PFX_BASE64` - Base64-encoded certificate file
- `WINDOWS_PFX_PASSWORD` - Certificate password

---

## Update Strategies

### 🔄 For Local/Personal Use (Current Setup)

**Current Approach:**
- Users manually download new versions when available
- Simple and suitable for personal/family use
- No automatic updates

**Best Practice for Users:**
1. Check GitHub Releases page periodically
2. Download new APK/installer
3. Install over existing version (data preserved)

### 🔄 Future Update Strategies

#### Option 1: In-App Update Notifications (Recommended for MVP)

**Implementation:**
1. Add version checking endpoint to API:
   ```csharp
   app.MapGet("/api/version", () => new { version = "1.0.0" });
   ```

2. MAUI app checks on startup:
   ```csharp
   public async Task CheckForUpdates()
   {
       var currentVersion = new Version("1.0.0");
       var latestVersion = await apiService.GetLatestVersion();
       if (latestVersion > currentVersion)
       {
           // Show update notification
       }
   }
   ```

3. Provide download link to GitHub Releases

#### Option 2: Automatic Updates (Advanced)

**For Android:**
- Use Google Play In-App Updates API (requires Play Store distribution)
- Or implement custom APK download and install mechanism

**For Windows:**
- Use Microsoft Store automatic updates
- Or implement Squirrel.Windows or ClickOnce deployment

#### Option 3: GitHub Releases Integration

Create releases automatically on version tags:

1. Add release creation step to workflows:
   ```yaml
   - name: Create Release
     uses: softprops/action-gh-release@v1
     if: startsWith(github.ref, 'refs/tags/')
     with:
       files: |
         *.apk
         *.msix
   ```

2. Users can subscribe to release notifications
3. Download from GitHub Releases page

### 📅 Recommended Update Schedule

**For Personal Use:**
- Check for updates monthly
- Apply critical security updates immediately
- Test updates in development environment first

**For Production:**
- Implement automated version checking
- Schedule maintenance windows
- Use staged rollouts (staging → production)

---

## Security Considerations

### 🔐 Secrets Management

Never commit secrets to the repository. Use GitHub Secrets for:

- **Signing certificates** (Android keystore, Windows PFX)
- **API keys** for external services
- **Database connection strings**
- **Authentication tokens**

### 🔒 Code Signing

**Android:**
- Generate a keystore for production releases
- Keep keystore secure and backed up
- Use different keys for debug/release builds

**Windows:**
- Obtain a code signing certificate
- Consider EV certificates for better trust
- Sign all production releases

### 🛡️ API Security

For production deployment:

1. **Enable HTTPS:**
   ```csharp
   app.UseHttpsRedirection();
   app.UseHsts();
   ```

2. **Add authentication:**
   - JWT tokens
   - API keys
   - OAuth 2.0

3. **Configure CORS properly:**
   - Restrict to known domains
   - Don't use wildcard (*) in production

4. **Rate limiting:**
   - Prevent abuse
   - Use middleware like AspNetCoreRateLimit

### 🔄 Environment Configuration

Use different `appsettings.{Environment}.json` files:

- `appsettings.Development.json` - Local development
- `appsettings.Staging.json` - Testing environment
- `appsettings.Production.json` - Production deployment

Configure via environment variables:
```bash
export ASPNETCORE_ENVIRONMENT=Production
```

---

## 📝 Deployment Checklist

### Before First Deployment

- [ ] Review and test all workflows
- [ ] Configure GitHub Secrets if using signed builds
- [ ] Test API deployment on target platform
- [ ] Test MAUI app installation on target devices
- [ ] Set up monitoring/logging (optional but recommended)
- [ ] Configure backup strategy for data (when persistence is added)

### For Each Release

- [ ] Update version numbers in project files
- [ ] Update CHANGELOG.md (if exists)
- [ ] Run all tests locally
- [ ] Create and test builds locally
- [ ] Tag the release
- [ ] Verify GitHub Actions workflows complete successfully
- [ ] Download and test artifacts
- [ ] Create GitHub Release with release notes
- [ ] Notify users of new version

---

## 🆘 Troubleshooting

### Workflow Fails

**"MAUI workload not found"**
- Ensure the workflow includes `dotnet workload install maui-android` step
- Check .NET SDK version compatibility

**"Tests failed"**
- Review test output in GitHub Actions logs
- Run tests locally to reproduce
- Fix failing tests before merging

### Deployment Issues

**API won't start**
- Check .NET runtime is installed on target system
- Verify port 5215 is available
- Check appsettings.json configuration
- Review application logs

**MAUI app won't install**
- Enable installation from unknown sources (Android)
- Check minimum OS version compatibility
- Verify app permissions

---

## 📚 Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET MAUI Deployment](https://learn.microsoft.com/en-us/dotnet/maui/deployment/)
- [ASP.NET Core Deployment](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [Android App Signing](https://developer.android.com/studio/publish/app-signing)
- [Windows App Packaging](https://learn.microsoft.com/en-us/windows/msix/)

---

## 📧 Support

For deployment issues, please:
1. Check this guide and troubleshooting section
2. Review GitHub Actions workflow logs
3. Create an issue on GitHub with:
   - Workflow/deployment step that failed
   - Error messages
   - Platform/environment details
