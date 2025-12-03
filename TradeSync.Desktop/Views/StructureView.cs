using System.Text.Json;
using TradeSync.Core.Models;

namespace TradeSync.Desktop.Views
{
    public class StructureView : UserControl
    {
        private const string StructureFile = "structure.json";
        private ListBox _lstTables;
        private TextBox _txtName1C;
        private TextBox _txtSqlTable;
        private DataGridView _gridFields;
        private List<TableSchema> _tables;

        public StructureView()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            InitializeLayout();
            LoadDataAsync();
        }

        private void InitializeLayout()
        {
            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(10) };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            // LEFT
            var leftPanel = new Panel { Dock = DockStyle.Fill };
            _lstTables = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false, Font = new Font("Segoe UI", 10) };
            _lstTables.SelectedIndexChanged += OnTableSelected;

            var btnPanel = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 40, ColumnCount = 2 };
            btnPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            btnPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            btnPanel.Controls.Add(CreateButton("+ Add", Color.Gainsboro, Color.Black, () => AddTable()), 0, 0);
            btnPanel.Controls.Add(CreateButton("- Del", Color.IndianRed, Color.White, () => DeleteTable()), 1, 0);

            // Кнопка імпорту
            var btnImport = CreateButton("📂 Імпорт JSON...", Color.SteelBlue, Color.White, () => ImportJson());
            btnImport.Dock = DockStyle.Bottom;
            btnImport.Height = 40;

            leftPanel.Controls.Add(_lstTables);
            leftPanel.Controls.Add(btnPanel);
            leftPanel.Controls.Add(btnImport); // Added Import
            leftPanel.Controls.Add(new Label { Text = "Таблиці:", Dock = DockStyle.Top, Height = 25 });

            // RIGHT
            var rightLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3 };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            var metaLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            metaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            metaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            _txtName1C = new TextBox { Dock = DockStyle.Top, Font = new Font("Consolas", 10) };
            _txtName1C.TextChanged += (s, e) => UpdateModel();
            _txtSqlTable = new TextBox { Dock = DockStyle.Top, Font = new Font("Consolas", 10) };
            _txtSqlTable.TextChanged += (s, e) => UpdateModel();

            metaLayout.Controls.Add(new Label { Text = "Назва 1C:", AutoSize = true }, 0, 0);
            metaLayout.Controls.Add(new Label { Text = "SQL Table:", AutoSize = true }, 1, 0);
            metaLayout.Controls.Add(_txtName1C, 0, 1);
            metaLayout.Controls.Add(_txtSqlTable, 1, 1);

            _gridFields = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false };
            _gridFields.Columns.Add("Human", "Local Name");
            _gridFields.Columns.Add("Sql", "SQL Name");

            rightLayout.Controls.Add(metaLayout, 0, 0);
            rightLayout.Controls.Add(_gridFields, 0, 1);
            rightLayout.Controls.Add(CreateButton("💾 Зберегти зміни", Color.SeaGreen, Color.White, () => SaveJson()), 0, 2);

            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(rightLayout, 1, 0);
            this.Controls.Add(mainLayout);
        }

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

        // Кнопка Імпорту
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
                        MessageBox.Show("Структуру імпортовано!");
                    }
                    catch (Exception ex) { MessageBox.Show("Помилка: " + ex.Message); }
                }
            }
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
            _txtName1C.Text = t.Name1C;
            _txtSqlTable.Text = t.SqlTableNameSource; // ВИПРАВЛЕНО: тепер поле заповнюється

            _gridFields.Rows.Clear();
            foreach (var f in t.Fields) _gridFields.Rows.Add(f.Key, f.Value);
        }

        private void UpdateModel()
        {
            int idx = _lstTables.SelectedIndex;
            if (idx >= 0) { _tables[idx].Name1C = _txtName1C.Text; _tables[idx].SqlTableNameSource = _txtSqlTable.Text; }
        }

        private void AddTable() { _tables.Add(new TableSchema { Name1C = "New", SqlTableNameSource = "_Tbl", Fields = new Dictionary<string, string>() }); RefreshList(); }
        private void DeleteTable() { int idx = _lstTables.SelectedIndex; if (idx >= 0) { _tables.RemoveAt(idx); RefreshList(); } }

        private void SaveJson()
        {
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
            MessageBox.Show("Saved!");
        }

        private Button CreateButton(string t, Color bg, Color fg, Action act)
        {
            var b = new Button { Text = t, Dock = DockStyle.Fill, FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = fg, Cursor = Cursors.Hand, Margin = new Padding(2) };
            b.Click += (s, e) => act();
            return b;
        }
    }
}