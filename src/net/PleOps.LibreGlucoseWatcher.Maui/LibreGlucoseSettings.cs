using PleOps.LibreGlucose.Connection;
using System.Text.Json;

namespace PleOps.LibreGlucoseWatcher.Maui;

internal static class LibreGlucoseSettings
{
    public static string AuthTokenKey => "auth_token";

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
            SecureStorage.Default.Remove(AuthTokenKey);
            throw;
        }
    }

    public static async Task SetAuthDataAsync(AuthData authData)
    {
        string authJson = JsonSerializer.Serialize(authData);
        await SecureStorage.Default.SetAsync(AuthTokenKey, authJson);
    }
}
