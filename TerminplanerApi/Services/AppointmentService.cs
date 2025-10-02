using TerminplanerApi.Models;

namespace TerminplanerApi.Services;

public class AppointmentService
{
    private readonly List<Appointment> _appointments = new();
    private int _nextId = 1;

    public AppointmentService()
    {
        // Add some sample data
        _appointments.Add(new Appointment { Id = _nextId++, Text = "Zahnarzttermin", Category = "Gesundheit", Color = "#FF0000", Priority = 1 });
        _appointments.Add(new Appointment { Id = _nextId++, Text = "Projekt abschließen", Category = "Arbeit", Color = "#0000FF", Priority = 2 });
        _appointments.Add(new Appointment { Id = _nextId++, Text = "Einkaufen gehen", Category = "Privat", Color = "#00FF00", Priority = 3 });
    }

    public List<Appointment> GetAll() => _appointments.OrderBy(a => a.Priority).ToList();

    public Appointment? GetById(int id) => _appointments.FirstOrDefault(a => a.Id == id);

    public Appointment Create(Appointment appointment)
    {
        appointment.Id = _nextId++;
        appointment.CreatedAt = DateTime.Now;

        // Set priority to last if not specified
        if (appointment.Priority == 0)
        {
            appointment.Priority = _appointments.Count > 0 ? _appointments.Max(a => a.Priority) + 1 : 1;
        }

        _appointments.Add(appointment);
        return appointment;
    }

    public Appointment? Update(int id, Appointment updatedAppointment)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null) return null;

        appointment.Text = updatedAppointment.Text;
        appointment.Category = updatedAppointment.Category;
        appointment.Color = updatedAppointment.Color;
        appointment.Priority = updatedAppointment.Priority;

        return appointment;
    }

    public bool Delete(int id)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null) return false;

        _appointments.Remove(appointment);
        return true;
    }

    public void UpdatePriorities(Dictionary<int, int> priorities)
    {
        foreach (var kvp in priorities)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == kvp.Key);
            if (appointment != null)
            {
                appointment.Priority = kvp.Value;
            }
        }
    }
}
