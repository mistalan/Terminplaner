using System.Text.Json.Serialization;

namespace TerminplanerApi.Models;

public class Appointment
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = "Standard";
    public string Color { get; set; } = "#808080";
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ScheduledDate { get; set; }
    public string? Duration { get; set; }
    public bool IsOutOfHome { get; set; }
}
