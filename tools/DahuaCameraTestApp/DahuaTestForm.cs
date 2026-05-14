using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DahuaCameraTestApp
{
    internal sealed class DahuaTestForm : Form
    {
        private readonly DahuaSdkClient _client = new DahuaSdkClient();
        private TextBox _ipTextBox;
        private TextBox _portTextBox;
        private TextBox _usernameTextBox;
        private TextBox _passwordTextBox;
        private TextBox _channelTextBox;
        private Panel _previewPanel;
        private RichTextBox _logTextBox;
        private Label _previewHintLabel;

        public DahuaTestForm()
        {
            Text = "Dahua Camera Test App";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1100, 700);
            Width = 1200;
            Height = 760;

            BuildUi();
            LoadDefaults();
            Shown += OnShown;
            FormClosing += OnFormClosing;
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            Controls.Add(root);

            var topLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 6,
                RowCount = 3,
                AutoSize = true
            };

            for (int i = 0; i < 6; i++)
                topLayout.ColumnStyles.Add(new ColumnStyle(i % 2 == 0 ? SizeType.AutoSize : SizeType.Percent, i % 2 == 0 ? 0f : 50f));

            _ipTextBox = AddLabeledTextBox(topLayout, 0, "IP", 0);
            _portTextBox = AddLabeledTextBox(topLayout, 2, "Port", 0);
            _usernameTextBox = AddLabeledTextBox(topLayout, 4, "Username", 0);
            _passwordTextBox = AddLabeledTextBox(topLayout, 0, "Password", 1);
            _passwordTextBox.UseSystemPasswordChar = true;
            _channelTextBox = AddLabeledTextBox(topLayout, 2, "Channel", 1);

            var buttonFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            buttonFlow.Controls.Add(CreateButton("Login", OnLoginClick));
            buttonFlow.Controls.Add(CreateButton("Start Preview", OnStartPreviewClick));
            buttonFlow.Controls.Add(CreateButton("Stop Preview", OnStopPreviewClick));
            buttonFlow.Controls.Add(CreateButton("Capture Snapshot", OnCaptureSnapshotClick));
            buttonFlow.Controls.Add(CreateButton("Logout", OnLogoutClick));
            buttonFlow.Controls.Add(CreateButton("SDK Status", OnSdkStatusClick));

            topLayout.Controls.Add(buttonFlow, 0, 2);
            topLayout.SetColumnSpan(buttonFlow, 6);
            root.Controls.Add(topLayout, 0, 0);

            var contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));

            _previewPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Margin = new Padding(0, 8, 8, 0)
            };

            _previewHintLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gainsboro,
                Text = "Preview panel\r\nSDK OK se render video vao day.",
                BackColor = Color.Transparent
            };
            _previewPanel.Controls.Add(_previewHintLabel);

            _logTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10f),
                Margin = new Padding(8, 8, 0, 0)
            };

            contentLayout.Controls.Add(_previewPanel, 0, 0);
            contentLayout.Controls.Add(_logTextBox, 1, 0);
            root.Controls.Add(contentLayout, 0, 1);
        }

        private void LoadDefaults()
        {
            _ipTextBox.Text = ReadAppSetting("DAHUA_IP", "192.168.1.142");
            _portTextBox.Text = ReadAppSetting("DAHUA_PORT", "37777");
            _usernameTextBox.Text = ReadAppSetting("DAHUA_USERNAME", "admin");
            _passwordTextBox.Text = ReadAppSetting("DAHUA_PASSWORD", string.Empty);
            _channelTextBox.Text = ReadAppSetting("DAHUA_CHANNEL", "1");
        }

        private void OnShown(object sender, EventArgs e)
        {
            AppendLog("App started.");
            bool ok = _client.Init(AppendLog);
            AppendLog(ok ? "Init on startup: PASS" : "Init on startup: FAIL");
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            _client.Cleanup(AppendLog);
        }

        private void OnLoginClick(object sender, EventArgs e)
        {
            try
            {
                string ip = _ipTextBox.Text.Trim();
                ushort port;
                if (!ushort.TryParse(_portTextBox.Text.Trim(), out port))
                {
                    AppendLog("Login FAIL | invalid port.");
                    return;
                }

                AppendLog(string.Format("Login requested | ip={0} port={1} user={2}", ip, port, _usernameTextBox.Text.Trim()));
                _client.Login(ip, port, _usernameTextBox.Text.Trim(), _passwordTextBox.Text, AppendLog);
            }
            catch (Exception ex)
            {
                AppendLog("Login FAIL | " + ex);
            }
        }

        private void OnStartPreviewClick(object sender, EventArgs e)
        {
            try
            {
                int channel;
                if (!int.TryParse(_channelTextBox.Text.Trim(), out channel) || channel <= 0)
                {
                    AppendLog("StartPreview FAIL | invalid channel.");
                    return;
                }

                _previewHintLabel.Visible = false;
                bool ok = _client.StartPreview(channel, _previewPanel.Handle, AppendLog);
                if (ok)
                {
                    _previewHintLabel.Text = "Preview started.\r\nNeu panel van den, kiem tra SDK/channel/NVR.";
                    _previewHintLabel.Visible = false;
                }
                else
                {
                    _previewHintLabel.Text = "Preview failed.\r\nXem log ben phai.";
                    _previewHintLabel.Visible = true;
                }
            }
            catch (Exception ex)
            {
                AppendLog("StartPreview FAIL | " + ex);
            }
        }

        private void OnStopPreviewClick(object sender, EventArgs e)
        {
            try
            {
                _client.StopPreview(AppendLog);
                _previewHintLabel.Visible = true;
                _previewHintLabel.Text = "Preview stopped.";
            }
            catch (Exception ex)
            {
                AppendLog("StopPreview FAIL | " + ex);
            }
        }

        private void OnCaptureSnapshotClick(object sender, EventArgs e)
        {
            try
            {
                string snapshotFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DahuaSnapshots");
                string fileName = string.Format(
                    "snapshot_ch{0}_{1}.bmp",
                    _channelTextBox.Text.Trim(),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"));
                string snapshotPath = Path.Combine(snapshotFolder, fileName);
                _client.CaptureSnapshot(snapshotPath, AppendLog);
            }
            catch (Exception ex)
            {
                AppendLog("CaptureSnapshot FAIL | " + ex);
            }
        }

        private void OnLogoutClick(object sender, EventArgs e)
        {
            try
            {
                _client.Logout(AppendLog);
                _previewHintLabel.Visible = true;
                _previewHintLabel.Text = "Logged out.";
            }
            catch (Exception ex)
            {
                AppendLog("Logout FAIL | " + ex);
            }
        }

        private void OnSdkStatusClick(object sender, EventArgs e)
        {
            AppendLog(_client.GetSdkSearchSummary());
            if (!string.IsNullOrWhiteSpace(_client.LoadedSdkAssemblyPath))
                AppendLog("Loaded SDK assembly: " + _client.LoadedSdkAssemblyPath);
        }

        private void AppendLog(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AppendLog), message);
                return;
            }

            _logTextBox.AppendText(
                string.Format(
                    "[{0}] {1}{2}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    message,
                    Environment.NewLine));
            _logTextBox.ScrollToCaret();
        }

        private static Button CreateButton(string text, EventHandler onClick)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                Padding = new Padding(8, 4, 8, 4),
                Margin = new Padding(0, 0, 8, 0)
            };
            button.Click += onClick;
            return button;
        }

        private static TextBox AddLabeledTextBox(TableLayoutPanel layout, int column, string label, int row)
        {
            var title = new Label
            {
                Text = label,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 6, 8, 6)
            };
            var textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 3, 16, 3)
            };

            layout.Controls.Add(title, column, row);
            layout.Controls.Add(textBox, column + 1, row);
            return textBox;
        }

        private static string ReadAppSetting(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
    }
}
