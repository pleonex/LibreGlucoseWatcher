using PleOps.LibreGlucose.Connection;
using PleOps.LibreGlucose.Patiens;
using System.Net.Http.Headers;

namespace PleOps.LibreGlucose;

public class LibreGlucoseClient
{
    internal const string ApiUrl = "https://api-eu.libreview.io/";
    internal const string ApiProduct = "llu.ios";
    internal const string ApiVersion = "4.1.1";

    private readonly HttpClient client;

    public LibreGlucoseClient()
    {
        client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("version", ApiVersion);
        client.DefaultRequestHeaders.Add("product", ApiProduct);

        Login = new LoginHandler(client);
        Patients = new PatientsHandler(client);
    }

    public LoginHandler Login { get; }

    public PatientsHandler Patients { get; }
}
