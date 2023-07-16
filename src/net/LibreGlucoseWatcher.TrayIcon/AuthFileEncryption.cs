using PleOps.LibreGlucose.Connection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace PleOps.LibreGlucoseWatcher.TrayIcon;

public static class AuthFileEncryption
{
    public static string AuthPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath!)!, ".auth");

    public static AuthData? ReadToken()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            File.Decrypt(AuthPath);
        }

        string authJson;
        try
        {
            authJson = File.ReadAllText(AuthPath);
        }
        finally
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                File.Encrypt(AuthPath);
            }
        }

        var authData = JsonSerializer.Deserialize<AuthData>(authJson);

        if (authData?.AuthTicket?.Expires is not null)
        {
            var expiration = DateTimeOffset.FromUnixTimeSeconds(authData.AuthTicket.Expires)
                .AddMilliseconds(authData.AuthTicket.Duration);
            if (DateTimeOffset.UtcNow > expiration)
            {
                return null;
            }
        }

        return authData;
    }

    public static void WriteToken(AuthData authData)
    {
        string authJson = JsonSerializer.Serialize(authData);
        File.WriteAllText(AuthPath, authJson);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            File.Encrypt(AuthPath);
        }
    }
}
