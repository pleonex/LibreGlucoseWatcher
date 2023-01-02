using System.Text.Json.Serialization;
using PleOps.LibreGlucose.Connection;

namespace PleOps.LibreGlucose.Patiens;

public record PatientsResult
{
    public int Status { get; init; }

    public Datum[] Data { get; init; } = Array.Empty<Datum>();

    public TicketInfo Ticket { get; init; }
}

public record Datum
{
    public string Id { get; set; }
    public string PatientId { get; set; }
    public string Country { get; set; }
    public int Status { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int TargetLow { get; set; }
    public int TargetHigh { get; set; }

    [JsonPropertyName("uom")]
    public int MeasureUnits { get; set; }

    public SensorInfo Sensor { get; set; }
    public AlarmRulesInfo AlarmRules { get; set; }
    public GlucoseMeasurement GlucoseMeasurement { get; set; }
    public GlucoseMeasurement GlucoseItem { get; set; }
    public PatientDevice PatientDevice { get; set; }
    public long Created { get; set; }
}

public record SensorInfo
{
    public string DeviceId { get; set; }

    [JsonPropertyName("sn")]
    public string SerialNumber { get; set; }

    public int a { get; set; }
    public int w { get; set; }
    public int pt { get; set; }
    public bool s { get; set; }
    public bool lj { get; set; }
}

public record AlarmRulesInfo
{
    public bool c { get; set; }
    public H h { get; set; }
    public F f { get; set; }
    public L l { get; set; }
    public Nd nd { get; set; }
    public int p { get; set; }
    public int r { get; set; }
}

public record H
{
    public int th { get; set; }
    public float thmm { get; set; }
    public int d { get; set; }
    public float f { get; set; }
}

public record F
{
    public int th { get; set; }
    public int thmm { get; set; }
    public int d { get; set; }
    public int tl { get; set; }
    public float tlmm { get; set; }
}

public record L
{
    public int th { get; set; }
    public float thmm { get; set; }
    public int d { get; set; }
    public int tl { get; set; }
    public float tlmm { get; set; }
}

public record Nd
{
    public int i { get; set; }
    public int r { get; set; }
    public int l { get; set; }
}

public record GlucoseMeasurement
{
    public string FactoryTimestamp { get; set; }
    public string Timestamp { get; set; }
    public int Type { get; set; }
    public int ValueInMgPerDl { get; set; }
    public int TrendArrow { get; set; }
    public object TrendMessage { get; set; }
    public int MeasurementColor { get; set; }
    public int GlucoseUnits { get; set; }
    public float Value { get; set; }
    public bool IsHigh { get; set; }
    public bool IsLow { get; set; }
}

public record PatientDevice
{
    public string did { get; set; }
    public int dtid { get; set; }

    [JsonPropertyName("v")]
    public string Version { get; set; }

    public bool l { get; set; }

    [JsonPropertyName("ll")]
    public int LowAlarmThreshold { get; set; }

    [JsonPropertyName("hl")]
    public int HighAlarmThreshold { get; set; }

    public int u { get; set; }
    public FixedLowAlarmValue fixedLowAlarmValues { get; set; }
    public bool Alarms { get; set; }
    public int FixedLowThreshold { get; set; }
}

public record FixedLowAlarmValue
{
    public int mgdl { get; set; }
    public float mmoll { get; set; }
}
