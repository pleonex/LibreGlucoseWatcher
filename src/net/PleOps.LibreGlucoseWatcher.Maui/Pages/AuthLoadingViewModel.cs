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
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Connection;
using PleOps.LibreGlucoseWatcher.Maui.Mvvm;

namespace PleOps.LibreGlucoseWatcher.Maui.Pages;

public partial class AuthLoadingViewModel
{
    private readonly LibreGlucoseClient client;
    private readonly ILogger<AuthLoadingViewModel> logger;

    public AuthLoadingViewModel(LibreGlucoseClient client, ILogger<AuthLoadingViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(logger);

        this.client = client;
        this.logger = logger;
    }

    public AsyncInteraction FoundValidSettings { get; } = new();

    public AsyncInteraction FoundInvalidToken { get; } = new();

    public AsyncInteraction NeedPatientSelection { get; } = new();

    [RelayCommand]
    private async Task FindToken()
    {
        try
        {
            var authData = await UserSettings.GetAuthDataAsync();
            if (authData is null || HasExpired(authData))
            {
                await FoundInvalidToken.HandleAsync();
            }
            else
            {
                client.Login.SetAuthentication(authData);

                if (!string.IsNullOrEmpty(UserSettings.GetPatientId())) {
                    await FoundValidSettings.HandleAsync();
                } else {
                    await NeedPatientSelection.HandleAsync();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve auth token");
            await FoundInvalidToken.HandleAsync();
        }
    }

    private static bool HasExpired(AuthData authData)
    {
        if (authData?.AuthTicket?.Expires is not null)
        {
            var expiration = DateTimeOffset.FromUnixTimeSeconds(authData.AuthTicket.Expires)
                .AddMilliseconds(authData.AuthTicket.Duration);
            if (DateTimeOffset.UtcNow > expiration)
            {
                return true;
            }
        }

        return false;
    }
}
