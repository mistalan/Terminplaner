# Test Cases Documentation - Terminplaner

## Overview
This document describes all test cases for the Terminplaner application. The tests follow a comprehensive testing strategy covering unit tests and integration tests.

## Test Strategy

### 1. Unit Tests (xUnit)
Unit tests focus on testing individual components in isolation, particularly the business logic in the AppointmentService class.

**Rationale:** xUnit is the modern, preferred testing framework for .NET with better performance and cleaner syntax compared to NUnit.

### 2. Integration Tests (xUnit + WebApplicationFactory)
Integration tests verify the complete API endpoints including HTTP request/response handling and middleware.

**Rationale:** WebApplicationFactory provides an in-memory test server for testing ASP.NET Core applications without network overhead.

### 3. Excluded: BDD/Gherkin Tests
**Decision:** Not implemented for this project due to:
- Small project scope with straightforward business logic
- No complex business rules requiring stakeholder collaboration
- Overhead of Gherkin syntax not justified for simple CRUD operations

### 4. Excluded: MAUI Frontend Tests
**Decision:** MAUI tests excluded due to:
- Requires MAUI workloads not available in standard CI environments
- Focus on API backend as the core business logic
- UI testing would require emulators/simulators

## Test Cases

### Unit Tests - AppointmentService

#### TC-U001: GetAll Returns Empty List When No Appointments Exist
**Given:** AppointmentService with no sample data  
**When:** GetAll() is called  
**Then:** An empty list is returned

#### TC-U002: GetAll Returns Appointments Ordered By Priority
**Given:** Multiple appointments with different priorities  
**When:** GetAll() is called  
**Then:** Appointments are returned in ascending priority order

#### TC-U003: GetById Returns Appointment When ID Exists
**Given:** An appointment with ID 1 exists  
**When:** GetById(1) is called  
**Then:** The correct appointment is returned

#### TC-U004: GetById Returns Null When ID Does Not Exist
**Given:** No appointment with ID 999 exists  
**When:** GetById(999) is called  
**Then:** Null is returned

#### TC-U005: Create Assigns Sequential ID
**Given:** A new appointment without an ID  
**When:** Create() is called  
**Then:** The appointment is assigned the next sequential ID

#### TC-U006: Create Sets CreatedAt Timestamp
**Given:** A new appointment  
**When:** Create() is called  
**Then:** CreatedAt is set to current date/time

#### TC-U007: Create Assigns Priority When Not Specified
**Given:** A new appointment with Priority = 0  
**When:** Create() is called  
**Then:** Priority is set to last position (max existing priority + 1)

#### TC-U008: Create Assigns Priority 1 When First Appointment
**Given:** No existing appointments and new appointment with Priority = 0  
**When:** Create() is called  
**Then:** Priority is set to 1

#### TC-U009: Create Preserves Specified Priority
**Given:** A new appointment with Priority = 5  
**When:** Create() is called  
**Then:** Priority remains 5

#### TC-U010: Create Adds Appointment to Collection
**Given:** A new appointment  
**When:** Create() is called  
**Then:** The appointment is added to the internal collection

#### TC-U011: Update Modifies Existing Appointment
**Given:** An existing appointment  
**When:** Update() is called with new values  
**Then:** The appointment properties are updated

#### TC-U012: Update Returns Updated Appointment
**Given:** An existing appointment  
**When:** Update() is called  
**Then:** The updated appointment is returned

#### TC-U013: Update Returns Null When ID Does Not Exist
**Given:** No appointment with ID 999  
**When:** Update(999, appointment) is called  
**Then:** Null is returned

#### TC-U014: Update Does Not Modify ID
**Given:** An existing appointment with ID 1  
**When:** Update() is called  
**Then:** The ID remains unchanged (1)

#### TC-U015: Update Modifies All Properties
**Given:** An existing appointment  
**When:** Update() is called with new Text, Category, Color, Priority  
**Then:** All properties are updated

#### TC-U016: Delete Removes Appointment
**Given:** An existing appointment  
**When:** Delete() is called  
**Then:** The appointment is removed from collection

#### TC-U017: Delete Returns True When Successful
**Given:** An existing appointment with ID 1  
**When:** Delete(1) is called  
**Then:** True is returned

#### TC-U018: Delete Returns False When ID Does Not Exist
**Given:** No appointment with ID 999  
**When:** Delete(999) is called  
**Then:** False is returned

#### TC-U019: UpdatePriorities Updates Multiple Appointments
**Given:** Multiple existing appointments  
**When:** UpdatePriorities() is called with a dictionary of ID->Priority mappings  
**Then:** All specified appointments have their priorities updated

#### TC-U020: UpdatePriorities Ignores Non-Existent IDs
**Given:** A dictionary containing non-existent ID 999  
**When:** UpdatePriorities() is called  
**Then:** No error occurs, only existing appointments are updated

#### TC-U021: Sample Data Is Initialized
**Given:** New AppointmentService instance  
**When:** Service is created  
**Then:** Three sample appointments exist with correct data

### Integration Tests - API Endpoints

#### TC-I001: GET /api/appointments Returns 200 OK
**Given:** API is running  
**When:** GET /api/appointments is called  
**Then:** HTTP 200 OK is returned

