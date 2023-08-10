namespace PleOps.LibreGlucoseWatcher.TrayIcon
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnLogin = new Button();
            groupLogin = new GroupBox();
            labelToken = new Label();
            labelTokenInfo = new Label();
            labelPassword = new Label();
            textBoxPassword = new TextBox();
            textBoxEmail = new TextBox();
            labelEmail = new Label();
            labelGlucoseInfo = new Label();
            labelGlucose = new Label();
            groupBoxSettings = new GroupBox();
            checkShowNotification = new CheckBox();
            labelHighThresholdLlu = new Label();
            labelLowThresholdLlu = new Label();
            boxHighThreshold = new NumericUpDown();
            boxLowThreshold = new NumericUpDown();
            labelHighThr = new Label();
            labelLowThrText = new Label();
            checkPlayMusic = new CheckBox();
            boxPatientId = new NumericUpDown();
            labelPatientId = new Label();
            boxRefreshPeriod = new NumericUpDown();
            labelRefresh = new Label();
            radioButtonUnitMmolL = new RadioButton();
            radioButtonUnitMgDl = new RadioButton();
            labelUnits = new Label();
            trayIcon = new NotifyIcon(components);
            contextMenuTrayIcon = new ContextMenuStrip(components);
            trayIconOpenSettings = new ToolStripMenuItem();
            trayIconExit = new ToolStripMenuItem();
            refreshGlucoseTimer = new System.Windows.Forms.Timer(components);
            btnCheck = new Button();
            groupLogin.SuspendLayout();
            groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)boxHighThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)boxLowThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)boxPatientId).BeginInit();
            ((System.ComponentModel.ISupportInitialize)boxRefreshPeriod).BeginInit();
            contextMenuTrayIcon.SuspendLayout();
            SuspendLayout();
            //
            // btnLogin
            //
            btnLogin.Enabled = false;
            btnLogin.Location = new Point(6, 163);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(112, 34);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "Login!";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += LoginClicked;
            //
            // groupLogin
            //
            groupLogin.Controls.Add(labelToken);
            groupLogin.Controls.Add(labelTokenInfo);
            groupLogin.Controls.Add(labelPassword);
            groupLogin.Controls.Add(textBoxPassword);
            groupLogin.Controls.Add(btnLogin);
            groupLogin.Controls.Add(textBoxEmail);
            groupLogin.Controls.Add(labelEmail);
            groupLogin.Location = new Point(12, 12);
            groupLogin.Name = "groupLogin";
            groupLogin.Size = new Size(398, 209);
            groupLogin.TabIndex = 0;
            groupLogin.TabStop = false;
            groupLogin.Text = "Login details";
            //
            // labelToken
            //
            labelToken.AutoSize = true;
            labelToken.Location = new Point(103, 115);
            labelToken.Name = "labelToken";
            labelToken.Size = new Size(248, 25);
            labelToken.TabIndex = 6;
            labelToken.Text = "Invalid - Re-enter login details";
            //
            // labelTokenInfo
            //
            labelTokenInfo.AutoSize = true;
            labelTokenInfo.Location = new Point(6, 115);
            labelTokenInfo.Name = "labelTokenInfo";
            labelTokenInfo.Size = new Size(62, 25);
            labelTokenInfo.TabIndex = 5;
            labelTokenInfo.Text = "Token:";
            //
            // labelPassword
            //
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(6, 70);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(91, 25);
            labelPassword.TabIndex = 3;
            labelPassword.Text = "Password:";
            //
            // textBoxPassword
            //
            textBoxPassword.Location = new Point(103, 67);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.PlaceholderText = "LibreView password";
            textBoxPassword.Size = new Size(271, 31);
            textBoxPassword.TabIndex = 4;
            textBoxPassword.UseSystemPasswordChar = true;
            textBoxPassword.TextChanged += LoginTextChanged;
            //
            // textBoxEmail
            //
            textBoxEmail.Location = new Point(103, 30);
            textBoxEmail.Name = "textBoxEmail";
            textBoxEmail.PlaceholderText = "LibreView email";
            textBoxEmail.Size = new Size(271, 31);
            textBoxEmail.TabIndex = 2;
            textBoxEmail.TextChanged += LoginTextChanged;
            //
            // labelEmail
            //
            labelEmail.AutoSize = true;
            labelEmail.Location = new Point(6, 33);
            labelEmail.Name = "labelEmail";
            labelEmail.Size = new Size(58, 25);
            labelEmail.TabIndex = 1;
            labelEmail.Text = "Email:";
            //
            // labelGlucoseInfo
            //
            labelGlucoseInfo.AutoSize = true;
            labelGlucoseInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            labelGlucoseInfo.Location = new Point(18, 350);
            labelGlucoseInfo.Name = "labelGlucoseInfo";
            labelGlucoseInfo.Size = new Size(111, 32);
            labelGlucoseInfo.TabIndex = 6;
            labelGlucoseInfo.Text = "Glucose:";
            //
            // labelGlucose
            //
            labelGlucose.AutoSize = true;
            labelGlucose.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            labelGlucose.Location = new Point(135, 350);
            labelGlucose.Name = "labelGlucose";
            labelGlucose.Size = new Size(206, 32);
            labelGlucose.TabIndex = 7;
            labelGlucose.Text = "waiting for login";
            //
            // groupBoxSettings
            //
            groupBoxSettings.Controls.Add(checkShowNotification);
            groupBoxSettings.Controls.Add(labelHighThresholdLlu);
            groupBoxSettings.Controls.Add(labelLowThresholdLlu);
            groupBoxSettings.Controls.Add(boxHighThreshold);
            groupBoxSettings.Controls.Add(boxLowThreshold);
            groupBoxSettings.Controls.Add(labelHighThr);
            groupBoxSettings.Controls.Add(labelLowThrText);
            groupBoxSettings.Controls.Add(checkPlayMusic);
            groupBoxSettings.Controls.Add(boxPatientId);
            groupBoxSettings.Controls.Add(labelPatientId);
            groupBoxSettings.Controls.Add(boxRefreshPeriod);
            groupBoxSettings.Controls.Add(labelRefresh);
            groupBoxSettings.Controls.Add(radioButtonUnitMmolL);
            groupBoxSettings.Controls.Add(radioButtonUnitMgDl);
            groupBoxSettings.Controls.Add(labelUnits);
            groupBoxSettings.Location = new Point(416, 12);
            groupBoxSettings.Name = "groupBoxSettings";
            groupBoxSettings.Size = new Size(356, 318);
            groupBoxSettings.TabIndex = 8;
            groupBoxSettings.TabStop = false;
            groupBoxSettings.Text = "Settings";
            //
            // checkShowNotification
            //
            checkShowNotification.AutoSize = true;
            checkShowNotification.Checked = true;
            checkShowNotification.CheckState = CheckState.Checked;
            checkShowNotification.Location = new Point(158, 153);
            checkShowNotification.Name = "checkShowNotification";
            checkShowNotification.Size = new Size(184, 29);
            checkShowNotification.TabIndex = 14;
            checkShowNotification.Text = "Show notification?";
            checkShowNotification.UseVisualStyleBackColor = true;
            //
            // labelHighThresholdLlu
            //
            labelHighThresholdLlu.AutoSize = true;
            labelHighThresholdLlu.Location = new Point(298, 228);
            labelHighThresholdLlu.Name = "labelHighThresholdLlu";
            labelHighThresholdLlu.Size = new Size(44, 25);
            labelHighThresholdLlu.TabIndex = 13;
            labelHighThresholdLlu.Text = "TBD";
            //
            // labelLowThresholdLlu
            //
            labelLowThresholdLlu.AutoSize = true;
            labelLowThresholdLlu.Location = new Point(298, 193);
            labelLowThresholdLlu.Name = "labelLowThresholdLlu";
            labelLowThresholdLlu.Size = new Size(44, 25);
            labelLowThresholdLlu.TabIndex = 12;
            labelLowThresholdLlu.Text = "TBD";
            //
            // boxHighThreshold
            //
            boxHighThreshold.Location = new Point(188, 228);
            boxHighThreshold.Maximum = new decimal(new int[] { 612, 0, 0, 0 });
            boxHighThreshold.Minimum = new decimal(new int[] { 126, 0, 0, 0 });
            boxHighThreshold.Name = "boxHighThreshold";
            boxHighThreshold.Size = new Size(104, 31);
            boxHighThreshold.TabIndex = 11;
            boxHighThreshold.Value = new decimal(new int[] { 180, 0, 0, 0 });
            //
            // boxLowThreshold
            //
            boxLowThreshold.Location = new Point(188, 191);
            boxLowThreshold.Maximum = new decimal(new int[] { 126, 0, 0, 0 });
            boxLowThreshold.Minimum = new decimal(new int[] { 54, 0, 0, 0 });
            boxLowThreshold.Name = "boxLowThreshold";
            boxLowThreshold.Size = new Size(104, 31);
            boxLowThreshold.TabIndex = 10;
            boxLowThreshold.Value = new decimal(new int[] { 90, 0, 0, 0 });
            //
            // labelHighThr
            //
            labelHighThr.AutoSize = true;
            labelHighThr.Location = new Point(6, 230);
            labelHighThr.Name = "labelHighThr";
            labelHighThr.Size = new Size(134, 25);
            labelHighThr.TabIndex = 9;
            labelHighThr.Text = "High threshold:";
            //
            // labelLowThrText
            //
            labelLowThrText.AutoSize = true;
            labelLowThrText.Location = new Point(6, 193);
            labelLowThrText.Name = "labelLowThrText";
            labelLowThrText.Size = new Size(128, 25);
            labelLowThrText.TabIndex = 8;
            labelLowThrText.Text = "Low threshold:";
            //
            // checkPlayMusic
            //
            checkPlayMusic.AutoSize = true;
            checkPlayMusic.Location = new Point(6, 153);
            checkPlayMusic.Name = "checkPlayMusic";
            checkPlayMusic.Size = new Size(123, 29);
            checkPlayMusic.TabIndex = 7;
            checkPlayMusic.Text = "Play song?";
            checkPlayMusic.UseVisualStyleBackColor = true;
            //
            // boxPatientId
            //
            boxPatientId.Location = new Point(188, 107);
            boxPatientId.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            boxPatientId.Name = "boxPatientId";
            boxPatientId.Size = new Size(156, 31);
            boxPatientId.TabIndex = 6;
            boxPatientId.ValueChanged += SettingChanged;
            //
            // labelPatientId
            //
            labelPatientId.AutoSize = true;
            labelPatientId.Location = new Point(6, 109);
            labelPatientId.Name = "labelPatientId";
            labelPatientId.Size = new Size(92, 25);
            labelPatientId.TabIndex = 5;
            labelPatientId.Text = "Patient ID:";
            //
            // boxRefreshPeriod
            //
            boxRefreshPeriod.Location = new Point(188, 70);
            boxRefreshPeriod.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            boxRefreshPeriod.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            boxRefreshPeriod.Name = "boxRefreshPeriod";
            boxRefreshPeriod.Size = new Size(156, 31);
            boxRefreshPeriod.TabIndex = 4;
            boxRefreshPeriod.Value = new decimal(new int[] { 15, 0, 0, 0 });
            boxRefreshPeriod.ValueChanged += BoxRefreshPeriodValueChanged;
            //
            // labelRefresh
            //
            labelRefresh.AutoSize = true;
            labelRefresh.Location = new Point(6, 70);
            labelRefresh.Name = "labelRefresh";
            labelRefresh.Size = new Size(176, 25);
            labelRefresh.TabIndex = 3;
            labelRefresh.Text = "Refresh period (min):";
            //
            // radioButtonUnitMmolL
            //
            radioButtonUnitMmolL.AutoSize = true;
            radioButtonUnitMmolL.Location = new Point(245, 33);
            radioButtonUnitMmolL.Name = "radioButtonUnitMmolL";
            radioButtonUnitMmolL.Size = new Size(99, 29);
            radioButtonUnitMmolL.TabIndex = 2;
            radioButtonUnitMmolL.Text = "mmol/L";
            radioButtonUnitMmolL.UseVisualStyleBackColor = true;
            //
            // radioButtonUnitMgDl
            //
            radioButtonUnitMgDl.AutoSize = true;
            radioButtonUnitMgDl.Checked = true;
            radioButtonUnitMgDl.Location = new Point(133, 33);
            radioButtonUnitMgDl.Name = "radioButtonUnitMgDl";
            radioButtonUnitMgDl.Size = new Size(90, 29);
            radioButtonUnitMgDl.TabIndex = 1;
            radioButtonUnitMgDl.TabStop = true;
            radioButtonUnitMgDl.Text = "mg/dL";
            radioButtonUnitMgDl.UseVisualStyleBackColor = true;
            radioButtonUnitMgDl.CheckedChanged += SettingChanged;
            //
            // labelUnits
            //
            labelUnits.AutoSize = true;
            labelUnits.Location = new Point(6, 33);
            labelUnits.Name = "labelUnits";
            labelUnits.Size = new Size(121, 25);
            labelUnits.TabIndex = 0;
            labelUnits.Text = "Glucose units:";
            //
            // trayIcon
            //
            trayIcon.ContextMenuStrip = contextMenuTrayIcon;
            trayIcon.Text = "Waiting for login";
            trayIcon.Visible = true;
            trayIcon.MouseDoubleClick += TrayIconMouseDoubleClick;
            //
            // contextMenuTrayIcon
            //
            contextMenuTrayIcon.ImageScalingSize = new Size(24, 24);
            contextMenuTrayIcon.Items.AddRange(new ToolStripItem[] { trayIconOpenSettings, trayIconExit });
            contextMenuTrayIcon.Name = "contextMenuTrayIcon";
            contextMenuTrayIcon.Size = new Size(149, 68);
            //
            // trayIconOpenSettings
            //
            trayIconOpenSettings.Name = "trayIconOpenSettings";
            trayIconOpenSettings.Size = new Size(148, 32);
            trayIconOpenSettings.Text = "Settings";
            trayIconOpenSettings.Click += TrayIconOpenSettingClicked;
            //
            // trayIconExit
            //
            trayIconExit.Name = "trayIconExit";
            trayIconExit.Size = new Size(148, 32);
            trayIconExit.Text = "Exit";
            trayIconExit.Click += TrayIconExitClicked;
            //
            // refreshGlucoseTimer
            //
            refreshGlucoseTimer.Tick += RefreshGlucoseTimerTick;
            //
            // btnCheck
            //
            btnCheck.Enabled = false;
            btnCheck.Location = new Point(660, 358);
            btnCheck.Name = "btnCheck";
            btnCheck.Size = new Size(112, 34);
            btnCheck.TabIndex = 9;
            btnCheck.Text = "Check now!";
            btnCheck.UseVisualStyleBackColor = true;
            btnCheck.Click += BtnCheckClick;
            //
            // MainWindow
            //
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(789, 404);
            Controls.Add(btnCheck);
            Controls.Add(groupBoxSettings);
            Controls.Add(labelGlucose);
            Controls.Add(labelGlucoseInfo);
            Controls.Add(groupLogin);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainWindow";
            Text = "LibreGlucoseWatcher ~~ by pleonex";
            FormClosing += MainWindow_FormClosing;
            Load += MainWindowLoad;
            MouseMove += MainWindow_MouseMove;
            Resize += MainWindowResizing;
            groupLogin.ResumeLayout(false);
            groupLogin.PerformLayout();
            groupBoxSettings.ResumeLayout(false);
            groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)boxHighThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)boxLowThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)boxPatientId).EndInit();
            ((System.ComponentModel.ISupportInitialize)boxRefreshPeriod).EndInit();
            contextMenuTrayIcon.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnLogin;
        private GroupBox groupLogin;
        private Label labelPassword;
        private TextBox textBoxPassword;
        private TextBox textBoxEmail;
        private Label labelEmail;
        private Label labelGlucoseInfo;
        private Label labelGlucose;
        private Label labelToken;
        private Label labelTokenInfo;
        private GroupBox groupBoxSettings;
        private RadioButton radioButtonUnitMmolL;
        private RadioButton radioButtonUnitMgDl;
        private Label labelUnits;
        private NumericUpDown boxRefreshPeriod;
        private Label labelRefresh;
        private NumericUpDown boxPatientId;
        private Label labelPatientId;
        private NotifyIcon trayIcon;
        private ContextMenuStrip contextMenuTrayIcon;
        private ToolStripMenuItem trayIconOpenSettings;
        private ToolStripMenuItem trayIconExit;
        private System.Windows.Forms.Timer refreshGlucoseTimer;
        private CheckBox checkPlayMusic;
        private NumericUpDown boxHighThreshold;
        private NumericUpDown boxLowThreshold;
        private Label labelHighThr;
        private Label labelLowThrText;
        private Label labelHighThresholdLlu;
        private Label labelLowThresholdLlu;
        private CheckBox checkShowNotification;
        private Button btnCheck;
    }
}
