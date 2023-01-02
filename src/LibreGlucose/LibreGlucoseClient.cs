using PleOps.LibreGlucose.Connection;

namespace PleOps.LibreGlucose;

public class LibreGlucoseClient
{
    private readonly HttpClient client;

    public LibreGlucoseClient()
    {
        client = new HttpClient();

        Connection = new ConnectionHandler(client);
    }

    public ConnectionHandler Connection { get; }
}
