using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Patients;
using System.Diagnostics.Metrics;
using System.Globalization;

namespace PleOps.LibreGlucoseWatcher.Maui.Pages;

public partial class HomeViewModel : ObservableObject, IDisposable
{
    private const string ArrowUp = "\u2191";
    private const string ArrowUp45 = "\u2197";
    private const string ArrowRight = "\u2192";
    private const string ArrowDown45 = "\u2198";
    private const string ArrowDown = "\u2193";

    private const int FetchTimerMs = 5 * 60 * 1000;

    private readonly LibreGlucoseClient client;
    private readonly ILogger<HomeViewModel> logger;
    private readonly Timer periodicFetchTimer;

    private GlucoseMeasurement? currentMeasurement;

    [ObservableProperty]
    private string currentGlucose = string.Empty;

    [ObservableProperty]
    private string currentTrend = string.Empty;

    [ObservableProperty]
    private Color currentGlucoseColor = Colors.Gray;

    [ObservableProperty]
    private string unitText = "mg/dL";

    [ObservableProperty]
    private string patientName = "Unknown";

    [ObservableProperty]
    private string measurementTime = string.Empty;

    [ObservableProperty]
    private string previousMeasurementOverview = string.Empty;

    public HomeViewModel(LibreGlucoseClient client, ILogger<HomeViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(logger);

        this.client = client;
        this.logger = logger;
        periodicFetchTimer = new Timer(
            async (_) => await FetchGlucose(),
            null,
            0,
            FetchTimerMs);
    }

    [RelayCommand]
    private async Task FetchGlucose()
    {
        try
        {
            var previous = currentMeasurement;

            var latestMeasurement = await client.Patients.Get();
            var patientData = latestMeasurement.Data[0];
            currentMeasurement = patientData.GlucoseMeasurement;

            CurrentGlucose = currentMeasurement.ValueInMgPerDl.ToString();
            CurrentTrend = TrendToUnicode(currentMeasurement.TrendArrow);
            CurrentGlucoseColor = MeasurementColorToColor(currentMeasurement.MeasurementColor);
            PatientName = $"{patientData.FirstName} {patientData.LastName}";

            var timestamp = ParseMeasurementTimestamp(currentMeasurement.Timestamp);
            var diff = DateTime.UtcNow - timestamp;
            MeasurementTime = (diff.TotalHours < 24)
                ? timestamp.ToShortTimeString()
                : $"{timestamp.ToShortDateString()} {timestamp.ToShortTimeString()}";

            if (previous is not null)
            {
                var prevTimestamp = ParseMeasurementTimestamp(previous.Timestamp);
                PreviousMeasurementOverview = $"{previous.ValueInMgPerDl} mg/dL " +
                    $"({currentMeasurement.ValueInMgPerDl - previous.ValueInMgPerDl:+#;-#;0})\n" +
                    $"{prevTimestamp.ToShortTimeString()}";
            }
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
            2 => Colors.DarkOrange,
            3 => Colors.OrangeRed,
            _ => Colors.Gray,
        };

    private static DateTime ParseMeasurementTimestamp(string timestampText)
    {
        DateTime timestamp = DateTime.ParseExact(
                timestampText,
                "M/d/yyyy h:m:s tt",
                CultureInfo.InvariantCulture);
        var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        
        return TimeZoneInfo.ConvertTimeToUtc(timestamp, sourceTimeZone);
    }

    public void Dispose()
    {
        try
        {
            periodicFetchTimer.Dispose();
        }
        catch (Exception)
        {
        }

        GC.SuppressFinalize(this);
    }
}
