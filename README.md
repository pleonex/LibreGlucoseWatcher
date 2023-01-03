# Libre Glucose Watcher

> **Not for treatment decisions**  
> The information presented in these apps should not be used for treatment or
> dosing decisions. Consult the glucose-monitoring system and/or a healthcare
> professional.

Applications for Garmin sport watches to monitoring glucose levels from
_FreeStyle Libre_ sensors (via _LibreLinkUp_ app connection).

## Installation

This is a _personal use **at your own risk**_ application. For that reason, it's
not published to the Connect IQ store. If you want to play and give it a try,
you will need to perform a manual installation as follow:

1. Follow the [compile](#compile) steps.
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
