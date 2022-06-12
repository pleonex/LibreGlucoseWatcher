import Toybox.Activity;
import Toybox.Application;
import Toybox.Lang;
import Toybox.Time;
import Toybox.WatchUi;

class LibreGlucoseViewView extends WatchUi.SimpleDataField {

    public function initialize() {
        SimpleDataField.initialize();
        label = "Glucose (mg/dL)";
    }

    function compute(info as Activity.Info) as Numeric or Duration or String or Null {
        var sugar = Application.getApp().getProperty(SUGAR_DATA);
        if (sugar == null) {
            return "?";
        } else {
            return sugar;
        }
    }
}
