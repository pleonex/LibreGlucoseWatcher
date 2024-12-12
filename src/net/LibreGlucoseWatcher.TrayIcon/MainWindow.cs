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
using Microsoft.Toolkit.Uwp.Notifications;
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Connection;
using PleOps.LibreGlucose.Patients;
using System.Globalization;
using System.Media;

namespace PleOps.LibreGlucoseWatcher.TrayIcon;

public partial class MainWindow : Form
{
    private readonly LibreGlucoseClient client;
    private readonly SoundPlayer soundPlayer;
    private PatientInfo[]? lastMeasurement;
    private bool wantToClose = false;

    public MainWindow()
    {
        client = new LibreGlucoseClient();
        soundPlayer = new SoundPlayer();

        InitializeComponent();
        refreshGlucoseTimer.Interval = (int)boxRefreshPeriod.Value * 60 * 1000;

        ReadLoginToken();

        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
    }

    private void ReadLoginToken()
    {
        bool logged = false;
        if (File.Exists(AuthFileEncryption.AuthPath))
        {
            var authData = AuthFileEncryption.ReadToken();
            if (authData is null)
            {
                labelToken.Text = "Invalid token file - Re-enter login details";
                UpdateTrayIcon("?", "", MeasurementColor.Unknown, "Login first");
                File.Delete(AuthFileEncryption.AuthPath);
            } else
            {
                client.Login.SetAuthentication(authData);
                logged = true;
                labelToken.Text = "Valid token - No need details";
            }
        } else
        {
            labelToken.Text = "Missing token - Enter login details.";
            UpdateTrayIcon("?", "", MeasurementColor.Unknown, "Login first");
        }

        if (logged)
        {
            btnCheck.Enabled = true;
            refreshGlucoseTimer.Start();
        }
    }

    private void LoginTextChanged(object sender, EventArgs e)
    {
        btnLogin.Enabled = (textBoxEmail.Text.Length > 0) &&
            (textBoxPassword.Text.Length > 0);
    }

    private async void LoginClicked(object sender, EventArgs e)
    {
        try
        {
            var parameters = new LoginParameters(textBoxEmail.Text, textBoxPassword.Text);
            await client.Login.LoginAsync(parameters).ConfigureAwait(true);

            var authData = client.Login.AuthenticationData;
            AuthFileEncryption.WriteToken(authData);
            labelToken.Text = "New token created";

            await FetchGlucose().ConfigureAwait(true);
            DisplayGlucose();

            refreshGlucoseTimer.Start();
            btnCheck.Enabled = true;
        } catch (Exception ex)
        {
            labelGlucose.Text = ex.Message;
            Show();
            ShowInTaskbar = true;
        }
    }

    private async Task FetchGlucose()
    {
        var patients = await client.Patients.GetConnections().ConfigureAwait(false);
        lastMeasurement = patients.Data;
    }

    private void DisplayGlucose()
    {
        if (lastMeasurement is null)
        {
            return;
        }

        if (boxPatientId.Value >= lastMeasurement.Length)
        {
            labelGlucose.Text = $"Invalid patient ID. Maximum: {lastMeasurement.Length - 1}";
            UpdateTrayIcon("?", "", MeasurementColor.Unknown, "Invalid settings");
            return;
        }

        var patientData = lastMeasurement[(int)boxPatientId.Value];
        var measurement = patientData.GlucoseMeasurement;
        (var glucose, string units) = radioButtonUnitMgDl.Checked
            ? (measurement.ValueInMgPerDl, "mg/dL")
            : (measurement.Value, "mmol/L");
        string arrowText = measurement.TrendArrow switch
        {
            TrendArrow.DecreasingRapidly => "↓↓",
            TrendArrow.Decreasing => "↓",
            TrendArrow.Stable => "→",
            TrendArrow.Increasing => "↑",
            TrendArrow.IncreasingRapidly => "↑↑",
            _ => "?",
        };
        DateTime timestamp = DateTime.ParseExact(
            measurement.Timestamp,
            "M/d/yyyy h:m:s tt",
            CultureInfo.InvariantCulture);
        var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        timestamp = TimeZoneInfo.ConvertTimeToUtc(timestamp, sourceTimeZone);
        var diff = DateTime.UtcNow - timestamp;

        labelGlucose.Text = $"{glucose} {units} {arrowText} @ {(int)diff.TotalMinutes} minutes ago";

        UpdateTrayIcon($"{glucose} {arrowText}", units, measurement.MeasurementColor, measurement.Timestamp);

        if (radioButtonUnitMgDl.Checked)
        {
            labelHighThresholdLlu.Text = patientData.TargetHigh.ToString();
            labelLowThresholdLlu.Text = patientData.TargetLow.ToString();
        } else
        {
            labelHighThresholdLlu.Text = (patientData.TargetHigh / 18.0).ToString("F1");
            labelLowThresholdLlu.Text = (patientData.TargetLow / 18.0).ToString("F1");
        }

        bool isHigh = glucose >= (float)boxHighThreshold.Value;
        bool isLow = glucose <= (float)boxLowThreshold.Value;
        if (isHigh || isLow)
        {
            if (checkShowNotification.Checked)
            {
                new ToastContentBuilder()
                    .AddText("Glucose over threshold!")
                    .AddText($"{glucose} {units} {arrowText}")
                    .AddText(measurement.Timestamp)
                    .AddCustomTimeStamp(timestamp)
                    .Show(toast =>
                    {
                        toast.Tag = "23946";
                    });
            }

            // if (checkPlayMusic.Checked)
            // {
            //     soundPlayer.Play();
            // }
        }
    }

