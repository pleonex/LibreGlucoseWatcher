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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Connection;
using PleOps.LibreGlucoseWatcher.Maui.Mvvm;

namespace PleOps.LibreGlucoseWatcher.Maui.Pages;

public partial class LoginViewModel : ObservableObject
{
    private readonly LibreGlucoseClient client;
    private readonly ILogger logger;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public LoginViewModel(LibreGlucoseClient client, ILogger<LoginViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(client);

        this.client = client;
        this.logger = logger;
    }

    public AsyncInteraction DisplayLoginError { get; } = new();

    public AsyncInteraction SuccessfulLogin { get; } = new();

    [RelayCommand]
    private async Task Login()
    {
        var loginParams = new LoginParameters(Email, Password);
        try
        {
            await client.Login.LoginAsync(loginParams);

            await UserSettings.SetAuthDataAsync(client.Login.AuthenticationData);

            await SuccessfulLogin.HandleAsync();
        } catch (Exception ex)
        {
            logger.LogError(ex, "Failed to login");
            await DisplayLoginError.HandleAsync();
        }
    }
}
