using System.Diagnostics;
using TradeSync.Desktop.Logic;
using Timer = System.Windows.Forms.Timer;

namespace TradeSync.Desktop.Views
{
    public class ServiceView : UserControl
    {
        private readonly ServiceAdmin _admin = new ServiceAdmin();

        // UI Елементи
        private Label _lblStatus;
        private Button _btnStart, _btnStop, _btnInstall, _btnUninstall;
        private RichTextBox _rtbLog;

        // Фільтри логів
        private ComboBox _cmbLogType; // <--- НОВЕ: Вибір типу (Info/Error)
        private ComboBox _cmbLogFiles; // Вибір конкретного файлу
        private CheckBox _chkAutoScroll;

        private Timer _monitorTimer;
        private string _currentLogPath = "";
        private long _lastLogPos = 0;

        public ServiceView()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(240, 240, 240);

            InitializeLayout();

            _monitorTimer = new Timer { Interval = 1000 };
            _monitorTimer.Tick += (s, e) => { UpdateStatus(); ReadLiveLog(); };
            _monitorTimer.Start();

            UpdateStatus();

            // Ініціалізація вибору типів логів
            _cmbLogType.SelectedIndex = 0; // За замовчуванням "Загальний"
        }

        private void InitializeLayout()
        {
            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(10) };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F)); // Toolbar (збільшив висоту)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Log

            // === 1. HEADER ===
            var headerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20, 10, 20, 10) };

            _lblStatus = new Label { Text = "Статус: ...", Dock = DockStyle.Left, AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.DimGray };

            var btnFlow = new FlowLayoutPanel { Dock = DockStyle.Right, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };

            _btnUninstall = CreateSecondaryButton("🗑 Видалити");
            _btnUninstall.Click += (s, e) => SafeExec(_admin.Uninstall);

            _btnInstall = CreateSecondaryButton("📥 Встановити");
            _btnInstall.Click += (s, e) => SafeExec(() => InstallAuto());

            _btnStop = CreateMainButton("⏹ СТОП", Color.IndianRed);
            _btnStop.Click += (s, e) => SafeExec(_admin.Stop);

            _btnStart = CreateMainButton("▶ СТАРТ", Color.SeaGreen);
            _btnStart.Click += (s, e) => SafeExec(_admin.Start);

            btnFlow.Controls.Add(_btnStart);
            btnFlow.Controls.Add(_btnStop);
            btnFlow.Controls.Add(CreateSpacer(20));
            btnFlow.Controls.Add(_btnInstall);
            btnFlow.Controls.Add(_btnUninstall);

            headerPanel.Controls.Add(btnFlow);
            headerPanel.Controls.Add(_lblStatus);

            // === 2. TOOLBAR (Фільтри логів) ===
            var toolPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 0) };

            // Тип логу (Info vs Error)
            var lblType = new Label { Text = "Тип:", Dock = DockStyle.Left, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10), Padding = new Padding(0, 5, 0, 0) };
            _cmbLogType = new ComboBox { Dock = DockStyle.Left, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10), FlatStyle = FlatStyle.Flat };
            _cmbLogType.Items.AddRange(new string[] { "📘 Загальний (Info)", "📕 Помилки (Errors)" });
            _cmbLogType.SelectedIndexChanged += (s, e) => RefreshLogFilesList(); // При зміні типу оновлюємо список файлів

            // Файл логу
            var lblFile = new Label { Text = "Файл:", Dock = DockStyle.Left, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10), Padding = new Padding(10, 5, 0, 0) };
            _cmbLogFiles = new ComboBox { Dock = DockStyle.Left, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10), FlatStyle = FlatStyle.Flat };
            _cmbLogFiles.SelectedIndexChanged += (s, e) => ChangeLogFile();

            // Кнопки
            var btnFolder = new Button { Text = "📂 Папка", Dock = DockStyle.Left, Width = 80, FlatStyle = FlatStyle.Flat, BackColor = Color.White, Cursor = Cursors.Hand, Margin = new Padding(10, 0, 0, 0) };
            btnFolder.Click += (s, e) => OpenLogFolder();

            var btnOpen = new Button { Text = "📄 Відкрити", Dock = DockStyle.Left, Width = 80, FlatStyle = FlatStyle.Flat, BackColor = Color.White, Cursor = Cursors.Hand };
            btnOpen.Click += (s, e) => OpenCurrentLogFile();

            _chkAutoScroll = new CheckBox { Text = "Авто-прокрутка", Dock = DockStyle.Right, Checked = true, Font = new Font("Segoe UI", 10), AutoSize = true, Padding = new Padding(0, 5, 0, 0) };

            // Додаємо зліва направо (порядок важливий для Dock=Left)
            toolPanel.Controls.Add(_chkAutoScroll); // Right dock

            toolPanel.Controls.Add(btnOpen);
            toolPanel.Controls.Add(btnFolder);
            toolPanel.Controls.Add(CreateSpacer(10));
            toolPanel.Controls.Add(_cmbLogFiles);
            toolPanel.Controls.Add(lblFile);
            toolPanel.Controls.Add(_cmbLogType);
            toolPanel.Controls.Add(lblType);

            // === 3. LOG VIEWER ===
            var logContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };
            _rtbLog = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 10),
                ReadOnly = true,
                BorderStyle = BorderStyle.None
            };
            logContainer.Controls.Add(_rtbLog);

            mainLayout.Controls.Add(headerPanel, 0, 0);
            mainLayout.Controls.Add(toolPanel, 0, 1);
            mainLayout.Controls.Add(logContainer, 0, 2);

            this.Controls.Add(mainLayout);
        }

        // --- ЛОГІКА ЛОГІВ ---

        private void RefreshLogFilesList()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(dir)) return;

            // Визначаємо патерн пошуку залежно від вибраного типу
            string searchPattern = _cmbLogType.SelectedIndex == 0 ? "service-*.log" : "errors-*.log";

            // Змінюємо колір тексту залежно від типу, щоб було наглядно
            if (_cmbLogType.SelectedIndex == 0)
                _rtbLog.ForeColor = Color.LightGreen;
            else
                _rtbLog.ForeColor = Color.OrangeRed;

            var files = new DirectoryInfo(dir).GetFiles(searchPattern)
                .OrderByDescending(f => f.LastWriteTime)
                .ToArray();

            _cmbLogFiles.Items.Clear();
            _cmbLogFiles.ResetText();

            if (files.Length == 0)
            {
                _cmbLogFiles.Items.Add("(файлів немає)");
                _cmbLogFiles.SelectedIndex = 0;
                _rtbLog.Clear();
                _currentLogPath = "";
                return;
            }

            foreach (var f in files)
            {
                _cmbLogFiles.Items.Add(f.Name);
            }

            // Вибираємо найсвіжіший
            _cmbLogFiles.SelectedIndex = 0;
        }

        private void ChangeLogFile()
        {
            if (_cmbLogFiles.SelectedItem == null || _cmbLogFiles.SelectedItem.ToString() == "(файлів немає)") return;

            string fileName = _cmbLogFiles.SelectedItem.ToString();
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", fileName);

            if (_currentLogPath != fullPath)
            {
                _currentLogPath = fullPath;
                _lastLogPos = 0;
                _rtbLog.Clear();
                _rtbLog.AppendText($"--- Завантаження: {fileName} ---\n");
                ReadLiveLog(); // Примусово читаємо відразу
            }
        }

        private void ReadLiveLog()
        {
            if (string.IsNullOrEmpty(_currentLogPath) || !File.Exists(_currentLogPath)) return;

            try
            {
                using (var fs = new FileStream(_currentLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Якщо файл був перезаписаний (новий запуск), скидаємо позицію
                    if (fs.Length < _lastLogPos) _lastLogPos = 0;

                    if (fs.Length > _lastLogPos)
                    {
                        fs.Seek(_lastLogPos, SeekOrigin.Begin);
                        using (var sr = new StreamReader(fs))
                        {
                            string newText = sr.ReadToEnd();
                            _rtbLog.AppendText(newText);

                            if (_chkAutoScroll.Checked)
                            {
                                _rtbLog.SelectionStart = _rtbLog.Text.Length;
                                _rtbLog.ScrollToCaret();
                            }
                        }
                        _lastLogPos = fs.Position;
                    }
                }
            }
            catch { /* ігноруємо конфлікти читання */ }
        }

        private void OpenLogFolder() { string d = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"); if (Directory.Exists(d)) Process.Start("explorer.exe", d); }
        private void OpenCurrentLogFile() { if (File.Exists(_currentLogPath)) new Process { StartInfo = new ProcessStartInfo(_currentLogPath) { UseShellExecute = true } }.Start(); }

        // --- ЛОГІКА СТАТУСУ --- (Без змін, тільки копіюємо для повноти)
        private void UpdateStatus()
        {
            string s = _admin.GetStatus();
            if (s == "Running")
            {
                _lblStatus.Text = "Статус: ПРАЦЮЄ"; _lblStatus.ForeColor = Color.SeaGreen;
                _btnStart.Visible = false; _btnStop.Visible = true; _btnInstall.Visible = false; _btnUninstall.Visible = false;
            }
            else if (s == "Stopped")
            {
                _lblStatus.Text = "Статус: ЗУПИНЕНО"; _lblStatus.ForeColor = Color.IndianRed;
                _btnStart.Visible = true; _btnStop.Visible = false; _btnInstall.Visible = false; _btnUninstall.Visible = true;
            }
            else
            {
                _lblStatus.Text = "Статус: НЕ ВСТАНОВЛЕНО"; _lblStatus.ForeColor = Color.Gray;
                _btnStart.Visible = false; _btnStop.Visible = false; _btnInstall.Visible = true; _btnUninstall.Visible = false;
            }
        }

        // --- HELPERS ---
        private void InstallAuto() { string p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TradeSync.Service.exe"); if (File.Exists(p)) SafeExec(() => _admin.Install(p)); else MessageBox.Show("Файл exe не знайдено"); }
        private void SafeExec(Action a) { try { a(); UpdateStatus(); } catch (Exception ex) { MessageBox.Show(ex.Message); } }
        private Panel CreateSpacer(int w) => new Panel { Dock = DockStyle.Left, Width = w };

        private Button CreateMainButton(string t, Color bg)
        {
            return new Button { Text = t, BackColor = bg, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(130, 40), Cursor = Cursors.Hand, Margin = new Padding(5, 0, 0, 0), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        }
        private Button CreateSecondaryButton(string t)
        {
            return new Button { Text = t, BackColor = Color.Transparent, ForeColor = Color.DimGray, FlatStyle = FlatStyle.Flat, Size = new Size(130, 40), Cursor = Cursors.Hand, Margin = new Padding(5, 0, 0, 0), Font = new Font("Segoe UI", 9) };
        }
    }
}