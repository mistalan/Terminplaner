using TerminplanerApi.Models;

namespace TerminplanerApi.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment> CreateAsync(Appointment appointment);
    Task<Appointment?> GetByIdAsync(string id);
    Task<List<Appointment>> GetAllAsync();
    Task<Appointment?> UpdateAsync(string id, Appointment appointment);
    Task<bool> DeleteAsync(string id);
    Task UpdatePrioritiesAsync(Dictionary<string, int> priorities);
}
