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
import Toybox.Activity;
import Toybox.Application;
import Toybox.Lang;
import Toybox.Time;
import Toybox.WatchUi;

class LibreGlucoseViewView extends WatchUi.SimpleDataField {

    public function initialize() {
        SimpleDataField.initialize();
        label = "Glucose";
    }

    function compute(info as Activity.Info) as Numeric or Duration or String or Null {
        var sugar = LibreGlucoseStorage.getSugarValue();
        if (sugar == null) {
            sugar = "?";
        }

        var loginStatus = LibreGlucoseStorage.getLoginStatus();
        switch (loginStatus) {
            case LibreGlucoseStorage.LOGIN_MISSING_SETTINGS:
                return sugar + " (error #1)";
            case LibreGlucoseStorage.LOGIN_IN_PROGRESS:
                return sugar + " (progress #1)";
            case LibreGlucoseStorage.LOGIN_INVALID:
                return sugar + " (error #2)";

            case LibreGlucoseStorage.LOGIN_SUCCESS:
                return getSugar();
        }

        return sugar;
    }

    function getSugar() as String {
        var sugar = LibreGlucoseStorage.getSugarValue();
        if (sugar == null) {
            sugar = "?";
        }

        var requestStatus = LibreGlucoseStorage.getRequestStatus();
        switch (requestStatus) {
            case LibreGlucoseStorage.REQUEST_INVALID_DATA:
                return sugar + " (error #3)";
            case LibreGlucoseStorage.REQUEST_INVALID_PATNUM:
                return sugar + " (error #4)";
            case LibreGlucoseStorage.REQUEST_IN_PROGRESS:
                return sugar + " (progress #2)";

            case LibreGlucoseStorage.REQUEST_SUCCESS:
                var units = LibreGlucoseStorage.getSettingUnits();
                if (units == LibreGlucoseStorage.SUGAR_UNIT_MGDL) {
                    sugar += " mg/dL";
                } else {
                    sugar += " mmol/L";
                }

                // var time = LibreGlucoseStorage.getSugarTimestamp();
                // var infoTime = Gregorian.info(new Time.Moment(time), Time.FORMAT_SHORT);

                // var diff = new Time.Moment(Time.now().value() - time);
                // var infoDiff = Gregorian.info(diff, Time.FORMAT_SHORT);
                // text += "\n@" + infoTime.hour + ":" + infoTime.min + " (+" + infoDiff.min + " min)";

                return sugar;
        }

        return sugar;
    }
}
