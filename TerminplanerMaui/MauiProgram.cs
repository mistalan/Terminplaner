using Microsoft.Extensions.Logging;
using TerminplanerMaui.Pages;
using TerminplanerMaui.Services;
using TerminplanerMaui.ViewModels;

namespace TerminplanerMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register Services
        builder.Services.AddSingleton<AppointmentApiService>();

        // Register ViewModels
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<EditAppointmentViewModel>();

        // Register Pages
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<EditAppointmentPage>();

        return builder.Build();
    }
}
