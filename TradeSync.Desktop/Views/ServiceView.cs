using TradeSync.Desktop.Logic;
using Timer = System.Windows.Forms.Timer;

namespace TradeSync.Desktop.Views
{
    public class ServiceView : UserControl
    {
        private readonly ServiceAdmin _admin = new ServiceAdmin();
        private RichTextBox _rtbLiveLog;
        private Label _lblStatus;
        private Button _btnStart, _btnStop, _btnInstall, _btnUninstall; // Додано Unistall

        private Timer _logTimer;
        private long _lastLogPos = 0;
        private string _currentLogFile = "";

        public ServiceView()
        {
            this.BackColor = Color.White;
            InitializeLayout();

            _logTimer = new Timer { Interval = 2000 };
            _logTimer.Tick += (s, e) => { UpdateLog(); UpdateStatus(); };
            _logTimer.Start();
            UpdateStatus();
        }

        private void InitializeLayout()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(20) };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Header
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Buttons
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Log

            _lblStatus = new Label { Text = "Status: Checking...", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true };

            var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill };

            _btnStart = CreateBtn("▶ START", Color.MediumSeaGreen, () => SafeExec(_admin.Start));
            _btnStop = CreateBtn("⏹ STOP", Color.IndianRed, () => SafeExec(_admin.Stop));
            _btnInstall = CreateBtn("📥 INSTALL", Color.Gray, () => InstallAuto());
            _btnUninstall = CreateBtn("🗑 UNINSTALL", Color.Gray, () => SafeExec(_admin.Uninstall)); // Додано

            btnPanel.Controls.AddRange(new Control[] { _btnStart, _btnStop, _btnInstall, _btnUninstall });

            var grpLog = new GroupBox { Text = "Service Live Log (logs/*.log)", Dock = DockStyle.Fill };
            _rtbLiveLog = new RichTextBox { Dock = DockStyle.Fill, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ReadOnly = true };
            grpLog.Controls.Add(_rtbLiveLog);

            layout.Controls.Add(_lblStatus, 0, 0);
            layout.Controls.Add(btnPanel, 0, 1);
            layout.Controls.Add(grpLog, 0, 2);
            this.Controls.Add(layout);
        }

        private void UpdateLog()
        {
            try
            {
                // Точний шлях до папки з exe
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string logDir = Path.Combine(baseDir, "logs");

                if (!Directory.Exists(logDir)) return;

                var f = new DirectoryInfo(logDir).GetFiles("service-*.log")
                    .OrderByDescending(x => x.LastWriteTime).FirstOrDefault();

                if (f == null) return;

                if (_currentLogFile != f.FullName)
                {
                    _currentLogFile = f.FullName;
                    _lastLogPos = 0;
                    _rtbLiveLog.Clear();
                    _rtbLiveLog.AppendText($"--- Log file: {f.Name} ---\n");
                }

                using var fs = new FileStream(_currentLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (fs.Length > _lastLogPos)
                {
                    fs.Seek(_lastLogPos, SeekOrigin.Begin);
                    using var sr = new StreamReader(fs);
                    _rtbLiveLog.AppendText(sr.ReadToEnd());
                    _rtbLiveLog.ScrollToCaret();
                    _lastLogPos = fs.Position;
                }
            }
            catch { }
        }

        private void UpdateStatus()
        {
            string s = _admin.GetStatus();
            _lblStatus.Text = $"SERVICE STATUS: {s.ToUpper()}";
            _lblStatus.ForeColor = s == "Running" ? Color.Green : (s == "Stopped" ? Color.Red : Color.Gray);

            bool installed = s != "Not Installed";
            _btnStart.Enabled = installed && s != "Running";
            _btnStop.Enabled = s == "Running";
            _btnInstall.Enabled = !installed;
            _btnUninstall.Enabled = installed;
        }

        private void InstallAuto()
        {
            string exe = "TradeSync.Service.exe";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exe);
            if (File.Exists(path)) SafeExec(() => _admin.Install(path));
            else MessageBox.Show($"File not found: {path}");
        }

        private void SafeExec(Action act) { try { act(); UpdateStatus(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }

        private Button CreateBtn(string t, Color c, Action act)
        {
            var b = new Button { Text = t, BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 110, Height = 40, Cursor = Cursors.Hand, Margin = new Padding(0, 0, 10, 0) };
            b.Click += (s, e) => act();
            return b;
        }
    }
}