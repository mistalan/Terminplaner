using TerminplanerMaui.ViewModels;

namespace TerminplanerMaui.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is MainViewModel viewModel)
        {
            await viewModel.LoadAppointmentsCommand.ExecuteAsync(null);
        }
    }
}
