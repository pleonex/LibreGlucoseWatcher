using Microsoft.Extensions.Logging;
using PleOps.LibreGlucose;
using PleOps.LibreGlucoseWatcher.Maui.Pages;

namespace PleOps.LibreGlucoseWatcher.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
                .UseMauiApp<App>()
                .RegisterAppServices()
                .RegisterViewModelsWithViews()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FiraSans-Regular.ttf", "FiraSansRegular");
                    fonts.AddFont("FiraSans-Italic.ttf", "FiraSansItalic");
                    fonts.AddFont("FiraSans-Bold.ttf", "FiraSansBold");
                    fonts.AddFont("Symbols-Nerd-Font.ttf", "SymbolsRegular");
                });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<LibreGlucoseClient>();

        return builder;
    }

    private static MauiAppBuilder RegisterViewModelsWithViews(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<AuthLoadingPage>();
        builder.Services.AddSingleton<AuthLoadingViewModel>();

        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LoginViewModel>();

        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<HomeViewModel>();

        return builder;
    }
}
