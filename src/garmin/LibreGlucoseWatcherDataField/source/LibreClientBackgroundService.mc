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
import Toybox.System;
import Toybox.Communications;
import Toybox.Lang;

(:background)
var tries = 0;

(:background)
class LibreClientBackgroundService extends System.ServiceDelegate
{
    private var libreLinkUpUrl = "https://api-eu.libreview.io/";
    private var userAgent = "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Mobile Safari/537.36";
    private var libreLinkUpVersion = "4.7.0";
    private var libreLinkUpProduct = "llu.android";

    function initialize() {
        ServiceDelegate.initialize();
    }

    function onTemporalEvent() {
        var token = LibreGlucoseStorage.getToken();
        if (token == null || token.length() == 0) {
            loginAndQuery();
        } else {
            query(token);
        }
    }

    function onReceivedConnectionInfo(responseCode as Number, data as Dictionary or Null or String) as Void {
        System.println("Query response: " + responseCode);
        if (responseCode == 200 && data != null) {
            $.tries = 0;

            var resultData = data["data"] as Array<Object> or Null;
            if (resultData == null) {
                LibreGlucoseStorage.setRequestStatus(LibreGlucoseStorage.REQUEST_INVALID_DATA);
                Background.exit(null);
                return;
            }

            var patNum = LibreGlucoseStorage.getSettingPatNum();
            if (resultData.size() <= patNum) {
                LibreGlucoseStorage.setRequestStatus(LibreGlucoseStorage.REQUEST_INVALID_PATNUM);
                Background.exit(null);
                return;
            }

            var measurement = resultData[patNum]["glucoseMeasurement"];
            if (measurement == null) {
                LibreGlucoseStorage.setRequestStatus(LibreGlucoseStorage.REQUEST_INVALID_DATA);
                Background.exit(null);
                return;
            }

            var sugarMgDl = measurement["ValueInMgPerDl"];
            var sugarMmolL = measurement["Value"];
            LibreGlucoseStorage.setSugarValue(sugarMgDl, sugarMmolL);

            LibreGlucoseStorage.setRequestStatus(LibreGlucoseStorage.REQUEST_SUCCESS);
            Background.exit(null);
        } else if (responseCode == 401) {
            LibreGlucoseStorage.deleteToken();
            loginAndQuery();
        } else {
            LibreGlucoseStorage.setRequestStatus(LibreGlucoseStorage.REQUEST_INVALID_DATA);
            if ($.tries < 5) {
                System.println("Retrying " + $.tries);
                query(LibreGlucoseStorage.getToken());
            }
        }
    }

    function query(token as String) as Void {
        $.tries = $.tries + 1;
        System.println("Querying... " + $.tries);
        LibreGlucoseStorage.setRequestStatus(LibreGlucoseStorage.REQUEST_IN_PROGRESS);

        var url = libreLinkUpUrl + "llu/connections";
        var options = {
            :method => Communications.HTTP_REQUEST_METHOD_GET,
            :headers => {
                "User-Agent" => userAgent,
                "Content-Type" => Communications.REQUEST_CONTENT_TYPE_JSON,
                "version" => libreLinkUpVersion,
                "product" => libreLinkUpProduct,
                "Authorization" => "Bearer " + token
            },
            :responseType => Communications.HTTP_RESPONSE_CONTENT_TYPE_JSON
        };

        Communications.makeWebRequest(url, null, options, method(:onReceivedConnectionInfo));
    }

    function onReceivedLogin(responseCode as Number, data as Dictionary or Null or String) as Void {
        System.println("Login response: " + responseCode);
        if (responseCode == 200 and data["data"] != null and data["data"]["authTicket"] != null) {
            var token = data["data"]["authTicket"]["token"];
            LibreGlucoseStorage.setToken(token);
            LibreGlucoseStorage.setLoginStatus(LibreGlucoseStorage.LOGIN_SUCCESS);

            query(token);
        } else {
            LibreGlucoseStorage.setLoginStatus(LibreGlucoseStorage.LOGIN_INVALID);
            Background.exit(null);
        }
    }

    public function loginAndQuery() as Void {
        $.tries = 0;
        LibreGlucoseStorage.setLoginStatus(LibreGlucoseStorage.LOGIN_IN_PROGRESS);
        System.println("Login...");

        var url = libreLinkUpUrl + "llu/auth/login";
        var options = {
            :method => Communications.HTTP_REQUEST_METHOD_POST,
            :headers => {
                "User-Agent" => userAgent,
                "Content-Type" => Communications.REQUEST_CONTENT_TYPE_JSON,
                "version" => libreLinkUpVersion,
                "product" => libreLinkUpProduct,
            },
            :responseType => Communications.HTTP_RESPONSE_CONTENT_TYPE_JSON
        };

        var username = LibreGlucoseStorage.getSettingUsername();
        var pwd = LibreGlucoseStorage.getSettingPassword();
        if (username == null or username.length() == 0 or pwd == null or pwd.length() == 0) {
            LibreGlucoseStorage.setLoginStatus(LibreGlucoseStorage.LOGIN_MISSING_SETTINGS);
            return;
        }

        var params = {
            "email" => username,
            "password" => pwd
        };
        Communications.makeWebRequest(url, params, options, method(:onReceivedLogin));
    }
}
