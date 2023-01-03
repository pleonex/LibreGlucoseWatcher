# Libre Glucose Watcher

> **Not for treatment decisions**  
> The information presented in these apps should not be used for treatment or
> dosing decisions. Consult the glucose-monitoring system and/or a healthcare
> professional.

Applications to monitor glucose levels _FreeStyle Libre_ sensors (via
_LibreLinkUp_ app connection).

Current support:

- Garmin watches: simple widget
- .NET library + console application

`#WeAreNotWaiting`

## Installation

This is a _personal use **at your own risk**_ application. For that reason, it's
not published to the Connect IQ store. If you want to play and give it a try,
you will need to perform a manual installation as follow:

1. Follow the [compile](#compile) steps.
   - You may want to add compatibility to your product. Usually it would be just
     adding your watch brand in the `manifest.xml` file by running the command
     _Monkey C: Edit Products_.
2. Connect your Garmin device into your computer.
3. Copy the generated `*.prg` file into the Garmin device folder `GARMIN/APPS`.
4. Due to a limitation in Garmin, we can't use the setting UI from Garmin
   Connect to set the user name and password (see
   [issue](https://forums.garmin.com/developer/connect-iq/f/discussion/2121/modifying-settings-on-side-loaded-app)).
   Follow this step instead:
   1. Open the project with VS Code.
   2. Open any \*.mc file.
   3. Go to the tab "_Run and Debug_" and click its button. It will compile and
      run the simulator.
   4. Set your email and password in the simulator from _File > Edit Persistent
      Storage > Edit Application.Properties data_
   5. Copy the setting file from the simulator (temp folder, in Windows at
      `%TEMP%\com.garmin.connectiq\GARMIN\APPS\SETTINGS`) into your Garmin
      device: `/garmin/apps/settings`.

## Compile

1. Install Garmin Connect IQ SDK and generate a developer license following
   [their instructions](https://developer.garmin.com/connect-iq/connect-iq-basics/getting-started/).
2. Open VS Code in the subfolder of the project to compile. A file
   `monkey.jungle` must be in the top-level workspace.
3. Run the VS Code command "_Build for Device_".

## TODO

Garmin:

- Update data field app
- Create barrel
- Refactor into client + background class
- Display timestamp of last value
- Display arrow of graph
- Display message if value is quite old (> 5 min)
- Alert when it reaches threshold low and high
- Show data with colors
- Add graph like Dexcom app

Desktop:

- Integrate with Oh My Posh
- Show windows popup notifications
- Create system tray icon
