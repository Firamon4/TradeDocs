using System.Data;
using System.Data.SQLite;
using System.Text.Json;
using TradeSync.Core.Models;
using TradeSync.Desktop.Logic;

namespace TradeSync.Desktop.Views
{
    public class DataView : UserControl
    {
        private ConfigManager _configMgr = new ConfigManager();
        private SqliteBuilder _sqliteBuilder = new SqliteBuilder();
        private ServiceConfig _config;

        private ComboBox _cmbTables;
        private DataGridView _grid;
        private RichTextBox _rtbLog;
        private ProgressBar _progress;
        private Label _lblStatus;
        private Button _btnSync;

        public DataView()
        {
            this.BackColor = Color.White;
            InitializeLayout();
            ReloadConfig();
        }

        public async void ReloadConfig()
        {
            _config = await _configMgr.LoadAsync();
            await LoadTablesAsync();
        }

        private void InitializeLayout()
        {
            // Головна сітка: Ліва колонка фіксована (300px), права розтягується
            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320F)); // Трішки ширше
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // === ЛІВА ПАНЕЛЬ (КЕРУВАННЯ) ===
            var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = Color.WhiteSmoke };

            // 1. Блок вибору таблиці
            var lblTbl = new Label { Text = "1. Оберіть таблицю:", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            _cmbTables = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11), Height = 35 };
            _cmbTables.SelectedIndexChanged += (s, e) => LoadPreview();

            // 2. Кнопка сінку (Велика)
            var btnPanel = new Panel { Dock = DockStyle.Top, Height = 80, Padding = new Padding(0, 20, 0, 0) }; // Відступ зверху
            _btnSync = new Button
            {
                Text = "🔄 СИНХРОНІЗУВАТИ",
                Dock = DockStyle.Fill,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnSync.FlatAppearance.BorderSize = 0;
            _btnSync.Click += (s, e) => RunSync();
            btnPanel.Controls.Add(_btnSync);

            // 3. Лог
            var grpLog = new GroupBox { Text = "Лог подій", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9) };
            _rtbLog = new RichTextBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, BackColor = Color.White, ReadOnly = true, Font = new Font("Consolas", 9) };
            grpLog.Controls.Add(_rtbLog);

            // Прокладка для логу (щоб він був знизу)
            var logContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 20, 0, 0) };
            logContainer.Controls.Add(grpLog);

            // 4. Статус бар (Прогрес)
            var statusPanel = new Panel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(0, 10, 0, 0) };
            _lblStatus = new Label { Text = "Готовий до роботи", Dock = DockStyle.Top, Height = 15, ForeColor = Color.Gray };
            _progress = new ProgressBar { Dock = DockStyle.Bottom, Height = 20 };
            statusPanel.Controls.Add(_lblStatus);
            statusPanel.Controls.Add(_progress);

            // Додаємо в ліву панель (Порядок: знизу-вверх для DockStyle.Top, або використати BringToFront)
            // Але простіше додавати зверху вниз:
            leftPanel.Controls.Add(logContainer); // Fill (останній доданий займе все місце)
            leftPanel.Controls.Add(statusPanel);  // Bottom
            leftPanel.Controls.Add(btnPanel);     // Top (після таблиці)
            leftPanel.Controls.Add(_cmbTables);   // Top
            leftPanel.Controls.Add(lblTbl);       // Top

            // === ПРАВА ПАНЕЛЬ (ГРІД) ===
            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15), BackColor = Color.White };

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 40 };
            var lblGrid = new Label { Text = "Попередній перегляд (Локальна база SQLite)", Dock = DockStyle.Left, AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Regular), ForeColor = Color.FromArgb(64, 64, 64) };
            headerPanel.Controls.Add(lblGrid);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = Color.LightGray
            };
            _grid.EnableHeadersVisualStyles = false;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            rightPanel.Controls.Add(_grid);
            rightPanel.Controls.Add(headerPanel);

            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(rightPanel, 1, 0);
            this.Controls.Add(mainLayout);
        }

        private async Task LoadTablesAsync()
        {
            if (_config == null || !File.Exists(_config.SyncSettings.StructureFile)) return;
            try
            {
                var json = await File.ReadAllTextAsync(_config.SyncSettings.StructureFile);
                var tables = JsonSerializer.Deserialize<List<TableSchema>>(json);
                _cmbTables.Items.Clear();
                if (tables != null) foreach (var t in tables) _cmbTables.Items.Add(t.Name1C);
            }
            catch (Exception ex) { Log("Помилка JSON: " + ex.Message); }
        }

        private async void RunSync()
        {
            _btnSync.Enabled = false;
            Log("=== Старт ===");
            try
            {
                var mgr = new SyncManager(_config.ConnectionStrings.AuxDb, "local_store.db", _config.SyncSettings.StructureFile);
                mgr.OnLog += m => Invoke(() => Log(m));
                mgr.OnProgress += (c, t) => Invoke(() => { _progress.Maximum = t; _progress.Value = c; _lblStatus.Text = $"{c} / {t}"; });

                await Task.Run(() => mgr.RunSyncAsync());
                Log("=== Готово ===");
                LoadPreview();
            }
            catch (Exception ex) { Log("Error: " + ex.Message); }
            finally { _btnSync.Enabled = true; }
        }

        private void LoadPreview()
        {
            if (_cmbTables.SelectedItem == null) return;
            try
            {
                string tbl = _sqliteBuilder.GetLocalTableName(_cmbTables.SelectedItem.ToString());
                using var conn = new SQLiteConnection($"Data Source=local_store.db;Version=3;");
                conn.Open();
                var da = new SQLiteDataAdapter($"SELECT * FROM [{tbl}] LIMIT 50", conn);
                var dt = new DataTable(); da.Fill(dt);
                _grid.DataSource = dt;
            }
            catch { }
        }

        private void Log(string m) { _rtbLog.AppendText($"[{DateTime.Now:T}] {m}\n"); _rtbLog.ScrollToCaret(); }
    }
}