using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Logging;
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Patients;
using SkiaSharp;

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

    [ObservableProperty]
    private Axis[] graphYAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private RectangularSection[] graphSections = Array.Empty<RectangularSection>();

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
                new LineSeries<GlucoseMeasurement>
                {
                    // For some reason the type DateTimePoint breaks the tooltips, workaround use a mapper.
                    //Values = measurements.Select(m => new DateTimePoint(m.UtcTimestamp, m.ValueInMgPerDl)),
                    Values = measurements,
                    Stroke = new SolidColorPaint(new SKColor(33, 150, 242)) { StrokeThickness = 4 },
                    Fill = null,
                    GeometryFill = null,
                    GeometryStroke = null,
                    LineSmoothness = 0,
                    Mapping = (measurement, point) =>
                        point.Coordinate = new LiveChartsCore.Kernel.Coordinate(
                            measurement.UtcTimestamp.Ticks,
                            measurement.ValueInMgPerDl),
                }
            };

            GraphXAxes = new Axis[] {
                new Axis
                {
                    Name = string.Empty, // takes too much space and it's obvious
                    Labeler = tick => tick.AsDate().ToString("HH:mm"),
                    UnitWidth = TimeSpan.FromMinutes(1).Ticks,
                    MinLimit = (DateTime.Now - TimeSpan.FromHours(5)).Ticks,
                    MaxLimit = (DateTime.Now + TimeSpan.FromMinutes(30)).Ticks,
                },
            };

            GraphYAxes = new Axis[] {
                new Axis {
                    MinLimit = 18,
                    MaxLimit = Math.Max(measurements.Max(x => x.ValueInMgPerDl), 306),
                },
            };

            GraphSections = new[] {
                new RectangularSection {
                    Yi = patientInfo.TargetHigh,
                    Yj = patientInfo.TargetLow,
                    Fill = new SolidColorPaint(SKColors.LightGreen.WithAlpha(204)),
                },
                new RectangularSection {
                    Yi = patientInfo.AlarmRules.High.Threshold,
                    Yj = patientInfo.AlarmRules.High.Threshold,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 1 },
                },
                new RectangularSection {
                    Yi = patientInfo.AlarmRules.Low.Threshold,
                    Yj = patientInfo.AlarmRules.Low.Threshold,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 1 },
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
