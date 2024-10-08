﻿// Copyright (C) 2023  Benito Palacios Sánchez
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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel;
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
    private readonly RectangularSection selectedDateRegion;

    private IList<GlucoseMeasurement>? currentGraphData;
    private GlucoseMeasurement? currentMeasurement;

    private DateTime selectedDate;
    private TimeSpan selectedTime;

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
    private string fetchTime = string.Empty;

    [ObservableProperty]
    private ISeries[] graphData = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] graphXAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private Axis[] graphYAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private RectangularSection[] graphSections = Array.Empty<RectangularSection>();

    [ObservableProperty]
    private string sensorEndsText = string.Empty;

    [ObservableProperty]
    private DateTime firstValueDateTime = DateTime.Now;

    [ObservableProperty]
    private DateTime lastValueDateTime = DateTime.Now;

    [ObservableProperty]
    private string selectedGlucoseText = string.Empty;

    public HomeViewModel(LibreGlucoseClient client, ILogger<HomeViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(logger);

        this.client = client;
        this.logger = logger;

        var now = DateTime.Now;
        selectedDateRegion = new RectangularSection {
            Xi = DateTime.Now.Ticks,
            Xj = DateTime.Now.Ticks,
            Fill = null,
            Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 1 },
            IsVisible = true,
        };
        selectedDate = DateTime.Today;
        selectedTime = now.TimeOfDay;

        periodicFetchTimer = new Timer(
            async (_) => await FetchGlucose(),
            null,
            0,
            FetchTimerMs);
    }

    public DateTime SelectedDate {
        get => selectedDate;
        set {
            var date = value.Date;
            if (SetProperty(ref selectedDate, date)) {
                UpdateSelectedRegion();
            }
        }
    }

    public TimeSpan SelectedTime {
        get => selectedTime;
        set {
            var time = new TimeSpan(value.Hours, value.Minutes, 0);
            if (SetProperty(ref selectedTime, time)) {
                UpdateSelectedRegion();
            }
        }
    }

    private void UpdateSelectedRegion()
    {
        long selectedTicks = (SelectedDate + SelectedTime).Ticks;
        selectedDateRegion.Xi = selectedTicks;
        selectedDateRegion.Xj = selectedTicks;

        var glucose = currentGraphData?.FirstOrDefault(m => m.UtcTimestamp.ToLocalTime().Ticks >= selectedTicks);
        SelectedGlucoseText = glucose is null
            ? string.Empty
            : $"Selected: {glucose.ValueInMgPerDl} mg/dL";

        if ((GraphXAxes.Length > 0) && ((selectedTicks > GraphXAxes[0].MaxLimit) || (selectedTicks < GraphXAxes[0].MinLimit))) {
            GraphXAxes[0].MinLimit = selectedTicks - TimeSpan.FromHours(5).Ticks;
            GraphXAxes[0].MaxLimit = selectedTicks + TimeSpan.FromMinutes(30).Ticks;
        }
    }

    [RelayCommand]
    private void BackwardSelectedTime()
    {
        if (SelectedTime.TotalMinutes < 5) {
            int diff = 5 - (int)SelectedTime.TotalMinutes;
            SelectedDate = SelectedDate.AddDays(-1);
            SelectedTime = new TimeSpan(23, 60 - diff, 0);
        } else {
            SelectedTime -= TimeSpan.FromMinutes(5);
        }
    }

    [RelayCommand]
    private void ForwardSelectedTime()
    {
        if (SelectedTime.TotalMinutes > ((24 * 60) - 5)) {
            int diff = 5 - (int)SelectedTime.TotalMinutes;
            SelectedDate = SelectedDate.AddDays(1);
            SelectedTime = new TimeSpan(0, diff, 0);
        } else {
            SelectedTime += TimeSpan.FromMinutes(5);
        }
    }

    [RelayCommand]
    private void GoToNow()
    {
        if (currentGraphData is null) {
            return;
        }

        SelectedTime = currentGraphData[^1].UtcTimestamp.TimeOfDay;
        SelectedDate = currentGraphData[^1].UtcTimestamp;
    }

    [RelayCommand]
    private void ChartPointSelected(ChartPoint? point)
    {
        if (point is null) {
            return;
        }

        var time = point.Coordinate.SecondaryValue.AsDate();
        SelectedTime = time.TimeOfDay;
        SelectedDate = time.Date;
    }

    [RelayCommand]
    private async Task FetchGlucose()
    {
        try
        {
            // TODO: wow, this needs a good refactor!
            string patId = UserSettings.GetPatientId()!;

            var previous = currentMeasurement;
            var graphResult = await client.Patients.GetGraph(patId);
            currentMeasurement = graphResult.Data.Patient.GlucoseMeasurement;

            var patientInfo = graphResult.Data.Patient;
            PatientName = $"{patientInfo.FirstName} {patientInfo.LastName}";

            var timestamp = currentMeasurement.UtcTimestamp;
            var now = DateTime.UtcNow;
            var diff = now - timestamp;

            CurrentGlucose = currentMeasurement.ValueInMgPerDl.ToString();
            CurrentTrend = TrendToUnicode(currentMeasurement.TrendArrow);

            var color = (diff.TotalMinutes > 15) ? MeasurementColor.Unknown : currentMeasurement.MeasurementColor;
            CurrentGlucoseColor = MeasurementColorToColor(color);

            MeasurementTime = (diff.TotalHours < 24)
                ? timestamp.ToLocalTime().ToShortTimeString()
                : $"{timestamp.ToLocalTime().ToShortDateString()} {timestamp.ToLocalTime().ToShortTimeString()}";

            FetchTime = $"Refreshed {now.ToLocalTime().ToShortTimeString()}";

            var sensorEnd = patientInfo.Sensor!.StartDate + TimeSpan.FromDays(14);
            var sensorRem = sensorEnd - DateTimeOffset.UtcNow;
            string sensorRemText = (sensorRem.TotalHours > 24)
                ? $"{(int)sensorRem.TotalDays} days"
                : $"{(int)sensorRem.TotalHours} hours";

            string sensorEndDateText = sensorEnd.ToLocalTime().ToString("dd.MM HH:mm");
            SensorEndsText = $"Sensor ends in {sensorRemText} @ {sensorEndDateText}";

            bool hasChanged = previous is not null && previous.Timestamp != currentMeasurement.Timestamp;
            bool isFirst = previous is null;
            if (!hasChanged && !isFirst) {
                return;
            }

            var measurements = graphResult.Data.GraphData;

            // It seems they have a bug and the connection value is sooner than graph...
            var lastGlucPatient = graphResult.Data.Patient.GlucoseMeasurement;
            var lastGlucGraph = graphResult.Data.GraphData[^1];
            if (lastGlucPatient.UtcTimestamp > lastGlucGraph.UtcTimestamp) {
                measurements = graphResult.Data.GraphData.Append(graphResult.Data.Patient.GlucoseMeasurement).ToArray();
            }

            currentGraphData = measurements;

            if (isFirst) {
                SelectedTime = measurements[^1].UtcTimestamp.ToLocalTime().TimeOfDay;
            }

            GraphData = new ISeries[] {
                new LineSeries<GlucoseMeasurement>
                {
                    Values = measurements,
                    Stroke = new SolidColorPaint(new SKColor(33, 150, 242)) { StrokeThickness = 4 },
                    Fill = null,
                    GeometryFill = null,
                    GeometryStroke = null,
                    LineSmoothness = 0,
                    // For some reason the type DateTimePoint breaks the tooltips, workaround use a mapper.
                    Mapping = (measurement, point) =>
                        new LiveChartsCore.Kernel.Coordinate(
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
                    MinLimit = Math.Min(measurements.Min(x => x.ValueInMgPerDl), 54),
                    MaxLimit = Math.Max(measurements.Max(x => x.ValueInMgPerDl), 252),
                },
            };

            GraphSections = new[] {
                selectedDateRegion,
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

            FirstValueDateTime = measurements[0].UtcTimestamp;
            LastValueDateTime = measurements[^1].UtcTimestamp;
        } catch (HttpRequestException ex) when (ex.StatusCode is System.Net.HttpStatusCode.Unauthorized) {
            logger.LogError(ex, "Auth required again");
            await MainThread.InvokeOnMainThreadAsync(async () => {
                await Shell.Current.DisplayAlert("Connection expired", "Please login again", "Ok");
                await Shell.Current.GoToAsync("//Login");
            });
        } catch (Exception ex) {
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
