using TerminplanerApi.Models;

namespace TerminplanerApi.Repositories;

public class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _appointments = new();
    private int _nextId = 1;

    public InMemoryAppointmentRepository()
    {
        // Add some sample data matching legacy scheduler
        _appointments.Add(new Appointment
        {
            Id = _nextId++.ToString(),
            Text = "Zahnarzttermin",
            Category = "Gesundheit",
            Color = "#FF0000",
            Priority = 1,
            ScheduledDate = DateTime.Today.AddHours(10),
            Duration = "1 Std"
        });
        _appointments.Add(new Appointment
        {
            Id = _nextId++.ToString(),
            Text = "Projekt abschließen",
            Category = "Arbeit",
            Color = "#0000FF",
            Priority = 2,
            ScheduledDate = DateTime.Today.AddHours(14),
            Duration = "2-3 Std"
        });
        _appointments.Add(new Appointment
        {
            Id = _nextId++.ToString(),
            Text = "Lebensmittel einkaufen",
            Category = "Privat",
            Color = "#00FF00",
            Priority = 3,
            ScheduledDate = DateTime.Today.AddHours(16),
            Duration = "30 min",
            IsOutOfHome = true
        });
    }

    public Task<List<Appointment>> GetAllAsync()
    {
        var result = _appointments.OrderBy(a => a.Priority).ToList();
        return Task.FromResult(result);
    }

    public Task<Appointment?> GetByIdAsync(string id)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(appointment);
    }

    public Task<Appointment> CreateAsync(Appointment appointment)
    {
        appointment.Id = _nextId++.ToString();
        appointment.CreatedAt = DateTime.Now;

        // Set priority to last if not specified
        if (appointment.Priority == 0)
        {
            appointment.Priority = _appointments.Count > 0 ? _appointments.Max(a => a.Priority) + 1 : 1;
        }

        _appointments.Add(appointment);
        return Task.FromResult(appointment);
    }

    public Task<Appointment?> UpdateAsync(string id, Appointment updatedAppointment)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null) return Task.FromResult<Appointment?>(null);

        appointment.Text = updatedAppointment.Text;
        appointment.Category = updatedAppointment.Category;
        appointment.Color = updatedAppointment.Color;
        appointment.Priority = updatedAppointment.Priority;
        appointment.ScheduledDate = updatedAppointment.ScheduledDate;
        appointment.Duration = updatedAppointment.Duration;
        appointment.IsOutOfHome = updatedAppointment.IsOutOfHome;

        return Task.FromResult<Appointment?>(appointment);
    }

    public Task<bool> DeleteAsync(string id)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null) return Task.FromResult(false);

        _appointments.Remove(appointment);
        return Task.FromResult(true);
    }

    public Task UpdatePrioritiesAsync(Dictionary<string, int> priorities)
    {
        foreach (var kvp in priorities)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == kvp.Key);
            if (appointment != null)
            {
                appointment.Priority = kvp.Value;
            }
        }
        return Task.CompletedTask;
    }
}
