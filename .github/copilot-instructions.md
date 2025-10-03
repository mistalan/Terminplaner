# Copilot Instructions for Terminplaner

## Project Overview

**Terminplaner** is a custom appointment/schedule planner application being developed as a personal project. The repository name translates from German as "appointment planner" or "scheduler". It consists of a backend API and a cross-platform mobile/desktop frontend.

### Repository Information
- **Type**: Multi-project solution with ASP.NET Core Web API backend and .NET MAUI frontend
- **Primary Language**: C#
- **Target Framework**: .NET 9.0
- **Project Structure**: 
  - **TerminplanerApi**: ASP.NET Core Web API with minimal API pattern (backend)
  - **TerminplanerApi.Tests**: xUnit test project with unit and integration tests
  - **TerminplanerMaui**: .NET MAUI cross-platform app (frontend)
- **Size**: Small personal project
- **Repository State**: Active development with both backend and frontend projects
- **Test Coverage**: 42 tests (23 unit tests, 19 integration tests) with 100% pass rate

### Technology Stack
- **Runtime**: .NET SDK 9.0.x or later
- **Backend Framework**: ASP.NET Core 9.0 (Web API)
- **Frontend Framework**: .NET MAUI (Multi-platform App UI) with XAML
- **Pattern**: MVVM (Model-View-ViewModel) with CommunityToolkit.Mvvm
- **Data Storage**: In-Memory (List) in backend
- **API**: RESTful API with JSON
- **Build Tool**: dotnet CLI
- **Version Control**: Git
- **CI/CD**: GitHub Actions (Copilot workflow available)
- **Target Platforms**: Android, iOS, Windows, macOS

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

The project already exists with three main components:
- **TerminplanerApi**: Backend Web API
- **TerminplanerApi.Tests**: Test project with unit and integration tests
- **TerminplanerMaui**: Frontend MAUI app

If you need to add additional projects to the solution:

```bash
# Add a new project to the existing solution
dotnet new <template> -n <ProjectName> --framework net9.0
dotnet sln add <ProjectName>/<ProjectName>.csproj
```

### Build Commands

**IMPORTANT**: 
- For the **API project**, run commands from `/TerminplanerApi` directory or specify the project path
- For the **MAUI project**, run commands from `/TerminplanerMaui` directory or specify the project path
- For **full solution** builds, run from the repository root

**Clean build artifacts:**
```bash
# Clean API project
cd TerminplanerApi
dotnet clean

# Clean MAUI project  
cd TerminplanerMaui
dotnet clean

# Clean entire solution
dotnet clean
```
- Removes bin/ and obj/ directories
- Run this if you encounter build issues
- Takes ~2-3 seconds per project

**Restore dependencies:**
```bash
# Restore API project
cd TerminplanerApi
dotnet restore

# Restore MAUI project (requires MAUI workload)
cd TerminplanerMaui
dotnet restore

# Restore entire solution
dotnet restore
```
- Downloads NuGet packages
- MAUI restore requires MAUI workloads to be installed
- Takes ~3-5 seconds for API, longer for MAUI

**Build the API project:**
```bash
cd TerminplanerApi
dotnet build
```
- Compiles the API project
- Takes ~10-15 seconds for initial build, ~3-5 seconds for incremental
- Output: bin/Debug/net9.0/TerminplanerApi.dll

**Build the MAUI project:**
```bash
cd TerminplanerMaui

# For Android
dotnet build -t:Build -f net9.0-android

# For Windows (Windows only)
dotnet build -t:Build -f net9.0-windows10.0.19041.0

# For iOS (macOS only)
dotnet build -t:Build -f net9.0-ios

# For macOS (macOS only)
dotnet build -t:Build -f net9.0-maccatalyst
```
- Compiles for the specified platform
- Requires platform-specific SDKs and workloads
- Takes longer than API builds (30-60 seconds)

**Build with specific configuration:**
```bash
dotnet build --configuration Release
```

### Testing Commands

**IMPORTANT**: Tests MUST be executed after each build. A pull request is only considered 'Done' if all tests succeed.

**Run tests:**
```bash
dotnet test
```
- Runs all test projects in the solution
- **42 tests** total (23 unit tests, 19 integration tests)
- Takes ~3-5 seconds to run all tests
- **Exit code 0** indicates all tests passed
- **IMPORTANT**: All tests must pass before committing code changes

