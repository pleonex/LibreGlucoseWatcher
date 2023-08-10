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
import Toybox.Application.Storage;
import Toybox.Application.Properties;
import Toybox.Lang;
using Toybox.Time;

(:background)
module LibreGlucoseStorage
{
    var LOGIN_STATUS_PROPNAME = "lgc_login_status" as String;
    var CLIENT_TOKEN_PROPNAME = "lgc_token" as String;
    var REQUEST_STATUS_PROPNAME = "lgc_request_status" as String;
    var SUGAR_MGDL_LATEST_VALUE_PROPNAME = "lgc_sugar_mgdl_latest_value" as String;
    var SUGAR_MMOLL_LATEST_VALUE_PROPNAME = "lgc_sugar_mmoll_latest_value" as String;
    var SUGAR_LATEST_TIMESTAMP_PROPNAME = "lgc_sugar_latest_timestamp" as String;

    enum LoginStatus {
        LOGIN_IN_PROGRESS = 1,
        LOGIN_MISSING_SETTINGS = 2,
        LOGIN_INVALID = 3,
        LOGIN_SUCCESS = 4,
    }

    enum RequestStatus {
        REQUEST_SUCCESS = 1,
        REQUEST_INVALID_DATA = 2,
        REQUEST_INVALID_PATNUM = 3,
        REQUEST_IN_PROGRESS = 4,
    }

    enum SugarUnits {
        SUGAR_UNIT_MGDL = 0,
        SUGAR_UNIT_MMOLL = 1,
    }

    function setLoginStatus(status as LoginStatus) as Void {
        Storage.setValue(LOGIN_STATUS_PROPNAME, status);
    }
    function getLoginStatus() as LoginStatus {
        var status = Storage.getValue(LOGIN_STATUS_PROPNAME);
        if (status == null) {
            return LOGIN_IN_PROGRESS;
        }

        return status as LoginStatus;
    }

    function setToken(token as String) as Void {
        Storage.setValue(CLIENT_TOKEN_PROPNAME, token);
    }
    function getToken() as String or Null {
        return Storage.getValue(CLIENT_TOKEN_PROPNAME);
    }
    function deleteToken() as Void {
        Storage.deleteValue(CLIENT_TOKEN_PROPNAME);
    }

    function setRequestStatus(status as RequestStatus) as Void {
        Storage.setValue(REQUEST_STATUS_PROPNAME, status);
    }
    function getRequestStatus() as RequestStatus {
        var status = Storage.getValue(REQUEST_STATUS_PROPNAME);
        if (status == null) {
            return REQUEST_IN_PROGRESS;
        }

        return status as RequestStatus;
    }

    function setSugarValue(mgDl as String, mmolL as String) as Void {
        Storage.setValue(SUGAR_MGDL_LATEST_VALUE_PROPNAME, mgDl);
        Storage.setValue(SUGAR_MMOLL_LATEST_VALUE_PROPNAME, mmolL);

        var now = Time.now().value();
        Storage.setValue(SUGAR_LATEST_TIMESTAMP_PROPNAME, now);
    }
    function getSugarValue() as String {
        var units = getSettingUnits();
        switch (units) {
            case SUGAR_UNIT_MGDL:
                return Storage.getValue(SUGAR_MGDL_LATEST_VALUE_PROPNAME);
            case SUGAR_UNIT_MMOLL:
                return Storage.getValue(SUGAR_MMOLL_LATEST_VALUE_PROPNAME);
        }

        return "Unknown unit";
    }
    function getSugarTimestamp() as Number {
        return Storage.getValue(SUGAR_LATEST_TIMESTAMP_PROPNAME);
    }

    function getSettingUsername() as String {
        return Properties.getValue("username");
    }
    function getSettingPassword() as String {
        return Properties.getValue("password");
    }
    function getSettingPatNum() as Number {
        return Properties.getValue("patNum") as Number;
    }
    function getSettingUnits() as SugarUnits {
        return Properties.getValue("units") as SugarUnits;
    }
}
