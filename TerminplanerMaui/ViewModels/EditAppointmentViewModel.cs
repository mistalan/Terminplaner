using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TerminplanerMaui.Models;
using TerminplanerMaui.Services;

namespace TerminplanerMaui.ViewModels;

[QueryProperty(nameof(Appointment), "Appointment")]
public partial class EditAppointmentViewModel : ObservableObject
{
    private readonly AppointmentApiService _apiService;

    [ObservableProperty]
    private Appointment? appointment;

    [ObservableProperty]
    private string editText = string.Empty;

    [ObservableProperty]
    private string editCategory = string.Empty;

    [ObservableProperty]
    private string editColor = "#808080";

    [ObservableProperty]
    private DateTime? editScheduledDate = DateTime.Today;

    [ObservableProperty]
    private TimeSpan editScheduledTime = new TimeSpan(9, 0, 0);

    [ObservableProperty]
    private string editDuration = string.Empty;

    [ObservableProperty]
    private bool editIsOutOfHome;

    [ObservableProperty]
    private bool isDateTimePickerVisible = false;

    // Predefined colors for color picker
    public List<string> AvailableColors { get; } = new()
    {
        "#FF0000", "#FF6B6B", "#FFA500", "#FFD700", "#00FF00", 
        "#00CED1", "#0000FF", "#4B0082", "#8B008B", "#FF1493",
        "#808080", "#A9A9A9", "#000000", "#FFFFFF"
    };

    public string FormattedScheduledDateTime
    {
        get
        {
            if (EditScheduledDate.HasValue)
            {
                var date = EditScheduledDate.Value;
                var dateTime = date.Date + EditScheduledTime;
                return $"{dateTime:dddd, d.M.yyyy} - {dateTime:HH:mm} Uhr";
            }
            return "Kein Datum ausgewählt";
        }
    }

    public EditAppointmentViewModel(AppointmentApiService apiService)
    {
        _apiService = apiService;
    }

    partial void OnEditScheduledDateChanged(DateTime? value)
    {
        OnPropertyChanged(nameof(FormattedScheduledDateTime));
    }

    partial void OnEditScheduledTimeChanged(TimeSpan value)
    {
        OnPropertyChanged(nameof(FormattedScheduledDateTime));
    }

    partial void OnAppointmentChanged(Appointment? value)
    {
        if (value != null)
        {
            EditText = value.Text;
            EditCategory = value.Category;
            EditColor = value.Color;
            
            if (value.ScheduledDate.HasValue)
            {
                EditScheduledDate = value.ScheduledDate.Value.Date;
                EditScheduledTime = value.ScheduledDate.Value.TimeOfDay;
            }
            else
            {
                EditScheduledDate = DateTime.Today;
                EditScheduledTime = new TimeSpan(9, 0, 0);
            }
            
            EditDuration = value.Duration ?? string.Empty;
            EditIsOutOfHome = value.IsOutOfHome;
        }
    }

    [RelayCommand]
    private void SelectColor(string color)
    {
        EditColor = color;
    }

    [RelayCommand]
    private void ShowDateTimePicker()
    {
        IsDateTimePickerVisible = true;
    }

    [RelayCommand]
    private void HideDateTimePicker()
    {
        IsDateTimePickerVisible = false;
        OnPropertyChanged(nameof(FormattedScheduledDateTime));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Appointment == null) return;

        if (string.IsNullOrWhiteSpace(EditText))
        {
            await Shell.Current.DisplayAlert("Fehler", "Bitte gib einen Termin ein!", "OK");
            return;
        }

        // Combine date and time
        DateTime? scheduledDateTime = null;
        if (EditScheduledDate.HasValue)
        {
            scheduledDateTime = EditScheduledDate.Value.Date + EditScheduledTime;
        }

        var updatedAppointment = new Appointment
        {
            Id = Appointment.Id,
            Text = EditText,
            Category = string.IsNullOrWhiteSpace(EditCategory) ? "Standard" : EditCategory,
            Color = EditColor,
            Priority = Appointment.Priority,
            CreatedAt = Appointment.CreatedAt,
            ScheduledDate = scheduledDateTime,
            Duration = string.IsNullOrWhiteSpace(EditDuration) ? null : EditDuration,
            IsOutOfHome = EditIsOutOfHome
        };

        var result = await _apiService.UpdateAppointmentAsync(Appointment.Id, updatedAppointment);
        if (result != null)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.DisplayAlert("Fehler", "Termin konnte nicht aktualisiert werden!", "OK");
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
