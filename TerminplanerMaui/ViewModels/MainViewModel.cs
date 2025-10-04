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
    private DateTime? newAppointmentScheduledDate = DateTime.Today;

    [ObservableProperty]
    private TimeSpan newAppointmentScheduledTime = new TimeSpan(9, 0, 0);

    [ObservableProperty]
    private string newAppointmentDuration = string.Empty;

    [ObservableProperty]
    private bool newAppointmentIsOutOfHome;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private Appointment? selectedAppointment;

    [ObservableProperty]
    private DateTime currentViewDate = DateTime.Today;

    [ObservableProperty]
    private int daysToShow = 3;

    public string CurrentDateDisplay => $"{CurrentViewDate:dddd, d. MMMM yyyy} - {CurrentViewDate.AddDays(DaysToShow - 1):dddd, d. MMMM yyyy}";

    // Predefined colors for color picker
    public List<string> AvailableColors { get; } = new()
    {
        "#FF0000", "#FF6B6B", "#FFA500", "#FFD700", "#00FF00", 
        "#00CED1", "#0000FF", "#4B0082", "#8B008B", "#FF1493",
        "#808080", "#A9A9A9", "#000000", "#FFFFFF"
    };

    public MainViewModel(AppointmentApiService apiService)
    {
        _apiService = apiService;
    }

    partial void OnCurrentViewDateChanged(DateTime value)
    {
        OnPropertyChanged(nameof(CurrentDateDisplay));
    }

    partial void OnDaysToShowChanged(int value)
    {
        OnPropertyChanged(nameof(CurrentDateDisplay));
    }

    [RelayCommand]
    private async Task LoadAppointmentsAsync()
    {
        IsRefreshing = true;
        try
        {
            var items = await _apiService.GetAllAppointmentsAsync();
            Appointments.Clear();
            
            // Filter appointments to show only those within the current view date range
            var startDate = CurrentViewDate.Date;
            var endDate = CurrentViewDate.AddDays(DaysToShow).Date;
            
            var filteredItems = items
                .Where(a => !a.ScheduledDate.HasValue || 
                           (a.ScheduledDate.Value.Date >= startDate && a.ScheduledDate.Value.Date < endDate))
                .OrderBy(a => a.ScheduledDate ?? DateTime.MaxValue)
                .ThenBy(a => a.Priority);
            
            foreach (var item in filteredItems)
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
    private async Task GoToPreviousDayAsync()
    {
        CurrentViewDate = CurrentViewDate.AddDays(-1);
        await LoadAppointmentsAsync();
    }

    [RelayCommand]
    private async Task GoToNextDayAsync()
    {
        CurrentViewDate = CurrentViewDate.AddDays(1);
        await LoadAppointmentsAsync();
    }

    [RelayCommand]
    private async Task GoToTodayAsync()
    {
        CurrentViewDate = DateTime.Today;
        await LoadAppointmentsAsync();
    }

    [RelayCommand]
    private async Task SetDaysToShowAsync(string days)
    {
        if (int.TryParse(days, out int daysValue))
        {
            DaysToShow = daysValue;
            await LoadAppointmentsAsync();
        }
    }

    [RelayCommand]
    private void SelectColor(string color)
    {
        NewAppointmentColor = color;
    }

    [RelayCommand]
    private async Task AddAppointmentAsync()
    {
        if (string.IsNullOrWhiteSpace(NewAppointmentText))
        {
            await Shell.Current.DisplayAlert("Fehler", "Bitte gib einen Termin ein!", "OK");
            return;
        }

        // Combine date and time
        DateTime? scheduledDateTime = null;
        if (NewAppointmentScheduledDate.HasValue)
        {
            scheduledDateTime = NewAppointmentScheduledDate.Value.Date + NewAppointmentScheduledTime;
        }

        var appointment = new Appointment
        {
            Text = NewAppointmentText,
            Category = string.IsNullOrWhiteSpace(NewAppointmentCategory) ? "Standard" : NewAppointmentCategory,
            Color = NewAppointmentColor,
            Priority = 0,
            ScheduledDate = scheduledDateTime,
            Duration = string.IsNullOrWhiteSpace(NewAppointmentDuration) ? null : NewAppointmentDuration,
            IsOutOfHome = NewAppointmentIsOutOfHome
        };

        var created = await _apiService.CreateAppointmentAsync(appointment);
        if (created != null)
        {
            NewAppointmentText = string.Empty;
            NewAppointmentCategory = string.Empty;
            NewAppointmentColor = "#808080";
            NewAppointmentScheduledDate = DateTime.Today;
            NewAppointmentScheduledTime = new TimeSpan(9, 0, 0);
            NewAppointmentDuration = string.Empty;
            NewAppointmentIsOutOfHome = false;
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
