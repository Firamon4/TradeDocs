using System.Data;
using System.Text.Json;
using System.Data.SQLite;
using TradeSync.Desktop.Logic;
using TradeSync.Core.Models;

namespace TradeSync.Desktop
{
    public partial class MainForm : Form
    {
        private const string AuxConnString = "Server=localhost\\SQLEXPRESS;Database=POSTradeDB;Trusted_Connection=True;TrustServerCertificate=True;";
        private const string LocalDbPath = "local_store.db";
        private const string StructureFile = "structure.json";

        private readonly SqliteBuilder _sqliteBuilder;
        private readonly ServiceAdmin _serviceAdmin; // Додали адміна

        private long _lastLogPosition = 0;
        private string _currentLogFile = "";

        public MainForm()
        {
            InitializeComponent();
            _sqliteBuilder = new SqliteBuilder();
            _serviceAdmin = new ServiceAdmin();
        }

        // 1. У MainForm_Load запускаємо таймер
        private async void MainForm_Load(object sender, EventArgs e)
        {
            Log("Запуск програми...");
            CheckAndInstallServiceAuto();
            UpdateServiceStatus();

            // Запуск читання логів сервісу
            timerServiceLog.Start();

            await LoadTableListAsync();
        }

        // 2. Логіка Таймера (читання файлу)
        private void timerServiceLog_Tick(object sender, EventArgs e)
        {
            try
            {
                // Папка логів (там де лежить exe)
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

                if (!Directory.Exists(logDir))
                {
                    if (rtbServiceLog.Text != "Папка logs ще не створена.")
                        rtbServiceLog.Text = "Папка logs ще не створена.";
                    return;
                }

                // Шукаємо найсвіжіший файл: service-20231025.log
                var newestFile = new DirectoryInfo(logDir)
                    .GetFiles("service-*.log")
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

                if (newestFile == null) return;

                // Якщо файл змінився (настав новий день), скидаємо позицію
                if (_currentLogFile != newestFile.FullName)
                {
                    _currentLogFile = newestFile.FullName;
                    _lastLogPosition = 0;
                    rtbServiceLog.Clear();
                    rtbServiceLog.AppendText($"=== Знайдено новий лог: {newestFile.Name} ===\n");
                }

                // Читаємо тільки нові дані (Incremental Read)
                using (var fs = new FileStream(_currentLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.Length > _lastLogPosition)
                    {
                        fs.Seek(_lastLogPosition, SeekOrigin.Begin);
                        using (var sr = new StreamReader(fs))
                        {
                            string newContent = sr.ReadToEnd();

                            // Додаємо текст
                            rtbServiceLog.AppendText(newContent);

                            // Автоскрол вниз
                            rtbServiceLog.SelectionStart = rtbServiceLog.Text.Length;
                            rtbServiceLog.ScrollToCaret();
                        }
                        _lastLogPosition = fs.Position;
                    }
                }
            }
            catch (Exception ex)
            {
                // Тихо ігноруємо помилки доступу (може файл заблокований на мілісекунду)
            }
        }

