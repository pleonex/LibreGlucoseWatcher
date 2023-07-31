using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.Logging;
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Patients;

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
    private string? patientName;

    [ObservableProperty]
    private string measurementTime = string.Empty;

    [ObservableProperty]
    private string previousMeasurementOverview = string.Empty;

    [ObservableProperty]
    private string fetchTime = string.Empty;

    [ObservableProperty]
    private ISeries[] graphData = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] graphXAxes = Array.Empty<Axis>();

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
            string patId = UserSettings.GetPatientId()!;

            var previous = currentMeasurement;
            var graphResult = await client.Patients.GetGraph(patId);
            currentMeasurement = graphResult.Data.Patient.GlucoseMeasurement;

            var patientInfo = graphResult.Data.Patient;
            PatientName = $"{patientInfo.FirstName} {patientInfo.LastName}";

            CurrentGlucose = currentMeasurement.ValueInMgPerDl.ToString();
            CurrentTrend = TrendToUnicode(currentMeasurement.TrendArrow);
            CurrentGlucoseColor = MeasurementColorToColor(currentMeasurement.MeasurementColor);

            var timestamp = currentMeasurement.UtcTimestamp;
            var now = DateTime.UtcNow;
            var diff = now - timestamp;
            MeasurementTime = (diff.TotalHours < 24)
                ? timestamp.ToLocalTime().ToShortTimeString()
                : $"{timestamp.ToLocalTime().ToShortDateString()} {timestamp.ToLocalTime().ToShortTimeString()}";

            if (previous is not null && previous.Timestamp != currentMeasurement.Timestamp)
            {
                PreviousMeasurementOverview = $"{previous.ValueInMgPerDl} mg/dL " +
                    $"({currentMeasurement.ValueInMgPerDl - previous.ValueInMgPerDl:+#;-#;0})\n" +
                    $"{previous.UtcTimestamp.ToShortTimeString()}";
            }

            FetchTime = $"@ {now.ToLocalTime().ToShortTimeString()}";

            // It seems they have a bug and the connection value is sooner than graph...
            var measurements = graphResult.Data.GraphData.Append(graphResult.Data.Patient.GlucoseMeasurement);

            GraphData = new ISeries[] {
                new LineSeries<int>
                {
                    Values = measurements.Select(m => m.ValueInMgPerDl),
                }
            };

            GraphXAxes = new Axis[] {
                new Axis
                {
                    Name = "Time",
                    Labels = measurements.Select(m => m.UtcTimestamp.ToLocalTime().ToShortTimeString()).ToList(),
                    
                },
            };
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
