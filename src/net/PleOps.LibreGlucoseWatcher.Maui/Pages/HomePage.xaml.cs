namespace PleOps.LibreGlucoseWatcher.Maui.Pages;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }

    internal HomeViewModel ViewModel => (BindingContext as HomeViewModel)!;
}