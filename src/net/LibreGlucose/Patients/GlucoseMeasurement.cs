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
