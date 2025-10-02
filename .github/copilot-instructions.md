# Copilot Instructions for Terminplaner

## Project Overview

**Terminplaner** is a custom appointment/schedule planner application being developed as a personal project. The repository name translates from German as "appointment planner" or "scheduler".

### Repository Information
- **Type**: .NET Web Application (ASP.NET Core Web API)
- **Primary Language**: C#
- **Target Framework**: .NET 9.0 or later
- **Project Structure**: ASP.NET Core Web API with minimal API pattern
- **Size**: Small personal project (currently in early development)
- **Repository State**: The main branch contains only README.md and .gitignore. Active development occurs in feature branches.

### Technology Stack
- **Runtime**: .NET SDK 9.0.x or later
- **Framework**: ASP.NET Core (Web API)
- **Build Tool**: dotnet CLI
- **Version Control**: Git
- **CI/CD**: GitHub Actions (Copilot workflow available)

## Build and Development Commands

### Environment Setup

**Required Tools:**
- .NET SDK 9.0 or later
- Git

**Available .NET SDKs in CI environment:**
- .NET 8.0.120, 8.0.206, 8.0.317, 8.0.414
- .NET 9.0.110, 9.0.205, 9.0.305 (default)
- .NET 10.0.x is NOT currently available in the CI environment

**Verify .NET installation:**
```bash
dotnet --version
dotnet --list-sdks
```

### Creating a New Project

If no project exists yet, create a new ASP.NET Core Web API project:

```bash
# Create a new Web API project
dotnet new webapi -n TerminplanerApi --framework net9.0

# Or create with a solution file
dotnet new sln -n Terminplaner
dotnet new webapi -n TerminplanerApi --framework net9.0
dotnet sln add TerminplanerApi/TerminplanerApi.csproj
```

### Build Commands

**IMPORTANT**: Always run commands from the repository root or the project directory containing the .csproj file.

**Clean build artifacts:**
```bash
dotnet clean
```
- Removes bin/ and obj/ directories
- Run this if you encounter build issues
- Takes ~2-3 seconds

**Restore dependencies:**
```bash
dotnet restore
```
- Downloads NuGet packages
- Usually automatic when building, but run explicitly if package issues occur
- Takes ~3-5 seconds for small projects

**Build the project:**
```bash
dotnet build
```
- Compiles the project
- Automatically runs restore if needed
- Takes ~10-15 seconds for initial build, ~3-5 seconds for incremental
- Output: bin/Debug/net9.0/[ProjectName].dll

**Build with specific configuration:**
```bash
dotnet build --configuration Release
```

### Testing Commands

**Run tests:**
```bash
dotnet test
```
- Runs all test projects in the solution
- If no test project exists, this command will complete quickly with no tests found
- Takes ~4-5 seconds when no tests exist

**Run tests with detailed output:**
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Code Formatting and Linting

**Format code (verify only):**
```bash
dotnet format --verify-no-changes
```
- Checks if code follows formatting rules
- **IMPORTANT**: This command may report whitespace formatting errors
- Exit code 0 indicates success (no formatting needed)

**Format code (apply changes):**
```bash
dotnet format
```
- Automatically fixes formatting issues
- Run this before committing if `--verify-no-changes` fails

### Running the Application

**Run in development mode:**
```bash
dotnet run
```
- Starts the web server
- Default URLs: http://localhost:5216 (HTTPS) or http://localhost:5215 (HTTP)
- Press Ctrl+C to stop

**Run with specific environment:**
```bash
dotnet run --environment Production
```

**Run with hot reload (watch mode):**
```bash
dotnet watch run
```
- Automatically rebuilds and restarts on file changes
- Useful for development

## Project Layout and Architecture

### Repository Root Files
```
/
├── .gitignore          # Visual Studio/C# gitignore patterns (comprehensive)
├── README.md           # Basic project description
├── .github/
│   └── copilot-instructions.md  # This file
└── [ProjectName]/      # Main project directory (e.g., TerminplanerApi/)
```

### Typical ASP.NET Core Project Structure
```
TerminplanerApi/
├── Program.cs          # Application entry point and configuration
├── appsettings.json    # Application configuration (production)
├── appsettings.Development.json  # Development-specific configuration
├── [ProjectName].csproj  # Project file (dependencies, target framework)
├── [ProjectName].http  # HTTP request examples (optional)
├── Properties/
│   └── launchSettings.json  # Development launch profiles
├── Controllers/        # API controllers (if using controller pattern)
├── Models/             # Data models
├── Services/           # Business logic
├── wwwroot/            # Static files
├── bin/                # Build output (ignored by git)
└── obj/                # Intermediate build files (ignored by git)
```

