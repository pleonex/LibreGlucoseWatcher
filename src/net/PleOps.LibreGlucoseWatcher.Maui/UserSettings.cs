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
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Connection;
using System.Text.Json;

namespace PleOps.LibreGlucoseWatcher.Maui;

internal static class UserSettings
{
    private static string AuthTokenKey => "auth_token";
    private static string PatientIdKey => "patient_id";
    private static string UnitsKey => "units_format";

    public static async Task<AuthData?> GetAuthDataAsync()
    {
        try
        {
            string authToken = await SecureStorage.Default.GetAsync(AuthTokenKey)
                .ConfigureAwait(true);
            if (authToken is null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<AuthData>(authToken);
        }
        catch (Exception)
        {
            _ = SecureStorage.Default.Remove(AuthTokenKey);
            throw;
        }
    }

    public static async Task SetAuthDataAsync(AuthData authData)
    {
        string authJson = JsonSerializer.Serialize(authData);
        await SecureStorage.Default.SetAsync(AuthTokenKey, authJson);
    }

    public static string? GetPatientId()
        => Preferences.Default.Get<string?>(PatientIdKey, null);

    public static void SetPatientId(string patientId)
        => Preferences.Default.Set(PatientIdKey, patientId);

    public static GlucoseUnit GetUnitFormat()
        => (GlucoseUnit)Preferences.Default.Get(UnitsKey, (int)GlucoseUnit.MgDl);

    public static void SetUnitFormat(GlucoseUnit unit)
        => Preferences.Default.Set(UnitsKey, (int)unit);
}
