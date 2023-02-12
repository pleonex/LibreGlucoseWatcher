// Copyright(c) 2022 Benito Palacios Sanchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
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
