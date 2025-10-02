using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TerminplanerMaui.Models;
using TerminplanerMaui.Services;

namespace TerminplanerMaui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AppointmentApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Appointment> appointments = new();

    [ObservableProperty]
    private string newAppointmentText = string.Empty;

    [ObservableProperty]
    private string newAppointmentCategory = string.Empty;

    [ObservableProperty]
    private string newAppointmentColor = "#808080";

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private Appointment? selectedAppointment;

    public MainViewModel(AppointmentApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadAppointmentsAsync()
    {
        IsRefreshing = true;
        try
        {
            var items = await _apiService.GetAllAppointmentsAsync();
            Appointments.Clear();
            foreach (var item in items.OrderBy(a => a.Priority))
            {
                Appointments.Add(item);
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task AddAppointmentAsync()
    {
        if (string.IsNullOrWhiteSpace(NewAppointmentText))
        {
            await Shell.Current.DisplayAlert("Fehler", "Bitte gib einen Termin ein!", "OK");
            return;
        }

        var appointment = new Appointment
        {
            Text = NewAppointmentText,
            Category = string.IsNullOrWhiteSpace(NewAppointmentCategory) ? "Standard" : NewAppointmentCategory,
            Color = NewAppointmentColor,
            Priority = 0
        };

        var created = await _apiService.CreateAppointmentAsync(appointment);
        if (created != null)
        {
            NewAppointmentText = string.Empty;
            NewAppointmentCategory = string.Empty;
            NewAppointmentColor = "#808080";
            await LoadAppointmentsAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert("Fehler", "Termin konnte nicht erstellt werden!", "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteAppointmentAsync(Appointment appointment)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Löschen bestätigen",
            $"Möchtest du '{appointment.Text}' wirklich löschen?",
            "Ja",
            "Nein");

        if (!confirmed) return;

        var success = await _apiService.DeleteAppointmentAsync(appointment.Id);
        if (success)
        {
            await LoadAppointmentsAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert("Fehler", "Termin konnte nicht gelöscht werden!", "OK");
        }
    }

    [RelayCommand]
    private async Task EditAppointmentAsync(Appointment appointment)
    {
        SelectedAppointment = appointment;
        
        // Navigate to edit page
        var parameters = new Dictionary<string, object>
        {
            { "Appointment", appointment }
        };
        
        await Shell.Current.GoToAsync("EditAppointmentPage", parameters);
    }

    [RelayCommand]
    private async Task MovePriorityUpAsync(Appointment appointment)
    {
        var index = Appointments.IndexOf(appointment);
        if (index <= 0) return;

        var otherAppointment = Appointments[index - 1];
        
        var priorities = new Dictionary<int, int>
        {
            { appointment.Id, otherAppointment.Priority },
            { otherAppointment.Id, appointment.Priority }
        };

        var success = await _apiService.UpdatePrioritiesAsync(priorities);
        if (success)
        {
            await LoadAppointmentsAsync();
        }
    }

    [RelayCommand]
    private async Task MovePriorityDownAsync(Appointment appointment)
    {
        var index = Appointments.IndexOf(appointment);
        if (index >= Appointments.Count - 1) return;

        var otherAppointment = Appointments[index + 1];
        
        var priorities = new Dictionary<int, int>
        {
            { appointment.Id, otherAppointment.Priority },
            { otherAppointment.Id, appointment.Priority }
        };

        var success = await _apiService.UpdatePrioritiesAsync(priorities);
        if (success)
        {
            await LoadAppointmentsAsync();
        }
    }
}
