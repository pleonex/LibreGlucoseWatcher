using PleOps.LibreGlucose.Connection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace PleOps.LibreGlucoseWatcher.CLI;

public static class AuthFileEncryption
{
    public static string AuthPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath!)!, ".auth");

    public static async Task<AuthData?> ReadToken()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            File.Decrypt(AuthPath);
        }

        string authJson = await File.ReadAllTextAsync(AuthPath).ConfigureAwait(false);
        var authData = JsonSerializer.Deserialize<AuthData>(authJson);

        if (authData?.AuthTicket?.Expires is not null)
        {
            var expiration = DateTimeOffset.FromUnixTimeSeconds(authData.AuthTicket.Expires)
                .AddSeconds(authData.AuthTicket.Duration);
            if (DateTimeOffset.UtcNow > expiration)
            {
                return null;
            }
        }

        return authData;
    }

    public static async Task WriteToken(AuthData authData)
    {
        string authJson = JsonSerializer.Serialize(authData);
        await File.WriteAllTextAsync(AuthPath, authJson).ConfigureAwait(false);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            File.Encrypt(AuthPath);
        }
    }
}
