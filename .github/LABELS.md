# GitHub Labels Documentation

This document describes the label system used in the Terminplaner repository.

## Overview

Labels are automatically synchronized using the **Label Sync** workflow, which runs when `.github/labels.yml` is modified or can be triggered manually.

## Label Categories

### Dependency Management
Used by Dependabot for automated dependency updates:

- **dependencies** - Pull requests that update a dependency file
- **backend** - Backend API related changes  
- **frontend** - Frontend MAUI app related changes
- **tests** - Test-related changes
- **github-actions** - GitHub Actions workflow updates

### Feature Development
- **feature** - New feature or request
- **enhancement** - Improvement to existing functionality
- **feat** - New feature (conventional commits)

### Bug Fixes
- **fix** - Bug fix
- **bugfix** - Bug fix
- **bug** - Something isn't working

### Documentation
- **documentation** - Improvements or additions to documentation
- **docs** - Documentation changes

### Maintenance
- **chore** - Maintenance tasks
- **refactor** - Code refactoring

### Performance
- **performance** - Performance improvements
- **perf** - Performance related

### Security
- **security** - Security-related changes

### Version Control
Used by Release Drafter for semantic versioning:

- **major** - Major version bump (breaking changes)
- **breaking** - Breaking change
- **minor** - Minor version bump (new features)
- **patch** - Patch version bump (bug fixes)

### Issue Management
- **pinned** - Issue or PR that should not be marked as stale
- **help-wanted** - Extra attention is needed
- **work-in-progress** - Work in progress, do not merge
- **good-first-issue** - Good for newcomers
- **duplicate** - This issue or pull request already exists
- **invalid** - This doesn't seem right
- **wontfix** - This will not be worked on
- **question** - Further information is requested

## Adding New Labels

1. Edit `.github/labels.yml`
2. Add your label in YAML format:
   ```yaml
   - name: my-label
     description: Description of the label
     color: 'ff0000'  # Hex color without #
   ```
3. Commit and push to main branch
4. The Label Sync workflow will automatically create the label

## Manual Label Sync

You can manually trigger the label sync workflow from the GitHub Actions tab:
1. Go to Actions → Sync Labels
2. Click "Run workflow"
3. Select the branch (main)
4. Click "Run workflow"

## Color Codes

Common colors used in this repository:
- `0366d6` - Blue (dependencies)
- `fbca04` - Yellow (backend, work-in-progress)
- `0e8a16` - Green (frontend, minor)
- `d73a4a` - Red (bugs, tests, fix)
- `000000` - Black (github-actions)
- `a2eeef` - Light blue (features, enhancements)
- `0075ca` - Dark blue (documentation)
- `fef2c0` - Light yellow (chore, refactor)
- `c5def5` - Very light blue (performance, patch)
- `ee0701` - Bright red (security)
- `b60205` - Dark red (major, breaking)
- `ffd700` - Gold (pinned)
- `008672` - Teal (help-wanted)
- `7057ff` - Purple (good-first-issue)
- `cfd3d7` - Gray (duplicate)
- `e4e669` - Yellow-green (invalid)
- `ffffff` - White (wontfix)
- `d876e3` - Pink (question)

## References

- [GitHub Labels Documentation](https://docs.github.com/en/issues/using-labels-and-milestones-to-track-work/managing-labels)
- [EndBug/label-sync Action](https://github.com/EndBug/label-sync)
- [Dependabot Configuration](https://docs.github.com/en/code-security/dependabot/dependabot-version-updates/configuration-options-for-the-dependabot.yml-file)
