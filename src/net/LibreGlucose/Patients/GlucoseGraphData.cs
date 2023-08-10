// Copyright (C) 2023  Benito Palacios Sánchez
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
