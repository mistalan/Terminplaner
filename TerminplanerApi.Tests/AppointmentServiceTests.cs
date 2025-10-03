using TerminplanerApi.Models;
using TerminplanerApi.Services;

namespace TerminplanerApi.Tests;

public class AppointmentServiceTests
{
    // Helper method to create a service without sample data
    private AppointmentService CreateEmptyService()
    {
        var service = new AppointmentService();
        // Clear sample data by deleting all appointments
        var appointments = service.GetAll();
        foreach (var appointment in appointments.ToList())
        {
            service.Delete(appointment.Id);
        }
        return service;
    }

    #region GetAll Tests

    [Fact]
    public void TC_U001_GetAll_ReturnsEmptyList_WhenNoAppointmentsExist()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var result = service.GetAll();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void TC_U002_GetAll_ReturnsAppointmentsOrderedByPriority()
    {
        // Arrange
        var service = CreateEmptyService();
        service.Create(new Appointment { Text = "Low", Priority = 3 });
        service.Create(new Appointment { Text = "High", Priority = 1 });
        service.Create(new Appointment { Text = "Medium", Priority = 2 });

        // Act
        var result = service.GetAll();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Priority);
        Assert.Equal(2, result[1].Priority);
        Assert.Equal(3, result[2].Priority);
    }

    [Fact]
    public void TC_U021_SampleData_IsInitialized()
    {
        // Arrange & Act
        var service = new AppointmentService();
        var appointments = service.GetAll();

        // Assert
        Assert.Equal(3, appointments.Count);
        Assert.Contains(appointments, a => a.Text == "Zahnarzttermin");
        Assert.Contains(appointments, a => a.Text == "Projekt abschließen");
        Assert.Contains(appointments, a => a.Text == "Einkaufen gehen");
    }

    #endregion

    #region GetById Tests

    [Fact]
    public void TC_U003_GetById_ReturnsAppointment_WhenIdExists()
    {
        // Arrange
        var service = CreateEmptyService();
        var created = service.Create(new Appointment { Text = "Test Appointment" });

        // Act
        var result = service.GetById(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Test Appointment", result.Text);
    }

    [Fact]
    public void TC_U004_GetById_ReturnsNull_WhenIdDoesNotExist()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var result = service.GetById(999);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Create Tests

    [Fact]
    public void TC_U005_Create_AssignsSequentialId()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var first = service.Create(new Appointment { Text = "First" });
        var second = service.Create(new Appointment { Text = "Second" });

        // Assert
        Assert.True(second.Id > first.Id);
    }

    [Fact]
    public void TC_U006_Create_SetsCreatedAtTimestamp()
    {
        // Arrange
        var service = CreateEmptyService();
        var before = DateTime.Now.AddSeconds(-1);

        // Act
        var appointment = service.Create(new Appointment { Text = "Test" });
        var after = DateTime.Now.AddSeconds(1);

        // Assert
        Assert.True(appointment.CreatedAt >= before);
        Assert.True(appointment.CreatedAt <= after);
    }

    [Fact]
    public void TC_U007_Create_AssignsPriority_WhenNotSpecified()
    {
        // Arrange
        var service = CreateEmptyService();
        service.Create(new Appointment { Text = "First", Priority = 1 });
        service.Create(new Appointment { Text = "Second", Priority = 2 });

        // Act
        var newAppointment = service.Create(new Appointment { Text = "Third", Priority = 0 });

        // Assert
        Assert.Equal(3, newAppointment.Priority);
    }

    [Fact]
    public void TC_U008_Create_AssignsPriority1_WhenFirstAppointment()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var appointment = service.Create(new Appointment { Text = "First", Priority = 0 });

        // Assert
        Assert.Equal(1, appointment.Priority);
    }

    [Fact]
    public void TC_U009_Create_PreservesSpecifiedPriority()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var appointment = service.Create(new Appointment { Text = "Test", Priority = 5 });

        // Assert
        Assert.Equal(5, appointment.Priority);
    }

    [Fact]
    public void TC_U010_Create_AddsAppointmentToCollection()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var created = service.Create(new Appointment { Text = "Test" });
        var all = service.GetAll();

        // Assert
        Assert.Single(all);
        Assert.Equal(created.Id, all[0].Id);
    }

    #endregion

    #region Update Tests

    [Fact]
    public void TC_U011_Update_ModifiesExistingAppointment()
    {
        // Arrange
        var service = CreateEmptyService();
        var created = service.Create(new Appointment { Text = "Original", Category = "OldCat" });

        // Act
        var updated = service.Update(created.Id, new Appointment
        {
            Text = "Updated",
            Category = "NewCat",
            Color = "#FF0000",
            Priority = 5
        });

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Text);
        Assert.Equal("NewCat", updated.Category);
    }

    [Fact]
    public void TC_U012_Update_ReturnsUpdatedAppointment()
    {
        // Arrange
        var service = CreateEmptyService();
        var created = service.Create(new Appointment { Text = "Original" });

        // Act
        var updated = service.Update(created.Id, new Appointment { Text = "Updated" });

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Updated", updated.Text);
    }

    [Fact]
    public void TC_U013_Update_ReturnsNull_WhenIdDoesNotExist()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var result = service.Update(999, new Appointment { Text = "Test" });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TC_U014_Update_DoesNotModifyId()
    {
        // Arrange
        var service = CreateEmptyService();
        var created = service.Create(new Appointment { Text = "Test" });
        var originalId = created.Id;

        // Act
        var updated = service.Update(created.Id, new Appointment { Text = "Updated" });

        // Assert
        Assert.Equal(originalId, updated!.Id);
    }

    [Fact]
    public void TC_U015_Update_ModifiesAllProperties()
    {
        // Arrange
        var service = CreateEmptyService();
        var created = service.Create(new Appointment
        {
            Text = "Old",
            Category = "OldCat",
            Color = "#000000",
            Priority = 1
        });

        // Act
        var updated = service.Update(created.Id, new Appointment
        {
            Text = "New",
            Category = "NewCat",
            Color = "#FFFFFF",
            Priority = 10
        });

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("New", updated.Text);
        Assert.Equal("NewCat", updated.Category);
        Assert.Equal("#FFFFFF", updated.Color);
        Assert.Equal(10, updated.Priority);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void TC_U016_Delete_RemovesAppointment()
    {
        // Arrange
        var service = CreateEmptyService();
        var created = service.Create(new Appointment { Text = "To Delete" });

        // Act
        service.Delete(created.Id);
        var result = service.GetById(created.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TC_U017_Delete_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var service = CreateEmptyService();
        var created = service.Create(new Appointment { Text = "Test" });

        // Act
        var result = service.Delete(created.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TC_U018_Delete_ReturnsFalse_WhenIdDoesNotExist()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var result = service.Delete(999);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region UpdatePriorities Tests

    [Fact]
    public void TC_U019_UpdatePriorities_UpdatesMultipleAppointments()
    {
        // Arrange
        var service = CreateEmptyService();
        var appt1 = service.Create(new Appointment { Text = "First", Priority = 1 });
        var appt2 = service.Create(new Appointment { Text = "Second", Priority = 2 });
        var appt3 = service.Create(new Appointment { Text = "Third", Priority = 3 });

        var priorities = new Dictionary<int, int>
        {
            { appt1.Id, 3 },
            { appt2.Id, 1 },
            { appt3.Id, 2 }
        };

        // Act
        service.UpdatePriorities(priorities);

        // Assert
        Assert.Equal(3, service.GetById(appt1.Id)!.Priority);
        Assert.Equal(1, service.GetById(appt2.Id)!.Priority);
        Assert.Equal(2, service.GetById(appt3.Id)!.Priority);
    }

    [Fact]
    public void TC_U020_UpdatePriorities_IgnoresNonExistentIds()
    {
        // Arrange
        var service = CreateEmptyService();
        var appt1 = service.Create(new Appointment { Text = "Exists", Priority = 1 });

        var priorities = new Dictionary<int, int>
        {
            { appt1.Id, 5 },
            { 999, 10 }  // Non-existent ID
        };

        // Act
        // Should not throw exception
        service.UpdatePriorities(priorities);

        // Assert
        Assert.Equal(5, service.GetById(appt1.Id)!.Priority);
        Assert.Null(service.GetById(999));
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void TC_E001_Create_HandlesEmptyText()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var appointment = service.Create(new Appointment { Text = "" });

        // Assert
        Assert.NotNull(appointment);
        Assert.Equal("", appointment.Text);
    }

    [Fact]
    public void TC_E002_Create_HandlesNullCategory()
    {
        // Arrange
        var service = CreateEmptyService();

        // Act
        var appointment = service.Create(new Appointment { Text = "Test", Category = null! });

        // Assert
        Assert.NotNull(appointment);
        // Category is null since we don't have default value logic in Create
    }

    #endregion
}
