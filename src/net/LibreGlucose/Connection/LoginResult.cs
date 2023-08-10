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