    private void SettingChanged(object sender, EventArgs e)
    {
        if (sender == radioButtonUnitMgDl || sender == radioButtonUnitMmolL)
        {
            decimal oldHighValue = boxHighThreshold.Value;
            decimal oldLowValue = boxLowThreshold.Value;
            if (radioButtonUnitMgDl.Checked)
            {
                boxHighThreshold.DecimalPlaces = 0;
                boxHighThreshold.Maximum *= 18.0m;
                boxHighThreshold.Minimum *= 18.0m;
                boxHighThreshold.Value = oldHighValue * 18.0m;

                boxLowThreshold.DecimalPlaces = 0;
                boxLowThreshold.Minimum *= 18.0m;
                boxLowThreshold.Maximum *= 18.0m;
                boxLowThreshold.Value = oldLowValue * 18.0m;
            } else
            {
                boxHighThreshold.DecimalPlaces = 1;
                boxHighThreshold.Minimum /= 18.0m;
                boxHighThreshold.Maximum /= 18.0m;
                boxHighThreshold.Value = oldHighValue / 18.0m;

                boxLowThreshold.DecimalPlaces = 1;
                boxLowThreshold.Minimum /= 18.0m;
                boxLowThreshold.Maximum /= 18.0m;
                boxLowThreshold.Value = oldLowValue / 18.0m;
            }
        }

        DisplayGlucose();
    }

    private void UpdateTrayIcon(string glucose, string units, MeasurementColor colorId, string timestamp)
    {
        string text = glucose.Split(' ')[0];//.Replace(" ", "\n");
        var size = new Size(64, 64);
        int padding = 10;

        Font font;
        SizeF textSize;
        int fontSize = 23;
        do {
            font = new Font(FontFamily.GenericSansSerif, fontSize);

            using var dummyImage = new Bitmap(256, 256);
            using var measurementGraphic = Graphics.FromImage(dummyImage);
            textSize = measurementGraphic.MeasureString(text, font);
            fontSize--;
        } while (textSize.Width > size.Width + padding || textSize.Height > size.Height + padding);

        var image = new Bitmap(size.Width, size.Height);
        var graphic = Graphics.FromImage(image);

        Brush color = colorId switch
        {
            MeasurementColor.InRange => Brushes.LightGreen,
            MeasurementColor.OutsideRange => Brushes.Orange,
            MeasurementColor.HighAlarm => Brushes.Red,
            MeasurementColor.LowAlarm => Brushes.Red,
            _ => Brushes.White,
        };
        graphic.FillRectangle(color, 0, 0, image.Width, image.Height);

        graphic.DrawString(text, font, Brushes.Black, (size.Width - textSize.Width) / 2, (size.Height - textSize.Height) / 2);

        var icon = Icon.FromHandle(image.GetHicon());
        trayIcon.Icon = icon;
        trayIcon.Text = $"{glucose} {units} @ {timestamp}";

        Icon = icon;
    }

    private void TrayIconExitClicked(object sender, EventArgs e)
    {
        wantToClose = true;
        Close();
    }

    private void TrayIconOpenSettingClicked(object sender, EventArgs e)
    {
        Show();
        ShowInTaskbar = true;
        // soundPlayer.Stop();
    }

    private void MainWindowResizing(object sender, EventArgs e)
    {
        // if (WindowState is FormWindowState.Normal)
        // {
        //     soundPlayer.Stop();
        // }
    }

    private void TrayIconMouseDoubleClick(object sender, MouseEventArgs e)
    {
        Show();
        ShowInTaskbar = true;
        // soundPlayer.Stop();
    }

    private async void RefreshGlucoseTimerTick(object sender, EventArgs e)
    {
        if (client.Login.AuthenticationData == null)
        {
            return;
        }

        try
        {
            await FetchGlucose().ConfigureAwait(false);
            this.Invoke(() => DisplayGlucose());
        } catch (Exception ex)
        {
            labelGlucose.Invoke(() => labelGlucose.Text = ex.Message);

            Show();
            ShowInTaskbar = true;
        }
    }

    private void BoxRefreshPeriodValueChanged(object sender, EventArgs e)
    {
        refreshGlucoseTimer.Interval = (int)boxRefreshPeriod.Value * 60 * 1000;
    }

    private async void MainWindowLoad(object sender, EventArgs e)
    {
        try
        {
            // string songId = "PleOps.LibreGlucoseWatcher.TrayIcon.Songs.Bach - Air - 30sec.wav";
            // soundPlayer.Stream = typeof(MainWindow).Assembly.GetManifestResourceStream(songId);
            // soundPlayer.Load();

            if (client.Login.AuthenticationData != null)
            {
                await FetchGlucose().ConfigureAwait(true);
                DisplayGlucose();
            }
        } catch (Exception ex)
        {
            labelGlucose.Text = ex.Message;
            Show();
            ShowInTaskbar = true;
        }
    }

    private void MainWindow_MouseMove(object sender, MouseEventArgs e)
    {
        // soundPlayer.Stop();
    }

    private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!wantToClose)
        {
            e.Cancel = true;
            ShowInTaskbar = false;
            Hide();
        }
    }

    private async void BtnCheckClick(object sender, EventArgs e)
    {
        try
        {

            if (client.Login.AuthenticationData != null)
            {
                await FetchGlucose().ConfigureAwait(true);
                DisplayGlucose();
            }
        } catch (Exception ex)
        {
            labelGlucose.Text = ex.Message;
            Show();
            ShowInTaskbar = true;
        }
    }
}
