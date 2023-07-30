namespace PleOps.LibreGlucoseWatcher.Maui.Pages;

public partial class InitialSetupPage : ContentPage
{
    public InitialSetupPage(InitialSetupViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();

        viewModel.SettingsSaved.RegisterHandler(ContinueToHomePage);
    }

    internal InitialSetupViewModel ViewModel => (BindingContext as InitialSetupViewModel)!;

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        ViewModel.RefreshPatientsCommand.Execute(null);
    }

    private async Task ContinueToHomePage() =>
        await Shell.Current.GoToAsync("//Home");
}
