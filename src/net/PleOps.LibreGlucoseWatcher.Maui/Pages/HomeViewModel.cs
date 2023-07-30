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

    private const int FetchTimerMs = 1 * 60 * 1000;

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

    [ObservableProperty]
    private string fetchTime = string.Empty;

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

            var latestMeasurement = await client.Patients.GetConnections();
            var patientData = latestMeasurement.Data[0];
            currentMeasurement = patientData.GlucoseMeasurement;

            CurrentGlucose = currentMeasurement.ValueInMgPerDl.ToString();
            CurrentTrend = TrendToUnicode(currentMeasurement.TrendArrow);
            CurrentGlucoseColor = MeasurementColorToColor(currentMeasurement.MeasurementColor);
            PatientName = $"{patientData.FirstName} {patientData.LastName}";

            var timestamp = ParseMeasurementTimestamp(currentMeasurement.Timestamp);
            var now = DateTime.UtcNow;
            var diff = now - timestamp;
            MeasurementTime = (diff.TotalHours < 24)
                ? timestamp.ToLocalTime().ToShortTimeString()
                : $"{timestamp.ToLocalTime().ToShortDateString()} {timestamp.ToLocalTime().ToShortTimeString()}";

            if (previous is not null && previous.Timestamp != currentMeasurement.Timestamp)
            {
                var prevTimestamp = ParseMeasurementTimestamp(previous.Timestamp);
                PreviousMeasurementOverview = $"{previous.ValueInMgPerDl} mg/dL " +
                    $"({currentMeasurement.ValueInMgPerDl - previous.ValueInMgPerDl:+#;-#;0})\n" +
                    $"{prevTimestamp.ToShortTimeString()}";
            }

            FetchTime = $"Last check: {now.ToLocalTime().ToShortTimeString()}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch patient data");
        }
    }

    private static string TrendToUnicode(TrendArrow tendency) =>
        tendency switch
        {
            TrendArrow.DecreasingRapidly => ArrowDown,
            TrendArrow.Decreasing => ArrowDown45,
            TrendArrow.Stable => ArrowRight,
            TrendArrow.Increasing => ArrowUp45,
            TrendArrow.IncreasingRapidly => ArrowUp,
            _ => "?",
        };

    private static Color MeasurementColorToColor(MeasurementColor color) =>
        color switch
        {
            MeasurementColor.InRange => Colors.Green,
            MeasurementColor.OutsideRange => Colors.DarkOrange,
            MeasurementColor.HighAlarm => Colors.OrangeRed,
            MeasurementColor.LowAlarm => Colors.OrangeRed,
            _ => Colors.Gray,
        };

    private static DateTime ParseMeasurementTimestamp(string timestampText)
    {
        DateTime timestamp = DateTime.ParseExact(
                timestampText,
                GlucoseMeasurement.TimeStampFormat,
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
