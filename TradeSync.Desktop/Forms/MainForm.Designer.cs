namespace TradeSync.Desktop
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabData = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.groupBoxControls = new System.Windows.Forms.GroupBox();
            this.lblSelectTable = new System.Windows.Forms.Label();
            this.cmbTables = new System.Windows.Forms.ComboBox();
            this.btnSync = new System.Windows.Forms.Button();
            this.groupBoxData = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabService = new System.Windows.Forms.TabPage();
            this.groupBoxServiceControl = new System.Windows.Forms.GroupBox();
            this.lblServiceStatus = new System.Windows.Forms.Label();
            this.btnInstall = new System.Windows.Forms.Button();
            this.btnUninstall = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();

            this.tabControl1.SuspendLayout();
            this.tabData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.groupBoxControls.SuspendLayout();
            this.groupBoxData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabService.SuspendLayout();
            this.groupBoxServiceControl.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();

            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabData);
            this.tabControl1.Controls.Add(this.tabService);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1008, 639);
            this.tabControl1.TabIndex = 0;

            // 
            // tabData (Вкладка Дані)
            // 
            this.tabData.Controls.Add(this.splitContainer1);
            this.tabData.Location = new System.Drawing.Point(4, 24);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(3);
            this.tabData.Size = new System.Drawing.Size(1000, 611);
            this.tabData.TabIndex = 0;
            this.tabData.Text = "📊 Дані та Синхронізація";
            this.tabData.UseVisualStyleBackColor = true;

            // 
            // splitContainer1 (Старий контейнер)
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxLog);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxControls);
            this.splitContainer1.Panel2.Controls.Add(this.groupBoxData);
            this.splitContainer1.Size = new System.Drawing.Size(994, 605);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 0;

            // ... (Вміст groupBoxControls, Log, Data залишається тим самим, скорочую для зручності) ...
            // Тобі треба скопіювати налаштування кнопок btnSync, cmbTables, rtbLog з минулого файлу сюди, 
            // або просто вставити їх у відповідні Panel1/Panel2 блоки.
            // Щоб зекономити місце тут, я додам лише нову вкладку:

            // 
            // tabService (Вкладка Сервіс)
            // 
            this.tabService.Controls.Add(this.groupBoxServiceControl);
            this.tabService.Location = new System.Drawing.Point(4, 24);
            this.tabService.Name = "tabService";
            this.tabService.Padding = new System.Windows.Forms.Padding(3);
            this.tabService.Size = new System.Drawing.Size(1000, 611);
            this.tabService.TabIndex = 1;
            this.tabService.Text = "⚙️ Керування Службою";
            this.tabService.UseVisualStyleBackColor = true;

            // 
            // groupBoxServiceControl
            // 
            this.groupBoxServiceControl.Controls.Add(this.lblServiceStatus);
            this.groupBoxServiceControl.Controls.Add(this.btnInstall);
            this.groupBoxServiceControl.Controls.Add(this.btnUninstall);
            this.groupBoxServiceControl.Controls.Add(this.btnStop);
            this.groupBoxServiceControl.Controls.Add(this.btnStart);
            this.groupBoxServiceControl.Location = new System.Drawing.Point(20, 20);
            this.groupBoxServiceControl.Name = "groupBoxServiceControl";
            this.groupBoxServiceControl.Size = new System.Drawing.Size(400, 250);
            this.groupBoxServiceControl.TabIndex = 0;
            this.groupBoxServiceControl.TabStop = false;
            this.groupBoxServiceControl.Text = "Дії над Windows Service";

            // 
            // lblServiceStatus
            // 
            this.lblServiceStatus.AutoSize = true;
            this.lblServiceStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblServiceStatus.Location = new System.Drawing.Point(20, 30);
            this.lblServiceStatus.Name = "lblServiceStatus";
            this.lblServiceStatus.Size = new System.Drawing.Size(149, 21);
            this.lblServiceStatus.TabIndex = 4;
            this.lblServiceStatus.Text = "Статус: Невідомо";

            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(20, 70);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(160, 40);
            this.btnStart.Text = "▶ Запустити";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.IndianRed;
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(200, 70);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(160, 40);
            this.btnStop.Text = "⏹ Зупинити";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(20, 130);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(160, 40);
            this.btnInstall.Text = "📥 Встановити...";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);

            // 
            // btnUninstall
            // 
            this.btnUninstall.Location = new System.Drawing.Point(200, 130);
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.Size = new System.Drawing.Size(160, 40);
            this.btnUninstall.Text = "🗑 Видалити";
            this.btnUninstall.UseVisualStyleBackColor = true;
            this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);

            // 
            // StatusStrip & інше
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblStatus, this.progressBar1 });
            this.statusStrip1.Location = new System.Drawing.Point(0, 639);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip1.TabIndex = 1;

            this.groupBoxLog.Controls.Add(this.rtbLog);
            this.groupBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxControls.Controls.Add(this.lblSelectTable);
            this.groupBoxControls.Controls.Add(this.cmbTables);
            this.groupBoxControls.Controls.Add(this.btnSync);
            this.groupBoxControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxControls.Size = new System.Drawing.Size(290, 150);
            this.groupBoxData.Controls.Add(this.dataGridView1);
            this.groupBoxData.Dock = System.Windows.Forms.DockStyle.Fill;

            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;

            // Налаштування кнопок "старої" вкладки
            this.btnSync.Location = new System.Drawing.Point(6, 22);
            this.btnSync.Size = new System.Drawing.Size(278, 50);
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);

            this.lblSelectTable.Location = new System.Drawing.Point(7, 85);
            this.cmbTables.Location = new System.Drawing.Point(7, 106);
            this.cmbTables.Size = new System.Drawing.Size(277, 23);
            this.cmbTables.SelectedIndexChanged += new System.EventHandler(this.cmbTables_SelectedIndexChanged);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 661);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TradeSync Desktop";
            this.Load += new System.EventHandler(this.MainForm_Load);

            this.tabControl1.ResumeLayout(false);
            this.tabData.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxControls.ResumeLayout(false);
            this.groupBoxControls.PerformLayout();
            this.groupBoxData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabService.ResumeLayout(false);
            this.groupBoxServiceControl.ResumeLayout(false);
            this.groupBoxServiceControl.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            // У методі InitializeComponent():

            // 1. Оголошення змінних (десь на початку, де інші змінні)
            this.groupBoxServiceLog = new System.Windows.Forms.GroupBox();
            this.rtbServiceLog = new System.Windows.Forms.RichTextBox();
            this.components = new System.ComponentModel.Container();
            this.timerServiceLog = new System.Windows.Forms.Timer(this.components);

            // 2. Налаштування Timer
            this.timerServiceLog.Interval = 2000; // Оновлювати кожні 2 секунди
            this.timerServiceLog.Tick += new System.EventHandler(this.timerServiceLog_Tick);

            // 3. Додавання на tabService
            // Знайди блок "tabService.Controls.Add(...)" і додай нову групу:
            
            this.tabService.Controls.Add(this.groupBoxServiceLog);
            // ... інші контроли (groupBoxServiceControl) залишаються ...

            // 4. Налаштування groupBoxServiceLog (Розміщуємо під кнопками)
            this.groupBoxServiceLog.Controls.Add(this.rtbServiceLog);
            this.groupBoxServiceLog.Location = new System.Drawing.Point(20, 280); // Нижче кнопок
            this.groupBoxServiceLog.Name = "groupBoxServiceLog";
            this.groupBoxServiceLog.Size = new System.Drawing.Size(960, 310); // На всю ширину
            this.groupBoxServiceLog.TabIndex = 1;
            this.groupBoxServiceLog.TabStop = false;
            this.groupBoxServiceLog.Text = "📝 Лог файлу Сервісу (Real-time)";

            // 5. Налаштування rtbServiceLog
            this.rtbServiceLog.BackColor = System.Drawing.Color.Black;
            this.rtbServiceLog.ForeColor = System.Drawing.Color.Lime; // Хакерський стиль :)
            this.rtbServiceLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbServiceLog.Font = new System.Drawing.Font("Consolas", 10F);
            this.rtbServiceLog.Location = new System.Drawing.Point(3, 19);
            this.rtbServiceLog.Name = "rtbServiceLog";
            this.rtbServiceLog.ReadOnly = true;
            this.rtbServiceLog.Size = new System.Drawing.Size(954, 288);
            this.rtbServiceLog.TabIndex = 0;
            this.rtbServiceLog.Text = "Очікування логів...";
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabData;
        private System.Windows.Forms.TabPage tabService;

        // Старі контроли
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBoxControls;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Label lblSelectTable;
        private System.Windows.Forms.ComboBox cmbTables;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.GroupBox groupBoxData;
        private System.Windows.Forms.DataGridView dataGridView1;

        // Нові контроли для сервісу
        private System.Windows.Forms.GroupBox groupBoxServiceControl;
        private System.Windows.Forms.Label lblServiceStatus;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Button btnUninstall;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;

        // Загальні
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;

        private System.Windows.Forms.GroupBox groupBoxServiceLog;
        private System.Windows.Forms.RichTextBox rtbServiceLog;
        private System.Windows.Forms.Timer timerServiceLog;
    }
}