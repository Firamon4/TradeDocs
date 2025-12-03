using Microsoft.Data.SqlClient;
using System.Data;
using TradeSync.Desktop.Logic;

namespace TradeSync.Desktop.Views
{
    public class SettingsView : UserControl, ISaveable
    {
        private readonly ConfigManager _configMgr = new ConfigManager();
        private readonly ServiceAdmin _admin = new ServiceAdmin();

        public event Action OnConfigChanged;

        // Поля 1С
        private TextBox _srcServer, _srcDb, _srcUser, _srcPass;
        private CheckBox _chkSrcWinAuth;
        private Button _btnTestSrc; // Кнопка тесту 1

        // Поля Aux
        private TextBox _auxServer, _auxDb, _auxUser, _auxPass;
        private CheckBox _chkAuxWinAuth;
        private Button _btnTestAux; // Кнопка тесту 2

        private bool _isLoaded = false;
        private bool _hasUnsavedChanges = false;
        public bool HasUnsavedChanges => _hasUnsavedChanges;

        public SettingsView()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            InitializeLayout();
            LoadConfigAsync();
        }

        private void InitializeLayout()
        {
            // Основна сітка
            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(20) };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Групи налаштувань розтягуються
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Кнопка збереження фіксована знизу

            // Створюємо групи (з кнопками тесту всередині)
            var grpSource = CreateConnectionGroup("Джерело (1C MSSQL)",
                out _srcServer, out _srcDb, out _srcUser, out _srcPass, out _chkSrcWinAuth, out _btnTestSrc);

            var grpAux = CreateConnectionGroup("Проміжна база (Aux MSSQL)",
                out _auxServer, out _auxDb, out _auxUser, out _auxPass, out _chkAuxWinAuth, out _btnTestAux);

            // Кнопка збереження (загальна)
            var btnSave = new Button
            {
                Text = "💾 Зберегти налаштування",
                Dock = DockStyle.Fill,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.Click += async (s, e) => await SaveAsync();

            // Додаємо події для кнопок тестування
            _btnTestSrc.Click += async (s, e) => await TestConnectionAsync(_srcServer, _srcDb, _srcUser, _srcPass, _chkSrcWinAuth);
            _btnTestAux.Click += async (s, e) => await TestConnectionAsync(_auxServer, _auxDb, _auxUser, _auxPass, _chkAuxWinAuth);

            // Розміщуємо на формі
            mainLayout.Controls.Add(grpSource, 0, 0);
            mainLayout.Controls.Add(grpAux, 1, 0);

            var btnPanel = new Panel { Dock = DockStyle.Fill };
            btnPanel.Controls.Add(btnSave);
            mainLayout.SetColumnSpan(btnPanel, 2);
            mainLayout.Controls.Add(btnPanel, 0, 1);

            this.Controls.Add(mainLayout);
        }

