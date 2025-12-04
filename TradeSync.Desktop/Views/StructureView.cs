using System.Text.Json;
using TradeSync.Core.Models;

namespace TradeSync.Desktop.Views
{
    public class StructureView : UserControl
    {
        private const string StructureFile = "structure.json";
        private List<TableSchema> _tables;
        private bool _isEditMode = false;
        private bool _isUpdatingUi = false;

        // UI
        private ListBox _lstTables;
        private TextBox _txtName, _txtSql;
        private DataGridView _grid;
        private Button _btnEdit, _btnSave, _btnAdd, _btnDel;

        public StructureView()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            InitializeLayout();
            LoadData();
        }

        private void InitializeLayout()
        {
            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 350, SplitterWidth = 5, FixedPanel = FixedPanel.Panel1 };

            // === ЛІВА ПАНЕЛЬ ===
            var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            _lstTables = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false, Font = new Font("Segoe UI", 10), HorizontalScrollbar = true };
            _lstTables.SelectedIndexChanged += (s, e) => LoadDetails();

            var leftBtns = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 40, ColumnCount = 2 };
            leftBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); leftBtns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _btnAdd = Btn("+", Color.Gainsboro, Color.Black, AddTable);
            _btnDel = Btn("-", Color.IndianRed, Color.White, DeleteTable);
            leftBtns.Controls.Add(_btnAdd, 0, 0); leftBtns.Controls.Add(_btnDel, 1, 0);

            var btnImport = Btn("📂 Імпорт JSON", Color.Orange, Color.White, ImportJson);
            btnImport.Dock = DockStyle.Bottom;

            left.Controls.Add(_lstTables);
            left.Controls.Add(leftBtns);
            left.Controls.Add(btnImport);
            left.Controls.Add(new Label { Text = "Таблиці:", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9, FontStyle.Bold) });

            // === ПРАВА ПАНЕЛЬ ===
            var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            var header = new Panel { Dock = DockStyle.Top, Height = 45 };
            _btnEdit = Btn("🔒 Режим перегляду", Color.LightYellow, Color.Black, ToggleEdit);
            _btnEdit.Dock = DockStyle.Right; _btnEdit.Width = 200;
            header.Controls.Add(_btnEdit);
            header.Controls.Add(new Label { Text = "Налаштування:", Dock = DockStyle.Left, AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) });

            var meta = new TableLayoutPanel { Dock = DockStyle.Top, Height = 60, ColumnCount = 2 };
            meta.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); meta.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            _txtName = new TextBox { Dock = DockStyle.Top, ReadOnly = true };
            _txtName.TextChanged += (s, e) => UpdateModelHeader(); // Оновлення назви в реальному часі

            _txtSql = new TextBox { Dock = DockStyle.Top, ReadOnly = true };
            _txtSql.TextChanged += (s, e) => UpdateModelHeader();

            meta.Controls.Add(new Label { Text = "Назва 1С:", AutoSize = true }, 0, 0);
            meta.Controls.Add(new Label { Text = "SQL Table:", AutoSize = true }, 1, 0);
            meta.Controls.Add(_txtName, 0, 1);
            meta.Controls.Add(_txtSql, 1, 1);

            // Грід з типами
            _grid = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.WhiteSmoke, ReadOnly = true, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false };

            _grid.Columns.Add("Local", "Локальне ім'я");
            _grid.Columns.Add("Sql", "Поле в SQL");

            // Колонка з типами
            var typeCol = new DataGridViewComboBoxColumn();
            typeCol.HeaderText = "Тип даних";
            typeCol.Name = "Type";
            typeCol.Items.AddRange("String", "Guid", "Boolean", "Decimal", "Int", "Binary", "DateTime");
            _grid.Columns.Add(typeCol);

            _btnSave = Btn("💾 Зберегти структуру", Color.SteelBlue, Color.White, SaveJson);
            _btnSave.Dock = DockStyle.Bottom; _btnSave.Enabled = false;

            right.Controls.Add(_grid);
            right.Controls.Add(new Label { Text = "Маппінг:", Dock = DockStyle.Top, Height = 25 });
            right.Controls.Add(meta);
            right.Controls.Add(header);
            right.Controls.Add(_btnSave);

            split.Panel1.Controls.Add(left);
            split.Panel2.Controls.Add(right);
            this.Controls.Add(split);
        }

        private void LoadDetails()
        {
            int idx = _lstTables.SelectedIndex;
            if (idx < 0 || idx >= _tables.Count) return;
            var t = _tables[idx];

            _isUpdatingUi = true;
            _txtName.Text = t.Name;
            _txtSql.Text = t.SQLTable;

            _grid.Rows.Clear();
            foreach (var c in t.Columns)
            {
                _grid.Rows.Add(c.Local, c.Sql, c.Type);
            }
            _isUpdatingUi = false;
        }

        private void UpdateModelHeader()
        {
            if (_isUpdatingUi) return;
            int idx = _lstTables.SelectedIndex;
            if (idx >= 0)
            {
                _tables[idx].Name = _txtName.Text;
                _tables[idx].SQLTable = _txtSql.Text;
            }
        }

        private void SaveJson()
        {
            // Зберігаємо Грід в модель перед записом
            int idx = _lstTables.SelectedIndex;
            if (idx >= 0)
            {
                var newCols = new List<ColumnInfo>();
                foreach (DataGridViewRow r in _grid.Rows)
                {
                    if (!r.IsNewRow && r.Cells[0].Value != null)
                    {
                        newCols.Add(new ColumnInfo
                        {
                            Local = r.Cells[0].Value?.ToString(),
                            Sql = r.Cells[1].Value?.ToString(),
                            Type = r.Cells[2].Value?.ToString() ?? "String"
                        });
                    }
                }
                _tables[idx].Columns = newCols;
            }

            var opts = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(StructureFile, JsonSerializer.Serialize(_tables, opts));
            MessageBox.Show("Збережено!");
        }

        private void ToggleEdit()
        {
            _isEditMode = !_isEditMode;
            _btnEdit.Text = _isEditMode ? "🔓 РЕДАГУВАННЯ" : "🔒 Режим перегляду";
            _btnEdit.BackColor = _isEditMode ? Color.Orange : Color.LightYellow;

            _txtName.ReadOnly = !_isEditMode;
            _txtSql.ReadOnly = !_isEditMode;
            _grid.ReadOnly = !_isEditMode;
            _grid.BackgroundColor = _isEditMode ? Color.White : Color.WhiteSmoke;
            _grid.AllowUserToAddRows = _isEditMode;
            _grid.AllowUserToDeleteRows = _isEditMode;
            _btnSave.Enabled = _isEditMode;
            _btnAdd.Enabled = _isEditMode;
            _btnDel.Enabled = _isEditMode;
        }

        // --- Helpers ---
        private async void LoadData()
        {
            if (File.Exists(StructureFile))
            {
                var j = await File.ReadAllTextAsync(StructureFile);
                _tables = JsonSerializer.Deserialize<List<TableSchema>>(j) ?? new();
                RefreshList();
            }
            else _tables = new();
        }
        private void RefreshList() { _lstTables.Items.Clear(); foreach (var t in _tables) _lstTables.Items.Add(t.Name); }

        private void AddTable()
        {
            _tables.Add(new TableSchema { Name = "New", SQLTable = "_Tbl", Columns = new() });
            RefreshList(); _lstTables.SelectedIndex = _tables.Count - 1;
        }

        private void DeleteTable()
        {
            int i = _lstTables.SelectedIndex;
            if (i >= 0 && MessageBox.Show("Del?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            { _tables.RemoveAt(i); RefreshList(); }
        }

        private void ImportJson()
        {
            using var o = new OpenFileDialog();
            if (o.ShowDialog() == DialogResult.OK)
            {
                var j = File.ReadAllText(o.FileName);
                _tables = JsonSerializer.Deserialize<List<TableSchema>>(j);
                RefreshList();
            }
        }

        private Button Btn(string t, Color b, Color f, Action a) { var btn = new Button { Text = t, BackColor = b, ForeColor = f, FlatStyle = FlatStyle.Flat, Height = 35, Margin = new Padding(2) }; btn.Click += (s, e) => a(); return btn; }
    }
}