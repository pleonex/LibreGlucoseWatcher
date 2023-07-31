using System.Text.Json.Serialization;

namespace PleOps.LibreGlucose.Patients;

public record GlucoseGraphData
{
    public ActiveSensor[] ActiveSensors { get; set; } = Array.Empty<ActiveSensor>();

    [JsonPropertyName("connection")]
    public PatientInfo Patient { get; set; } = new();

    public GlucoseMeasurement[] GraphData { get; set; } = Array.Empty<GlucoseMeasurement>();
}

public record ActiveSensor
{
    public PatientDevice Device { get; set; } = new();

    public SensorInfo Sensor { get; set; } = new();
}
