using TerminplanerApi.Models;
using TerminplanerApi.Repositories;

namespace TerminplanerApi.Tests;

public class AppointmentRepositoryTests
{
    // Helper method to create a repository without sample data
    private async Task<IAppointmentRepository> CreateEmptyRepositoryAsync()
    {
        var repository = new InMemoryAppointmentRepository();
        // Clear sample data by deleting all appointments
        var appointments = await repository.GetAllAsync();
        foreach (var appointment in appointments.ToList())
        {
            await repository.DeleteAsync(appointment.Id);
        }
        return repository;
    }

    #region GetAll Tests

    [Fact]
    public async Task TC_U001_GetAll_ReturnsEmptyList_WhenNoAppointmentsExist()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task TC_U002_GetAll_ReturnsAppointmentsOrderedByPriority()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        await repository.CreateAsync(new Appointment { Text = "Low", Priority = 3 });
        await repository.CreateAsync(new Appointment { Text = "High", Priority = 1 });
        await repository.CreateAsync(new Appointment { Text = "Medium", Priority = 2 });

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Priority);
        Assert.Equal(2, result[1].Priority);
        Assert.Equal(3, result[2].Priority);
    }

    [Fact]
    public async Task TC_U021_SampleData_IsInitialized()
    {
        // Arrange & Act
        var repository = new InMemoryAppointmentRepository();
        var appointments = await repository.GetAllAsync();

        // Assert
        Assert.Equal(3, appointments.Count);
        Assert.Contains(appointments, a => a.Text == "Zahnarzttermin");
        Assert.Contains(appointments, a => a.Text == "Projekt abschließen");
        Assert.Contains(appointments, a => a.Text == "Lebensmittel einkaufen");
    }

    [Fact]
    public async Task TC_U022_SampleData_HasCorrectProperties()
    {
        // Arrange & Act
        var repository = new InMemoryAppointmentRepository();
        var appointments = await repository.GetAllAsync();

        // Assert - verify sample data properties are correctly set
        var zahnarzt = appointments.First(a => a.Text == "Zahnarzttermin");
        Assert.Equal("Gesundheit", zahnarzt.Category);
        Assert.Equal("#FF0000", zahnarzt.Color);
        Assert.Equal(1, zahnarzt.Priority);
        Assert.NotNull(zahnarzt.ScheduledDate);
        Assert.Equal("1 Std", zahnarzt.Duration);

        var projekt = appointments.First(a => a.Text == "Projekt abschließen");
        Assert.Equal("Arbeit", projekt.Category);
        Assert.Equal("#0000FF", projekt.Color);
        Assert.Equal(2, projekt.Priority);
        Assert.NotNull(projekt.ScheduledDate);
        Assert.Equal("2-3 Std", projekt.Duration);

        var einkaufen = appointments.First(a => a.Text == "Lebensmittel einkaufen");
        Assert.Equal("Privat", einkaufen.Category);
        Assert.Equal("#00FF00", einkaufen.Color);
        Assert.Equal(3, einkaufen.Priority);
        Assert.NotNull(einkaufen.ScheduledDate);
        Assert.Equal("30 min", einkaufen.Duration);
        Assert.True(einkaufen.IsOutOfHome);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task TC_U003_GetById_ReturnsAppointment_WhenIdExists()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var created = await repository.CreateAsync(new Appointment { Text = "Test Appointment" });

        // Act
        var result = await repository.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Test Appointment", result.Text);
    }

    [Fact]
    public async Task TC_U004_GetById_ReturnsNull_WhenIdDoesNotExist()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var result = await repository.GetByIdAsync("999");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task TC_U005_Create_AssignsSequentialId()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var first = await repository.CreateAsync(new Appointment { Text = "First" });
        var second = await repository.CreateAsync(new Appointment { Text = "Second" });

        // Assert
        Assert.True(int.Parse(second.Id) > int.Parse(first.Id));
    }

    [Fact]
    public async Task TC_U006_Create_SetsCreatedAtTimestamp()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var before = DateTime.Now.AddSeconds(-1);

        // Act
        var appointment = await repository.CreateAsync(new Appointment { Text = "Test" });
        var after = DateTime.Now.AddSeconds(1);

        // Assert
        Assert.True(appointment.CreatedAt >= before);
        Assert.True(appointment.CreatedAt <= after);
    }

    [Fact]
    public async Task TC_U006b_Create_SetsCreatedAtWhenMinValue()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var before = DateTime.Now.AddSeconds(-1);

        // Act
        var appointment = await repository.CreateAsync(new Appointment 
        { 
            Text = "Test",
            CreatedAt = DateTime.MinValue 
        });
        var after = DateTime.Now.AddSeconds(1);

        // Assert
        Assert.True(appointment.CreatedAt >= before);
        Assert.True(appointment.CreatedAt <= after);
        Assert.NotEqual(DateTime.MinValue, appointment.CreatedAt);
    }

    [Fact]
    public async Task TC_U006c_Create_PreservesProvidedCreatedAt()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var specificTime = DateTime.Now.AddDays(-5);

        // Act
        var appointment = await repository.CreateAsync(new Appointment 
        { 
            Text = "Test",
            CreatedAt = specificTime
        });

        // Assert
        Assert.Equal(specificTime, appointment.CreatedAt);
    }

    [Fact]
    public async Task TC_U007_Create_AssignsPriority_WhenNotSpecified()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        await repository.CreateAsync(new Appointment { Text = "First", Priority = 1 });
        await repository.CreateAsync(new Appointment { Text = "Second", Priority = 2 });

        // Act
        var newAppointment = await repository.CreateAsync(new Appointment { Text = "Third", Priority = 0 });

        // Assert
        Assert.Equal(3, newAppointment.Priority);
    }

    [Fact]
    public async Task TC_U023_Create_AssignsPriority1_WhenEmptyRepository()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var newAppointment = await repository.CreateAsync(new Appointment { Text = "First", Priority = 0 });

        // Assert
        Assert.Equal(1, newAppointment.Priority);
    }

    [Fact]
    public async Task TC_U008_Create_AssignsPriority1_WhenFirstAppointment()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var appointment = await repository.CreateAsync(new Appointment { Text = "First", Priority = 0 });

        // Assert
        Assert.Equal(1, appointment.Priority);
    }

    [Fact]
    public async Task TC_U009_Create_PreservesSpecifiedPriority()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var appointment = await repository.CreateAsync(new Appointment { Text = "Test", Priority = 5 });

        // Assert
        Assert.Equal(5, appointment.Priority);
    }

    [Fact]
    public async Task TC_U010_Create_AddsAppointmentToCollection()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var created = await repository.CreateAsync(new Appointment { Text = "Test" });
        var all = await repository.GetAllAsync();

        // Assert
        Assert.Single(all);
        Assert.Equal(created.Id, all[0].Id);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task TC_U011_Update_ModifiesExistingAppointment()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var created = await repository.CreateAsync(new Appointment { Text = "Original", Category = "OldCat" });

        // Act
        var updated = await repository.UpdateAsync(created.Id, new Appointment
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
    public async Task TC_U012_Update_ReturnsUpdatedAppointment()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var created = await repository.CreateAsync(new Appointment { Text = "Original" });

        // Act
        var updated = await repository.UpdateAsync(created.Id, new Appointment { Text = "Updated" });

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Updated", updated.Text);
    }

    [Fact]
    public async Task TC_U013_Update_ReturnsNull_WhenIdDoesNotExist()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var result = await repository.UpdateAsync("999", new Appointment { Text = "Test" });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TC_U014_Update_DoesNotModifyId()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var created = await repository.CreateAsync(new Appointment { Text = "Test" });
        var originalId = created.Id;

        // Act
        var updated = await repository.UpdateAsync(created.Id, new Appointment { Text = "Updated" });

        // Assert
        Assert.Equal(originalId, updated!.Id);
    }

    [Fact]
    public async Task TC_U015_Update_ModifiesAllProperties()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var created = await repository.CreateAsync(new Appointment
        {
            Text = "Old",
            Category = "OldCat",
            Color = "#000000",
            Priority = 1
        });

        // Act
        var updated = await repository.UpdateAsync(created.Id, new Appointment
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
    public async Task TC_U016_Delete_RemovesAppointment()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var created = await repository.CreateAsync(new Appointment { Text = "To Delete" });

        // Act
        await repository.DeleteAsync(created.Id);
        var result = await repository.GetByIdAsync(created.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TC_U017_Delete_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var created = await repository.CreateAsync(new Appointment { Text = "Test" });

        // Act
        var result = await repository.DeleteAsync(created.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TC_U018_Delete_ReturnsFalse_WhenIdDoesNotExist()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var result = await repository.DeleteAsync("999");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region UpdatePriorities Tests

    [Fact]
    public async Task TC_U019_UpdatePriorities_UpdatesMultipleAppointments()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var appt1 = await repository.CreateAsync(new Appointment { Text = "First", Priority = 1 });
        var appt2 = await repository.CreateAsync(new Appointment { Text = "Second", Priority = 2 });
        var appt3 = await repository.CreateAsync(new Appointment { Text = "Third", Priority = 3 });

        var priorities = new Dictionary<string, int>
        {
            { appt1.Id, 3 },
            { appt2.Id, 1 },
            { appt3.Id, 2 }
        };

        // Act
        await repository.UpdatePrioritiesAsync(priorities);

        // Assert
        Assert.Equal(3, (await repository.GetByIdAsync(appt1.Id))!.Priority);
        Assert.Equal(1, (await repository.GetByIdAsync(appt2.Id))!.Priority);
        Assert.Equal(2, (await repository.GetByIdAsync(appt3.Id))!.Priority);
    }

    [Fact]
    public async Task TC_U020_UpdatePriorities_IgnoresNonExistentIds()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();
        var appt1 = await repository.CreateAsync(new Appointment { Text = "Exists", Priority = 1 });

        var priorities = new Dictionary<string, int>
        {
            { appt1.Id, 5 },
            { "999", 10 }  // Non-existent ID
        };

        // Act
        // Should not throw exception
        await repository.UpdatePrioritiesAsync(priorities);

        // Assert
        Assert.Equal(5, (await repository.GetByIdAsync(appt1.Id))!.Priority);
        Assert.Null(await repository.GetByIdAsync("999"));
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public async Task TC_E001_Create_HandlesEmptyText()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var appointment = await repository.CreateAsync(new Appointment { Text = "" });

        // Assert
        Assert.NotNull(appointment);
        Assert.Equal("", appointment.Text);
    }

    [Fact]
    public async Task TC_E002_Create_HandlesNullCategory()
    {
        // Arrange
        var repository = await CreateEmptyRepositoryAsync();

        // Act
        var appointment = await repository.CreateAsync(new Appointment { Text = "Test", Category = null! });

        // Assert
        Assert.NotNull(appointment);
        // Category is null since we don't have default value logic in Create
    }

    #endregion
}
