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
