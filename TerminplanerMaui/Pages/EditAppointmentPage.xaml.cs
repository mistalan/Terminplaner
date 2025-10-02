using TerminplanerMaui.ViewModels;

namespace TerminplanerMaui.Pages;

public partial class EditAppointmentPage : ContentPage
{
    public EditAppointmentPage(EditAppointmentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
