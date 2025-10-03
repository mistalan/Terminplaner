# 🔧 GitHub Apps and Extensions Recommendations - Terminplaner

This document provides a curated list of free GitHub Apps and Extensions that can enhance the Terminplaner repository's CI/CD pipeline, code quality, and deployment workflows.

## 📋 Table of Contents

- [Test Coverage Tools](#test-coverage-tools)
- [CI/CD Enhancement Tools](#cicd-enhancement-tools)
- [Code Quality and Security](#code-quality-and-security)
- [Docker and Container Tools](#docker-and-container-tools)
- [Azure and Cloud Integration](#azure-and-cloud-integration)
- [Documentation and Project Management](#documentation-and-project-management)
- [Implementation Guide](#implementation-guide)

---

## 🧪 Test Coverage Tools

### 1. **Codecov** ⭐ RECOMMENDED
- **Link**: https://github.com/marketplace/codecov
- **Free Tier**: Yes (unlimited public repos)
- **Purpose**: Code coverage reporting and visualization
- **Benefits**:
  - Integrates with .NET test coverage tools (coverlet)
  - Visual coverage reports on PRs
  - Coverage badges for README
  - Line-by-line coverage in PR diffs
  - Historical coverage tracking
- **Installation**: Can be installed via GitHub Marketplace
- **Setup Required**:
  ```yaml
  # Add to .github/workflows/ci.yml
  - name: Run tests with coverage
    run: dotnet test --collect:"XPlat Code Coverage"
  
  - name: Upload coverage to Codecov
    uses: codecov/codecov-action@v4
    with:
      files: '**/coverage.cobertura.xml'
      token: ${{ secrets.CODECOV_TOKEN }}
  ```

### 2. **Coveralls**
- **Link**: https://github.com/marketplace/coveralls
- **Free Tier**: Yes (public repos)
- **Purpose**: Alternative code coverage reporting
- **Benefits**:
  - Similar to Codecov
  - Good .NET support
  - Coverage tracking over time
- **Note**: Codecov recommended as primary choice

### 3. **SonarCloud** ⭐ RECOMMENDED
- **Link**: https://github.com/marketplace/sonarcloud
- **Free Tier**: Yes (public repos)
- **Purpose**: Code quality, coverage, and security analysis
- **Benefits**:
  - Code coverage visualization
  - Code quality metrics (bugs, code smells, technical debt)
  - Security vulnerability detection
  - Duplicate code detection
  - Pull request decoration with quality gate status
  - Supports C# and .NET
- **Installation**: Can be installed via GitHub Marketplace

---

## 🔄 CI/CD Enhancement Tools

### 4. **Dependabot** ⭐ RECOMMENDED (Built-in)
- **Link**: Built into GitHub (Settings → Security → Code security and analysis)
- **Free Tier**: Yes (all repos)
- **Purpose**: Automated dependency updates
- **Benefits**:
  - Automatic PRs for NuGet package updates
  - Security vulnerability alerts
  - Keep dependencies up-to-date
  - Reduces manual maintenance
- **Installation**: Enable in repository settings
- **Setup Required**:
  ```yaml
  # Create .github/dependabot.yml
  version: 2
  updates:
    - package-ecosystem: "nuget"
      directory: "/TerminplanerApi"
      schedule:
        interval: "weekly"
    - package-ecosystem: "nuget"
      directory: "/TerminplanerApi.Tests"
      schedule:
        interval: "weekly"
    - package-ecosystem: "nuget"
      directory: "/TerminplanerMaui"
      schedule:
        interval: "weekly"
    - package-ecosystem: "github-actions"
      directory: "/"
      schedule:
        interval: "weekly"
  ```

### 5. **GitHub Actions Importer**
- **Link**: https://github.com/marketplace/actions-importer
- **Free Tier**: Yes
- **Purpose**: Helps migrate from other CI/CD platforms
- **Benefits**: N/A (already using GitHub Actions)

### 6. **Release Drafter** ⭐ RECOMMENDED
- **Link**: https://github.com/marketplace/actions/release-drafter
- **Free Tier**: Yes (open source)
- **Purpose**: Automatically drafts release notes
- **Benefits**:
  - Auto-generates release notes from PRs
  - Categorizes changes (features, fixes, etc.)
  - Saves time on release documentation
- **Setup Required**:
  ```yaml
  # Create .github/workflows/release-drafter.yml
  name: Release Drafter
  on:
    push:
      branches: [main]
    pull_request:
      types: [opened, reopened, synchronize]
  
  jobs:
    update_release_draft:
      runs-on: ubuntu-latest
      steps:
        - uses: release-drafter/release-drafter@v6
          env:
            GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
  ```

### 7. **Semantic Release** ⭐ RECOMMENDED
- **Link**: https://github.com/semantic-release/semantic-release
- **Free Tier**: Yes (open source)
- **Purpose**: Automated versioning and changelog generation
- **Benefits**:
  - Automatic version bumping based on commit messages
  - Generates CHANGELOG.md
  - Creates GitHub releases
  - Follows semantic versioning
- **Note**: Requires conventional commit messages

---

## 🔒 Code Quality and Security

### 8. **CodeQL** ⭐ RECOMMENDED (Built-in)
- **Link**: Built into GitHub (Settings → Security → Code security and analysis)
- **Free Tier**: Yes (public repos)
- **Purpose**: Advanced security scanning
- **Benefits**:
  - Detects security vulnerabilities
  - Finds coding errors
  - Supports C#
  - Integration with GitHub Security tab
- **Installation**: Enable in repository settings
- **Setup Required**:
  ```yaml
  # Create .github/workflows/codeql.yml
  name: "CodeQL"
  on:
    push:
      branches: [main]
    pull_request:
      branches: [main]
    schedule:
      - cron: '0 0 * * 1'  # Weekly on Monday
  
  jobs:
    analyze:
      name: Analyze
      runs-on: ubuntu-latest
      permissions:
        actions: read
        contents: read
        security-events: write
      steps:
        - name: Checkout
          uses: actions/checkout@v4
        - name: Initialize CodeQL
          uses: github/codeql-action/init@v3
          with:
            languages: csharp
        - name: Autobuild
          uses: github/codeql-action/autobuild@v3
        - name: Perform CodeQL Analysis
          uses: github/codeql-action/analyze@v3
  ```

### 9. **Snyk** ⭐ RECOMMENDED
- **Link**: https://github.com/marketplace/snyk
- **Free Tier**: Yes (limited scans per month)
- **Purpose**: Dependency vulnerability scanning
- **Benefits**:
  - Scans NuGet packages for vulnerabilities
  - Automatic fix PRs
  - Container scanning
  - License compliance checking
- **Installation**: Can be installed via GitHub Marketplace

### 10. **LGTM (now part of GitHub Code Scanning)**
- **Note**: LGTM has been integrated into GitHub Code Scanning (CodeQL)
- Use CodeQL instead

### 11. **Better Code Hub**
- **Link**: https://github.com/marketplace/better-code-hub
- **Free Tier**: Limited
- **Purpose**: Code quality analysis
- **Note**: SonarCloud recommended as better alternative

---

## 🐳 Docker and Container Tools

### 12. **Docker Hub** ⭐ RECOMMENDED
- **Link**: https://hub.docker.com/
- **Free Tier**: Yes (1 private repo, unlimited public repos)
- **Purpose**: Container registry
- **Benefits**:
  - Host Docker images
  - Integrate with GitHub Actions
  - Automated builds
- **Setup Required**:
  ```dockerfile
  # Create Dockerfile for API
  FROM mcr.microsoft.com/dotnet/aspnet:9.0
  WORKDIR /app
  COPY TerminplanerApi/publish/ .
  EXPOSE 5215
  ENTRYPOINT ["dotnet", "TerminplanerApi.dll"]
  ```
  
  ```yaml
  # Add to workflow
  - name: Build and push Docker image
    uses: docker/build-push-action@v5
    with:
      context: .
      push: true
      tags: ${{ secrets.DOCKER_USERNAME }}/terminplaner-api:latest
  ```

### 13. **GitHub Container Registry (ghcr.io)** ⭐ RECOMMENDED
- **Link**: Built into GitHub
- **Free Tier**: Yes (500MB free, then pay-as-you-go)
- **Purpose**: GitHub's native container registry
- **Benefits**:
  - Integrated with GitHub
  - Good free tier
  - No external dependencies
  - Same authentication as GitHub
- **Setup Required**:
  ```yaml
  # Add to workflow
  - name: Log in to GitHub Container Registry
    uses: docker/login-action@v3
    with:
      registry: ghcr.io
      username: ${{ github.actor }}
      password: ${{ secrets.GITHUB_TOKEN }}
  
  - name: Build and push
    uses: docker/build-push-action@v5
    with:
      push: true
      tags: ghcr.io/${{ github.repository }}/api:latest
  ```

### 14. **Hadolint** ⭐ RECOMMENDED
- **Link**: https://github.com/marketplace/actions/hadolint-action
- **Free Tier**: Yes (open source)
- **Purpose**: Dockerfile linting
- **Benefits**:
  - Best practices for Dockerfiles
  - Security recommendations
  - Optimization suggestions
- **Setup**: Add to CI workflow once Dockerfiles are created

---

## ☁️ Azure and Cloud Integration

### 15. **Azure Login Action** ⭐ RECOMMENDED
- **Link**: https://github.com/marketplace/actions/azure-login
- **Free Tier**: Yes (action is free, Azure costs separate)
- **Purpose**: Authenticate with Azure in workflows
- **Benefits**:
  - Deploy to Azure App Service
  - Manage Azure resources
  - Integration with Azure DevOps
- **Setup Required**:
  ```yaml
  - uses: azure/login@v2
    with:
      creds: ${{ secrets.AZURE_CREDENTIALS }}
  
  - name: Deploy to Azure Web App
    uses: azure/webapps-deploy@v3
    with:
      app-name: terminplaner-api
      package: ./publish
  ```

### 16. **Azure Container Apps Deploy**
- **Link**: https://github.com/marketplace/actions/azure-container-apps-deploy
- **Free Tier**: Yes (action is free)
- **Purpose**: Deploy containers to Azure Container Apps
- **Benefits**:
  - Serverless container deployment
  - Auto-scaling
  - Cost-effective for small apps
- **Note**: Requires Azure subscription

### 17. **Terraform** ⭐ RECOMMENDED
- **Link**: https://github.com/marketplace/actions/hashicorp-setup-terraform
- **Free Tier**: Yes (action is free)
- **Purpose**: Infrastructure as Code
- **Benefits**:
  - Automated infrastructure provisioning
  - Works with Azure, AWS, GCP
  - Version-controlled infrastructure
  - Reusable modules
- **Setup**: Create Terraform configurations for cloud resources

---

## 📚 Documentation and Project Management

### 18. **GitHub Wiki**
- **Link**: Built into GitHub (Wiki tab)
- **Free Tier**: Yes
- **Purpose**: Documentation wiki
- **Benefits**: Additional documentation space
- **Note**: Current markdown docs in repo are sufficient

### 19. **GitHub Projects** ⭐ RECOMMENDED (Built-in)
- **Link**: Built into GitHub (Projects tab)
- **Free Tier**: Yes
- **Purpose**: Project management and issue tracking
- **Benefits**:
  - Kanban boards
  - Roadmap planning
  - Issue and PR tracking
  - Automation rules
- **Installation**: Create via Projects tab

### 20. **Pull Request Size Labeler**
- **Link**: https://github.com/marketplace/actions/pull-request-size-labeler
- **Free Tier**: Yes (open source)
- **Purpose**: Auto-label PRs by size
- **Benefits**:
  - Easier PR review management
  - Identifies large PRs that need splitting

### 21. **Auto-assign Issues and PRs**
- **Link**: https://github.com/marketplace/actions/auto-assign-action
- **Free Tier**: Yes (open source)
- **Purpose**: Automatically assign issues/PRs to team members
- **Benefits**: Better issue management

### 22. **Stale Bot** ⭐ RECOMMENDED
- **Link**: https://github.com/marketplace/stale
- **Free Tier**: Yes
- **Purpose**: Close stale issues and PRs
- **Benefits**:
  - Keeps issue tracker clean
  - Automatic notifications before closing
- **Setup Required**:
  ```yaml
  # Create .github/workflows/stale.yml
  name: Close stale issues and PRs
  on:
    schedule:
      - cron: '0 0 * * *'
  
  jobs:
    stale:
      runs-on: ubuntu-latest
      steps:
        - uses: actions/stale@v9
          with:
            repo-token: ${{ secrets.GITHUB_TOKEN }}
            stale-issue-message: 'This issue has been automatically marked as stale.'
            days-before-stale: 60
            days-before-close: 7
  ```

---

## 🎖️ Badges and Status

### 23. **Shields.io Badges**
- **Link**: https://shields.io/
- **Free Tier**: Yes
- **Purpose**: Status badges for README
- **Benefits**:
  - Build status badges
  - Coverage badges
  - Version badges
  - Custom badges
- **Setup**: Add to README.md
  ```markdown
  ![CI Status](https://github.com/mistalan/Terminplaner/workflows/CI%20-%20Build%20and%20Test/badge.svg)
  ![Coverage](https://codecov.io/gh/mistalan/Terminplaner/branch/main/graph/badge.svg)
  ![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4)
  ```

---

## 🚀 Implementation Guide

### Priority 1: Immediate Implementation (No External Apps Required)

These can be implemented immediately via configuration files:

1. **Dependabot** - Create `.github/dependabot.yml`
2. **CodeQL** - Create `.github/workflows/codeql.yml`
3. **Release Drafter** - Create `.github/workflows/release-drafter.yml`
4. **Stale Bot** - Create `.github/workflows/stale.yml`

### Priority 2: Requires Marketplace Installation

These require installing apps from GitHub Marketplace:

1. **Codecov** - Install from marketplace, add to CI workflow
2. **SonarCloud** - Install from marketplace, add to CI workflow
3. **Snyk** - Install from marketplace
4. **GitHub Projects** - Enable in repository

### Priority 3: Future Implementation

These require additional setup or infrastructure:

1. **Docker/Container Registry** - After creating Dockerfiles
2. **Azure Integration** - When cloud deployment is needed
3. **Terraform** - For infrastructure as code

---

## 📝 Recommended Configuration Files

The following configuration files should be created:

### 1. `.github/dependabot.yml`
Enables automated dependency updates for NuGet packages and GitHub Actions.

### 2. `.github/workflows/codeql.yml`
Enables security scanning for C# code.

### 3. `.github/workflows/release-drafter.yml`
Automates release note generation.

### 4. `.github/release-drafter.yml`
Configuration for release note categorization.

### 5. `.github/workflows/stale.yml`
Manages stale issues and PRs.

### 6. `Dockerfile` (API)
For containerizing the backend API.

### 7. `.dockerignore`
Excludes unnecessary files from Docker builds.

---

## 🎯 Summary of Recommendations

### ⭐ Top Recommended Apps (Free Tier):

1. **Codecov** - Test coverage reporting
2. **SonarCloud** - Code quality and security
3. **Dependabot** - Dependency updates (built-in)
4. **CodeQL** - Security scanning (built-in)
5. **Snyk** - Vulnerability scanning
6. **Release Drafter** - Release automation
7. **GitHub Container Registry** - Container hosting
8. **GitHub Projects** - Project management (built-in)
9. **Stale Bot** - Issue management

### 📊 Benefits Overview:

- **CI/CD**: Enhanced with automated releases and better dependency management
- **Test Coverage**: Visual coverage reports on every PR
- **Docker**: Container support for easier deployment
- **Azure/Cloud**: Integration ready for cloud deployments
- **Security**: Automated vulnerability scanning and code analysis
- **Code Quality**: Continuous monitoring of code health

### 💡 Next Steps:

1. Create configuration files for built-in tools (Dependabot, CodeQL)
2. Install Codecov and SonarCloud from GitHub Marketplace
3. Add Docker support with Dockerfile and container registry
4. Enable GitHub Projects for better project management
5. Set up badges in README for visibility

---

## 📧 Additional Resources

- [GitHub Marketplace](https://github.com/marketplace)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Codecov Documentation](https://docs.codecov.com/)
- [SonarCloud Documentation](https://docs.sonarcloud.io/)
- [Docker Documentation](https://docs.docker.com/)
- [Azure GitHub Actions](https://github.com/Azure/actions)

---

**Last Updated**: 2025-01-20
**Maintained By**: Terminplaner Development Team
