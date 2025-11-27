using System.Data;
using System.Text.Json;
using System.Data.SQLite;
using TradeSync.Desktop.Logic;
using TradeSync.Core.Models;

namespace TradeSync.Desktop
{
    public partial class MainForm : Form
    {
        // ! Замініть на реальні дані вашого сервера !
        private const string AuxConnString = "Server=YOUR_SERVER;Database=TradeAux;User Id=sa;Password=pass;TrustServerCertificate=True;";
        private const string LocalDbPath = "local_store.db";
        private const string StructureFile = "structure.json";

        private readonly SqliteBuilder _sqliteBuilder;

        public MainForm()
        {
            InitializeComponent();
            _sqliteBuilder = new SqliteBuilder();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            Log("Запуск програми...");
            await LoadTableListAsync();
        }

        // Завантаження списку таблиць з JSON в ComboBox
        private async Task LoadTableListAsync()
        {
            if (!File.Exists(StructureFile))
            {
                Log("Помилка: файл structure.json не знайдено!");
                return;
            }

            try
            {
                var json = await File.ReadAllTextAsync(StructureFile);
                var tables = JsonSerializer.Deserialize<List<TableSchema>>(json);

                cmbTables.Items.Clear();
                if (tables != null)
                {
                    foreach (var table in tables)
                    {
                        // Додаємо в список зрозумілі імена
                        cmbTables.Items.Add(table.Name1C);
                    }
                }
                Log("Структуру завантажено успішно.");
            }
            catch (Exception ex)
            {
                Log($"Помилка читання JSON: {ex.Message}");
            }
        }

        private async void btnSync_Click(object sender, EventArgs e)
        {
            btnSync.Enabled = false;
            progressBar1.Value = 0;
            Log("=== Початок синхронізації ===");

            try
            {
                var syncManager = new SyncManager(AuxConnString, LocalDbPath, StructureFile);

                // Підписка на події для оновлення UI з іншого потоку
                syncManager.OnLog += (msg) => Invoke(() => Log(msg));
                syncManager.OnProgress += (curr, total) => Invoke(() =>
                {
                    progressBar1.Maximum = total;
                    progressBar1.Value = curr;
                    lblStatus.Text = $"Обробка таблиці {curr} з {total}";
                });

                // Запуск у фоновому режимі, щоб не зависав інтерфейс
                await Task.Run(() => syncManager.RunSyncAsync());

                Log("=== Синхронізацію завершено ===");
                lblStatus.Text = "Готово";
                MessageBox.Show("Синхронізація успішна!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Оновити поточну таблицю, якщо обрана
                if (cmbTables.SelectedItem != null)
                {
                    LoadPreviewData(cmbTables.SelectedItem.ToString());
                }
            }
            catch (Exception ex)
            {
                Log($"КРИТИЧНА ПОМИЛКА: {ex.Message}");
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSync.Enabled = true;
            }
        }

        private void cmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTables.SelectedItem != null)
            {
                LoadPreviewData(cmbTables.SelectedItem.ToString());
            }
        }

        private void LoadPreviewData(string onEcName)
        {
            // Перетворюємо ім'я з 1С (напр. "Справочник.Номенклатура") в ім'я таблиці SQLite
            string tableName = _sqliteBuilder.GetLocalTableName(onEcName);

            Log($"Завантаження попереднього перегляду для: {tableName}");

            try
            {
                string connStr = $"Data Source={LocalDbPath};Version=3;";

                // Перевірка чи існує файл БД
                if (!File.Exists(LocalDbPath)) return;

                using (var conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    // Ліміт 1000 записів, щоб не забити пам'ять
                    var cmd = new SQLiteCommand($"SELECT * FROM [{tableName}] LIMIT 1000", conn);
                    var da = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                    lblStatus.Text = $"Відображено {dt.Rows.Count} записів з {tableName}";
                }
            }
            catch (Exception ex)
            {
                Log($"Не вдалося завантажити дані (можливо таблиця ще пуста): {ex.Message}");
            }
        }

        // Метод для безпечного додавання тексту в лог
        private void Log(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            rtbLog.AppendText($"[{time}] {message}{Environment.NewLine}");
            rtbLog.ScrollToCaret();
        }
    }
}