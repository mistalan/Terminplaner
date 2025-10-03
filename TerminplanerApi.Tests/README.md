# TerminplanerApi.Tests

This project contains comprehensive unit and integration tests for the Terminplaner API.

## Test Framework

- **xUnit**: Modern, flexible testing framework for .NET
- **Microsoft.AspNetCore.Mvc.Testing**: WebApplicationFactory for integration testing

## Test Coverage

### Unit Tests (23 tests)
Tests for `AppointmentService` business logic:
- **GetAll**: Empty list handling, ordering by priority
- **GetById**: Finding existing/non-existing appointments
- **Create**: ID assignment, timestamp creation, priority management
- **Update**: Modifying appointments, handling non-existent IDs
- **Delete**: Removing appointments, return values
- **UpdatePriorities**: Batch priority updates
- **Edge Cases**: Empty text, null category handling

### Integration Tests (19 tests)
End-to-end API endpoint tests:
- **GET /api/appointments**: List retrieval, ordering
- **GET /api/appointments/{id}**: Single appointment retrieval, 404 handling
- **POST /api/appointments**: Creation, 201 status, location header
- **PUT /api/appointments/{id}**: Updates, 404 handling
- **DELETE /api/appointments/{id}**: Deletion, 204 status, 404 handling
- **PUT /api/appointments/priorities**: Batch priority updates
- **CORS**: Cross-origin configuration

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run with detailed output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run specific test
```bash
dotnet test --filter "TC_U001"
```

### Run only unit tests
```bash
dotnet test --filter "FullyQualifiedName~AppointmentServiceTests"
```

### Run only integration tests
```bash
dotnet test --filter "FullyQualifiedName~AppointmentApiIntegrationTests"
```

## Test Results

**Total Tests**: 42  
**Passed**: 42  
**Failed**: 0  
**Success Rate**: 100%

## Test Naming Convention

Tests follow the pattern `TC_<Type><Number>_<Description>`:
- `TC_U###`: Unit test
- `TC_I###`: Integration test
- `TC_E###`: Edge case test

Examples:
- `TC_U001_GetAll_ReturnsEmptyList_WhenNoAppointmentsExist`
- `TC_I007_PostAppointment_Returns201Created`

## Test Documentation

For detailed test case documentation, see [TEST_CASES.md](../TEST_CASES.md) in the repository root.

## Dependencies

- **xunit**: ^2.4.2
- **xunit.runner.visualstudio**: ^2.4.5
- **coverlet.collector**: ^6.0.0
- **Microsoft.NET.Test.Sdk**: ^17.8.0
- **Microsoft.AspNetCore.Mvc.Testing**: ^9.0.9

## CI/CD Integration

Tests run automatically on:
- Pull requests
- Commits to main branch
- Manual workflow triggers

## Future Enhancements

1. Code coverage reporting
2. Performance benchmarking tests
3. Mutation testing
4. Load/stress tests
5. MAUI frontend tests (when CI supports MAUI workloads)