**Run tests with detailed output:**
```bash
dotnet test --logger "console;verbosity=detailed"
```
- Provides detailed test execution information
- Useful for debugging test failures

**Run specific test:**
```bash
dotnet test --filter "TC_U001"
```
- Runs a single test by name
- Test naming convention: TC_<Type><Number>_<Description>
  - TC_U###: Unit test
  - TC_I###: Integration test
  - TC_E###: Edge case test

**Run only unit tests:**
```bash
dotnet test --filter "FullyQualifiedName~AppointmentServiceTests"
```
- Runs all 23 unit tests for AppointmentService

**Run only integration tests:**
```bash
dotnet test --filter "FullyQualifiedName~AppointmentApiIntegrationTests"
```
- Runs all 19 integration tests for API endpoints

**Test project location:**
- Tests are in `TerminplanerApi.Tests/` directory
- Test documentation: See [TEST_CASES.md](../TEST_CASES.md) and [TerminplanerApi.Tests/README.md](../TerminplanerApi.Tests/README.md)

**Test coverage:**
- **Unit Tests (23)**: AppointmentService business logic
  - GetAll, GetById, Create, Update, Delete, UpdatePriorities
  - Edge cases: empty text, null category
- **Integration Tests (19)**: API endpoints
  - GET /api/appointments, GET /api/appointments/{id}
  - POST /api/appointments, PUT /api/appointments/{id}
  - DELETE /api/appointments/{id}, PUT /api/appointments/priorities
  - CORS configuration

### Code Formatting and Linting

**Format code (verify only):**
```bash
# Format API project
cd TerminplanerApi
dotnet format --verify-no-changes

# Format MAUI project
cd TerminplanerMaui
dotnet format --verify-no-changes

# Format entire solution
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

**Run the Backend API in development mode:**
```bash
cd TerminplanerApi
dotnet run
```
- Starts the web server
- Default URL: **http://localhost:5215**
- Press Ctrl+C to stop
- **IMPORTANT**: The API must be running for the MAUI app to function

**Run the MAUI App:**
```bash
cd TerminplanerMaui

# For Android (requires Android emulator or device)
dotnet build -t:Run -f net9.0-android

# For Windows (Windows only)
dotnet build -t:Run -f net9.0-windows10.0.19041.0

# For iOS (macOS only)
dotnet build -t:Run -f net9.0-ios

