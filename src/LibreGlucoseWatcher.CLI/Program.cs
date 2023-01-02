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
            client.Connection.AuthenticationData = authData;
            logged = true;
            AnsiConsole.MarkupLine("[green]Valid token[/]");
        }
    }

    if (!logged)
    {
        await Login().ConfigureAwait(false);
        var authData = client.Connection.AuthenticationData;
        await AuthFileEncryption.WriteToken(authData).ConfigureAwait(false);
    }
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
    var parameters = new ConnectionParameters(email, password);

    await AnsiConsole.Status()
        .StartAsync(
            "Login...",
            async ctx => await client.Connection.LoginAsync(parameters).ConfigureAwait(false))
        .ConfigureAwait(false);
    AnsiConsole.MarkupLine("[green]Succeed :check_mark:[/]");
}
