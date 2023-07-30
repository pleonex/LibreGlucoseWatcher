﻿using CommunityToolkit.Mvvm.Input;
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
                client.Login.AuthenticationData = authData;

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
