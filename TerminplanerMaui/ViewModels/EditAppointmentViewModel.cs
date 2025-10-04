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
    private DateTime? editScheduledDate;

    [ObservableProperty]
    private string editDuration = string.Empty;

    [ObservableProperty]
    private bool editIsOutOfHome;

    public EditAppointmentViewModel(AppointmentApiService apiService)
    {
        _apiService = apiService;
    }

    partial void OnAppointmentChanged(Appointment? value)
    {
        if (value != null)
        {
            EditText = value.Text;
            EditCategory = value.Category;
            EditColor = value.Color;
            EditScheduledDate = value.ScheduledDate;
            EditDuration = value.Duration ?? string.Empty;
            EditIsOutOfHome = value.IsOutOfHome;
        }
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

        var updatedAppointment = new Appointment
        {
            Id = Appointment.Id,
            Text = EditText,
            Category = string.IsNullOrWhiteSpace(EditCategory) ? "Standard" : EditCategory,
            Color = EditColor,
            Priority = Appointment.Priority,
            CreatedAt = Appointment.CreatedAt,
            ScheduledDate = EditScheduledDate,
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
