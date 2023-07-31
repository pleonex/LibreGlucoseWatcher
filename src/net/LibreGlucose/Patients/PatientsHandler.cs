using System.Net.Http.Json;

namespace PleOps.LibreGlucose.Patients;

// APIs:
// GET user: account info
// GET user/dismissMessage/{msgId}
// GET account
// POST llu/user/updateaccount + { account info }
// POST llu/alarm/dismiss + { alarmType, patientId, updated }
// GET  llu/config/country + query { country, version }
// (no token) GET llu/config/base
// (no token) GET llu/config/regulatory + query { country, language, version }
// GET llu/connections: get connections and their info
// DELETE llu/connections/{id}: remove connection
// GET llu/invitations/receiver/{accountId}: get invitations
// POST llu/invitations/accept/{invitationId}: accept invitation
// DELETE sharing/llu/invitations/{invitationId}: reject invitation
// GET llu/connections/{patientId}/graph: get last 14 hours of glucose
// GET llu/connections/{patientId}/logbook: get last 15 days of alarm glucose values
// GET llu/notification/settings/{id}:
// POST llu/notification/savesettings + { connectionId, alarmRules }
// POST llu/notification/storepushtoken + { deviceId, token, type }
// DELETE llu/notification/deletepushtoken + { token }
// GET document/{docId}?lang={lang}: get EULA and privacy documents (IDs from config)
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

    public async Task<QueryResult<GlucoseGraphData>> GetGraph(string patientId)
    {
        string uri = $"llu/connections/{patientId}/graph";

        return await client.GetFromJsonAsync<QueryResult<GlucoseGraphData>>(uri).ConfigureAwait(false)
            ?? throw new FormatException("Invalid server reply");
    }

    public async Task<QueryResult<GlucoseMeasurement[]>> GetLogbook(string patientId)
    {
        string uri = $"llu/connections/{patientId}/logbook";

        return await client.GetFromJsonAsync<QueryResult<GlucoseMeasurement[]>>(uri).ConfigureAwait(false)
            ?? throw new FormatException("Invalid server reply");
    }
}
