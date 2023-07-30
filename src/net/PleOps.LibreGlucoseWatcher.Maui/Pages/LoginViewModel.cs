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
