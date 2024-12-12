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
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace PleOps.LibreGlucose.Connection;

// APIs:
// POST llu/auth/login: login with email and password
// POST llu/auth/register + data payload with account info: register new account
// GET auth/continue: register step 2 to validate email
// POST auth/continue/{type}: email validation?
// POST auth/resendVerifyEmail
// POST auth/forgotpassword + { email }
// POST auth/changepassword + { oldpassword, password }
// GET llu/legacytoken + header { UserToken }
// POST llu/auth/deleteaccount + { email, password }
public class LoginHandler
{
    private readonly HttpClient client;
    private AuthData? authData;

    internal LoginHandler(HttpClient client)
    {
        this.client = client;
    }

    public AuthData? AuthenticationData => authData;

    public async Task LoginAsync(LoginParameters parameters)
    {
        string uri = "llu/auth/login";

        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Remove("Account-Id");
        var result = await client.PostAsJsonAsync(uri, parameters).ConfigureAwait(false);
        _ = result.EnsureSuccessStatusCode();

        var content = await result.Content.ReadFromJsonAsync<LoginResult>().ConfigureAwait(false);

        AuthData newAuth = content?.Data ?? throw new InvalidOperationException("Invalid server login reply");
        SetAuthentication(newAuth);
    }

    public void SetAuthentication(AuthData newAuthentication)
    {
        if (newAuthentication?.AuthTicket?.Token is null) {
            throw new Exception("Empty authentication token");
        }

        authData = newAuthentication;

        string token = newAuthentication.AuthTicket.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Starting 4.11: account-id
        JsonWebToken jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);
        string jwtId = jwt.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? throw new NotSupportedException("Missing token ID");
        byte[] jwtIdBinary = Encoding.ASCII.GetBytes(jwtId);
        byte[] jwtIdHash = SHA256.HashData(jwtIdBinary);
        string hexJwtIdHash = BitConverter.ToString(jwtIdHash).Replace("-", "");
        client.DefaultRequestHeaders.Add("Account-Id", hexJwtIdHash);
    }
}
