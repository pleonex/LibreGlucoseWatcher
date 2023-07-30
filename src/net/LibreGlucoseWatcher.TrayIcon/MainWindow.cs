using Microsoft.Toolkit.Uwp.Notifications;
using PleOps.LibreGlucose;
using PleOps.LibreGlucose.Connection;
using PleOps.LibreGlucose.Patients;
using System.Globalization;
using System.Media;
using System.Resources;

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
                client.Login.AuthenticationData = authData;
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

            if (checkPlayMusic.Checked)
            {
                soundPlayer.Play();
            }
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
        var font = new Font(FontFamily.GenericSerif, 20);
        string text = glucose.Replace(" ", "\n");

        using var dummyImage = new Bitmap(256, 256);
        using var measurementGraphic = Graphics.FromImage(dummyImage);
        SizeF textSize = measurementGraphic.MeasureString(text, font);

        var image = new Bitmap((int)textSize.Width, (int)textSize.Height);
        var graphic = Graphics.FromImage(image);

        Brush color = colorId switch
        {
            MeasurementColor.InRange => Brushes.Green,
            MeasurementColor.OutsideRange => Brushes.Orange,
            MeasurementColor.HighAlarm => Brushes.Red,
            MeasurementColor.LowAlarm => Brushes.Red,
            _ => Brushes.White,
        };
        graphic.FillRectangle(color, 0, 0, image.Width, image.Height);
        graphic.DrawString(text, font, Brushes.Black, 0, 0);

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
        soundPlayer.Stop();
    }

    private void MainWindowResizing(object sender, EventArgs e)
    {
        if (WindowState is FormWindowState.Normal)
        {
            soundPlayer.Stop();
        }
    }

    private void TrayIconMouseDoubleClick(object sender, MouseEventArgs e)
    {
        Show();
        ShowInTaskbar = true;
        soundPlayer.Stop();
    }

    private async void RefreshGlucoseTimerTick(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(client.Login.Token))
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
            string songId = "PleOps.LibreGlucoseWatcher.TrayIcon.Songs.Bach - Air - 30sec.wav";
            soundPlayer.Stream = typeof(MainWindow).Assembly.GetManifestResourceStream(songId);
            soundPlayer.Load();

            if (!string.IsNullOrEmpty(client.Login.Token))
            {
                await FetchGlucose().ConfigureAwait(true);
                DisplayGlucose();
            }
        } catch (Exception ex)
        {
            labelGlucose.Text = ex.Message;
        }
    }

    private void MainWindow_MouseMove(object sender, MouseEventArgs e)
    {
        soundPlayer.Stop();
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

            if (!string.IsNullOrEmpty(client.Login.Token))
            {
                await FetchGlucose().ConfigureAwait(true);
                DisplayGlucose();
            }
        } catch (Exception ex)
        {
            labelGlucose.Text = ex.Message;
        }
    }
}
