using Microsoft.Azure.Cosmos;
using TerminplanerApi.Models;

namespace TerminplanerApi.Repositories;

public class CosmosAppointmentRepository : IAppointmentRepository
{
    private readonly Container _container;

    public CosmosAppointmentRepository(CosmosClient cosmosClient, string databaseId, string containerId)
    {
        _container = cosmosClient.GetContainer(databaseId, containerId);
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        // Generate a new ID if not provided
        if (string.IsNullOrEmpty(appointment.Id))
        {
            appointment.Id = Guid.NewGuid().ToString();
        }
        
        appointment.CreatedAt = DateTime.UtcNow;

        var response = await _container.CreateItemAsync(
            appointment,
            new PartitionKey(appointment.Id)
        );
        
        return response.Resource;
    }

    public async Task<Appointment?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Appointment>(
                id,
                new PartitionKey(id)
            );
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<List<Appointment>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Appointment>(
            "SELECT * FROM c ORDER BY c.Priority"
        );

        var appointments = new List<Appointment>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            appointments.AddRange(response);
        }

        return appointments;
    }

    public async Task<Appointment?> UpdateAsync(string id, Appointment updatedAppointment)
    {
        try
        {
            // Get the existing appointment first
            var existing = await GetByIdAsync(id);
            if (existing == null) return null;

            // Update properties
            existing.Text = updatedAppointment.Text;
            existing.Category = updatedAppointment.Category;
            existing.Color = updatedAppointment.Color;
            existing.Priority = updatedAppointment.Priority;
            existing.ScheduledDate = updatedAppointment.ScheduledDate;
            existing.Duration = updatedAppointment.Duration;
            existing.IsOutOfHome = updatedAppointment.IsOutOfHome;

            var response = await _container.ReplaceItemAsync(
                existing,
                id,
                new PartitionKey(id)
            );

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            await _container.DeleteItemAsync<Appointment>(
                id,
                new PartitionKey(id)
            );
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task UpdatePrioritiesAsync(Dictionary<string, int> priorities)
    {
        foreach (var kvp in priorities)
        {
            var appointment = await GetByIdAsync(kvp.Key);
            if (appointment != null)
            {
                appointment.Priority = kvp.Value;
                await _container.ReplaceItemAsync(
                    appointment,
                    appointment.Id,
                    new PartitionKey(appointment.Id)
                );
            }
        }
    }
}
