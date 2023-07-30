using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Patients;
using PleOps.LibreGlucoseWatcher.Maui.Mvvm;

namespace PleOps.LibreGlucoseWatcher.Maui.Pages;

public partial class InitialSetupViewModel : ObservableObject
{
    private static readonly List<(GlucoseUnit Unit, string Name)> unitsNameMap = new() {
        (GlucoseUnit.MgDl, "mg / dL" ),
        (GlucoseUnit.MmolL, "mmol / L"),
    };

    private readonly LibreGlucoseClient client;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAndContinueCommand))]
    private int selectedUnitIdx;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveAndContinueCommand))]
    private PatientInfo? selectedPatient;

    [ObservableProperty]
    private IEnumerable<PatientInfo> patients;

    public InitialSetupViewModel(LibreGlucoseClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        this.client = client;

        SelectedUnitIdx = 0;
        patients = Array.Empty<PatientInfo>();
        SettingsSaved = new AsyncInteraction();
    }

    public IList<string> GlucoseUnits { get; } = unitsNameMap.Select(x => x.Name).ToList();

    public AsyncInteraction SettingsSaved { get; }



    [RelayCommand]
    private async Task RefreshPatientsAsync()
    {
        var result = await client.Patients.GetConnections();
        Patients = result.Data;
        SelectedPatient = null;
    }

    [RelayCommand(CanExecute = nameof(CanSaveSettings))]
    private async Task SaveAndContinue()
    {
        var units = unitsNameMap[SelectedUnitIdx].Unit;
        UserSettings.SetUnitFormat(units);

        string patientId = SelectedPatient!.PatientId;
        UserSettings.SetPatientId(patientId);

        await SettingsSaved.HandleAsync();
    }

    private bool CanSaveSettings() =>
        SelectedUnitIdx >= 0 && SelectedUnitIdx < unitsNameMap.Count
        && (SelectedPatient is not null);
}
