namespace PleOps.LibreGlucoseWatcher.Maui.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();

        viewModel.DisplayLoginError.RegisterHandler(OnLoginErrorAsync);
        viewModel.SuccessfulLogin.RegisterHandler(OnSuccessfulLoginAsync);
    }

    private async Task OnLoginErrorAsync()
    {
        await DisplayAlert(
            "Failed",
            "Cannot complete login\nPlease review your email and password",
            "OK");
    }

    private async Task OnSuccessfulLoginAsync()
    {
        await Shell.Current.GoToAsync("//InitialSetup");
    }
}
