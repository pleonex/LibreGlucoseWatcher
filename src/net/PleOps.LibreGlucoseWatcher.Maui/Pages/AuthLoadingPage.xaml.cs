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
