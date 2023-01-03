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
import Toybox.Graphics;
import Toybox.WatchUi;
import Toybox.System;
import Toybox.Lang;

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

            case LibreGlucoseStorage.REQUEST_SUCCESS:
                var sugar = LibreGlucoseStorage.getSugarValue();
                var text = "Glucose:\n" + sugar;

                var units = LibreGlucoseStorage.getSettingUnits();
                if (units == LibreGlucoseStorage.SUGAR_UNIT_MGDL) {
                    text += " mg/dL";
                } else {
                    text += " mmol/L";
                }

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
