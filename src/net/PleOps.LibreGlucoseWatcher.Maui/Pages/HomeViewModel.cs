using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PleOps.LibreGlucose;

namespace PleOps.LibreGlucoseWatcher.Maui.Pages;

public partial class HomeViewModel : ObservableObject
{
    private const string ArrowUp = "\u2191";
    private const string ArrowUp45 = "\u2197";
    private const string ArrowRight = "\u2192";
    private const string ArrowDown45 = "\u2198";
    private const string ArrowDown = "\u2193";

    private readonly LibreGlucoseClient client;
    private readonly ILogger<HomeViewModel> logger;

    [ObservableProperty]
    private string currentBloodLevel = string.Empty;

    [ObservableProperty]
    private string currentTrend = string.Empty;

    [ObservableProperty]
    private Color currentGlucoseColor = Colors.Gray;

    [ObservableProperty]
    private string unitText = "mg/dL";

    public HomeViewModel(LibreGlucoseClient client, ILogger<HomeViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(logger);

        this.client = client;
        this.logger = logger;
    }

    [RelayCommand]
    private async Task FetchGlucose()
    {
        try
        {
            var latestMeasurement = await client.Patients.Get();
            var patientMeasurement = latestMeasurement.Data[0].GlucoseMeasurement;
            CurrentBloodLevel = patientMeasurement.ValueInMgPerDl.ToString();
            CurrentTrend = TrendToUnicode(patientMeasurement.TrendArrow);
            CurrentGlucoseColor = MeasurementColorToColor(patientMeasurement.MeasurementColor);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch patient data");
        }
    }

    private static string TrendToUnicode(int tendency) =>
        tendency switch
        {
            1 => ArrowDown,
            2 => ArrowDown45,
            3 => ArrowRight,
            4 => ArrowUp45,
            5 => ArrowUp,
            _ => "?",
        };

    private static Color MeasurementColorToColor(int color) =>
        color switch
        {
            1 => Colors.Green,
            2 => Colors.Orange,
            3 => Colors.Red,
            _ => Colors.Gray,
        };
}
