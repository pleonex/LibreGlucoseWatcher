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
import Toybox.Graphics;
import Toybox.WatchUi;
import Toybox.System;
import Toybox.Lang;
import Toybox.Time.Gregorian;

class WidgetView extends WatchUi.View {

    function initialize() {
        View.initialize();
    }

    // Load your resources here
    function onLayout(dc as Dc) as Void {
        setLayout(Rez.Layouts.MainLayout(dc));
    }

    // Called when this View is brought to the foreground. Restore
    // the state of this View and prepare it to be shown. This includes
    // loading resources into memory.
    function onShow() as Void {
    }

    // Update the view
    function onUpdate(dc as Dc) as Void {
        // Call the parent onUpdate function to redraw the layout
        View.onUpdate(dc);
        System.println("Updating layout");

        var loginStatus = LibreGlucoseStorage.getLoginStatus();
        switch (loginStatus) {
            case LibreGlucoseStorage.LOGIN_MISSING_SETTINGS:
                drawMessage(dc, "Missing login\nSet in app settings\nWait 5 min to retry");
                break;

            case LibreGlucoseStorage.LOGIN_IN_PROGRESS:
                drawMessage(dc, "Login...");
                break;

            case LibreGlucoseStorage.LOGIN_INVALID:
                drawMessage(dc, "Invalid email\nor password");
                break;

            case LibreGlucoseStorage.LOGIN_SUCCESS:
                drawSugar(dc);
                break;
        }
    }

    function drawMessage(dc as Dc, text as String) as Void {
        dc.setColor(Graphics.COLOR_WHITE, Graphics.COLOR_TRANSPARENT);
        dc.drawText(
            dc.getWidth() / 2,
            dc.getHeight() / 2,
            Graphics.FONT_MEDIUM,
            text,
            (Graphics.TEXT_JUSTIFY_CENTER | Graphics.TEXT_JUSTIFY_VCENTER));
    }

    function drawSugar(dc as Dc) as Void {
        var requestStatus = LibreGlucoseStorage.getRequestStatus();
        switch (requestStatus) {
            case LibreGlucoseStorage.REQUEST_INVALID_DATA:
                drawMessage(dc, "Error:\nInvalid data");
                break;
            case LibreGlucoseStorage.REQUEST_INVALID_PATNUM:
                drawMessage(dc, "Invalid patient\nnumber. Review\napp settings");
                break;

            case LibreGlucoseStorage.REQUEST_IN_PROGRESS:
                drawMessage(dc, "Fetching sugar");
                break;

            case LibreGlucoseStorage.REQUEST_SUCCESS:
                var sugar = LibreGlucoseStorage.getSugarValue();
                var text = "Glucose:\n" + sugar;

                var units = LibreGlucoseStorage.getSettingUnits();
                if (units == LibreGlucoseStorage.SUGAR_UNIT_MGDL) {
                    text += " mg/dL";
                } else {
                    text += " mmol/L";
                }

                var time = LibreGlucoseStorage.getSugarTimestamp();
                var infoTime = Gregorian.info(new Time.Moment(time), Time.FORMAT_SHORT);

                var diff = new Time.Moment(Time.now().value() - time);
                var infoDiff = Gregorian.info(diff, Time.FORMAT_SHORT);
                text += "\n@" + infoTime.hour + ":" + infoTime.min + " (+" + infoDiff.min + " min)";

                drawMessage(dc, text);
                break;
        }
    }

    // Called when this View is removed from the screen. Save the
    // state of this View here. This includes freeing resources from
    // memory.
    function onHide() as Void {
    }
}
