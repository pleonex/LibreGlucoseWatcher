namespace PleOps.LibreGlucoseWatcher.Maui.Pages;

public partial class AuthLoadingPage : ContentPage
{
    public AuthLoadingPage(AuthLoadingViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();

        viewModel.FoundValidSettings.RegisterHandler(OnValidTokenAsync);
        viewModel.FoundInvalidToken.RegisterHandler(OnInvalidTokenAsync);
        viewModel.NeedPatientSelection.RegisterHandler(OnPatientSelectionRequiredAsync);
    }

    internal AuthLoadingViewModel ViewModel => (BindingContext as AuthLoadingViewModel)!;

    private async Task OnValidTokenAsync() =>
        await Shell.Current.GoToAsync("//Home").ConfigureAwait(true);

    private async Task OnInvalidTokenAsync() =>
        await Shell.Current.GoToAsync("//Login").ConfigureAwait(true);

    private async Task OnPatientSelectionRequiredAsync() =>
        await Shell.Current.GoToAsync("//InitialSetup").ConfigureAwait(true);

    private async void ContentPage_Loaded(object sender, EventArgs e) =>
        await ViewModel.FindTokenCommand.ExecuteAsync(null);
}
