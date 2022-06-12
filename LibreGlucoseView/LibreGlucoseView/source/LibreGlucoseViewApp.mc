import Toybox.Application;
import Toybox.Background;
import Toybox.Lang;
import Toybox.Time;
import Toybox.WatchUi;
import Toybox.System;

var SUGAR_DATA = "sugar_data";

(:background)
class LibreGlucoseViewApp extends Application.AppBase {

    var view;

    function initialize() {
        AppBase.initialize();
    }

    function getServiceDelegate() {
        return [ new LibreClientBackgroundService() ];
    }

    function onBackgroundData(data) {
        System.println("Received data from BG service: " + data);
        Application.getApp().setProperty(SUGAR_DATA, data);
        WatchUi.requestUpdate();
    }

    function getInitialView() as Array<Views or InputDelegates>? {
        Background.registerForTemporalEvent(new Time.Duration(5 * 60));

        view = new LibreGlucoseViewView();
        return [ view ] as Array<Views or InputDelegates>;
    }
}

function getApp() as LibreGlucoseViewApp {
    return Application.getApp() as LibreGlucoseViewApp;
}