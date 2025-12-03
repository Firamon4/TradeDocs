using Microsoft.Data.SqlClient;
using System.Data;
using TradeSync.Desktop.Logic;
using TradeSync.Core.Logic; // Підключаємо нашу Core логіку
using TradeSync.Core.Models;

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
        private Button _btnTestSrc;

        // Поля Aux
        private TextBox _auxServer, _auxDb, _auxUser, _auxPass;
        private CheckBox _chkAuxWinAuth;
        private Button _btnTestAux;

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
            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(20) };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

            // Групи налаштувань
            var grpSource = CreateConnectionGroup("Джерело (1C MSSQL)",
                out _srcServer, out _srcDb, out _srcUser, out _srcPass, out _chkSrcWinAuth, out _btnTestSrc);

            var grpAux = CreateConnectionGroup("Проміжна база (Aux MSSQL)",
                out _auxServer, out _auxDb, out _auxUser, out _auxPass, out _chkAuxWinAuth, out _btnTestAux);

            // Кнопка збереження
            var btnSave = new Button
            {
                Text = "💾 ЗБЕРЕГТИ НАЛАШТУВАННЯ",
                Dock = DockStyle.Fill,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.Click += async (s, e) => await SaveAsync();

            // Події кнопок тесту
            _btnTestSrc.Click += async (s, e) => await TestConn(_srcServer, _srcDb, _srcUser, _srcPass, _chkSrcWinAuth);
            _btnTestAux.Click += async (s, e) => await TestConn(_auxServer, _auxDb, _auxUser, _auxPass, _chkAuxWinAuth);

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
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 10, Padding = new Padding(10) };

            var chkWin = new CheckBox { Text = "Windows Authentication", AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(0, 0, 0, 10) };
            chkWinOut = chkWin;
            layout.Controls.Add(chkWin, 0, 0);

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

            btnTestOut = new Button { Text = "🔌 Перевірити з'єднання", Dock = DockStyle.Bottom, Height = 35, BackColor = Color.WhiteSmoke, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9), Cursor = Cursors.Hand };
            layout.Controls.Add(btnTestOut, 0, 9);

            // Логіка блокування полів
            // Важливо: копіюємо змінні для замикання
            var u = tUsr; var p = tPwd; var c = chkWin;
            c.CheckedChanged += (s, e) => {
                u.Enabled = !c.Checked;
                p.Enabled = !c.Checked;
                if (c.Checked) { u.Clear(); p.Clear(); }
                MarkAsDirty();
            };

            grp.Controls.Add(layout);
            return grp;
        }

        private async Task TestConn(TextBox srv, TextBox db, TextBox usr, TextBox pwd, CheckBox chk)
        {
            string connStr = BuildStr(srv, db, usr, pwd, chk);
            var builder = new SqlConnectionStringBuilder(connStr);
            builder.ConnectTimeout = 3; // Швидкий тест

            try
            {
                Cursor = Cursors.WaitCursor;
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    await conn.OpenAsync();
                }
                MessageBox.Show("✅ З'єднання успішне!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Помилка:\n{ex.Message}", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { Cursor = Cursors.Default; }
        }

        private void MarkAsDirty() { if (_isLoaded) _hasUnsavedChanges = true; }
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
                    if (MessageBox.Show("Перезапустити сервіс для застосування змін?", "Збережено", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Cursor = Cursors.WaitCursor;
                        await Task.Run(() => { _admin.Stop(); Thread.Sleep(1000); _admin.Start(); });
                        Cursor = Cursors.Default;
                        MessageBox.Show("Сервіс перезапущено.");
                    }
                }
                else MessageBox.Show("Збережено.");
            }
            catch (Exception ex) { MessageBox.Show("Помилка: " + ex.Message); }
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
                if (!b.IntegratedSecurity) { usr.Text = b.UserID; pwd.Text = b.Password; }
                usr.Enabled = !chk.Checked; pwd.Enabled = !chk.Checked;
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
            if (!chk.Checked) { b.UserID = usr.Text; b.Password = pwd.Text; }
            return b.ConnectionString;
        }
    }
}