#### TC-I002: GET /api/appointments Returns JSON Array
**Given:** API is running  
**When:** GET /api/appointments is called  
**Then:** Response is a JSON array of appointments

#### TC-I003: GET /api/appointments Returns Ordered List
**Given:** Multiple appointments exist  
**When:** GET /api/appointments is called  
**Then:** Appointments are ordered by priority

#### TC-I004: GET /api/appointments/{id} Returns 200 OK When Exists
**Given:** Appointment with ID 1 exists  
**When:** GET /api/appointments/1 is called  
**Then:** HTTP 200 OK is returned with appointment data

#### TC-I005: GET /api/appointments/{id} Returns 404 When Not Exists
**Given:** Appointment with ID 999 does not exist  
**When:** GET /api/appointments/999 is called  
**Then:** HTTP 404 Not Found is returned

#### TC-I006: POST /api/appointments Creates New Appointment
**Given:** Valid appointment JSON data  
**When:** POST /api/appointments is called  
**Then:** A new appointment is created

#### TC-I007: POST /api/appointments Returns 201 Created
**Given:** Valid appointment JSON data  
**When:** POST /api/appointments is called  
**Then:** HTTP 201 Created is returned

#### TC-I008: POST /api/appointments Returns Location Header
**Given:** Valid appointment JSON data  
**When:** POST /api/appointments is called  
**Then:** Location header contains new appointment URL

#### TC-I009: POST /api/appointments Returns Created Appointment
**Given:** Valid appointment JSON data  
**When:** POST /api/appointments is called  
**Then:** Response body contains the created appointment with assigned ID

#### TC-I010: PUT /api/appointments/{id} Updates Appointment
**Given:** Existing appointment with ID 1  
**When:** PUT /api/appointments/1 is called with updated data  
**Then:** The appointment is updated

#### TC-I011: PUT /api/appointments/{id} Returns 200 OK
**Given:** Existing appointment with ID 1  
**When:** PUT /api/appointments/1 is called  
**Then:** HTTP 200 OK is returned

#### TC-I012: PUT /api/appointments/{id} Returns Updated Appointment
**Given:** Existing appointment  
**When:** PUT /api/appointments/{id} is called  
**Then:** Response contains the updated appointment data

#### TC-I013: PUT /api/appointments/{id} Returns 404 When Not Exists
**Given:** Appointment with ID 999 does not exist  
**When:** PUT /api/appointments/999 is called  
**Then:** HTTP 404 Not Found is returned

#### TC-I014: DELETE /api/appointments/{id} Removes Appointment
**Given:** Existing appointment with ID 1  
**When:** DELETE /api/appointments/1 is called  
**Then:** The appointment is removed

#### TC-I015: DELETE /api/appointments/{id} Returns 204 No Content
**Given:** Existing appointment with ID 1  
**When:** DELETE /api/appointments/1 is called  
**Then:** HTTP 204 No Content is returned

#### TC-I016: DELETE /api/appointments/{id} Returns 404 When Not Exists
**Given:** Appointment with ID 999 does not exist  
**When:** DELETE /api/appointments/999 is called  
**Then:** HTTP 404 Not Found is returned

#### TC-I017: PUT /api/appointments/priorities Updates Priorities
**Given:** Multiple appointments exist  
**When:** PUT /api/appointments/priorities is called with priority mappings  
**Then:** Appointment priorities are updated

#### TC-I018: PUT /api/appointments/priorities Returns 200 OK
**Given:** Valid priority mappings  
**When:** PUT /api/appointments/priorities is called  
**Then:** HTTP 200 OK is returned

#### TC-I019: CORS Headers Are Present
**Given:** API is running  
**When:** Any endpoint is called  
**Then:** CORS headers allow any origin

### Edge Cases and Error Handling

#### TC-E001: Create Handles Empty Text
**Given:** Appointment with empty text  
**When:** Create() is called  
**Then:** Appointment is created with empty text (no validation)

#### TC-E002: Create Handles Null Category
**Given:** Appointment with null category  
**When:** Create() is called  
**Then:** Default category "Standard" is used

#### TC-E003: Update Handles Partial Data
**Given:** Update with only some fields changed  
**When:** Update() is called  
**Then:** All provided fields are updated

#### TC-E004: Concurrent Operations Maintain Consistency
**Given:** Multiple simultaneous operations  
**When:** Create/Update/Delete are called concurrently  
**Then:** Data integrity is maintained (note: current implementation is not thread-safe)

## Test Execution

### Prerequisites
- .NET 9.0 SDK installed
- Test project added to solution

### Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with coverage (if configured)
dotnet test /p:CollectCoverage=true
```

## Coverage Goals

- **Unit Tests:** 100% coverage of AppointmentService methods
- **Integration Tests:** All API endpoints tested
- **Overall:** >90% code coverage for TerminplanerApi project

## Future Enhancements

1. **Performance Tests:** Test API response times under load
2. **Security Tests:** Test authentication/authorization when added
3. **MAUI UI Tests:** When CI environment supports MAUI workloads
4. **End-to-End Tests:** Full workflow tests with MAUI app + API
5. **Database Tests:** When persistent storage is implemented
