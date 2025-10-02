using TerminplanerMaui.Pages;

namespace TerminplanerMaui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(EditAppointmentPage), typeof(EditAppointmentPage));
    }
}
