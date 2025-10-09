using TerminplanerApi.Models;
using TerminplanerApi.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace TerminplanerApi.Tests;

public class HybridAppointmentRepositoryTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly Mock<ILogger<HybridAppointmentRepository>> _mockLogger;

    public HybridAppointmentRepositoryTests()
    {
        _testDbPath = $"test_hybrid_{Guid.NewGuid()}.db";
        _mockLogger = new Mock<ILogger<HybridAppointmentRepository>>();
    }

    public void Dispose()
    {
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }

    private SqliteAppointmentRepository CreateLocalRepository()
    {
        return new SqliteAppointmentRepository($"Data Source={_testDbPath}");
    }

    private InMemoryAppointmentRepository CreateRemoteRepository()
    {
        // Use InMemory as a mock remote repository (simpler than Cosmos for testing)
        return new InMemoryAppointmentRepository();
    }

    #region Sync Tests

    [Fact]
    public async Task TC_H001_Sync_AddsRemoteAppointmentsToLocal()
    {
        // Arrange
        var local = CreateLocalRepository();
        var remote = CreateRemoteRepository();
        
        // Add appointments to remote only
        var remoteAppt = await remote.CreateAsync(new Appointment { Text = "Remote Only" });

        var hybrid = new HybridAppointmentRepository(local, remote, _mockLogger.Object);

        // Act
        await hybrid.SyncAsync();

        // Assert - remote appointment should now be in local
        var localAppointments = await local.GetAllAsync();
        Assert.Contains(localAppointments, a => a.Text == "Remote Only");
    }

    [Fact]
    public async Task TC_H002_Sync_AddsLocalAppointmentsToRemote()
    {
        // Arrange
        var local = CreateLocalRepository();
        var remote = CreateRemoteRepository();
        
        // Clear remote sample data first
        var remoteSamples = await remote.GetAllAsync();
        foreach (var sample in remoteSamples)
        {
            await remote.DeleteAsync(sample.Id);
        }
        
        // Add appointment to local only
        await local.CreateAsync(new Appointment { Text = "Local Only", Id = "local-1" });

        var hybrid = new HybridAppointmentRepository(local, remote, _mockLogger.Object);

        // Act
        await hybrid.SyncAsync();

        // Assert - local appointment should now be in remote
        var remoteAppointments = await remote.GetAllAsync();
        Assert.Contains(remoteAppointments, a => a.Text == "Local Only");
    }

    [Fact]
    public async Task TC_H003_Sync_RemoteWinsOnConflict()
    {
        // Arrange
        var local = CreateLocalRepository();
        var remote = CreateRemoteRepository();
        
        // Clear remote sample data
        var remoteSamples = await remote.GetAllAsync();
        foreach (var sample in remoteSamples)
        {
            await remote.DeleteAsync(sample.Id);
        }
        
        // Add same ID to both with different content
        var sharedId = "shared-id-1";
        await local.CreateAsync(new Appointment { Id = sharedId, Text = "Local Version", Priority = 1 });
        await remote.CreateAsync(new Appointment { Id = sharedId, Text = "Remote Version", Priority = 2 });

        var hybrid = new HybridAppointmentRepository(local, remote, _mockLogger.Object);

        // Act
        await hybrid.SyncAsync();

        // Assert - local should have remote version (Cosmos wins)
        var localAppt = await local.GetByIdAsync(sharedId);
        Assert.NotNull(localAppt);
        Assert.Equal("Remote Version", localAppt.Text);
        Assert.Equal(2, localAppt.Priority);
    }

    [Fact]
    public async Task TC_H004_Sync_OnlyRunsOnce()
    {
        // Arrange
        var local = CreateLocalRepository();
        var remote = CreateRemoteRepository();
        var hybrid = new HybridAppointmentRepository(local, remote, _mockLogger.Object);

        // Act - call sync twice
        await hybrid.SyncAsync();
        
        // Add data to remote after first sync
        await remote.CreateAsync(new Appointment { Text = "After Sync" });
        
        // Call sync again
        await hybrid.SyncAsync();

        // Assert - second sync should not pull new data (already synced)
        var localAppointments = await local.GetAllAsync();
        Assert.DoesNotContain(localAppointments, a => a.Text == "After Sync");
    }

    [Fact]
    public async Task TC_H005_Sync_WorksWithoutRemoteRepository()
    {
        // Arrange
        var local = CreateLocalRepository();
        var hybrid = new HybridAppointmentRepository(local, null, _mockLogger.Object);

        // Act & Assert - should not throw
        await hybrid.SyncAsync();
    }

    #endregion

    #region CRUD Tests (Post-Sync)

    [Fact]
    public async Task TC_H006_Create_AddsToLocalAndRemote_WhenSynced()
    {
        // Arrange
        var local = CreateLocalRepository();
        var remote = CreateRemoteRepository();
        
        // Clear remote sample data
        var remoteSamples = await remote.GetAllAsync();
        foreach (var sample in remoteSamples)
        {
            await remote.DeleteAsync(sample.Id);
        }
        
        var hybrid = new HybridAppointmentRepository(local, remote, _mockLogger.Object);
        await hybrid.SyncAsync();

        // Act
        var created = await hybrid.CreateAsync(new Appointment { Text = "New Appointment" });

        // Assert
        var localAppt = await local.GetByIdAsync(created.Id);
        var remoteAppt = await remote.GetByIdAsync(created.Id);
        
        Assert.NotNull(localAppt);
        Assert.NotNull(remoteAppt);
        Assert.Equal("New Appointment", localAppt.Text);
        Assert.Equal("New Appointment", remoteAppt.Text);
    }

    [Fact]
    public async Task TC_H007_Create_OnlyAddsToLocal_WhenNoRemote()
    {
        // Arrange
        var local = CreateLocalRepository();
        var hybrid = new HybridAppointmentRepository(local, null, _mockLogger.Object);
        await hybrid.SyncAsync();

        // Act
        var created = await hybrid.CreateAsync(new Appointment { Text = "Local Only" });

        // Assert
        var localAppt = await local.GetByIdAsync(created.Id);
        Assert.NotNull(localAppt);
        Assert.Equal("Local Only", localAppt.Text);
    }

    [Fact]
    public async Task TC_H008_GetAll_ReturnsFromLocal()
    {
        // Arrange
        var local = CreateLocalRepository();
        var remote = CreateRemoteRepository();
        var hybrid = new HybridAppointmentRepository(local, remote, _mockLogger.Object);
        await hybrid.SyncAsync();

        await hybrid.CreateAsync(new Appointment { Text = "Test 1" });
        await hybrid.CreateAsync(new Appointment { Text = "Test 2" });

        // Act
        var appointments = await hybrid.GetAllAsync();

        // Assert - should have our 2 appointments plus 3 from remote sample data
        Assert.True(appointments.Count >= 2);
        Assert.Contains(appointments, a => a.Text == "Test 1");
        Assert.Contains(appointments, a => a.Text == "Test 2");
    }

    [Fact]
    public async Task TC_H009_Update_UpdatesBothLocalAndRemote()
    {
        // Arrange
        var local = CreateLocalRepository();
        var remote = CreateRemoteRepository();
        
        // Clear remote sample data
        var remoteSamples = await remote.GetAllAsync();
        foreach (var sample in remoteSamples)
        {
            await remote.DeleteAsync(sample.Id);
        }
        
        var hybrid = new HybridAppointmentRepository(local, remote, _mockLogger.Object);
        await hybrid.SyncAsync();

        var created = await hybrid.CreateAsync(new Appointment { Text = "Original" });

        // Act
        var updated = await hybrid.UpdateAsync(created.Id, new Appointment { Text = "Updated" });

        // Assert
        var localAppt = await local.GetByIdAsync(created.Id);
        var remoteAppt = await remote.GetByIdAsync(created.Id);
        
        Assert.NotNull(updated);
        Assert.Equal("Updated", localAppt!.Text);
        Assert.Equal("Updated", remoteAppt!.Text);
    }

    [Fact]
    public async Task TC_H010_Delete_DeletesFromBothLocalAndRemote()
    {
        // Arrange
        var local = CreateLocalRepository();
        var remote = CreateRemoteRepository();
        
        // Clear remote sample data
        var remoteSamples = await remote.GetAllAsync();
        foreach (var sample in remoteSamples)
        {
            await remote.DeleteAsync(sample.Id);
        }
        
        var hybrid = new HybridAppointmentRepository(local, remote, _mockLogger.Object);
        await hybrid.SyncAsync();

        var created = await hybrid.CreateAsync(new Appointment { Text = "To Delete" });

        // Act
        var deleted = await hybrid.DeleteAsync(created.Id);

        // Assert
        var localAppt = await local.GetByIdAsync(created.Id);
        var remoteAppt = await remote.GetByIdAsync(created.Id);
        
        Assert.True(deleted);
        Assert.Null(localAppt);
        Assert.Null(remoteAppt);
    }

    [Fact]
    public async Task TC_H011_UpdatePriorities_UpdatesBothLocalAndRemote()
    {
        // Arrange
        var local = CreateLocalRepository();
        var remote = CreateRemoteRepository();
        
        // Clear remote sample data
        var remoteSamples = await remote.GetAllAsync();
        foreach (var sample in remoteSamples)
        {
            await remote.DeleteAsync(sample.Id);
        }
        
        var hybrid = new HybridAppointmentRepository(local, remote, _mockLogger.Object);
        await hybrid.SyncAsync();

        var app1 = await hybrid.CreateAsync(new Appointment { Text = "First", Priority = 1 });
        var app2 = await hybrid.CreateAsync(new Appointment { Text = "Second", Priority = 2 });

        var priorities = new Dictionary<string, int>
        {
            { app1.Id, 2 },
            { app2.Id, 1 }
        };

        // Act
        await hybrid.UpdatePrioritiesAsync(priorities);

        // Assert
        var localApp1 = await local.GetByIdAsync(app1.Id);
        var remoteApp1 = await remote.GetByIdAsync(app1.Id);
        
        Assert.Equal(2, localApp1!.Priority);
        Assert.Equal(2, remoteApp1!.Priority);
    }

    #endregion

    #region Offline Behavior Tests

    [Fact]
    public async Task TC_H012_Create_ContinuesWhenRemoteFails()
    {
        // Arrange
        var local = CreateLocalRepository();
        
        // Create a mock remote that throws on Create
        var mockRemote = new Mock<IAppointmentRepository>();
        mockRemote.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Appointment>());
        mockRemote.Setup(r => r.CreateAsync(It.IsAny<Appointment>()))
            .ThrowsAsync(new Exception("Network error"));

        var hybrid = new HybridAppointmentRepository(local, mockRemote.Object, _mockLogger.Object);
        await hybrid.SyncAsync();

        // Act - should not throw even though remote fails
        var created = await hybrid.CreateAsync(new Appointment { Text = "Test" });

        // Assert - local should have it
        var localAppt = await local.GetByIdAsync(created.Id);
        Assert.NotNull(localAppt);
        Assert.Equal("Test", localAppt.Text);
    }

    #endregion
}