        private GroupBox CreateConnectionGroup(string title,
            out TextBox tSrv, out TextBox tDb, out TextBox tUsr, out TextBox tPwd, out CheckBox chkWinOut, out Button btnTestOut)
        {
            var grp = new GroupBox { Text = title, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            // Використовуємо 10 рядків: останній для кнопки тесту
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 10, Padding = new Padding(10) };

            // Чекбокс Windows Auth
            var chkWin = new CheckBox { Text = "Windows Authentication", AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(0, 0, 0, 10) };
            chkWinOut = chkWin;
            layout.Controls.Add(chkWin, 0, 0);

            // Локальна функція для полів
            void AddRow(string label, out TextBox txt, int row, bool isPass = false)
            {
                layout.Controls.Add(new Label { Text = label, AutoSize = true, Font = new Font("Segoe UI", 9) }, 0, row);
                txt = new TextBox { Dock = DockStyle.Top, Font = new Font("Consolas", 10) };
                if (isPass) txt.PasswordChar = '*';
                txt.TextChanged += (s, e) => MarkAsDirty();
                layout.Controls.Add(txt, 0, row + 1);
            }

            AddRow("Сервер:", out tSrv, 1);
            AddRow("База даних:", out tDb, 3);
            AddRow("Логін:", out tUsr, 5);
            AddRow("Пароль:", out tPwd, 7, true);

            // Кнопка перевірки підключення (всередині групи)
            btnTestOut = new Button
            {
                Text = "🔌 Перевірити підключення",
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            layout.Controls.Add(btnTestOut, 0, 9); // Додаємо в самий низ

            // Логіка блокування полів при Windows Auth
            var localUserBox = tUsr;
            var localPassBox = tPwd;
            var localCheckBox = chkWin;

            localCheckBox.CheckedChanged += (s, e) => {
                bool winAuth = localCheckBox.Checked;
                localUserBox.Enabled = !winAuth;
                localPassBox.Enabled = !winAuth;
                if (winAuth) { localUserBox.Clear(); localPassBox.Clear(); }
                MarkAsDirty();
            };

            grp.Controls.Add(layout);
            return grp;
        }

        // --- ЛОГІКА ПЕРЕВІРКИ ПІДКЛЮЧЕННЯ ---

        private async Task TestConnectionAsync(TextBox srv, TextBox db, TextBox usr, TextBox pwd, CheckBox chk)
        {
            // Збираємо рядок підключення прямо з полів
            string connStr = BuildStr(srv, db, usr, pwd, chk);

            // Додаємо Connection Timeout, щоб не чекати 30 секунд, якщо сервер не відповідає
            var builder = new SqlConnectionStringBuilder(connStr);
            builder.ConnectTimeout = 5; // 5 секунд на спробу

            try
            {
                Cursor = Cursors.WaitCursor;
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    await conn.OpenAsync();
                }
                MessageBox.Show("✅ Підключення успішне!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Помилка підключення:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // --- Standard Logic ---

        private void MarkAsDirty()
        {
            if (!_isLoaded) return;
            _hasUnsavedChanges = true;
        }

        public void DiscardChanges() => LoadConfigAsync();

        public async Task SaveAsync()
        {
            try
            {
                var cfg = await _configMgr.LoadAsync();
                cfg.ConnectionStrings.Source1C = BuildStr(_srcServer, _srcDb, _srcUser, _srcPass, _chkSrcWinAuth);
                cfg.ConnectionStrings.AuxDb = BuildStr(_auxServer, _auxDb, _auxUser, _auxPass, _chkAuxWinAuth);

                await _configMgr.SaveAsync(cfg);

                _hasUnsavedChanges = false;
                OnConfigChanged?.Invoke();

                if (_admin.GetStatus() == "Running")
                {
                    if (MessageBox.Show("Налаштування збережено. Перезапустити сервіс для застосування змін?", "Перезапуск", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Cursor = Cursors.WaitCursor;
                        await Task.Run(() => { _admin.Stop(); Thread.Sleep(1000); _admin.Start(); });
                        Cursor = Cursors.Default;
                        MessageBox.Show("Сервіс перезапущено.");
                    }
                }
                else MessageBox.Show("Налаштування збережено.");
            }
            catch (Exception ex) { MessageBox.Show("Помилка збереження: " + ex.Message); }
        }

        private async void LoadConfigAsync()
        {
            _isLoaded = false;
            var cfg = await _configMgr.LoadAsync();
            FillFields(cfg.ConnectionStrings.Source1C, _srcServer, _srcDb, _srcUser, _srcPass, _chkSrcWinAuth);
            FillFields(cfg.ConnectionStrings.AuxDb, _auxServer, _auxDb, _auxUser, _auxPass, _chkAuxWinAuth);
            _hasUnsavedChanges = false;
            _isLoaded = true;
        }

        private void FillFields(string connStr, TextBox srv, TextBox db, TextBox usr, TextBox pwd, CheckBox chk)
        {
            try
            {
                var b = new SqlConnectionStringBuilder(connStr);
                srv.Text = b.DataSource;
                db.Text = b.InitialCatalog;
                chk.Checked = b.IntegratedSecurity;
                if (!b.IntegratedSecurity)
                {
                    usr.Text = b.UserID;
                    pwd.Text = b.Password;
                }
                usr.Enabled = !chk.Checked;
                pwd.Enabled = !chk.Checked;
            }
            catch { }
        }

        private string BuildStr(TextBox srv, TextBox db, TextBox usr, TextBox pwd, CheckBox chk)
        {
            var b = new SqlConnectionStringBuilder
            {
                DataSource = srv.Text,
                InitialCatalog = db.Text,
                TrustServerCertificate = true,
                IntegratedSecurity = chk.Checked
            };
            if (!chk.Checked)
            {
                b.UserID = usr.Text;
                b.Password = pwd.Text;
            }
            return b.ConnectionString;
        }
    }
}