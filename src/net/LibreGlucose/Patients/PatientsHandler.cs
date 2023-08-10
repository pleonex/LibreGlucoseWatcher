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
