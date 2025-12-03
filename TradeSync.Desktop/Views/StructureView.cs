using System.Text.Json;
using TradeSync.Core.Models;

namespace TradeSync.Desktop.Views
{
    public class StructureView : UserControl
    {
        private const string StructureFile = "structure.json";

        // Дані
        private List<TableSchema> _tables;
        private bool _isUpdatingUi = false; // Блокувальник подій
        private bool _isEditMode = false;   // Режим редагування

        // UI Елементи
        private ListBox _lstTables;
        private TextBox _txtName1C;
        private TextBox _txtSqlTable;
        private DataGridView _gridFields;
        private Button _btnEditMode;
        private Button _btnSave;
        private Button _btnAdd;
        private Button _btnDel;

        public StructureView()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            InitializeLayout();
            LoadDataAsync();
        }

        private void InitializeLayout()
        {
            // Розподіл: 40% список, 60% деталі (щоб влазили назви)
            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(10) };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            // === ЛІВА ПАНЕЛЬ (СПИСОК) ===
            var leftPanel = new Panel { Dock = DockStyle.Fill };

            _lstTables = new ListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false,
                Font = new Font("Segoe UI", 10),
                HorizontalScrollbar = true // <--- ВАЖЛИВО: Скрол для довгих назв
            };
            _lstTables.SelectedIndexChanged += OnTableSelected;

            var btnPanelLeft = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 40, ColumnCount = 2 };
            btnPanelLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            btnPanelLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            _btnAdd = CreateButton("+", Color.MediumSeaGreen, Color.White, () => AddTable());
            _btnDel = CreateButton("-", Color.IndianRed, Color.White, () => DeleteTable());
            _btnAdd.Enabled = false; // Блокуємо, поки не увімкнуть редагування
            _btnDel.Enabled = false;

            btnPanelLeft.Controls.Add(_btnAdd, 0, 0);
            btnPanelLeft.Controls.Add(_btnDel, 1, 0);

            var btnImport = CreateButton("📂 Імпорт JSON...", Color.Orange, Color.White, () => ImportJson());
            btnImport.Dock = DockStyle.Bottom;
            btnImport.Height = 35;

            leftPanel.Controls.Add(_lstTables);
            leftPanel.Controls.Add(btnPanelLeft);
            leftPanel.Controls.Add(btnImport);
            leftPanel.Controls.Add(new Label { Text = "Список таблиць:", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9, FontStyle.Bold) });

            // === ПРАВА ПАНЕЛЬ (ДЕТАЛІ) ===
            var rightLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4 };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Header (Edit button)
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F)); // Meta Inputs
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Grid
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Save Button

            // 1. Верхній рядок: Кнопка "Редагувати"
            var headerPanel = new Panel { Dock = DockStyle.Fill };
            _btnEditMode = CreateButton("🔒 Режим перегляду (Тільки читання)", Color.LightYellow, Color.Black, () => ToggleEditMode());
            _btnEditMode.Dock = DockStyle.Right;
            _btnEditMode.Width = 250;
            headerPanel.Controls.Add(_btnEditMode);
            headerPanel.Controls.Add(new Label { Text = "Деталі:", Dock = DockStyle.Left, AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft });

            // 2. Поля введення (Meta)
            var metaLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
            metaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            metaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            _txtName1C = new TextBox { Dock = DockStyle.Top, Font = new Font("Consolas", 10), ReadOnly = true, BackColor = Color.WhiteSmoke };
            _txtName1C.TextChanged += (s, e) => UpdateModel();

            _txtSqlTable = new TextBox { Dock = DockStyle.Top, Font = new Font("Consolas", 10), ReadOnly = true, BackColor = Color.WhiteSmoke };
            _txtSqlTable.TextChanged += (s, e) => UpdateModel();

            metaLayout.Controls.Add(new Label { Text = "Назва 1C (Name):", AutoSize = true }, 0, 0);
            metaLayout.Controls.Add(new Label { Text = "SQL Таблиця (SQLTable):", AutoSize = true }, 1, 0);
            metaLayout.Controls.Add(_txtName1C, 0, 1);
            metaLayout.Controls.Add(_txtSqlTable, 1, 1);

            // 3. Грід
            _gridFields = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.WhiteSmoke, // Сірий, поки ReadOnly
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                ReadOnly = true, // Заблоковано за замовчуванням
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
            _gridFields.Columns.Add("Human", "Локальне ім'я (SQLite)");
            _gridFields.Columns.Add("Sql", "Поле в SQL 1C");

            // 4. Кнопка Save
            _btnSave = CreateButton("💾 Зберегти зміни", Color.SteelBlue, Color.White, () => SaveJson());
            _btnSave.Enabled = false; // Активна тільки в режимі редагування

            rightLayout.Controls.Add(headerPanel, 0, 0);
            rightLayout.Controls.Add(metaLayout, 0, 1);
            rightLayout.Controls.Add(_gridFields, 0, 2);
            rightLayout.Controls.Add(_btnSave, 0, 3);

            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(rightLayout, 1, 0);
            this.Controls.Add(mainLayout);
        }

        // --- ЛОГІКА РЕЖИМІВ ---

        private void ToggleEditMode()
        {
            _isEditMode = !_isEditMode;

            // Змінюємо вигляд кнопки режиму
            if (_isEditMode)
            {
                _btnEditMode.Text = "🔓 РЕЖИМ РЕДАГУВАННЯ (АКТИВНИЙ)";
                _btnEditMode.BackColor = Color.Orange;
                _gridFields.BackgroundColor = Color.White;
            }
            else
            {
                _btnEditMode.Text = "🔒 Режим перегляду (Тільки читання)";
                _btnEditMode.BackColor = Color.LightYellow;
                _gridFields.BackgroundColor = Color.WhiteSmoke;
            }

            // Розблокуємо/Блокуємо контроли
            _txtName1C.ReadOnly = !_isEditMode;
            _txtSqlTable.ReadOnly = !_isEditMode;

            _txtName1C.BackColor = _isEditMode ? Color.White : Color.WhiteSmoke;
            _txtSqlTable.BackColor = _isEditMode ? Color.White : Color.WhiteSmoke;

            _gridFields.ReadOnly = !_isEditMode;
            _gridFields.AllowUserToAddRows = _isEditMode;
            _gridFields.AllowUserToDeleteRows = _isEditMode;

            _btnAdd.Enabled = _isEditMode;
            _btnDel.Enabled = _isEditMode;
            _btnSave.Enabled = _isEditMode;
        }

        // --- ЛОГІКА ДАНИХ ---

        private async void LoadDataAsync()
        {
            if (!File.Exists(StructureFile)) return;
            try
            {
                var json = await File.ReadAllTextAsync(StructureFile);
                _tables = JsonSerializer.Deserialize<List<TableSchema>>(json) ?? new List<TableSchema>();
                RefreshList();
            }
            catch { _tables = new List<TableSchema>(); }
        }

        private void RefreshList()
        {
            _lstTables.Items.Clear();
            foreach (var t in _tables) _lstTables.Items.Add(t.Name1C);
        }

        private void OnTableSelected(object sender, EventArgs e)
        {
            int idx = _lstTables.SelectedIndex;
            if (idx < 0 || idx >= _tables.Count) return;

            var t = _tables[idx];

            // Вмикаємо прапорець, щоб TextChanged не спрацював і не подумав, що ми редагуємо
            _isUpdatingUi = true;

            _txtName1C.Text = t.Name1C;
            _txtSqlTable.Text = t.SqlTableNameSource; // Це значення з JSON (наприклад "_Reference283")

            _gridFields.Rows.Clear();
            foreach (var f in t.Fields) _gridFields.Rows.Add(f.Key, f.Value);

            _isUpdatingUi = false;
        }

        private void UpdateModel()
        {
            // Якщо ми просто завантажуємо дані в бокси - не оновлюємо модель
            if (_isUpdatingUi) return;

            int idx = _lstTables.SelectedIndex;
            if (idx >= 0)
            {
                _tables[idx].Name1C = _txtName1C.Text;
                _tables[idx].SqlTableNameSource = _txtSqlTable.Text;
            }
        }

        // --- ОПЕРАЦІЇ ---

        private void ImportJson()
        {
            using (var ofd = new OpenFileDialog { Filter = "JSON|*.json" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var json = File.ReadAllText(ofd.FileName);
                        _tables = JsonSerializer.Deserialize<List<TableSchema>>(json);
                        RefreshList();
                        MessageBox.Show("Структуру імпортовано! Не забудьте перевірити і зберегти.");
                    }
                    catch (Exception ex) { MessageBox.Show("Помилка: " + ex.Message); }
                }
            }
        }

        private void AddTable()
        {
            _tables.Add(new TableSchema { Name1C = "NewTable", SqlTableNameSource = "_Tbl", Fields = new Dictionary<string, string>() });
            RefreshList();
            _lstTables.SelectedIndex = _tables.Count - 1;
        }

        private void DeleteTable()
        {
            int idx = _lstTables.SelectedIndex;
            if (idx >= 0 && MessageBox.Show("Видалити таблицю?", "Підтвердження", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _tables.RemoveAt(idx);
                RefreshList();
                // Очистка
                _isUpdatingUi = true;
                _txtName1C.Clear(); _txtSqlTable.Clear(); _gridFields.Rows.Clear();
                _isUpdatingUi = false;
            }
        }

        private void SaveJson()
        {
            // Перед збереженням забираємо дані з Гріда в модель
            int idx = _lstTables.SelectedIndex;
            if (idx >= 0)
            {
                var dict = new Dictionary<string, string>();
                foreach (DataGridViewRow r in _gridFields.Rows)
                    if (!r.IsNewRow && r.Cells[0].Value != null)
                        dict[r.Cells[0].Value.ToString()] = r.Cells[1].Value?.ToString() ?? "";
                _tables[idx].Fields = dict;
            }

            var opts = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(StructureFile, JsonSerializer.Serialize(_tables, opts));
            MessageBox.Show("Файл structure.json успішно збережено!");
        }

        private Button CreateButton(string t, Color bg, Color fg, Action act)
        {
            var b = new Button { Text = t, Dock = DockStyle.Fill, FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = fg, Cursor = Cursors.Hand, Margin = new Padding(2) };
            b.FlatAppearance.BorderSize = 0;
            b.Click += (s, e) => act();
            return b;
        }
    }
}