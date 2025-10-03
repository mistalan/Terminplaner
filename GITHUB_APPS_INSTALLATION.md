# 📋 GitHub Apps Installation Guide

This document lists GitHub Apps that should be manually installed from the GitHub Marketplace for the Terminplaner repository.

## ✅ Already Configured (No Installation Needed)

The following tools are already configured via workflow files:

- ✅ **Dependabot** - Enabled via `.github/dependabot.yml`
- ✅ **CodeQL** - Enabled via `.github/workflows/codeql.yml`
- ✅ **Release Drafter** - Enabled via `.github/workflows/release-drafter.yml`
- ✅ **Stale Bot** - Enabled via `.github/workflows/stale.yml`
- ✅ **Docker/GHCR** - Enabled via `.github/workflows/docker.yml`

## 🔧 Recommended Apps for Manual Installation

These apps require installation from GitHub Marketplace and provide significant value:

### Priority 1: Essential Tools

#### 1. **Codecov**
- **URL**: https://github.com/marketplace/codecov
- **Purpose**: Code coverage reporting
- **Installation Steps**:
  1. Visit https://github.com/marketplace/codecov
  2. Click "Set up a plan"
  3. Select "Free for open source" plan
  4. Install on the Terminplaner repository
  5. Codecov will automatically detect the coverage workflow
- **Configuration**: Already configured in `.github/workflows/coverage.yml`
- **Benefit**: Visual coverage reports on every PR, coverage badges for README

#### 2. **SonarCloud**
- **URL**: https://github.com/marketplace/sonarcloud
- **Purpose**: Code quality and security analysis
- **Installation Steps**:
  1. Visit https://github.com/marketplace/sonarcloud
  2. Click "Set up a plan"
  3. Select "Free for open source" plan
  4. Install on the Terminplaner repository
  5. Complete SonarCloud setup wizard
  6. Add workflow integration (see below)
- **Additional Configuration Required**:
  ```yaml
  # Add to .github/workflows/ci.yml or create new workflow
  - name: SonarCloud Scan
    uses: SonarSource/sonarcloud-github-action@master
    env:
      GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
  ```
- **Benefit**: Detailed code quality metrics, security vulnerabilities, code smells

#### 3. **Snyk**
- **URL**: https://github.com/marketplace/snyk
- **Purpose**: Dependency vulnerability scanning
- **Installation Steps**:
  1. Visit https://github.com/marketplace/snyk
  2. Click "Set up a plan"
  3. Select "Free for open source" plan
  4. Install on the Terminplaner repository
  5. Snyk will automatically scan dependencies
- **Benefit**: Automatic PRs to fix vulnerabilities, dependency health monitoring

### Priority 2: Nice to Have

#### 4. **GitHub Projects**
- **Built-in Feature** - No marketplace installation needed
- **Enable**: Go to repository → Projects tab → Create new project
- **Purpose**: Kanban boards for issue tracking
- **Benefit**: Better project management and roadmap planning

#### 5. **Pull Request Size Labeler**
- **URL**: https://github.com/marketplace/actions/pull-request-size-labeler
- **Purpose**: Auto-label PRs by size
- **Installation**: Add workflow file (see configuration below)
- **Configuration**:
  ```yaml
  # Create .github/workflows/pr-size-labeler.yml
  name: PR Size Labeler
  on: pull_request
  jobs:
    label:
      runs-on: ubuntu-latest
      steps:
        - uses: codelytv/pr-size-labeler@v1
          with:
            GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
            xs_max_size: 10
            s_max_size: 100
            m_max_size: 500
            l_max_size: 1000
  ```

### Priority 3: Future Enhancements

#### 6. **Hadolint** (Already in workflows)
- **Purpose**: Dockerfile linting
- **Status**: Will run automatically once Docker workflows are active
- **No action required**

#### 7. **Trivy** (Already in workflows)
- **Purpose**: Container security scanning
- **Status**: Already integrated in `.github/workflows/docker.yml`
- **No action required**

## 🚀 Quick Installation Checklist

Use this checklist when installing apps:

- [ ] Install **Codecov** from marketplace
- [ ] Install **SonarCloud** from marketplace
  - [ ] Add SONAR_TOKEN to repository secrets
  - [ ] Create SonarCloud workflow
- [ ] Install **Snyk** from marketplace
- [ ] Enable **GitHub Projects** (Projects tab)
- [ ] Add **PR Size Labeler** workflow (optional)

## 🔑 Required Secrets

After installing apps, you may need to add these secrets to the repository (Settings → Secrets and variables → Actions):

| Secret Name | Source | Required For |
|-------------|--------|--------------|
| `CODECOV_TOKEN` | Codecov dashboard | Code coverage uploads |
| `SONAR_TOKEN` | SonarCloud dashboard | SonarCloud scans |
| `DOCKER_USERNAME` | Docker Hub (optional) | Docker Hub pushes (if using Docker Hub instead of GHCR) |

## 📊 Expected Results After Installation

Once all apps are installed, you'll see:

1. **PR Checks**: Every PR will show status checks for:
   - CI Build and Tests ✓
   - CodeQL Security Scan ✓
   - Code Coverage (Codecov) ✓
   - Code Quality (SonarCloud) ✓
   - Dependency Scan (Snyk) ✓
   - Docker Build ✓

2. **README Badges**: Status badges will display build status, coverage %, and more

3. **Automated PRs**: Dependabot and Snyk will create PRs for:
   - NuGet package updates
   - GitHub Actions updates
   - Security vulnerability fixes

4. **Release Notes**: Automatic draft releases with categorized changes

5. **Issue Management**: Stale issues/PRs auto-labeled and closed

## 🆘 Troubleshooting

**App not showing in PR checks:**
- Verify app is installed on the repository (not just organization)
- Check workflow file syntax
- Review app permissions in Settings → Integrations

**Secrets not working:**
- Ensure secret name matches exactly (case-sensitive)
- Re-add secret if copied from clipboard (hidden characters)
- Check app documentation for correct secret format

**Coverage not uploading:**
- Verify CODECOV_TOKEN is set
- Check coverage files are generated (`.cobertura.xml`)
- Review Codecov workflow logs

## 📚 Additional Resources

- [GitHub Marketplace](https://github.com/marketplace)
- [Managing GitHub Apps](https://docs.github.com/en/developers/apps/managing-github-apps)
- [Codecov Documentation](https://docs.codecov.com/docs)
- [SonarCloud Documentation](https://docs.sonarcloud.io/)
- [Snyk Documentation](https://docs.snyk.io/)

---

**Note**: All the apps listed are free for public repositories. Review pricing if converting to a private repository.
