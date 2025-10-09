using TerminplanerApi.Models;
using TerminplanerApi.Repositories;

namespace TerminplanerApi.Tests;

public class SqliteAppointmentRepositoryTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly SqliteAppointmentRepository _repository;

    public SqliteAppointmentRepositoryTests()
    {
        // Create a unique test database for each test run
        _testDbPath = $"test_appointments_{Guid.NewGuid()}.db";
        var connectionString = $"Data Source={_testDbPath}";
        _repository = new SqliteAppointmentRepository(connectionString);
    }

    public void Dispose()
    {
        // Clean up test database
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }

    #region Create Tests

    [Fact]
    public async Task TC_S001_Create_GeneratesId_WhenNotProvided()
    {
        // Arrange
        var appointment = new Appointment { Text = "Test Appointment" };

        // Act
        var created = await _repository.CreateAsync(appointment);

        // Assert
        Assert.NotNull(created.Id);
        Assert.NotEmpty(created.Id);
    }

    [Fact]
    public async Task TC_S002_Create_SetsCreatedAtTimestamp()
    {
        // Arrange
        var appointment = new Appointment { Text = "Test Appointment" };
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var created = await _repository.CreateAsync(appointment);
        var after = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(created.CreatedAt >= before);
        Assert.True(created.CreatedAt <= after);
    }

    [Fact]
    public async Task TC_S003_Create_PersistsAllProperties()
    {
        // Arrange
        var appointment = new Appointment
        {
            Text = "Test Appointment",
            Category = "Work",
            Color = "#FF0000",
            Priority = 5,
            ScheduledDate = DateTime.Today.AddDays(1),
            Duration = "2 hours",
            IsOutOfHome = true
        };

        // Act
        var created = await _repository.CreateAsync(appointment);
        var retrieved = await _repository.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(appointment.Text, retrieved.Text);
        Assert.Equal(appointment.Category, retrieved.Category);
        Assert.Equal(appointment.Color, retrieved.Color);
        Assert.Equal(appointment.Priority, retrieved.Priority);
        Assert.Equal(appointment.ScheduledDate, retrieved.ScheduledDate);
        Assert.Equal(appointment.Duration, retrieved.Duration);
        Assert.Equal(appointment.IsOutOfHome, retrieved.IsOutOfHome);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task TC_S004_GetById_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetByIdAsync("non-existent-id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TC_S005_GetById_ReturnsAppointment_WhenExists()
    {
        // Arrange
        var created = await _repository.CreateAsync(new Appointment { Text = "Test" });

        // Act
        var result = await _repository.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal(created.Text, result.Text);
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public async Task TC_S006_GetAll_ReturnsEmptyList_WhenNoAppointments()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task TC_S007_GetAll_ReturnsAppointmentsOrderedByPriority()
    {
        // Arrange
        await _repository.CreateAsync(new Appointment { Text = "Low", Priority = 3 });
        await _repository.CreateAsync(new Appointment { Text = "High", Priority = 1 });
        await _repository.CreateAsync(new Appointment { Text = "Medium", Priority = 2 });

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Priority);
        Assert.Equal(2, result[1].Priority);
        Assert.Equal(3, result[2].Priority);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task TC_S008_Update_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var appointment = new Appointment { Text = "Updated" };

        // Act
        var result = await _repository.UpdateAsync("non-existent-id", appointment);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TC_S009_Update_UpdatesAllProperties()
    {
        // Arrange
        var created = await _repository.CreateAsync(new Appointment
        {
            Text = "Original",
            Category = "Work",
            Color = "#FF0000",
            Priority = 1
        });

        var updated = new Appointment
        {
            Text = "Updated",
            Category = "Personal",
            Color = "#00FF00",
            Priority = 2,
            ScheduledDate = DateTime.Today,
            Duration = "1 hour",
            IsOutOfHome = true
        };

        // Act
        var result = await _repository.UpdateAsync(created.Id, updated);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Text);
        Assert.Equal("Personal", result.Category);
        Assert.Equal("#00FF00", result.Color);
        Assert.Equal(2, result.Priority);
        Assert.Equal(DateTime.Today, result.ScheduledDate);
        Assert.Equal("1 hour", result.Duration);
        Assert.True(result.IsOutOfHome);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task TC_S010_Delete_ReturnsFalse_WhenNotFound()
    {
        // Act
        var result = await _repository.DeleteAsync("non-existent-id");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TC_S011_Delete_RemovesAppointment_WhenExists()
    {
        // Arrange
        var created = await _repository.CreateAsync(new Appointment { Text = "Test" });

        // Act
        var deleted = await _repository.DeleteAsync(created.Id);
        var retrieved = await _repository.GetByIdAsync(created.Id);

        // Assert
        Assert.True(deleted);
        Assert.Null(retrieved);
    }

    #endregion

    #region UpdatePriorities Tests

    [Fact]
    public async Task TC_S012_UpdatePriorities_UpdatesMultipleAppointments()
    {
        // Arrange
        var app1 = await _repository.CreateAsync(new Appointment { Text = "First", Priority = 1 });
        var app2 = await _repository.CreateAsync(new Appointment { Text = "Second", Priority = 2 });
        var app3 = await _repository.CreateAsync(new Appointment { Text = "Third", Priority = 3 });

        var priorities = new Dictionary<string, int>
        {
            { app1.Id, 3 },
            { app2.Id, 1 },
            { app3.Id, 2 }
        };

        // Act
        await _repository.UpdatePrioritiesAsync(priorities);

        // Re-fetch to verify
        var updated1 = await _repository.GetByIdAsync(app1.Id);
        var updated2 = await _repository.GetByIdAsync(app2.Id);
        var updated3 = await _repository.GetByIdAsync(app3.Id);

        // Assert
        Assert.Equal(3, updated1!.Priority);
        Assert.Equal(1, updated2!.Priority);
        Assert.Equal(2, updated3!.Priority);
    }

    #endregion

    #region Persistence Tests

    [Fact]
    public async Task TC_S013_DataPersistsAcrossRepositoryInstances()
    {
        // Arrange
        var testDbPath = $"test_persistence_{Guid.NewGuid()}.db";
        var connectionString = $"Data Source={testDbPath}";

        try
        {
            // Create and populate database with first repository instance
            var repo1 = new SqliteAppointmentRepository(connectionString);
            var created = await repo1.CreateAsync(new Appointment { Text = "Persistent Test" });
            var createdId = created.Id;

            // Create new repository instance with same database
            var repo2 = new SqliteAppointmentRepository(connectionString);
            var retrieved = await repo2.GetByIdAsync(createdId);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(createdId, retrieved.Id);
            Assert.Equal("Persistent Test", retrieved.Text);
        }
        finally
        {
            // Cleanup
            if (File.Exists(testDbPath))
            {
                File.Delete(testDbPath);
            }
        }
    }

    #endregion
}