### Key Configuration Files

**Project File (*.csproj)**
- Defines target framework (e.g., net9.0)
- Lists NuGet package dependencies
- Located in project directory

**appsettings.json**
- Application configuration
- Connection strings, logging settings, etc.
- **IMPORTANT**: Never commit secrets; use appsettings.Development.json or environment variables for sensitive data

**launchSettings.json**
- Development server settings
- URLs, environment variables, profiles
- Located in Properties/ directory

## CI/CD and Validation

### GitHub Workflows

**Available Workflow:**
- Copilot SWE Agent workflow (dynamic/copilot-swe-agent/copilot)
- Automatically triggered by Copilot actions

**No traditional CI/CD workflows exist yet** on the main branch. If adding CI/CD:
- Create `.github/workflows/` directory
- Common workflows: build.yml, test.yml, deploy.yml

### Pre-commit Validation Steps

**Recommended validation sequence before committing:**

1. **Format code:**
   ```bash
   dotnet format
   ```

2. **Clean and build:**
   ```bash
   dotnet clean && dotnet build
   ```

3. **Run tests (if they exist):**
   ```bash
   dotnet test
   ```

4. **Verify formatting:**
   ```bash
   dotnet format --verify-no-changes
   ```

All commands should complete successfully with exit code 0.

## Common Issues and Troubleshooting

### Build Issues

**Issue: "Project not found" or "No project specified"**
- **Solution**: Ensure you're in the correct directory containing the .csproj file
- Or specify the project path: `dotnet build path/to/Project.csproj`

**Issue: NuGet package restore failures**
- **Solution**: Clear NuGet cache and restore:
  ```bash
  dotnet nuget locals all --clear
  dotnet restore
  ```

**Issue: Build errors after pulling changes**
- **Solution**: Clean and rebuild:
  ```bash
  dotnet clean
  dotnet build
  ```

### Formatting Issues

**Issue: `dotnet format --verify-no-changes` fails**
- This is expected if code doesn't match formatting rules
- **Solution**: Run `dotnet format` to fix automatically
- Common issues: whitespace, indentation, line endings

### Runtime Issues

**Issue: Port already in use when running `dotnet run`**
- **Solution**: Kill the process using the port or change port in launchSettings.json

**Issue: HTTPS certificate trust warnings**
- **Solution**: Trust the development certificate:
  ```bash
  dotnet dev-certs https --trust
  ```

## Important Notes for Coding Agents

### Always Do:
1. **Run `dotnet build` before and after making code changes** to verify compilation
2. **Run `dotnet format` before committing** to ensure code style consistency
3. **Use `dotnet clean` if encountering mysterious build errors**
4. **Check project directory structure** - commands must run from correct location
5. **Read the .csproj file** to understand dependencies and target framework
6. **Verify all paths are absolute** when working with files

### Never Do:
1. **Never commit bin/ or obj/ directories** - they're in .gitignore for a reason
2. **Never commit secrets** in appsettings.json or code
3. **Never modify .csproj manually** without understanding XML structure - use `dotnet add package` instead
4. **Never assume test projects exist** - verify first with `dotnet test`
5. **Never use `git reset --hard` or `git rebase`** - force push is not available

### Project-Specific Patterns

**API Pattern:**
- This project uses minimal API pattern (app.MapGet, app.MapPost, etc.) in Program.cs
- No separate controller classes needed for simple endpoints
- Middleware configuration in Program.cs

**Dependency Injection:**
- Services registered in Program.cs with builder.Services.Add*
- Use constructor injection in classes that need services

**Configuration:**
- Access via IConfiguration interface
- Bound to appsettings.json automatically

## Command Reference Summary

| Command | Purpose | Time | Exit Code |
|---------|---------|------|-----------|
| `dotnet --version` | Check .NET version | <1s | 0 |
| `dotnet restore` | Restore packages | 3-5s | 0 |
| `dotnet clean` | Remove build artifacts | 2-3s | 0 |
| `dotnet build` | Compile project | 10-15s | 0 if success |
| `dotnet test` | Run tests | 4-5s | 0 if all pass |
| `dotnet format` | Format code | 2-3s | 0 |
| `dotnet format --verify-no-changes` | Check formatting | 2-3s | 0 if formatted |
| `dotnet run` | Start application | N/A | N/A (runs until stopped) |
| `dotnet watch run` | Start with hot reload | N/A | N/A (runs until stopped) |

## Trust These Instructions

These instructions have been validated by:
- Creating and building a test project
- Running all commands successfully
- Verifying timing and exit codes
- Testing in the actual CI environment

**If you encounter issues not covered here, search the repository first. Only if the information is incomplete or incorrect should you perform additional exploration.**
