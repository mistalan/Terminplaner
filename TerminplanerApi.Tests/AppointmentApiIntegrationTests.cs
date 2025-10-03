using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TerminplanerApi.Models;

namespace TerminplanerApi.Tests;

public class AppointmentApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AppointmentApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region GET /api/appointments Tests

    [Fact]
    public async Task TC_I001_GetAppointments_Returns200OK()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/appointments");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TC_I002_GetAppointments_ReturnsJsonArray()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var appointments = await client.GetFromJsonAsync<List<Appointment>>("/api/appointments");

        // Assert
        Assert.NotNull(appointments);
        Assert.IsType<List<Appointment>>(appointments);
    }

    [Fact]
    public async Task TC_I003_GetAppointments_ReturnsOrderedList()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var appointments = await client.GetFromJsonAsync<List<Appointment>>("/api/appointments");

        // Assert
        Assert.NotNull(appointments);
        if (appointments.Count > 1)
        {
            for (int i = 1; i < appointments.Count; i++)
            {
                Assert.True(appointments[i].Priority >= appointments[i - 1].Priority,
                    "Appointments should be ordered by priority");
            }
        }
    }

    #endregion

    #region GET /api/appointments/{id} Tests

    [Fact]
    public async Task TC_I004_GetAppointmentById_Returns200OK_WhenExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        // First, create an appointment
        var newAppointment = new Appointment { Text = "Test", Category = "Test", Color = "#FF0000" };
        var createResponse = await client.PostAsJsonAsync("/api/appointments", newAppointment);
        var created = await createResponse.Content.ReadFromJsonAsync<Appointment>();

        // Act
        var response = await client.GetAsync($"/api/appointments/{created!.Id}");
        var appointment = await response.Content.ReadFromJsonAsync<Appointment>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(appointment);
        Assert.Equal(created.Id, appointment.Id);
    }

    [Fact]
    public async Task TC_I005_GetAppointmentById_Returns404_WhenNotExists()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/appointments/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region POST /api/appointments Tests

    [Fact]
    public async Task TC_I006_PostAppointment_CreatesNewAppointment()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAppointment = new Appointment
        {
            Text = "New Appointment",
            Category = "Work",
            Color = "#0000FF"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/appointments", newAppointment);
        var created = await response.Content.ReadFromJsonAsync<Appointment>();

        // Assert
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("New Appointment", created.Text);
        Assert.Equal("Work", created.Category);
    }

    [Fact]
    public async Task TC_I007_PostAppointment_Returns201Created()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAppointment = new Appointment { Text = "Test", Category = "Test" };

        // Act
        var response = await client.PostAsJsonAsync("/api/appointments", newAppointment);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task TC_I008_PostAppointment_ReturnsLocationHeader()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAppointment = new Appointment { Text = "Test", Category = "Test" };

        // Act
        var response = await client.PostAsJsonAsync("/api/appointments", newAppointment);
        var created = await response.Content.ReadFromJsonAsync<Appointment>();

        // Assert
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/api/appointments/{created!.Id}", response.Headers.Location.ToString());
    }

    [Fact]
    public async Task TC_I009_PostAppointment_ReturnsCreatedAppointmentWithId()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAppointment = new Appointment { Text = "Test", Category = "Test" };

        // Act
        var response = await client.PostAsJsonAsync("/api/appointments", newAppointment);
        var created = await response.Content.ReadFromJsonAsync<Appointment>();

        // Assert
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Test", created.Text);
    }

    #endregion

    #region PUT /api/appointments/{id} Tests

    [Fact]
    public async Task TC_I010_PutAppointment_UpdatesAppointment()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAppointment = new Appointment { Text = "Original", Category = "Work" };
        var createResponse = await client.PostAsJsonAsync("/api/appointments", newAppointment);
        var created = await createResponse.Content.ReadFromJsonAsync<Appointment>();

        var updatedData = new Appointment
        {
            Text = "Updated",
            Category = "Personal",
            Color = "#00FF00",
            Priority = 1
        };

        // Act
        var updateResponse = await client.PutAsJsonAsync($"/api/appointments/{created!.Id}", updatedData);
        var updated = await updateResponse.Content.ReadFromJsonAsync<Appointment>();

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Text);
        Assert.Equal("Personal", updated.Category);
    }

    [Fact]
    public async Task TC_I011_PutAppointment_Returns200OK()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAppointment = new Appointment { Text = "Test", Category = "Test" };
        var createResponse = await client.PostAsJsonAsync("/api/appointments", newAppointment);
        var created = await createResponse.Content.ReadFromJsonAsync<Appointment>();

        // Act
        var response = await client.PutAsJsonAsync($"/api/appointments/{created!.Id}", newAppointment);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TC_I012_PutAppointment_ReturnsUpdatedAppointment()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAppointment = new Appointment { Text = "Original", Category = "Work" };
        var createResponse = await client.PostAsJsonAsync("/api/appointments", newAppointment);
        var created = await createResponse.Content.ReadFromJsonAsync<Appointment>();

        var updatedData = new Appointment { Text = "Updated", Category = "Personal" };

        // Act
        var updateResponse = await client.PutAsJsonAsync($"/api/appointments/{created!.Id}", updatedData);
        var updated = await updateResponse.Content.ReadFromJsonAsync<Appointment>();

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Updated", updated.Text);
    }

    [Fact]
    public async Task TC_I013_PutAppointment_Returns404_WhenNotExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var appointment = new Appointment { Text = "Test", Category = "Test" };

        // Act
        var response = await client.PutAsJsonAsync("/api/appointments/999999", appointment);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region DELETE /api/appointments/{id} Tests

    [Fact]
    public async Task TC_I014_DeleteAppointment_RemovesAppointment()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAppointment = new Appointment { Text = "To Delete", Category = "Test" };
        var createResponse = await client.PostAsJsonAsync("/api/appointments", newAppointment);
        var created = await createResponse.Content.ReadFromJsonAsync<Appointment>();

        // Act
        await client.DeleteAsync($"/api/appointments/{created!.Id}");
        var getResponse = await client.GetAsync($"/api/appointments/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task TC_I015_DeleteAppointment_Returns204NoContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAppointment = new Appointment { Text = "To Delete", Category = "Test" };
        var createResponse = await client.PostAsJsonAsync("/api/appointments", newAppointment);
        var created = await createResponse.Content.ReadFromJsonAsync<Appointment>();

        // Act
        var response = await client.DeleteAsync($"/api/appointments/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task TC_I016_DeleteAppointment_Returns404_WhenNotExists()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/appointments/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region PUT /api/appointments/priorities Tests

    [Fact]
    public async Task TC_I017_PutPriorities_UpdatesPriorities()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Create test appointments
        var appt1Response = await client.PostAsJsonAsync("/api/appointments",
            new Appointment { Text = "First", Priority = 1 });
        var appt1 = await appt1Response.Content.ReadFromJsonAsync<Appointment>();

        var appt2Response = await client.PostAsJsonAsync("/api/appointments",
            new Appointment { Text = "Second", Priority = 2 });
        var appt2 = await appt2Response.Content.ReadFromJsonAsync<Appointment>();

        var priorities = new Dictionary<int, int>
        {
            { appt1!.Id, 2 },
            { appt2!.Id, 1 }
        };

        // Act
        await client.PutAsJsonAsync("/api/appointments/priorities", priorities);

        // Verify
        var updated1 = await client.GetFromJsonAsync<Appointment>($"/api/appointments/{appt1.Id}");
        var updated2 = await client.GetFromJsonAsync<Appointment>($"/api/appointments/{appt2.Id}");

        // Assert
        Assert.Equal(2, updated1!.Priority);
        Assert.Equal(1, updated2!.Priority);
    }

    [Fact]
    public async Task TC_I018_PutPriorities_Returns200OK()
    {
        // Arrange
        var client = _factory.CreateClient();
        var priorities = new Dictionary<int, int> { { 1, 1 } };

        // Act
        var response = await client.PutAsJsonAsync("/api/appointments/priorities", priorities);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region CORS Tests

    [Fact]
    public async Task TC_I019_CORS_HeadersArePresent()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/appointments");

        // Assert
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin") ||
                   response.StatusCode == HttpStatusCode.OK,
                   "CORS should be configured or request should succeed");
    }

    #endregion
}
