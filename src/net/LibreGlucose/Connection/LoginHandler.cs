using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PleOps.LibreGlucose.Connection;

public class LoginHandler
{
    private readonly HttpClient client;

    private AuthData? authData;

    internal LoginHandler(HttpClient client)
    {
        this.client = client;
    }

    public AuthData AuthenticationData
    {
        get => authData ?? throw new InvalidOperationException("Application is not logged");
        set
        {
            authData = value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }
    }

    public string Token => AuthenticationData.AuthTicket?.Token ?? throw new ArgumentNullException("Token is missing");

    public async Task LoginAsync(LoginParameters parameters)
    {
        string uri = LibreGlucoseClient.ApiUrl + "llu/auth/login";

        var result = await client.PostAsJsonAsync(uri, parameters).ConfigureAwait(false);
        result.EnsureSuccessStatusCode();

        var content = await result.Content.ReadFromJsonAsync<LoginResult>().ConfigureAwait(false);

        AuthenticationData = content?.Data ?? throw new InvalidOperationException("Invalid server login reply");
    }
}
