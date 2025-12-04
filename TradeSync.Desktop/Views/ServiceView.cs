using System.Diagnostics;
using TradeSync.Desktop.Logic;
using Timer = System.Windows.Forms.Timer;

namespace TradeSync.Desktop.Views
{
    public class ServiceView : UserControl
    {
        private readonly ServiceAdmin _admin = new ServiceAdmin();

        // UI
        private Label _lblStatus;
        private Button _btnStart, _btnStop, _btnInstall, _btnUninstall;
        private RichTextBox _rtbLog;
        private ComboBox _cmbLogType, _cmbLogFiles;
        private CheckBox _chkAutoScroll;
        private Timer _monitorTimer;

        private string _currentLogPath = "";
        private long _lastLogPos = 0;

        public ServiceView()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            InitializeLayout();

            _monitorTimer = new Timer { Interval = 2000 };
            _monitorTimer.Tick += (s, e) => { UpdateStatus(); ReadLiveLog(); };
            _monitorTimer.Start();
            UpdateStatus();
            RefreshFileList();
        }

        private void InitializeLayout()
        {
            // Верхня панель (Кнопки)
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(10) };

            var grpActions = new GroupBox { Text = "Керування службою", Dock = DockStyle.Left, Width = 450 };
            _lblStatus = new Label { Text = "Статус: ...", Location = new Point(15, 25), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };

            _btnStart = new Button { Text = "Запустити", Location = new Point(15, 55), Width = 100, Height = 30, BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
            _btnStart.Click += (s, e) => SafeExec(_admin.Start);

            _btnStop = new Button { Text = "Зупинити", Location = new Point(120, 55), Width = 100, Height = 30, BackColor = Color.LightSalmon, FlatStyle = FlatStyle.Flat };
            _btnStop.Click += (s, e) => SafeExec(_admin.Stop);

            _btnInstall = new Button { Text = "Встановити", Location = new Point(225, 55), Width = 100, Height = 30 };
            _btnInstall.Click += (s, e) => SafeExec(() => InstallAuto());

            _btnUninstall = new Button { Text = "Видалити", Location = new Point(330, 55), Width = 100, Height = 30 };
            _btnUninstall.Click += (s, e) => SafeExec(_admin.Uninstall);

            grpActions.Controls.AddRange(new Control[] { _lblStatus, _btnStart, _btnStop, _btnInstall, _btnUninstall });

            // Панель логів (справа зверху)
            var grpLogs = new GroupBox { Text = "Фільтри логу", Dock = DockStyle.Fill };
            var lbl1 = new Label { Text = "Тип:", Location = new Point(15, 25), AutoSize = true };
            _cmbLogType = new ComboBox { Location = new Point(50, 22), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbLogType.Items.AddRange(new string[] { "Загальний (Info)", "Помилки (Error)" });
            _cmbLogType.SelectedIndex = 0;
            _cmbLogType.SelectedIndexChanged += (s, e) => RefreshFileList();

            var lbl2 = new Label { Text = "Файл:", Location = new Point(210, 25), AutoSize = true };
            _cmbLogFiles = new ComboBox { Location = new Point(250, 22), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbLogFiles.SelectedIndexChanged += (s, e) => ChangeLogFile();

            _chkAutoScroll = new CheckBox { Text = "Авто-скрол", Location = new Point(460, 24), Checked = true, AutoSize = true };

            var btnOpen = new Button { Text = "Відкрити файл", Location = new Point(15, 55), Width = 120 };
            btnOpen.Click += (s, e) => OpenCurrentLogFile();

            grpLogs.Controls.AddRange(new Control[] { lbl1, _cmbLogType, lbl2, _cmbLogFiles, _chkAutoScroll, btnOpen });

            topPanel.Controls.Add(grpLogs);
            topPanel.Controls.Add(grpActions);

            // Лог (Центр)
            _rtbLog = new RichTextBox { Dock = DockStyle.Fill, BackColor = Color.Black, ForeColor = Color.Lime, Font = new Font("Consolas", 10), ReadOnly = true };

            this.Controls.Add(_rtbLog);
            this.Controls.Add(topPanel);
        }

        // --- ЛОГІКА ---
        private void RefreshFileList()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(dir)) return;
            string pattern = _cmbLogType.SelectedIndex == 0 ? "service-*.log" : "errors-*.log";
            _rtbLog.ForeColor = _cmbLogType.SelectedIndex == 0 ? Color.Lime : Color.Red;

            var files = new DirectoryInfo(dir).GetFiles(pattern).OrderByDescending(f => f.LastWriteTime).ToArray();
            _cmbLogFiles.Items.Clear();
            if (files.Length == 0) { _cmbLogFiles.Items.Add("(немає)"); _currentLogPath = ""; _rtbLog.Clear(); }
            else foreach (var f in files) _cmbLogFiles.Items.Add(f.Name);
            if (_cmbLogFiles.Items.Count > 0) _cmbLogFiles.SelectedIndex = 0;
        }

        private void ChangeLogFile()
        {
            if (_cmbLogFiles.SelectedItem == null) return;
            string name = _cmbLogFiles.SelectedItem.ToString();
            if (name == "(немає)") return;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", name);
            if (_currentLogPath != path) { _currentLogPath = path; _lastLogPos = 0; _rtbLog.Clear(); ReadLiveLog(); }
        }

        private void ReadLiveLog()
        {
            if (string.IsNullOrEmpty(_currentLogPath) || !File.Exists(_currentLogPath)) return;
            try
            {
                using var fs = new FileStream(_currentLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (fs.Length < _lastLogPos) _lastLogPos = 0;
                if (fs.Length > _lastLogPos)
                {
                    fs.Seek(_lastLogPos, SeekOrigin.Begin);
                    using var sr = new StreamReader(fs);
                    _rtbLog.AppendText(sr.ReadToEnd());
                    if (_chkAutoScroll.Checked) { _rtbLog.SelectionStart = _rtbLog.Text.Length; _rtbLog.ScrollToCaret(); }
                    _lastLogPos = fs.Position;
                }
            }
            catch { }
        }

        private void UpdateStatus()
        {
            string s = _admin.GetStatus();
            _lblStatus.Text = $"Статус: {s}";
            _lblStatus.ForeColor = s == "Running" ? Color.Green : Color.Red;
            bool installed = s != "Not Installed";
            _btnStart.Enabled = installed && s != "Running";
            _btnStop.Enabled = s == "Running";
            _btnInstall.Enabled = !installed;
            _btnUninstall.Enabled = installed;
        }

        private void InstallAuto() { string p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TradeSync.Service.exe"); if (File.Exists(p)) _admin.Install(p); }
        private void SafeExec(Action a) { try { a(); UpdateStatus(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }
        private void OpenCurrentLogFile() { if (File.Exists(_currentLogPath)) new Process { StartInfo = new ProcessStartInfo(_currentLogPath) { UseShellExecute = true } }.Start(); }
    }
}