namespace PleOps.LibreGlucose.Connection;

public record LoginResult(int? Status, AuthData? Data);

public record AuthData(TwoFactorAuthInfo? Step, UserInfo? User, TicketInfo? AuthTicket);

public record TwoFactorAuthInfo(string? Type, string? ComponentName, TwoFactorAuthParameters Props, string? Title);

public record TwoFactorAuthParameters(
    string? AccountType,
    string? PrimaryMethod,
    string? PrimaryValue,
    string? SecondaryMethod,
    string? SecondaryValue);

public record UserInfo(string? AccountType, string? Country, string? UiLanguage);

public record TicketInfo(string? Token, long Expires, long Duration);
