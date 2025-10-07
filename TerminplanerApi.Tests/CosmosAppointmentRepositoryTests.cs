using Microsoft.Azure.Cosmos;
using Moq;
using System.Net;
using TerminplanerApi.Models;
using TerminplanerApi.Repositories;

namespace TerminplanerApi.Tests;

/// <summary>
/// Unit tests for CosmosAppointmentRepository
/// Tests the Cosmos DB repository implementation using mocking
/// </summary>
public class CosmosAppointmentRepositoryTests
{
    private readonly Mock<CosmosClient> _mockCosmosClient;
    private readonly Mock<Container> _mockContainer;
    private readonly CosmosAppointmentRepository _repository;

    public CosmosAppointmentRepositoryTests()
    {
        _mockCosmosClient = new Mock<CosmosClient>();
        _mockContainer = new Mock<Container>();
        
        _mockCosmosClient
            .Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_mockContainer.Object);

        _repository = new CosmosAppointmentRepository(_mockCosmosClient.Object, "db_1", "container_1");
    }

    #region CreateAsync Tests

    [Fact]
    public async Task TC_C001_CreateAsync_GeneratesIdWhenNotProvided()
    {
        // Arrange
        var appointment = new Appointment { Text = "Test" };
        var mockResponse = new Mock<ItemResponse<Appointment>>();
        
        _mockContainer
            .Setup(c => c.CreateItemAsync(
                It.IsAny<Appointment>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, PartitionKey pk, ItemRequestOptions o, CancellationToken ct) =>
            {
                mockResponse.Setup(r => r.Resource).Returns(a);
                return mockResponse.Object;
            });

        // Act
        var result = await _repository.CreateAsync(appointment);

        // Assert
        Assert.NotNull(result.Id);
        Assert.False(string.IsNullOrEmpty(result.Id));
    }

    [Fact]
    public async Task TC_C002_CreateAsync_PreservesProvidedId()
    {
        // Arrange
        var appointment = new Appointment { Id = "custom-id", Text = "Test" };
        var mockResponse = new Mock<ItemResponse<Appointment>>();
        
        _mockContainer
            .Setup(c => c.CreateItemAsync(
                It.IsAny<Appointment>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, PartitionKey pk, ItemRequestOptions o, CancellationToken ct) =>
            {
                mockResponse.Setup(r => r.Resource).Returns(a);
                return mockResponse.Object;
            });

        // Act
        var result = await _repository.CreateAsync(appointment);

        // Assert
        Assert.Equal("custom-id", result.Id);
    }

    [Fact]
    public async Task TC_C003_CreateAsync_SetsCreatedAtTimestamp()
    {
        // Arrange
        var appointment = new Appointment { Text = "Test" };
        var mockResponse = new Mock<ItemResponse<Appointment>>();
        var before = DateTime.UtcNow.AddSeconds(-1);
        
        _mockContainer
            .Setup(c => c.CreateItemAsync(
                It.IsAny<Appointment>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, PartitionKey pk, ItemRequestOptions o, CancellationToken ct) =>
            {
                mockResponse.Setup(r => r.Resource).Returns(a);
                return mockResponse.Object;
            });

        // Act
        var result = await _repository.CreateAsync(appointment);
        var after = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(result.CreatedAt >= before);
        Assert.True(result.CreatedAt <= after);
    }

    [Fact]
    public async Task TC_C004_CreateAsync_UsesIdAsPartitionKey()
    {
        // Arrange
        var appointment = new Appointment { Id = "test-id", Text = "Test" };
        var mockResponse = new Mock<ItemResponse<Appointment>>();
        PartitionKey? capturedPartitionKey = null;
        
        _mockContainer
            .Setup(c => c.CreateItemAsync(
                It.IsAny<Appointment>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, PartitionKey pk, ItemRequestOptions o, CancellationToken ct) =>
            {
                capturedPartitionKey = pk;
                mockResponse.Setup(r => r.Resource).Returns(a);
                return mockResponse.Object;
            });

        // Act
        await _repository.CreateAsync(appointment);

        // Assert
        Assert.NotNull(capturedPartitionKey);
        Assert.Equal(new PartitionKey("test-id"), capturedPartitionKey.Value);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task TC_C005_GetByIdAsync_ReturnsAppointment_WhenExists()
    {
        // Arrange
        var appointment = new Appointment { Id = "test-id", Text = "Test" };
        var mockResponse = new Mock<ItemResponse<Appointment>>();
        mockResponse.Setup(r => r.Resource).Returns(appointment);
        
        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _repository.GetByIdAsync("test-id");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-id", result.Id);
        Assert.Equal("Test", result.Text);
    }

    [Fact]
    public async Task TC_C006_GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "non-existent",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, "", 0));

        // Act
        var result = await _repository.GetByIdAsync("non-existent");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task TC_C007_GetAllAsync_ReturnsAllAppointments()
    {
        // Arrange
        var appointments = new List<Appointment>
        {
            new Appointment { Id = "1", Text = "First", Priority = 1 },
            new Appointment { Id = "2", Text = "Second", Priority = 2 }
        };

        var mockIterator = new Mock<FeedIterator<Appointment>>();
        var mockResponse = new Mock<FeedResponse<Appointment>>();
        
        mockResponse.Setup(r => r.GetEnumerator()).Returns(appointments.GetEnumerator());
        
        var sequence = mockIterator.SetupSequence(i => i.HasMoreResults);
        sequence.Returns(true);
        sequence.Returns(false);
        
        mockIterator
            .Setup(i => i.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        _mockContainer
            .Setup(c => c.GetItemQueryIterator<Appointment>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
            .Returns(mockIterator.Object);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, a => a.Id == "1");
        Assert.Contains(result, a => a.Id == "2");
    }

    [Fact]
    public async Task TC_C008_GetAllAsync_ReturnsEmptyList_WhenNoAppointments()
    {
        // Arrange
        var mockIterator = new Mock<FeedIterator<Appointment>>();
        var mockResponse = new Mock<FeedResponse<Appointment>>();
        
        mockResponse.Setup(r => r.GetEnumerator()).Returns(new List<Appointment>().GetEnumerator());
        
        mockIterator.Setup(i => i.HasMoreResults).Returns(false);

        _mockContainer
            .Setup(c => c.GetItemQueryIterator<Appointment>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
            .Returns(mockIterator.Object);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task TC_C009_UpdateAsync_UpdatesExistingAppointment()
    {
        // Arrange
        var existing = new Appointment { Id = "test-id", Text = "Original", Category = "Work" };
        var updated = new Appointment { Text = "Updated", Category = "Personal" };
        
        var readResponse = new Mock<ItemResponse<Appointment>>();
        readResponse.Setup(r => r.Resource).Returns(existing);
        
        var replaceResponse = new Mock<ItemResponse<Appointment>>();
        
        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(readResponse.Object);

        _mockContainer
            .Setup(c => c.ReplaceItemAsync(
                It.IsAny<Appointment>(),
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, string id, PartitionKey pk, ItemRequestOptions o, CancellationToken ct) =>
            {
                replaceResponse.Setup(r => r.Resource).Returns(a);
                return replaceResponse.Object;
            });

        // Act
        var result = await _repository.UpdateAsync("test-id", updated);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Text);
        Assert.Equal("Personal", result.Category);
    }

    [Fact]
    public async Task TC_C010_UpdateAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var updated = new Appointment { Text = "Updated" };
        
        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "non-existent",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, "", 0));

        // Act
        var result = await _repository.UpdateAsync("non-existent", updated);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TC_C011_UpdateAsync_PreservesId()
    {
        // Arrange
        var existing = new Appointment { Id = "test-id", Text = "Original" };
        var updated = new Appointment { Text = "Updated" };
        
        var readResponse = new Mock<ItemResponse<Appointment>>();
        readResponse.Setup(r => r.Resource).Returns(existing);
        
        var replaceResponse = new Mock<ItemResponse<Appointment>>();
        
        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(readResponse.Object);

        _mockContainer
            .Setup(c => c.ReplaceItemAsync(
                It.IsAny<Appointment>(),
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, string id, PartitionKey pk, ItemRequestOptions o, CancellationToken ct) =>
            {
                replaceResponse.Setup(r => r.Resource).Returns(a);
                return replaceResponse.Object;
            });

        // Act
        var result = await _repository.UpdateAsync("test-id", updated);

        // Assert
        Assert.Equal("test-id", result!.Id);
    }

    [Fact]
    public async Task TC_C016_UpdateAsync_ReturnsNull_WhenReplaceThrowsNotFound()
    {
        // Arrange
        var existing = new Appointment { Id = "test-id", Text = "Original" };
        var updated = new Appointment { Text = "Updated" };
        
        var readResponse = new Mock<ItemResponse<Appointment>>();
        readResponse.Setup(r => r.Resource).Returns(existing);
        
        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(readResponse.Object);

        _mockContainer
            .Setup(c => c.ReplaceItemAsync(
                It.IsAny<Appointment>(),
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found during replace", HttpStatusCode.NotFound, 0, "", 0));

        // Act
        var result = await _repository.UpdateAsync("test-id", updated);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TC_C017_UpdateAsync_ThrowsException_WhenReplaceThrowsNonNotFoundError()
    {
        // Arrange
        var existing = new Appointment { Id = "test-id", Text = "Original" };
        var updated = new Appointment { Text = "Updated" };
        
        var readResponse = new Mock<ItemResponse<Appointment>>();
        readResponse.Setup(r => r.Resource).Returns(existing);
        
        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(readResponse.Object);

        _mockContainer
            .Setup(c => c.ReplaceItemAsync(
                It.IsAny<Appointment>(),
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Service unavailable", HttpStatusCode.ServiceUnavailable, 0, "", 0));

        // Act & Assert
        await Assert.ThrowsAsync<CosmosException>(
            async () => await _repository.UpdateAsync("test-id", updated)
        );
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task TC_C012_DeleteAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var mockResponse = new Mock<ItemResponse<Appointment>>();
        
        _mockContainer
            .Setup(c => c.DeleteItemAsync<Appointment>(
                "test-id",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _repository.DeleteAsync("test-id");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TC_C013_DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        // Arrange
        _mockContainer
            .Setup(c => c.DeleteItemAsync<Appointment>(
                "non-existent",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, "", 0));

        // Act
        var result = await _repository.DeleteAsync("non-existent");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region UpdatePrioritiesAsync Tests

    [Fact]
    public async Task TC_C014_UpdatePrioritiesAsync_UpdatesMultipleAppointments()
    {
        // Arrange
        var appt1 = new Appointment { Id = "1", Priority = 1 };
        var appt2 = new Appointment { Id = "2", Priority = 2 };
        
        var readResponse1 = new Mock<ItemResponse<Appointment>>();
        readResponse1.Setup(r => r.Resource).Returns(appt1);
        
        var readResponse2 = new Mock<ItemResponse<Appointment>>();
        readResponse2.Setup(r => r.Resource).Returns(appt2);
        
        var replaceResponse = new Mock<ItemResponse<Appointment>>();
        
        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "1",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(readResponse1.Object);

        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "2",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(readResponse2.Object);

        _mockContainer
            .Setup(c => c.ReplaceItemAsync(
                It.IsAny<Appointment>(),
                It.IsAny<string>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(replaceResponse.Object);

        var priorities = new Dictionary<string, int>
        {
            { "1", 5 },
            { "2", 3 }
        };

        // Act
        await _repository.UpdatePrioritiesAsync(priorities);

        // Assert
        _mockContainer.Verify(c => c.ReplaceItemAsync(
            It.Is<Appointment>(a => a.Id == "1" && a.Priority == 5),
            "1",
            It.IsAny<PartitionKey>(),
            It.IsAny<ItemRequestOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockContainer.Verify(c => c.ReplaceItemAsync(
            It.Is<Appointment>(a => a.Id == "2" && a.Priority == 3),
            "2",
            It.IsAny<PartitionKey>(),
            It.IsAny<ItemRequestOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TC_C015_UpdatePrioritiesAsync_IgnoresNonExistentIds()
    {
        // Arrange
        var appt1 = new Appointment { Id = "1", Priority = 1 };
        
        var readResponse = new Mock<ItemResponse<Appointment>>();
        readResponse.Setup(r => r.Resource).Returns(appt1);
        
        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "1",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(readResponse.Object);

        _mockContainer
            .Setup(c => c.ReadItemAsync<Appointment>(
                "999",
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, "", 0));

        var priorities = new Dictionary<string, int>
        {
            { "1", 5 },
            { "999", 10 }  // Non-existent
        };

        // Act & Assert (should not throw)
        await _repository.UpdatePrioritiesAsync(priorities);
        
        _mockContainer.Verify(c => c.ReplaceItemAsync(
            It.Is<Appointment>(a => a.Id == "1"),
            It.IsAny<string>(),
            It.IsAny<PartitionKey>(),
            It.IsAny<ItemRequestOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