        private void CheckAndInstallServiceAuto()
        {
            try
            {
                string status = _serviceAdmin.GetStatus();
                if (status == "Not Installed")
                {
                    Log("Сервіс не знайдено. Спроба автоматичної реєстрації...");
                    bool success = InstallServiceSilently();
                    if (success)
                    {
                        Log("Сервіс успішно зареєстровано.");
                        // Можна одразу і запустити, якщо треба:
                        // _serviceAdmin.Start(); 
                    }
                    else
                    {
                        Log("ПОМИЛКА: Не вдалося знайти файл TradeSync.Service.exe для авто-встановлення.");
                        MessageBox.Show("Критична помилка: Файл сервісу відсутній. Перевстановіть програму.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Помилка авто-встановлення сервісу: {ex.Message}");
            }
        }

        private bool InstallServiceSilently()
        {
            string serviceExeName = "TradeSync.Service.exe";
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;

            // Пріоритет пошуку:
            // 1. Поточна папка (завдяки кроку 1 у .csproj файлі він має бути тут)
            // 2. Папки розробки (на випадок, якщо крок 1 не спрацював)
            string[] pathsToCheck = new[]
            {
            Path.Combine(currentDir, serviceExeName),
            Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\..\TradeSync.Service\bin\Debug\net8.0", serviceExeName)),
            Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\..\TradeSync.Service\bin\Release\net8.0", serviceExeName))
            };

            string foundPath = pathsToCheck.FirstOrDefault(File.Exists);

            if (foundPath != null)
            {
                _serviceAdmin.Install(foundPath);
                return true;
            }
            return false;
        }

        // Кнопка "Встановити" тепер теж працює на автоматі (якщо користувач натисне вручну)
        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (InstallServiceSilently())
            {
                MessageBox.Show("Сервіс встановлено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateServiceStatus();
            }
            else
            {
                MessageBox.Show("Не вдалося знайти файл TradeSync.Service.exe автоматично.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- БЛОК КЕРУВАННЯ СЕРВІСОМ ---

        private void UpdateServiceStatus()
        {
            string status = _serviceAdmin.GetStatus();
            lblServiceStatus.Text = $"Статус сервісу: {status}";

            // Проста логіка активації кнопок
            bool isInstalled = status != "Not Installed";
            bool isRunning = status == "Running";

            btnInstall.Enabled = !isInstalled;
            btnUninstall.Enabled = isInstalled;

            btnStart.Enabled = isInstalled && !isRunning;
            btnStop.Enabled = isRunning;

            // Змінюємо колір для наочності
            if (isRunning) lblServiceStatus.ForeColor = Color.Green;
            else if (isInstalled) lblServiceStatus.ForeColor = Color.Orange;
            else lblServiceStatus.ForeColor = Color.Red;
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Видалити службу?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _serviceAdmin.Uninstall();
                    UpdateServiceStatus();
                }
                catch (Exception ex) { MessageBox.Show("Помилка: " + ex.Message); }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _serviceAdmin.Start();
                UpdateServiceStatus();
            }
            catch (Exception ex) { MessageBox.Show("Помилка запуску: " + ex.Message); }
            finally { Cursor = Cursors.Default; }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _serviceAdmin.Stop();
                UpdateServiceStatus();
            }
            catch (Exception ex) { MessageBox.Show("Помилка зупинки: " + ex.Message); }
            finally { Cursor = Cursors.Default; }
        }

        // --- БЛОК СИНХРОНІЗАЦІЇ (Залишається як був) ---

        private async Task LoadTableListAsync()
        {
            if (!File.Exists(StructureFile))
            {
                Log("УВАГА: structure.json не знайдено. Скопіюйте його в папку з програмою.");
                return;
            }
            try
            {
                var json = await File.ReadAllTextAsync(StructureFile);
                var tables = JsonSerializer.Deserialize<List<TableSchema>>(json);
                cmbTables.Items.Clear();
                if (tables != null)
                    foreach (var table in tables) cmbTables.Items.Add(table.Name1C);
            }
            catch (Exception ex) { Log("Помилка JSON: " + ex.Message); }
        }

        private async void btnSync_Click(object sender, EventArgs e)
        {
            btnSync.Enabled = false;
            progressBar1.Value = 0;
            Log("=== Початок синхронізації ===");

            try
            {
                var syncManager = new SyncManager(AuxConnString, LocalDbPath, StructureFile);

                syncManager.OnLog += (msg) => Invoke(() => Log(msg));
                syncManager.OnProgress += (curr, total) => Invoke(() =>
                {
                    progressBar1.Maximum = total;
                    progressBar1.Value = curr;
                    lblStatus.Text = $"Обробка: {curr}/{total}";
                });

                await Task.Run(() => syncManager.RunSyncAsync());

                Log("=== Завершено ===");
                lblStatus.Text = "Готово";

                if (cmbTables.SelectedItem != null)
                    LoadPreviewData(cmbTables.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                Log($"ПОМИЛКА: {ex.Message}");
            }
            finally
            {
                btnSync.Enabled = true;
            }
        }

        private void cmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTables.SelectedItem != null)
                LoadPreviewData(cmbTables.SelectedItem.ToString());
        }

        private void LoadPreviewData(string onEcName)
        {
            string tableName = _sqliteBuilder.GetLocalTableName(onEcName);
            Log($"Читання таблиці: {tableName}");

            try
            {
                string connStr = $"Data Source={LocalDbPath};Version=3;";
                if (!File.Exists(LocalDbPath)) return;

                using (var conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand($"SELECT * FROM [{tableName}] LIMIT 100", conn);
                    var da = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex) { Log("Не вдалося показати дані: " + ex.Message); }
        }

        private void Log(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            rtbLog.AppendText($"[{time}] {message}{Environment.NewLine}");
            rtbLog.ScrollToCaret();
        }
    }
}