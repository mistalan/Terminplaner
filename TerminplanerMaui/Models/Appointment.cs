namespace TerminplanerMaui.Models;

public class Appointment
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = "Standard";
    public string Color { get; set; } = "#808080";
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}