# For macOS (macOS only)
dotnet build -t:Run -f net9.0-maccatalyst
```
- Builds and runs the MAUI app on the specified platform
- Requires platform-specific SDKs and emulators/devices
- The backend API must be running first

**Run with specific environment:**
```bash
dotnet run --environment Production
```

**Run API with hot reload (watch mode):**
```bash
cd TerminplanerApi
dotnet watch run
```
- Automatically rebuilds and restarts on file changes
- Useful for API development
- Not applicable for MAUI apps

## Project Layout and Architecture

### Repository Root Files
```
/
├── .gitignore          # Visual Studio/C# gitignore patterns (comprehensive)
├── README.md           # Project documentation
├── QUICKSTART.md       # Quick start guide
├── TEST_CASES.md       # Detailed test case documentation
├── Terminplaner.sln    # Solution file containing all projects
├── .github/
│   └── copilot-instructions.md  # This file
├── TerminplanerApi/    # Backend Web API project
├── TerminplanerApi.Tests/  # Test project (unit & integration tests)
└── TerminplanerMaui/   # Frontend MAUI app project
```

### Backend API Project Structure (TerminplanerApi)
```
TerminplanerApi/
├── Program.cs          # API entry point and minimal API endpoints
├── appsettings.json    # Application configuration (production)
├── appsettings.Development.json  # Development-specific configuration
├── TerminplanerApi.csproj  # Backend project file
├── TerminplanerApi.http    # HTTP request examples
├── Properties/
│   └── launchSettings.json  # Development launch profiles
├── Models/
│   └── Appointment.cs  # Data model for appointments
├── Services/
│   └── AppointmentService.cs  # Business logic & in-memory storage
├── bin/                # Build output (ignored by git)
└── obj/                # Intermediate build files (ignored by git)
```

### Test Project Structure (TerminplanerApi.Tests)
```
TerminplanerApi.Tests/
├── AppointmentServiceTests.cs  # Unit tests (23 tests)
├── AppointmentApiIntegrationTests.cs  # Integration tests (19 tests)
├── README.md           # Test documentation
├── TerminplanerApi.Tests.csproj  # Test project file
├── bin/                # Build output (ignored by git)
└── obj/                # Intermediate build files (ignored by git)
```

**Test Framework:**
- xUnit 2.4.2 - Modern testing framework for .NET
- Microsoft.AspNetCore.Mvc.Testing 9.0.9 - WebApplicationFactory for integration testing
- Microsoft.NET.Test.Sdk 17.8.0 - Test SDK
- coverlet.collector 6.0.0 - Code coverage collector

**Test Naming Convention:**
- Pattern: `TC_<Type><Number>_<Description>`
- TC_U###: Unit test (e.g., TC_U001_GetAll_ReturnsEmptyList_WhenNoAppointmentsExist)
- TC_I###: Integration test (e.g., TC_I007_PostAppointment_Returns201Created)
- TC_E###: Edge case test (e.g., TC_E001_Create_HandlesEmptyText)

### Frontend MAUI Project Structure (TerminplanerMaui)
```
TerminplanerMaui/
├── MauiProgram.cs      # MAUI app initialization and DI setup
├── App.xaml            # App-level XAML definition
├── App.xaml.cs         # App-level code-behind
├── AppShell.xaml       # Shell/Navigation definition
├── AppShell.xaml.cs    # Shell code-behind
├── TerminplanerMaui.csproj  # MAUI project file (multi-targeted)
├── Models/
│   └── Appointment.cs  # Client-side data model
├── Services/
│   └── AppointmentApiService.cs  # API client service
├── ViewModels/
│   ├── MainViewModel.cs  # Main page ViewModel (MVVM)
│   └── EditAppointmentViewModel.cs  # Edit page ViewModel
├── Pages/
│   ├── MainPage.xaml   # Main UI page
│   ├── MainPage.xaml.cs  # Main page code-behind
│   ├── EditAppointmentPage.xaml  # Edit UI page
│   └── EditAppointmentPage.xaml.cs  # Edit page code-behind
├── Platforms/
│   ├── Android/        # Android-specific code and resources
│   ├── iOS/            # iOS-specific code and resources
│   ├── Windows/        # Windows-specific code and resources
│   └── MacCatalyst/    # macOS-specific code and resources
├── Resources/
│   ├── Images/         # App icons and images
│   ├── Fonts/          # Custom fonts
│   ├── Styles/         # XAML styles
│   └── Raw/            # Other resources
├── bin/                # Build output (ignored by git)
└── obj/                # Intermediate build files (ignored by git)
```

### Key Configuration Files

**Backend API Project Files:**

**TerminplanerApi.csproj**
- Defines target framework (net9.0)
- Lists NuGet package dependencies
- Uses Microsoft.NET.Sdk.Web SDK
- Contains `public partial class Program { }` for integration testing

**TerminplanerApi.Tests.csproj**
- Test project targeting net9.0
- References TerminplanerApi project
- Uses xUnit as testing framework
- Includes Microsoft.AspNetCore.Mvc.Testing for integration tests

**appsettings.json**
- Application configuration
- API settings, logging configuration
- **IMPORTANT**: Never commit secrets; use appsettings.Development.json or environment variables for sensitive data

**launchSettings.json**
- Development server settings
- Default URL: http://localhost:5215
- Located in Properties/ directory

**Frontend MAUI Project Files:**

**TerminplanerMaui.csproj**
- Multi-targeted for multiple platforms: net9.0-android, net9.0-ios, net9.0-maccatalyst, net9.0-windows10.0.19041.0
- Uses Microsoft.NET.Sdk SDK with UseMaui=true
- References MAUI NuGet packages (Microsoft.Maui.Controls, CommunityToolkit.Mvvm)
- Platform-specific settings (SupportedOSPlatformVersion, etc.)

**MauiProgram.cs**
- MAUI app initialization
- Dependency injection container setup
- Service, ViewModel, and Page registration

**App.xaml / App.xaml.cs**
- Application-level configuration
- App lifecycle events

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

**CRITICAL**: All tests MUST pass before code can be committed. A pull request is only considered 'Done' if all tests succeed.

1. **Format code:**
   ```bash
   # Format API
   cd TerminplanerApi
   dotnet format
   
   # Format MAUI
   cd ../TerminplanerMaui
   dotnet format
   
   # Format Tests
   cd ../TerminplanerApi.Tests
   dotnet format
   ```

2. **Clean and build API:**
   ```bash
   cd TerminplanerApi
   dotnet clean && dotnet build
   ```

3. **Build test project:**
   ```bash
   cd ../TerminplanerApi.Tests
   dotnet build
   ```

4. **Run tests (MANDATORY):**
   ```bash
   cd ..
   dotnet test
   ```
   - **CRITICAL**: All 42 tests MUST pass (exit code 0)
   - If any test fails, fix the issue before committing
   - Test failures indicate broken functionality

5. **Build MAUI (optional, requires workloads):**
   ```bash
   cd TerminplanerMaui
   dotnet build -f net9.0-android
   ```

6. **Verify formatting:**
   ```bash
   dotnet format --verify-no-changes
   ```

All commands should complete successfully with exit code 0. **Tests are mandatory and must pass.**

## Common Issues and Troubleshooting

### Build Issues

**Issue: "Project not found" or "No project specified"**
- **Solution**: Ensure you're in the correct project directory (TerminplanerApi or TerminplanerMaui)
- Or specify the project path: `dotnet build path/to/Project.csproj`

**Issue: MAUI workloads not installed**
- **Error**: `error NETSDK1147: To build this project, the following workloads must be installed: maui-android`
- **Solution**: Install required MAUI workloads:
  ```bash
  dotnet workload restore
  # Or install specific workloads
  dotnet workload install maui-android maui-ios maui-windows maui-maccatalyst
  ```
- **Note**: In CI environments, MAUI workloads may not be available. Focus on building the API project.

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

**Issue: MAUI app can't connect to API**
- **Solution**: Ensure the API is running on http://localhost:5215
- Check network settings on Android emulator (use 10.0.2.2:5215 instead of localhost:5215)

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

**Issue: Android emulator not found**
- **Solution**: Start Android emulator before running the MAUI app
- Or connect a physical Android device via USB with USB debugging enabled

## Important Notes for Coding Agents

### Always Do:
1. **Run `dotnet build` before and after making code changes** to verify compilation
2. **Run `dotnet test` after every code change** - all 42 tests MUST pass
3. **Build API and MAUI projects separately** - API can be built without MAUI workloads
4. **Run `dotnet format` before committing** to ensure code style consistency
5. **Use `dotnet clean` if encountering mysterious build errors**
6. **Check project directory structure** - commands must run from correct location (TerminplanerApi, TerminplanerApi.Tests, or TerminplanerMaui)
7. **Read the .csproj files** to understand dependencies and target frameworks
8. **Verify all paths are absolute** when working with files
9. **Ensure API is running** before starting the MAUI app for testing
10. **Verify test coverage** - if you add new functionality, add corresponding tests

### Never Do:
1. **Never commit bin/ or obj/ directories** - they're in .gitignore for a reason
2. **Never commit secrets** in appsettings.json or code
3. **Never modify .csproj manually** without understanding XML structure - use `dotnet add package` instead
4. **Never commit code without running tests** - all 42 tests must pass
5. **Never skip test execution** - tests are mandatory for every PR
6. **Never use `git reset --hard` or `git rebase`** - force push is not available
7. **Never assume MAUI workloads are available in CI** - focus on API builds in CI environments
8. **Never break existing tests** - if a test fails after your change, fix your code or update the test appropriately

### Project-Specific Patterns

**Backend API Pattern:**
- This project uses minimal API pattern (app.MapGet, app.MapPost, etc.) in Program.cs
- No separate controller classes needed for simple endpoints
- Middleware configuration in Program.cs
- Dependency injection: Services registered with builder.Services.Add*
- Configuration: Access via IConfiguration interface, bound to appsettings.json

**Frontend MAUI Pattern:**
- Uses MVVM pattern with CommunityToolkit.Mvvm
- ViewModels inherit from ObservableObject and use [ObservableProperty] attributes
- Commands use [RelayCommand] attributes
- Pages use XAML with data binding to ViewModels
- Navigation via AppShell
- Dependency injection: Services, ViewModels, and Pages registered in MauiProgram.cs
- API communication via AppointmentApiService (HttpClient)
- Platform-specific code in Platforms/ folders

## Command Reference Summary

### Backend API Commands

| Command | Purpose | Time | Exit Code |
|---------|---------|------|-----------|
| `dotnet --version` | Check .NET version | <1s | 0 |
| `cd TerminplanerApi && dotnet restore` | Restore API packages | 3-5s | 0 |
| `cd TerminplanerApi && dotnet clean` | Remove API build artifacts | 2-3s | 0 |
| `cd TerminplanerApi && dotnet build` | Compile API project | 10-15s | 0 if success |
| `cd TerminplanerApi && dotnet run` | Start API server | N/A | N/A (runs until stopped) |
| `cd TerminplanerApi && dotnet watch run` | Start API with hot reload | N/A | N/A (runs until stopped) |
| `cd TerminplanerApi && dotnet format` | Format API code | 2-3s | 0 |

### Frontend MAUI Commands

| Command | Purpose | Time | Exit Code |
|---------|---------|------|-----------|
| `cd TerminplanerMaui && dotnet restore` | Restore MAUI packages | 5-10s | 0 (requires workloads) |
| `cd TerminplanerMaui && dotnet clean` | Remove MAUI build artifacts | 2-3s | 0 |
| `cd TerminplanerMaui && dotnet build -f net9.0-android` | Build for Android | 30-60s | 0 if success |
| `cd TerminplanerMaui && dotnet build -t:Run -f net9.0-android` | Build and run on Android | N/A | N/A (launches app) |
| `cd TerminplanerMaui && dotnet build -f net9.0-windows10.0.19041.0` | Build for Windows | 30-60s | 0 if success |
| `cd TerminplanerMaui && dotnet format` | Format MAUI code | 2-3s | 0 |

### Solution-level Commands

| Command | Purpose | Time | Exit Code |
|---------|---------|------|-----------|
| `dotnet test` | Run all tests (MANDATORY) | 3-5s | 0 if all 42 tests pass |
| `dotnet test --logger "console;verbosity=detailed"` | Run tests with details | 3-5s | 0 if all tests pass |
| `dotnet test --filter "TC_U001"` | Run specific test | <1s | 0 if test passes |
| `dotnet test --filter "FullyQualifiedName~AppointmentServiceTests"` | Run unit tests only | 2-3s | 0 if all unit tests pass |
| `dotnet test --filter "FullyQualifiedName~AppointmentApiIntegrationTests"` | Run integration tests only | 2-3s | 0 if all integration tests pass |
| `dotnet format --verify-no-changes` | Check formatting | 2-3s | 0 if formatted |
| `dotnet format` | Format all code | 2-3s | 0 |

## Trust These Instructions

These instructions have been validated by:
- Examining the actual project structure with API, Tests, and MAUI projects
- Building and running the API project successfully
- Running all 42 tests successfully (100% pass rate)
- Reviewing the README.md, QUICKSTART.md, and TEST_CASES.md documentation
- Verifying project files (TerminplanerApi.csproj, TerminplanerApi.Tests.csproj, and TerminplanerMaui.csproj)
- Confirming available .NET SDKs in the CI environment

**Architecture Notes:**
- The repository contains THREE main projects: TerminplanerApi (backend), TerminplanerApi.Tests (tests), and TerminplanerMaui (frontend)
- The API uses minimal API pattern (no controllers)
- The test project uses xUnit with WebApplicationFactory for integration testing
- **42 comprehensive tests** cover AppointmentService business logic and API endpoints
- The MAUI app uses MVVM pattern with CommunityToolkit.Mvvm
- The MAUI app communicates with the API via REST calls
- Target platforms: Android, iOS, Windows, macOS

**Testing Requirements:**
- **All 42 tests MUST pass** before code can be committed
- Tests are executed with `dotnet test` command
- 23 unit tests cover AppointmentService business logic
- 19 integration tests cover API endpoints end-to-end
- A pull request is only considered 'Done' if all tests succeed
- Test documentation available in TEST_CASES.md and TerminplanerApi.Tests/README.md

**CI/CD Considerations:**
- MAUI workloads may not be available in all CI environments
- Focus on building and testing the API project in CI
- MAUI builds require platform-specific SDKs and emulators
- Tests can run in CI without MAUI workloads (they only test the API)

**If you encounter issues not covered here, check README.md and QUICKSTART.md first. Only if the information is incomplete or incorrect should you perform additional exploration.**
