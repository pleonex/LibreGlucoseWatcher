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
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Connection;
using PleOps.LibreGlucoseWatcher.CLI;
using Spectre.Console;

var client = new LibreGlucoseClient();

try
{
    bool logged = false;
    if (File.Exists(AuthFileEncryption.AuthPath))
    {
        var authData = await AuthFileEncryption.ReadToken().ConfigureAwait(false);
        if (authData is null)
        {
            AnsiConsole.MarkupLine("[red]Invalid token file[/]");
            File.Delete(AuthFileEncryption.AuthPath);
        }
        else
        {
            client.Login.AuthenticationData = authData;
            logged = true;
            AnsiConsole.MarkupLine("[green]Valid token[/]");
        }
    }

    if (!logged)
    {
        await Login().ConfigureAwait(false);
        var authData = client.Login.AuthenticationData;
        await AuthFileEncryption.WriteToken(authData).ConfigureAwait(false);
    }

    var patients = await client.Patients.GetConnections().ConfigureAwait(false);
    AnsiConsole.MarkupLine("[green]Glucose: {0} mg/dL[/]", patients.Data[0].GlucoseMeasurement.ValueInMgPerDl);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}

async Task Login()
{
    AnsiConsole.Write(new Rule("[green]Login[/]"));

    string email = AnsiConsole.Prompt(
        new TextPrompt<string>("LibreView [yellow]email[/]:")
            .PromptStyle("red"));
    string password = AnsiConsole.Prompt(
        new TextPrompt<string>("LibreView [yellow]password[/]:")
            .PromptStyle("red")
            .Secret());
    var parameters = new LoginParameters(email, password);

    await AnsiConsole.Status()
        .StartAsync(
            "Login...",
            async ctx => await client.Login.LoginAsync(parameters).ConfigureAwait(false))
        .ConfigureAwait(false);
    AnsiConsole.MarkupLine("[green]Succeed :check_mark:[/]");
}
