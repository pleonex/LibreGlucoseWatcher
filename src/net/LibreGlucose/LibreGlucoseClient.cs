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
using PleOps.LibreGlucose.Patients;
using System.Net.Http.Headers;

namespace PleOps.LibreGlucose;

public class LibreGlucoseClient
{
    internal const string ApiUrl = "https://api-eu.libreview.io/";
    internal const string ApiProduct = "llu.android";
    internal const string ApiVersion = "4.7.0"; // 4.11 requires sending Account-Id.

    private readonly HttpClient client;

    public LibreGlucoseClient()
    {
        client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("version", ApiVersion);
        client.DefaultRequestHeaders.Add("product", ApiProduct);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Mobile Safari/537.36");

        client.BaseAddress = new Uri(ApiUrl);

        Login = new LoginHandler(client);
        Patients = new PatientsHandler(client);
    }

    public LoginHandler Login { get; }

    public PatientsHandler Patients { get; }
}
