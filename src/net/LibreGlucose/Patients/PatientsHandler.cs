using System.Net.Http.Json;

namespace PleOps.LibreGlucose.Patients;

public class PatientsHandler
{
    private readonly HttpClient client;

    public PatientsHandler(HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        this.client = client;
    }

    public async Task<QueryResult<PatientInfo[]>> GetConnections()
    {
        string uri = "llu/connections";

        return await client.GetFromJsonAsync<QueryResult<PatientInfo[]>>(uri).ConfigureAwait(false)
            ?? throw new FormatException("Invalid server reply");
    }

        var result = await client.GetFromJsonAsync<PatientsResult>(uri).ConfigureAwait(false);
        if (result is null)
        {
            throw new FormatException("Invalid server reply");
        }

        return result;
    }
}
