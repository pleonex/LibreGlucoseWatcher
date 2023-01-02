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

    var patients = await client.Patients.Get().ConfigureAwait(false);
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
