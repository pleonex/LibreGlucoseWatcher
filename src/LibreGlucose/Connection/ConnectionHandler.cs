using System.Net.Http.Json;
using System.Text.Json;

namespace PleOps.LibreGlucose.Connection;

public class ConnectionHandler
{
    private const string ApiUrl = "https://api-eu.libreview.io/";

    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonOptions;

    private AuthData? authData;

    internal ConnectionHandler(HttpClient client)
    {
        this.client = client;
        jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public AuthData AuthenticationData
    {
        get => authData ?? throw new InvalidOperationException("Application is not logged");
        set => authData = value;
    }

    public string Token => AuthenticationData.AuthTicket?.Token ?? throw new ArgumentNullException("Token is missing");

    public async Task LoginAsync(ConnectionParameters parameters)
    {
        string uri = ApiUrl + "auth/login";

        var result = await client.PostAsJsonAsync(uri, parameters, jsonOptions).ConfigureAwait(false);
        result.EnsureSuccessStatusCode();

        var content = await result.Content.ReadFromJsonAsync<LoginResult>(jsonOptions).ConfigureAwait(false);

        AuthenticationData = content?.Data ?? throw new InvalidOperationException("Invalid server login reply");
    }
}
