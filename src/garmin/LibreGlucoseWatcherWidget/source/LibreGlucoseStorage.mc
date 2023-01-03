import Toybox.Application.Storage;
import Toybox.Application.Properties;
import Toybox.Lang;

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
    }

    enum SugarUnits {
        SUGAR_UNIT_MGDL = 0,
        SUGAR_UNIT_MMOLL = 1,
    }

    function setLoginStatus(status as LoginStatus) as Void {
        Storage.setValue(LOGIN_STATUS_PROPNAME, status);
    }
    function getLoginStatus() as LoginStatus {
        return Storage.getValue(LOGIN_STATUS_PROPNAME) as LoginStatus;
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
        return Storage.getValue(REQUEST_STATUS_PROPNAME) as RequestStatus;
    }

    function setSugarValue(mgDl as String, mmolL as String) as Void {
        Storage.setValue(SUGAR_MGDL_LATEST_VALUE_PROPNAME, mgDl);
        Storage.setValue(SUGAR_MMOLL_LATEST_VALUE_PROPNAME, mmolL);
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
