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
import Toybox.Application;
import Toybox.Background;
import Toybox.Lang;
import Toybox.Time;
import Toybox.WatchUi;
import Toybox.System;

(:background)
class LibreGlucoseViewApp extends Application.AppBase {

    function initialize() {
        AppBase.initialize();
    }

    function getInitialView() as Array<Views or InputDelegates>? {
        Background.registerForTemporalEvent(new Time.Duration(5 * 60));
        return [ new LibreGlucoseViewView() ] as Array<Views or InputDelegates>;
    }

    function getServiceDelegate() {
        return [ new LibreClientBackgroundService() ];
    }

    function onSettingsChanged() {
        WatchUi.requestUpdate();
    }

    function onBackgroundData(data) {
        WatchUi.requestUpdate();
    }
}

function getApp() as LibreGlucoseViewApp {
    return Application.getApp() as LibreGlucoseViewApp;
}
