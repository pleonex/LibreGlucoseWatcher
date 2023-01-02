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

## Compile

1. Install Garmin Connect IQ SDK and generate a developer license following
   [their instructions](https://developer.garmin.com/connect-iq/connect-iq-basics/getting-started/).
2. Open VS Code in the subfolder of the project to compile. A file
   `monkey.jungle` must be in the top-level workspace.
3. Run the VS Code command "_Build for Device_".
