// Copyright (C) 2023  Benito Palacios Sánchez
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
using Microsoft.Extensions.Logging;
using PleOps.LibreGlucose;
using PleOps.LibreGlucoseWatcher.Maui.Pages;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace PleOps.LibreGlucoseWatcher.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
                .UseMauiApp<App>()
                .UseSkiaSharp(true)
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
        _ = builder.Services.AddSingleton<LibreGlucoseClient>();

        return builder;
    }

    private static MauiAppBuilder RegisterViewModelsWithViews(this MauiAppBuilder builder)
    {
        _ = builder.Services.AddSingleton<AuthLoadingPage>()
            .AddSingleton<AuthLoadingViewModel>();

        _ = builder.Services.AddSingleton<LoginPage>()
            .AddSingleton<LoginViewModel>();

        _ = builder.Services.AddSingleton<InitialSetupPage>()
            .AddSingleton<InitialSetupViewModel>();

        _ = builder.Services.AddSingleton<HomePage>()
            .AddSingleton<HomeViewModel>();

        return builder;
    }
}
