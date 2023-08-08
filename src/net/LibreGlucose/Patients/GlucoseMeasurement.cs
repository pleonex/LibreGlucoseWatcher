using System.Globalization;
using System.Text.Json.Serialization;

namespace PleOps.LibreGlucose.Patients;

public record GlucoseMeasurement
{
    private DateTime? utcTimestamp;

    public static string TimeStampFormat => "M/d/yyyy h:m:s tt";

    /// <summary>
    /// UTC date and time of the measurement in string representation.
    /// </summary>
    public string FactoryTimestamp { get; set; } = string.Empty;

    /// <summary>
    /// Device local time of the measurement in string representation.
    /// </summary>
    public string Timestamp { get; set; } = string.Empty;

    [JsonIgnore]
    public DateTime UtcTimestamp => utcTimestamp ??= DateTime.ParseExact(
                FactoryTimestamp,
                TimeStampFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal);

    public MeasurementType Type { get; set; }
    public int ValueInMgPerDl { get; set; }
    public TrendArrow TrendArrow { get; set; }
    public string? TrendMessage { get; set; }
    public MeasurementColor MeasurementColor { get; set; }
    public GlucoseUnit GlucoseUnits { get; set; }
    public float Value { get; set; }
    public bool IsHigh { get; set; }
    public bool IsLow { get; set; }

    public MeasurementAlarmKind AlarmType { get; set; }
}
