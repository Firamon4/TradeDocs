using TradeSync.Core.Logic; 
using TradeSync.Desktop.Views;

namespace TradeSync.Desktop
{
    public partial class MainForm : Form
    {
        private Panel _menuPanel;
        private Panel _contentPanel;

        // В'юшки
        private DataView _viewData;
        private ServiceView _viewService;
        private StructureView _viewStructure;
        private SettingsView _viewSettings;

        public MainForm()
        {
            this.Text = "TradeSync Admin";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);

            InitializeShell();
            InitializeViews();
        }

        private void InitializeShell()
        {
            _menuPanel = new Panel { Dock = DockStyle.Left, Width = 220, BackColor = Color.FromArgb(32, 34, 37) };

            var lblLogo = new Label { Text = "TradeSync", Dock = DockStyle.Top, Height = 80, ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };

            var btnData = CreateMenuButton("📊 Дані", () => ShowView(_viewData));
            var btnService = CreateMenuButton("⚙️ Сервіс", () => ShowView(_viewService));
            var btnStruct = CreateMenuButton("📂 Структура", () => ShowView(_viewStructure));
            var btnSettings = CreateMenuButton("🛠 Налаштування", () => ShowView(_viewSettings));

            _menuPanel.Controls.AddRange(new Control[] { btnSettings, btnStruct, btnService, btnData, lblLogo });

            _contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke, Padding = new Padding(10) };

            this.Controls.Add(_contentPanel);
            this.Controls.Add(_menuPanel);
        }

        private void InitializeViews()
        {
            // Ініціалізація компонентів
            _viewData = new DataView();
            _viewService = new ServiceView();
            _viewStructure = new StructureView();
            _viewSettings = new SettingsView();

            // Зв'язок: Якщо в налаштуваннях змінили базу -> оновити DataView
            _viewSettings.OnConfigChanged += () => _viewData.ReloadConfig();

            ShowView(_viewData);
        }

        private async void ShowView(UserControl newView)
        {
            // 1. ПЕРЕВІРКА НА ЗБЕРЕЖЕННЯ
            if (_contentPanel.Controls.Count > 0)
            {
                var current = _contentPanel.Controls[0] as ISaveable;
                if (current != null && current.HasUnsavedChanges)
                {
                    var res = MessageBox.Show("Є незбережені зміни. Зберегти перед переходом?", "Увага", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (res == DialogResult.Cancel) return;
                    if (res == DialogResult.Yes) await current.SaveAsync();
                    else current.DiscardChanges();
                }
            }

            // 2. Переключення
            _contentPanel.Controls.Clear();
            if (newView != null)
            {
                newView.Dock = DockStyle.Fill;
                _contentPanel.Controls.Add(newView);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_contentPanel.Controls.Count > 0)
            {
                var current = _contentPanel.Controls[0] as ISaveable;
                if (current != null && current.HasUnsavedChanges)
                {
                    var res = MessageBox.Show("Зберегти зміни перед виходом?", "Вихід", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (res == DialogResult.Cancel) { e.Cancel = true; return; }
                    if (res == DialogResult.Yes)
                    {
                        // Синхронний запуск збереження (хак для OnClosing)
                        Task.Run(async () => await current.SaveAsync()).Wait();
                    }
                }
            }
            base.OnFormClosing(e);
        }

        private Button CreateMenuButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Text = "  " + text,
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(32, 34, 37),
                ForeColor = Color.Gainsboro,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => {
                foreach (Control c in _menuPanel.Controls) if (c is Button b) b.BackColor = Color.FromArgb(32, 34, 37);
                ((Button)s).BackColor = Color.FromArgb(45, 47, 50);
                onClick();
            };
            return btn;
        }
    }
}