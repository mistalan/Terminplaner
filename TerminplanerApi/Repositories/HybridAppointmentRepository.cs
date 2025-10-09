using TerminplanerApi.Models;

namespace TerminplanerApi.Repositories;

/// <summary>
/// Hybrid repository that combines local SQLite storage with cloud Cosmos DB synchronization.
/// Implements simple delta sync on startup and falls back to local-only mode when offline.
/// </summary>
public class HybridAppointmentRepository : IAppointmentRepository
{
    private readonly IAppointmentRepository _localRepository;
    private readonly IAppointmentRepository? _remoteRepository;
    private readonly ILogger<HybridAppointmentRepository>? _logger;
    private bool _isSynced = false;

    public HybridAppointmentRepository(
        IAppointmentRepository localRepository,
        IAppointmentRepository? remoteRepository = null,
        ILogger<HybridAppointmentRepository>? logger = null)
    {
        _localRepository = localRepository;
        _remoteRepository = remoteRepository;
        _logger = logger;
    }

    /// <summary>
    /// Performs initial synchronization between local and remote repositories.
    /// Must be called explicitly after construction, typically at application startup.
    /// </summary>
    public async Task SyncAsync()
    {
        if (_isSynced || _remoteRepository == null)
        {
            _logger?.LogInformation("Sync skipped - already synced or no remote repository configured");
            return;
        }

        try
        {
            _logger?.LogInformation("Starting synchronization between local and remote repositories");

            // Get all appointments from both repositories
            var localAppointments = await _localRepository.GetAllAsync();
            var remoteAppointments = await _remoteRepository.GetAllAsync();

            _logger?.LogInformation("Found {LocalCount} local and {RemoteCount} remote appointments",
                localAppointments.Count, remoteAppointments.Count);

            var localIds = localAppointments.Select(a => a.Id).ToHashSet();
            var remoteIds = remoteAppointments.Select(a => a.Id).ToHashSet();

            // Find appointments only in remote (add to local)
            var remoteOnly = remoteAppointments.Where(a => !localIds.Contains(a.Id)).ToList();
            foreach (var appointment in remoteOnly)
            {
                _logger?.LogInformation("Adding remote appointment {Id} to local repository", appointment.Id);
                // Create a copy with the same ID to preserve it
                var localCopy = new Appointment
                {
                    Id = appointment.Id,
                    Text = appointment.Text,
                    Category = appointment.Category,
                    Color = appointment.Color,
                    Priority = appointment.Priority,
                    CreatedAt = appointment.CreatedAt,
                    ScheduledDate = appointment.ScheduledDate,
                    Duration = appointment.Duration,
                    IsOutOfHome = appointment.IsOutOfHome
                };
                await _localRepository.CreateAsync(localCopy);
            }

            // Find appointments only in local (add to remote)
            var localOnly = localAppointments.Where(a => !remoteIds.Contains(a.Id)).ToList();
            foreach (var appointment in localOnly)
            {
                _logger?.LogInformation("Adding local appointment {Id} to remote repository", appointment.Id);
                // Create a copy with the same ID to preserve it
                var remoteCopy = new Appointment
                {
                    Id = appointment.Id,
                    Text = appointment.Text,
                    Category = appointment.Category,
                    Color = appointment.Color,
                    Priority = appointment.Priority,
                    CreatedAt = appointment.CreatedAt,
                    ScheduledDate = appointment.ScheduledDate,
                    Duration = appointment.Duration,
                    IsOutOfHome = appointment.IsOutOfHome
                };
                await _remoteRepository.CreateAsync(remoteCopy);
            }

            // Find appointments in both (update local if different - Cosmos wins)
            var commonIds = localIds.Intersect(remoteIds);
            foreach (var id in commonIds)
            {
                var local = localAppointments.First(a => a.Id == id);
                var remote = remoteAppointments.First(a => a.Id == id);

                // Check if appointments are different
                if (!AreAppointmentsEqual(local, remote))
                {
                    _logger?.LogInformation("Updating local appointment {Id} from remote (conflict resolution)", id);
                    await _localRepository.UpdateAsync(id, remote);
                }
            }

            _isSynced = true;
            _logger?.LogInformation("Synchronization completed successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to sync with remote repository - continuing in local-only mode");
            // Continue with local-only mode - don't throw
        }
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        // Create in local first
        var created = await _localRepository.CreateAsync(appointment);

        // Try to create in remote if available and synced
        if (_remoteRepository != null && _isSynced)
        {
            try
            {
                // Create a copy to send to remote with the same ID
                var remoteCopy = new Appointment
                {
                    Id = created.Id,
                    Text = created.Text,
                    Category = created.Category,
                    Color = created.Color,
                    Priority = created.Priority,
                    CreatedAt = created.CreatedAt,
                    ScheduledDate = created.ScheduledDate,
                    Duration = created.Duration,
                    IsOutOfHome = created.IsOutOfHome
                };
                await _remoteRepository.CreateAsync(remoteCopy);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to create appointment {Id} in remote repository", created.Id);
                // Continue anyway - local has the data
            }
        }

        return created;
    }

    public async Task<Appointment?> GetByIdAsync(string id)
    {
        // Always read from local (it's synced or we're offline)
        return await _localRepository.GetByIdAsync(id);
    }

    public async Task<List<Appointment>> GetAllAsync()
    {
        // Always read from local (it's synced or we're offline)
        return await _localRepository.GetAllAsync();
    }

    public async Task<Appointment?> UpdateAsync(string id, Appointment updatedAppointment)
    {
        // Update local first
        var updated = await _localRepository.UpdateAsync(id, updatedAppointment);
        
        if (updated == null) return null;

        // Try to update remote if available and synced
        if (_remoteRepository != null && _isSynced)
        {
            try
            {
                await _remoteRepository.UpdateAsync(id, updated);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to update appointment {Id} in remote repository", id);
                // Continue anyway - local has the data
            }
        }

        return updated;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        // Delete from local first
        var deleted = await _localRepository.DeleteAsync(id);

        if (!deleted) return false;

        // Try to delete from remote if available and synced
        if (_remoteRepository != null && _isSynced)
        {
            try
            {
                await _remoteRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to delete appointment {Id} from remote repository", id);
                // Continue anyway - local deletion succeeded
            }
        }

        return true;
    }

    public async Task UpdatePrioritiesAsync(Dictionary<string, int> priorities)
    {
        // Update local first
        await _localRepository.UpdatePrioritiesAsync(priorities);

        // Try to update remote if available and synced
        if (_remoteRepository != null && _isSynced)
        {
            try
            {
                await _remoteRepository.UpdatePrioritiesAsync(priorities);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to update priorities in remote repository");
                // Continue anyway - local has the data
            }
        }
    }

    /// <summary>
    /// Compares two appointments for equality (excluding CreatedAt which may differ slightly).
    /// </summary>
    private static bool AreAppointmentsEqual(Appointment a1, Appointment a2)
    {
        return a1.Text == a2.Text &&
               a1.Category == a2.Category &&
               a1.Color == a2.Color &&
               a1.Priority == a2.Priority &&
               a1.ScheduledDate == a2.ScheduledDate &&
               a1.Duration == a2.Duration &&
               a1.IsOutOfHome == a2.IsOutOfHome;
    }
}
