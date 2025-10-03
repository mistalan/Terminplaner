# TerminplanerApi.Tests

This project contains comprehensive unit and integration tests for the Terminplaner API.

## Quick Start

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

## Test Results

**Total Tests**: 42  
**Passed**: 42 (100%)  
**Coverage**: Unit tests (23) + Integration tests (19)

## Documentation

For detailed test case documentation and test strategy, see **[TEST_CASES.md](TEST_CASES.md)** in this folder.

## Test Framework

- **xUnit** 2.4.2 - Modern testing framework
- **Microsoft.AspNetCore.Mvc.Testing** 9.0.9 - Integration testing
- **coverlet.collector** 6.0.0 - Code coverage
