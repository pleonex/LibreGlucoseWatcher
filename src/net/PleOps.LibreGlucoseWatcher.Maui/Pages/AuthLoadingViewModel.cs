using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Connection;
using PleOps.LibreGlucoseWatcher.Maui.Mvvm;
using System.Text.Json;

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

    public AsyncInteraction FoundValidToken { get; } = new();

    public AsyncInteraction FoundInvalidToken { get; } = new();

    [RelayCommand]
    private async Task FindToken()
    {
        // To show my cool intro page, TODO: remove later...
        await Task.Delay(2_000).ConfigureAwait(true);

        try
        {
            var authData = await LibreGlucoseSettings.GetAuthDataAsync();
            if (authData is null || HasExpired(authData))
            {
                await FoundInvalidToken.HandleAsync();
            }
            else
            {
                client.Login.AuthenticationData = authData;
                await FoundValidToken.HandleAsync();
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
