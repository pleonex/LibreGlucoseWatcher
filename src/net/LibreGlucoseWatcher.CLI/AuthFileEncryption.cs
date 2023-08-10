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

        string authJson;
        try
        {
            authJson = await File.ReadAllTextAsync(AuthPath).ConfigureAwait(false);
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
