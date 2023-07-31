using System.Text.Json.Serialization;

namespace PleOps.LibreGlucose.Patients;

public record PatientInfo
{
    /// <summary>
    /// Gets or sets an (unknown) GUID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the GUID to use in further API queries.
    /// </summary>
    public string PatientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the two chars country code of the person.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    // TODO: Unknown
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name (maybe more than one).
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the combination of first and last names.
    /// </summary>
    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Gets or sets the patient configured target low for reports (not alarms).
    /// </summary>
    public int TargetLow { get; set; }

    /// <summary>
    /// Gets or sets the patient configured target high for reports (not alarms).
    /// </summary>
    public int TargetHigh { get; set; }

    [JsonPropertyName("uom")] // Units Of Measurement?
    public GlucoseUnit MeasureUnits { get; set; }

    /// <summary>
    /// Gets or sets the current sensor information.
    /// </summary>
    public SensorInfo? Sensor { get; set; }

    public AlarmRulesInfo AlarmRules { get; set; } = new();
    public GlucoseMeasurement GlucoseMeasurement { get; set; } = new();
    public GlucoseMeasurement GlucoseItem { get; set; } = new();
    public PatientDevice PatientDevice { get; set; } = new();
    public long Created { get; set; }
}

public record SensorInfo
{
    /// <summary>
    /// Gets or sets the ID of a physical reading device (not phone). TBC.
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the sensor ID (ending with 0).
    /// </summary>
    [JsonPropertyName("sn")]
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Unix epoch date (in seconds) when the sensor started.
    /// </summary>
    [JsonPropertyName("a")]
    public int StartEpoch { get; set; }

    /// <summary>
    /// Gets the date when the sensor started.
    /// </summary>
    public DateTimeOffset StartDate => DateTimeOffset.FromUnixTimeSeconds(StartEpoch);

    // TODO: wait time minutes 60?.
    public int w { get; set; }

    // TODO: small number (4?)
    public int pt { get; set; }

    // TODO: false?
    public bool s { get; set; }

    // TODO: false?
    public bool lj { get; set; }
}

public record AlarmRulesInfo
{
    // TBC: configured?
    public bool c { get; set; }

    /// <summary>
    /// Gets or sets the rule for the user-configured alarm for high glucose.
    /// </summary>
    [JsonPropertyName("h")]
    public AlarmRule High { get; set; } = new();

    /// <summary>
    /// Gets or sets the rule for the fixed low alarm.
    /// </summary>
    [JsonPropertyName("f")]
    public AlarmRule FixedLow { get; set; } = new();

    /// <summary>
    /// Gets or sets the rule for the user-configured alarm for low glucose.
    /// </summary>
    [JsonPropertyName("l")]
    public AlarmRule Low { get; set; } = new();

    /// <summary>
    /// Gets or sets the rule for the alarm when there isn't recent data.
    /// </summary>
    [JsonPropertyName("nd")]
    public NoRecentDataAlarmRule NoRecentData { get; set; } = new();

    // TBC: 5?
    public int p { get; set; }

    // TBC: 5?
    public int r { get; set; }
}

public record AlarmRule
{
    [JsonPropertyName("on")]
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the alarm threshold in mg/dL units.
    /// </summary>
    [JsonPropertyName("th")]
    public int Threshold { get; set; }

    /// <summary>
    /// Gets or sets the alarm threshold in mmol/L units.
    /// </summary>
    [JsonPropertyName("thmm")]
    public float ThresholdMmolL { get; set; }

    // TBC: 1440 or 6?
    public int d { get; set; }

    // TBC: only in high 0.1?
    public float? f { get; set; }

    // TBC: f and l, 10, when to repeat the alarm?
    public int? tl { get; set; }

    // TBC: f and l, tl / 18
    public float? tlmm { get; set; }
}


public record NoRecentDataAlarmRule
{
    /// <summary>
    /// Gets or sets the number of minutes without data to trigger the alarm.
    /// </summary>
    [JsonPropertyName("i")]
    public int Interval { get; set; }

    // TBC: 5? increments of the interval?
    public int r { get; set; }

    // TBC: 6?
    public int l { get; set; }
}

public enum MeasurementType
{
    Normal = 0,
    IncludeTrend = 1,
    Unknown = 2,
    FromAlarm = 3,
}

public enum TrendArrow
{
    NotAvailable = 0,
    DecreasingRapidly = 1,
    Decreasing = 2,
    Stable = 3,
    Increasing = 4,
    IncreasingRapidly = 5,
}

public enum MeasurementColor
{
    Unknown = 0,
    InRange = 1,
    OutsideRange = 2,
    HighAlarm = 3,
    LowAlarm = 4,
}

public enum MeasurementAlarmKind
{
    NotDefined = 0,
    LowGlucose = 1,
    HighGlucose = 2,
}

public record PatientDevice
{
    [JsonPropertyName("did")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("dtid")]
    public int dtid { get; set; }

    [JsonPropertyName("v")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the capture (Libre app) low alarm in the user units.
    /// </summary>
    [JsonPropertyName("ll")]
    public float LowAlarmThreshold { get; set; }

    /// <summary>
    /// Gets or sets the capture (Libre app) high alarm in the user units.
    /// </summary>
    [JsonPropertyName("hl")]
    public float HighAlarmThreshold { get; set; }

    [JsonPropertyName("fixedLowAlarmValues")]
    public FixedLowAlarmValue FixedLowAlarm { get; set; } = new();

    /// <summary>
    /// Gets or sets an Unix epoch timestamp.
    /// </summary>
    [JsonPropertyName("u")]
    public int UpdateTimestamp { get; set; }

    // Not used anymore?
    public bool Alarms { get; set; }

    // Not used anymore?
    public int FixedLowThreshold { get; set; }
}

public record FixedLowAlarmValue
{
    [JsonPropertyName("mgdl")]
    public int ThresholdMgDl { get; set; }

    [JsonPropertyName("mmoll")]
    public float ThresholdMmolL { get; set; }
}
