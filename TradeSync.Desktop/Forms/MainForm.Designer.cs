//namespace TradeSync.Desktop
//{
//    partial class MainForm
//    {
//        private System.ComponentModel.IContainer components = null;

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null)) components.Dispose();
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        private void InitializeComponent()
//        {
//            components = new System.ComponentModel.Container();
//            panelSideMenu = new Panel();
//            btnMenuSettings = new Button();
//            btnMenuStructure = new Button();
//            btnMenuService = new Button();
//            btnMenuData = new Button();
//            panelLogo = new Panel();
//            lblLogo = new Label();
//            panelHeader = new Panel();
//            lblServiceStatusHeader = new Label();
//            lblHeaderTitle = new Label();
//            panelContent = new Panel();
//            pageStructure = new Panel();
//            splitStructure = new SplitContainer();
//            lstStructureTables = new ListBox();
//            panelStructLeftBottom = new TableLayoutPanel();
//            btnAddTable = new Button();
//            btnRemoveTable = new Button();
//            lblStructList = new Label();
//            panelStructRight = new Panel();
//            gridStructFields = new DataGridView();
//            colHuman = new DataGridViewTextBoxColumn();
//            colSql = new DataGridViewTextBoxColumn();
//            lblGridHeader = new Label();
//            tblStructTopInputs = new TableLayoutPanel();
//            lblMeta1 = new Label();
//            lblMeta2 = new Label();
//            txtStructName1C = new TextBox();
//            txtStructSqlTable = new TextBox();
//            btnSaveStructure = new Button();
//            pageSettings = new Panel();
//            grpSource = new GroupBox();
//            txtSourceConn = new TextBox();
//            grpAux = new GroupBox();
//            txtAuxConn = new TextBox();
//            btnSaveConfig = new Button();
//            pageService = new Panel();
//            rtbServiceLog = new RichTextBox();
//            lblServiceLogTitle = new Label();
//            panelServiceControls = new Panel();
//            btnStart = new Button();
//            btnStop = new Button();
//            btnInstall = new Button();
//            btnUninstall = new Button();
//            lblServiceControlTitle = new Label();
//            pageData = new Panel();
//            splitContainerData = new SplitContainer();
//            grpLog = new Panel();
//            rtbLog = new RichTextBox();
//            lblLogTitle = new Label();
//            panelDataControls = new Panel();
//            btnSync = new Button();
//            cmbTables = new ComboBox();
//            lblSelectTable = new Label();
//            panelGridWrapper = new Panel();
//            dataGridView1 = new DataGridView();
//            lblGridTitle = new Label();
//            statusStrip1 = new StatusStrip();
//            lblStatus = new ToolStripStatusLabel();
//            progressBar1 = new ToolStripProgressBar();
//            timerServiceLog = new System.Windows.Forms.Timer(components);
//            panelSideMenu.SuspendLayout();
//            panelLogo.SuspendLayout();
//            panelHeader.SuspendLayout();
//            panelContent.SuspendLayout();
//            pageStructure.SuspendLayout();
//            ((System.ComponentModel.ISupportInitialize)splitStructure).BeginInit();
//            splitStructure.Panel1.SuspendLayout();
//            splitStructure.Panel2.SuspendLayout();
//            splitStructure.SuspendLayout();
//            panelStructLeftBottom.SuspendLayout();
//            panelStructRight.SuspendLayout();
//            ((System.ComponentModel.ISupportInitialize)gridStructFields).BeginInit();
//            tblStructTopInputs.SuspendLayout();
//            pageSettings.SuspendLayout();
//            grpSource.SuspendLayout();
//            grpAux.SuspendLayout();
//            pageService.SuspendLayout();
//            panelServiceControls.SuspendLayout();
//            pageData.SuspendLayout();
//            ((System.ComponentModel.ISupportInitialize)splitContainerData).BeginInit();
//            splitContainerData.Panel1.SuspendLayout();
//            splitContainerData.Panel2.SuspendLayout();
//            splitContainerData.SuspendLayout();
//            grpLog.SuspendLayout();
//            panelDataControls.SuspendLayout();
//            panelGridWrapper.SuspendLayout();
//            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
//            statusStrip1.SuspendLayout();
//            SuspendLayout();
//            // 
//            // panelSideMenu
//            // 
//            panelSideMenu.BackColor = Color.FromArgb(32, 34, 37);
//            panelSideMenu.Controls.Add(btnMenuSettings);
//            panelSideMenu.Controls.Add(btnMenuStructure);
//            panelSideMenu.Controls.Add(btnMenuService);
//            panelSideMenu.Controls.Add(btnMenuData);
//            panelSideMenu.Controls.Add(panelLogo);
//            panelSideMenu.Dock = DockStyle.Left;
//            panelSideMenu.Location = new Point(0, 0);
//            panelSideMenu.Name = "panelSideMenu";
//            panelSideMenu.Size = new Size(220, 639);
//            panelSideMenu.TabIndex = 2;
//            // 
//            // btnMenuSettings
//            // 
//            btnMenuSettings.Dock = DockStyle.Top;
//            btnMenuSettings.FlatAppearance.BorderSize = 0;
//            btnMenuSettings.FlatStyle = FlatStyle.Flat;
//            btnMenuSettings.Font = new Font("Segoe UI", 10F);
//            btnMenuSettings.ForeColor = Color.Gainsboro;
//            btnMenuSettings.Location = new Point(0, 230);
//            btnMenuSettings.Name = "btnMenuSettings";
//            btnMenuSettings.Padding = new Padding(15, 0, 0, 0);
//            btnMenuSettings.Size = new Size(220, 50);
//            btnMenuSettings.TabIndex = 0;
//            btnMenuSettings.Text = "  🛠  Налаштування";
//            btnMenuSettings.TextAlign = ContentAlignment.MiddleLeft;
//            btnMenuSettings.Click += btnMenu_Click;
//            // 
//            // btnMenuStructure
//            // 
//            btnMenuStructure.Dock = DockStyle.Top;
//            btnMenuStructure.FlatAppearance.BorderSize = 0;
//            btnMenuStructure.FlatStyle = FlatStyle.Flat;
//            btnMenuStructure.Font = new Font("Segoe UI", 10F);
//            btnMenuStructure.ForeColor = Color.Gainsboro;
//            btnMenuStructure.Location = new Point(0, 180);
//            btnMenuStructure.Name = "btnMenuStructure";
//            btnMenuStructure.Padding = new Padding(15, 0, 0, 0);
//            btnMenuStructure.Size = new Size(220, 50);
//            btnMenuStructure.TabIndex = 1;
//            btnMenuStructure.Text = "  📂  Структура";
//            btnMenuStructure.TextAlign = ContentAlignment.MiddleLeft;
//            btnMenuStructure.Click += btnMenu_Click;
//            // 
//            // btnMenuService
//            // 
//            btnMenuService.Dock = DockStyle.Top;
//            btnMenuService.FlatAppearance.BorderSize = 0;
//            btnMenuService.FlatStyle = FlatStyle.Flat;
//            btnMenuService.Font = new Font("Segoe UI", 10F);
//            btnMenuService.ForeColor = Color.Gainsboro;
//            btnMenuService.Location = new Point(0, 130);
//            btnMenuService.Name = "btnMenuService";
//            btnMenuService.Padding = new Padding(15, 0, 0, 0);
//            btnMenuService.Size = new Size(220, 50);
//            btnMenuService.TabIndex = 2;
//            btnMenuService.Text = "  ⚙️  Служба";
//            btnMenuService.TextAlign = ContentAlignment.MiddleLeft;
//            btnMenuService.Click += btnMenu_Click;
//            // 
//            // btnMenuData
//            // 
//            btnMenuData.Dock = DockStyle.Top;
//            btnMenuData.FlatAppearance.BorderSize = 0;
//            btnMenuData.FlatStyle = FlatStyle.Flat;
//            btnMenuData.Font = new Font("Segoe UI", 10F);
//            btnMenuData.ForeColor = Color.Gainsboro;
//            btnMenuData.Location = new Point(0, 80);
//            btnMenuData.Name = "btnMenuData";
//            btnMenuData.Padding = new Padding(15, 0, 0, 0);
//            btnMenuData.Size = new Size(220, 50);
//            btnMenuData.TabIndex = 3;
//            btnMenuData.Text = "  📊  Дані";
//            btnMenuData.TextAlign = ContentAlignment.MiddleLeft;
//            btnMenuData.Click += btnMenu_Click;
//            // 
//            // panelLogo
//            // 
//            panelLogo.BackColor = Color.FromArgb(25, 27, 29);
//            panelLogo.Controls.Add(lblLogo);
//            panelLogo.Dock = DockStyle.Top;
//            panelLogo.Location = new Point(0, 0);
//            panelLogo.Name = "panelLogo";
//            panelLogo.Size = new Size(220, 80);
//            panelLogo.TabIndex = 4;
//            // 
//            // lblLogo
//            // 
//            lblLogo.Dock = DockStyle.Fill;
//            lblLogo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
//            lblLogo.ForeColor = Color.White;
//            lblLogo.Location = new Point(0, 0);
//            lblLogo.Name = "lblLogo";
//            lblLogo.Size = new Size(220, 80);
//            lblLogo.TabIndex = 0;
//            lblLogo.Text = "TradeSync\r\nAdmin";
//            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
//            // 
//            // panelHeader
//            // 
//            panelHeader.BackColor = Color.White;
//            panelHeader.Controls.Add(lblServiceStatusHeader);
//            panelHeader.Controls.Add(lblHeaderTitle);
//            panelHeader.Dock = DockStyle.Top;
//            panelHeader.Location = new Point(220, 0);
//            panelHeader.Name = "panelHeader";
//            panelHeader.Size = new Size(788, 60);
//            panelHeader.TabIndex = 1;
//            // 
//            // lblServiceStatusHeader
//            // 
//            lblServiceStatusHeader.Dock = DockStyle.Right;
//            lblServiceStatusHeader.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
//            lblServiceStatusHeader.Location = new Point(588, 0);
//            lblServiceStatusHeader.Name = "lblServiceStatusHeader";
//            lblServiceStatusHeader.Padding = new Padding(0, 0, 20, 0);
//            lblServiceStatusHeader.Size = new Size(200, 60);
//            lblServiceStatusHeader.TabIndex = 0;
//            lblServiceStatusHeader.Text = "Service: ...";
//            lblServiceStatusHeader.TextAlign = ContentAlignment.MiddleRight;
//            // 
//            // lblHeaderTitle
//            // 
//            lblHeaderTitle.AutoSize = true;
//            lblHeaderTitle.Font = new Font("Segoe UI", 14F);
//            lblHeaderTitle.Location = new Point(20, 18);
//            lblHeaderTitle.Name = "lblHeaderTitle";
//            lblHeaderTitle.Size = new Size(104, 25);
//            lblHeaderTitle.TabIndex = 1;
//            lblHeaderTitle.Text = "Dashboard";
//            // 
//            // panelContent
//            // 
//            panelContent.BackColor = Color.WhiteSmoke;
//            panelContent.Controls.Add(pageStructure);
//            panelContent.Controls.Add(pageSettings);
//            panelContent.Controls.Add(pageService);
//            panelContent.Controls.Add(pageData);
//            panelContent.Dock = DockStyle.Fill;
//            panelContent.Location = new Point(220, 60);
//            panelContent.Name = "panelContent";
//            panelContent.Padding = new Padding(10);
//            panelContent.Size = new Size(788, 579);
//            panelContent.TabIndex = 0;
//            // 
//            // pageStructure
//            // 
//            pageStructure.Controls.Add(splitStructure);
//            pageStructure.Dock = DockStyle.Fill;
//            pageStructure.Location = new Point(10, 10);
//            pageStructure.Name = "pageStructure";
//            pageStructure.Size = new Size(768, 559);
//            pageStructure.TabIndex = 0;
//            // 
//            // splitStructure
//            // 
//            splitStructure.Dock = DockStyle.Fill;
//            splitStructure.FixedPanel = FixedPanel.Panel1;
//            splitStructure.Location = new Point(0, 0);
//            splitStructure.Name = "splitStructure";
//            // 
//            // splitStructure.Panel1
//            // 
//            splitStructure.Panel1.Controls.Add(lstStructureTables);
//            splitStructure.Panel1.Controls.Add(panelStructLeftBottom);
//            splitStructure.Panel1.Controls.Add(lblStructList);
//            splitStructure.Panel1.Padding = new Padding(10);
//            // 
//            // splitStructure.Panel2
//            // 
//            splitStructure.Panel2.Controls.Add(panelStructRight);
//            splitStructure.Size = new Size(768, 559);
//            splitStructure.SplitterDistance = 296;
//            splitStructure.TabIndex = 0;
//            // 
//            // lstStructureTables
//            // 
//            lstStructureTables.Dock = DockStyle.Fill;
//            lstStructureTables.ItemHeight = 15;
//            lstStructureTables.Location = new Point(10, 33);
//            lstStructureTables.Name = "lstStructureTables";
//            lstStructureTables.Size = new Size(276, 476);
//            lstStructureTables.TabIndex = 0;
//            lstStructureTables.SelectedIndexChanged += LstStructureTables_SelectedIndexChanged;
//            // 
//            // panelStructLeftBottom
//            // 
//            panelStructLeftBottom.ColumnCount = 2;
//            panelStructLeftBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
//            panelStructLeftBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
//            panelStructLeftBottom.Controls.Add(btnAddTable, 0, 0);
//            panelStructLeftBottom.Controls.Add(btnRemoveTable, 1, 0);
//            panelStructLeftBottom.Dock = DockStyle.Bottom;
//            panelStructLeftBottom.Location = new Point(10, 509);
//            panelStructLeftBottom.Name = "panelStructLeftBottom";
//            panelStructLeftBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
//            panelStructLeftBottom.Size = new Size(276, 40);
//            panelStructLeftBottom.TabIndex = 1;
//            // 
//            // btnAddTable
//            // 
//            btnAddTable.Dock = DockStyle.Fill;
//            btnAddTable.Location = new Point(3, 3);
//            btnAddTable.Name = "btnAddTable";
//            btnAddTable.Size = new Size(132, 34);
//            btnAddTable.TabIndex = 0;
//            btnAddTable.Text = "+";
//            btnAddTable.Click += AddNewTable;
//            // 
//            // btnRemoveTable
//            // 
//            btnRemoveTable.Dock = DockStyle.Fill;
//            btnRemoveTable.Location = new Point(141, 3);
//            btnRemoveTable.Name = "btnRemoveTable";
//            btnRemoveTable.Size = new Size(132, 34);
//            btnRemoveTable.TabIndex = 1;
//            btnRemoveTable.Text = "-";
//            btnRemoveTable.Click += RemoveCurrentTable;
//            // 
//            // lblStructList
//            // 
//            lblStructList.Dock = DockStyle.Top;
//            lblStructList.Location = new Point(10, 10);
//            lblStructList.Name = "lblStructList";
//            lblStructList.Size = new Size(276, 23);
//            lblStructList.TabIndex = 2;
//            lblStructList.Text = "Таблиці";
//            // 
//            // panelStructRight
//            // 
//            panelStructRight.Controls.Add(gridStructFields);
//            panelStructRight.Controls.Add(lblGridHeader);
//            panelStructRight.Controls.Add(tblStructTopInputs);
//            panelStructRight.Controls.Add(btnSaveStructure);
//            panelStructRight.Dock = DockStyle.Fill;
//            panelStructRight.Location = new Point(0, 0);
//            panelStructRight.Name = "panelStructRight";
//            panelStructRight.Padding = new Padding(10);
//            panelStructRight.Size = new Size(468, 559);
//            panelStructRight.TabIndex = 0;
//            // 
//            // gridStructFields
//            // 
//            gridStructFields.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
//            gridStructFields.BackgroundColor = Color.White;
//            gridStructFields.Columns.AddRange(new DataGridViewColumn[] { colHuman, colSql });
//            gridStructFields.Dock = DockStyle.Fill;
//            gridStructFields.Location = new Point(10, 93);
//            gridStructFields.Name = "gridStructFields";
//            gridStructFields.RowHeadersVisible = false;
//            gridStructFields.Size = new Size(448, 416);
//            gridStructFields.TabIndex = 0;
//            // 
//            // colHuman
//            // 
//            colHuman.HeaderText = "Local Name";
//            colHuman.Name = "colHuman";
//            // 
//            // colSql
//            // 
//            colSql.HeaderText = "SQL Name";
//            colSql.Name = "colSql";
//            // 
//            // lblGridHeader
//            // 
//            lblGridHeader.Dock = DockStyle.Top;
//            lblGridHeader.Location = new Point(10, 70);
//            lblGridHeader.Name = "lblGridHeader";
//            lblGridHeader.Size = new Size(448, 23);
//            lblGridHeader.TabIndex = 1;
//            lblGridHeader.Text = "Fields Mapping";
//            // 
//            // tblStructTopInputs
//            // 
//            tblStructTopInputs.ColumnCount = 2;
//            tblStructTopInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
//            tblStructTopInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
//            tblStructTopInputs.Controls.Add(lblMeta1, 0, 0);
//            tblStructTopInputs.Controls.Add(lblMeta2, 1, 0);
//            tblStructTopInputs.Controls.Add(txtStructName1C, 0, 1);
//            tblStructTopInputs.Controls.Add(txtStructSqlTable, 1, 1);
//            tblStructTopInputs.Dock = DockStyle.Top;
//            tblStructTopInputs.Location = new Point(10, 10);
//            tblStructTopInputs.Name = "tblStructTopInputs";
//            tblStructTopInputs.RowCount = 2;
//            tblStructTopInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
//            tblStructTopInputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
//            tblStructTopInputs.Size = new Size(448, 60);
//            tblStructTopInputs.TabIndex = 2;
//            // 
//            // lblMeta1
//            // 
//            lblMeta1.Location = new Point(3, 0);
//            lblMeta1.Name = "lblMeta1";
//            lblMeta1.Size = new Size(100, 20);
//            lblMeta1.TabIndex = 0;
//            lblMeta1.Text = "Name 1C";
//            // 
//            // lblMeta2
//            // 
//            lblMeta2.Location = new Point(227, 0);
//            lblMeta2.Name = "lblMeta2";
//            lblMeta2.Size = new Size(100, 20);
//            lblMeta2.TabIndex = 1;
//            lblMeta2.Text = "SQL Table";
//            // 
//            // txtStructName1C
//            // 
//            txtStructName1C.Dock = DockStyle.Fill;
//            txtStructName1C.Location = new Point(3, 23);
//            txtStructName1C.Name = "txtStructName1C";
//            txtStructName1C.Size = new Size(218, 23);
//            txtStructName1C.TabIndex = 2;
//            txtStructName1C.TextChanged += UpdateCurrentTableObj;
//            // 
//            // txtStructSqlTable
//            // 
//            txtStructSqlTable.Dock = DockStyle.Fill;
//            txtStructSqlTable.Location = new Point(227, 23);
//            txtStructSqlTable.Name = "txtStructSqlTable";
//            txtStructSqlTable.Size = new Size(218, 23);
//            txtStructSqlTable.TabIndex = 3;
//            txtStructSqlTable.TextChanged += UpdateCurrentTableObj;
//            // 
//            // btnSaveStructure
//            // 
//            btnSaveStructure.BackColor = Color.SteelBlue;
//            btnSaveStructure.Dock = DockStyle.Bottom;
//            btnSaveStructure.FlatStyle = FlatStyle.Flat;
//            btnSaveStructure.ForeColor = Color.White;
//            btnSaveStructure.Location = new Point(10, 509);
//            btnSaveStructure.Name = "btnSaveStructure";
//            btnSaveStructure.Size = new Size(448, 40);
//            btnSaveStructure.TabIndex = 3;
//            btnSaveStructure.Text = "Save JSON";
//            btnSaveStructure.UseVisualStyleBackColor = false;
//            btnSaveStructure.Click += BtnSaveStructure_Click;
//            // 
//            // pageSettings
//            // 
//            pageSettings.Controls.Add(grpSource);
//            pageSettings.Controls.Add(grpAux);
//            pageSettings.Controls.Add(btnSaveConfig);
//            pageSettings.Dock = DockStyle.Fill;
//            pageSettings.Location = new Point(10, 10);
//            pageSettings.Name = "pageSettings";
//            pageSettings.Padding = new Padding(20);
//            pageSettings.Size = new Size(768, 559);
//            pageSettings.TabIndex = 1;
//            // 
//            // grpSource
//            // 
//            grpSource.Controls.Add(txtSourceConn);
//            grpSource.Dock = DockStyle.Top;
//            grpSource.Location = new Point(20, 150);
//            grpSource.Name = "grpSource";
//            grpSource.Size = new Size(728, 80);
//            grpSource.TabIndex = 0;
//            grpSource.TabStop = false;
//            grpSource.Text = "Connection String: Джерело (1C)";
//            // 
//            // txtSourceConn
//            // 
//            txtSourceConn.Dock = DockStyle.Fill;
//            txtSourceConn.Location = new Point(3, 19);
//            txtSourceConn.Multiline = true;
//            txtSourceConn.Name = "txtSourceConn";
//            txtSourceConn.Size = new Size(722, 58);
//            txtSourceConn.TabIndex = 0;
//            // 
//            // grpAux
//            // 
//            grpAux.Controls.Add(txtAuxConn);
//            grpAux.Dock = DockStyle.Top;
//            grpAux.Location = new Point(20, 70);
//            grpAux.Name = "grpAux";
//            grpAux.Size = new Size(728, 80);
//            grpAux.TabIndex = 1;
//            grpAux.TabStop = false;
//            grpAux.Text = "Connection String: Aux (Intermediate DB)";
//            // 
//            // txtAuxConn
//            // 
//            txtAuxConn.Dock = DockStyle.Fill;
//            txtAuxConn.Location = new Point(3, 19);
//            txtAuxConn.Multiline = true;
//            txtAuxConn.Name = "txtAuxConn";
//            txtAuxConn.Size = new Size(722, 58);
//            txtAuxConn.TabIndex = 0;
//            // 
//            // btnSaveConfig
//            // 
//            btnSaveConfig.BackColor = Color.SteelBlue;
//            btnSaveConfig.Dock = DockStyle.Top;
//            btnSaveConfig.FlatStyle = FlatStyle.Flat;
//            btnSaveConfig.ForeColor = Color.White;
//            btnSaveConfig.Location = new Point(20, 20);
//            btnSaveConfig.Name = "btnSaveConfig";
//            btnSaveConfig.Size = new Size(728, 50);
//            btnSaveConfig.TabIndex = 2;
//            btnSaveConfig.Text = "Зберегти конфіг та Перезапустити";
//            btnSaveConfig.UseVisualStyleBackColor = false;
//            btnSaveConfig.Click += BtnSaveConfig_Click;
//            // 
//            // pageService
//            // 
//            pageService.Controls.Add(rtbServiceLog);
//            pageService.Controls.Add(lblServiceLogTitle);
//            pageService.Controls.Add(panelServiceControls);
//            pageService.Dock = DockStyle.Fill;
//            pageService.Location = new Point(10, 10);
//            pageService.Name = "pageService";
//            pageService.Padding = new Padding(10);
//            pageService.Size = new Size(768, 559);
//            pageService.TabIndex = 2;
//            // 
//            // rtbServiceLog
//            // 
//            rtbServiceLog.BackColor = Color.Black;
//            rtbServiceLog.Dock = DockStyle.Fill;
//            rtbServiceLog.Font = new Font("Consolas", 10F);
//            rtbServiceLog.ForeColor = Color.LimeGreen;
//            rtbServiceLog.Location = new Point(10, 140);
//            rtbServiceLog.Name = "rtbServiceLog";
//            rtbServiceLog.Size = new Size(748, 409);
//            rtbServiceLog.TabIndex = 0;
//            rtbServiceLog.Text = "";
//            // 
//            // lblServiceLogTitle
//            // 
//            lblServiceLogTitle.Dock = DockStyle.Top;
//            lblServiceLogTitle.Location = new Point(10, 110);
//            lblServiceLogTitle.Name = "lblServiceLogTitle";
//            lblServiceLogTitle.Size = new Size(748, 30);
//            lblServiceLogTitle.TabIndex = 1;
//            lblServiceLogTitle.Text = "Live Log (tail -f)";
//            lblServiceLogTitle.TextAlign = ContentAlignment.BottomLeft;
//            // 
//            // panelServiceControls
//            // 
//            panelServiceControls.Controls.Add(btnStart);
//            panelServiceControls.Controls.Add(btnStop);
//            panelServiceControls.Controls.Add(btnInstall);
//            panelServiceControls.Controls.Add(btnUninstall);
//            panelServiceControls.Controls.Add(lblServiceControlTitle);
//            panelServiceControls.Dock = DockStyle.Top;
//            panelServiceControls.Location = new Point(10, 10);
//            panelServiceControls.Name = "panelServiceControls";
//            panelServiceControls.Size = new Size(748, 100);
//            panelServiceControls.TabIndex = 2;
//            // 
//            // btnStart
//            // 
//            btnStart.BackColor = Color.MediumSeaGreen;
//            btnStart.FlatStyle = FlatStyle.Flat;
//            btnStart.ForeColor = Color.White;
//            btnStart.Location = new Point(0, 30);
//            btnStart.Name = "btnStart";
//            btnStart.Size = new Size(120, 40);
//            btnStart.TabIndex = 0;
//            btnStart.Text = "▶ Старт";
//            btnStart.UseVisualStyleBackColor = false;
//            btnStart.Click += btnStart_Click;
//            // 
//            // btnStop
//            // 
//            btnStop.BackColor = Color.IndianRed;
//            btnStop.FlatStyle = FlatStyle.Flat;
//            btnStop.ForeColor = Color.White;
//            btnStop.Location = new Point(130, 30);
//            btnStop.Name = "btnStop";
//            btnStop.Size = new Size(120, 40);
//            btnStop.TabIndex = 1;
//            btnStop.Text = "⏹ Стоп";
//            btnStop.UseVisualStyleBackColor = false;
//            btnStop.Click += btnStop_Click;
//            // 
//            // btnInstall
//            // 
//            btnInstall.BackColor = Color.White;
//            btnInstall.FlatStyle = FlatStyle.Flat;
//            btnInstall.Location = new Point(260, 30);
//            btnInstall.Name = "btnInstall";
//            btnInstall.Size = new Size(120, 40);
//            btnInstall.TabIndex = 2;
//            btnInstall.Text = "📥 Встановити";
//            btnInstall.UseVisualStyleBackColor = false;
//            btnInstall.Click += btnInstall_Click;
//            // 
//            // btnUninstall
//            // 
//            btnUninstall.BackColor = Color.White;
//            btnUninstall.FlatStyle = FlatStyle.Flat;
//            btnUninstall.Location = new Point(390, 30);
//            btnUninstall.Name = "btnUninstall";
//            btnUninstall.Size = new Size(120, 40);
//            btnUninstall.TabIndex = 3;
//            btnUninstall.Text = "🗑 Видалити";
//            btnUninstall.UseVisualStyleBackColor = false;
//            btnUninstall.Click += btnUninstall_Click;
//            // 
//            // lblServiceControlTitle
//            // 
//            lblServiceControlTitle.AutoSize = true;
//            lblServiceControlTitle.Location = new Point(0, 0);
//            lblServiceControlTitle.Name = "lblServiceControlTitle";
//            lblServiceControlTitle.Size = new Size(98, 15);
//            lblServiceControlTitle.TabIndex = 4;
//            lblServiceControlTitle.Text = "Дії над службою";
//            // 
//            // pageData
//            // 
//            pageData.Controls.Add(splitContainerData);
//            pageData.Dock = DockStyle.Fill;
//            pageData.Location = new Point(10, 10);
//            pageData.Name = "pageData";
//            pageData.Size = new Size(768, 559);
//            pageData.TabIndex = 3;
//            // 
//            // splitContainerData
//            // 
//            splitContainerData.Dock = DockStyle.Fill;
//            splitContainerData.FixedPanel = FixedPanel.Panel1;
//            splitContainerData.Location = new Point(0, 0);
//            splitContainerData.Name = "splitContainerData";
//            // 
//            // splitContainerData.Panel1
//            // 
//            splitContainerData.Panel1.Controls.Add(grpLog);
//            splitContainerData.Panel1.Controls.Add(panelDataControls);
//            splitContainerData.Panel1.Padding = new Padding(0, 0, 5, 0);
//            // 
//            // splitContainerData.Panel2
//            // 
//            splitContainerData.Panel2.Controls.Add(panelGridWrapper);
//            splitContainerData.Size = new Size(768, 559);
//            splitContainerData.SplitterDistance = 121;
//            splitContainerData.TabIndex = 0;
//            // 
//            // grpLog
//            // 
//            grpLog.Controls.Add(rtbLog);
//            grpLog.Controls.Add(lblLogTitle);
//            grpLog.Dock = DockStyle.Fill;
//            grpLog.Location = new Point(0, 130);
//            grpLog.Name = "grpLog";
//            grpLog.Size = new Size(116, 429);
//            grpLog.TabIndex = 0;
//            // 
//            // rtbLog
//            // 
//            rtbLog.BackColor = Color.WhiteSmoke;
//            rtbLog.BorderStyle = BorderStyle.None;
//            rtbLog.Dock = DockStyle.Fill;
//            rtbLog.Location = new Point(0, 25);
//            rtbLog.Name = "rtbLog";
//            rtbLog.Size = new Size(116, 404);
//            rtbLog.TabIndex = 0;
//            rtbLog.Text = "";
//            // 
//            // lblLogTitle
//            // 
//            lblLogTitle.Dock = DockStyle.Top;
//            lblLogTitle.Location = new Point(0, 0);
//            lblLogTitle.Name = "lblLogTitle";
//            lblLogTitle.Size = new Size(116, 25);
//            lblLogTitle.TabIndex = 1;
//            lblLogTitle.Text = "Лог подій";
//            // 
//            // panelDataControls
//            // 
//            panelDataControls.Controls.Add(btnSync);
//            panelDataControls.Controls.Add(cmbTables);
//            panelDataControls.Controls.Add(lblSelectTable);
//            panelDataControls.Dock = DockStyle.Top;
//            panelDataControls.Location = new Point(0, 0);
//            panelDataControls.Name = "panelDataControls";
//            panelDataControls.Size = new Size(116, 130);
//            panelDataControls.TabIndex = 1;
//            // 
//            // btnSync
//            // 
//            btnSync.BackColor = Color.SteelBlue;
//            btnSync.Dock = DockStyle.Top;
//            btnSync.FlatStyle = FlatStyle.Flat;
//            btnSync.ForeColor = Color.White;
//            btnSync.Location = new Point(0, 0);
//            btnSync.Name = "btnSync";
//            btnSync.Size = new Size(116, 45);
//            btnSync.TabIndex = 0;
//            btnSync.Text = "Синхронізувати";
//            btnSync.UseVisualStyleBackColor = false;
//            btnSync.Click += btnSync_Click;
//            // 
//            // cmbTables
//            // 
//            cmbTables.DropDownStyle = ComboBoxStyle.DropDownList;
//            cmbTables.Location = new Point(0, 95);
//            cmbTables.Name = "cmbTables";
//            cmbTables.Size = new Size(245, 23);
//            cmbTables.TabIndex = 1;
//            cmbTables.SelectedIndexChanged += cmbTables_SelectedIndexChanged;
//            // 
//            // lblSelectTable
//            // 
//            lblSelectTable.Location = new Point(0, 70);
//            lblSelectTable.Name = "lblSelectTable";
//            lblSelectTable.Size = new Size(245, 20);
//            lblSelectTable.TabIndex = 2;
//            lblSelectTable.Text = "Таблиця:";
//            // 
//            // panelGridWrapper
//            // 
//            panelGridWrapper.Controls.Add(dataGridView1);
//            panelGridWrapper.Controls.Add(lblGridTitle);
//            panelGridWrapper.Dock = DockStyle.Fill;
//            panelGridWrapper.Location = new Point(0, 0);
//            panelGridWrapper.Name = "panelGridWrapper";
//            panelGridWrapper.Size = new Size(643, 559);
//            panelGridWrapper.TabIndex = 0;
//            // 
//            // dataGridView1
//            // 
//            dataGridView1.BackgroundColor = Color.White;
//            dataGridView1.BorderStyle = BorderStyle.None;
//            dataGridView1.Dock = DockStyle.Fill;
//            dataGridView1.Location = new Point(0, 30);
//            dataGridView1.Name = "dataGridView1";
//            dataGridView1.RowHeadersVisible = false;
//            dataGridView1.Size = new Size(643, 529);
//            dataGridView1.TabIndex = 0;
//            // 
//            // lblGridTitle
//            // 
//            lblGridTitle.Dock = DockStyle.Top;
//            lblGridTitle.Location = new Point(0, 0);
//            lblGridTitle.Name = "lblGridTitle";
//            lblGridTitle.Size = new Size(643, 30);
//            lblGridTitle.TabIndex = 1;
//            lblGridTitle.Text = "Дані (SQLite Preview)";
//            // 
//            // statusStrip1
//            // 
//            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus, progressBar1 });
//            statusStrip1.Location = new Point(0, 639);
//            statusStrip1.Name = "statusStrip1";
//            statusStrip1.Size = new Size(1008, 22);
//            statusStrip1.TabIndex = 3;
//            // 
//            // lblStatus
//            // 
//            lblStatus.Name = "lblStatus";
//            lblStatus.Size = new Size(0, 17);
//            // 
//            // progressBar1
//            // 
//            progressBar1.Name = "progressBar1";
//            progressBar1.Size = new Size(100, 16);
//            // 
//            // timerServiceLog
//            // 
//            timerServiceLog.Interval = 2000;
//            timerServiceLog.Tick += timerServiceLog_Tick;
//            // 
//            // MainForm
//            // 
//            AutoScaleDimensions = new SizeF(7F, 15F);
//            AutoScaleMode = AutoScaleMode.Font;
//            ClientSize = new Size(1008, 661);
//            Controls.Add(panelContent);
//            Controls.Add(panelHeader);
//            Controls.Add(panelSideMenu);
//            Controls.Add(statusStrip1);
//            Name = "MainForm";
//            StartPosition = FormStartPosition.CenterScreen;
//            Text = "TradeSync Admin";
//            Load += MainForm_Load;
//            panelSideMenu.ResumeLayout(false);
//            panelLogo.ResumeLayout(false);
//            panelHeader.ResumeLayout(false);
//            panelHeader.PerformLayout();
//            panelContent.ResumeLayout(false);
//            pageStructure.ResumeLayout(false);
//            splitStructure.Panel1.ResumeLayout(false);
//            splitStructure.Panel2.ResumeLayout(false);
//            ((System.ComponentModel.ISupportInitialize)splitStructure).EndInit();
//            splitStructure.ResumeLayout(false);
//            panelStructLeftBottom.ResumeLayout(false);
//            panelStructRight.ResumeLayout(false);
//            ((System.ComponentModel.ISupportInitialize)gridStructFields).EndInit();
//            tblStructTopInputs.ResumeLayout(false);
//            tblStructTopInputs.PerformLayout();
//            pageSettings.ResumeLayout(false);
//            grpSource.ResumeLayout(false);
//            grpSource.PerformLayout();
//            grpAux.ResumeLayout(false);
//            grpAux.PerformLayout();
//            pageService.ResumeLayout(false);
//            panelServiceControls.ResumeLayout(false);
//            panelServiceControls.PerformLayout();
//            pageData.ResumeLayout(false);
//            splitContainerData.Panel1.ResumeLayout(false);
//            splitContainerData.Panel2.ResumeLayout(false);
//            ((System.ComponentModel.ISupportInitialize)splitContainerData).EndInit();
//            splitContainerData.ResumeLayout(false);
//            grpLog.ResumeLayout(false);
//            panelDataControls.ResumeLayout(false);
//            panelGridWrapper.ResumeLayout(false);
//            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
//            statusStrip1.ResumeLayout(false);
//            statusStrip1.PerformLayout();
//            ResumeLayout(false);
//            PerformLayout();
//        }

//        #endregion

//        // Global
//        private System.Windows.Forms.Panel panelSideMenu;
//        private System.Windows.Forms.Panel panelLogo;
//        private System.Windows.Forms.Label lblLogo;
//        private System.Windows.Forms.Button btnMenuSettings;
//        private System.Windows.Forms.Button btnMenuStructure;
//        private System.Windows.Forms.Button btnMenuService;
//        private System.Windows.Forms.Button btnMenuData;
//        private System.Windows.Forms.Panel panelHeader;
//        private System.Windows.Forms.Label lblServiceStatusHeader;
//        private System.Windows.Forms.Label lblHeaderTitle;
//        private System.Windows.Forms.Panel panelContent;
//        private System.Windows.Forms.StatusStrip statusStrip1;
//        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
//        private System.Windows.Forms.ToolStripProgressBar progressBar1;
//        private System.Windows.Forms.Timer timerServiceLog;

//        // DATA
//        private System.Windows.Forms.Panel pageData;
//        private System.Windows.Forms.SplitContainer splitContainerData;
//        private System.Windows.Forms.Panel grpLog;
//        private System.Windows.Forms.RichTextBox rtbLog;
//        private System.Windows.Forms.Label lblLogTitle;
//        private System.Windows.Forms.Panel panelDataControls;
//        private System.Windows.Forms.Button btnSync;
//        private System.Windows.Forms.ComboBox cmbTables;
//        private System.Windows.Forms.Label lblSelectTable;
//        private System.Windows.Forms.Panel panelGridWrapper;
//        private System.Windows.Forms.DataGridView dataGridView1;
//        private System.Windows.Forms.Label lblGridTitle;

//        // SERVICE
//        private System.Windows.Forms.Panel pageService;
//        private System.Windows.Forms.RichTextBox rtbServiceLog;
//        private System.Windows.Forms.Label lblServiceLogTitle;
//        private System.Windows.Forms.Panel panelServiceControls;
//        private System.Windows.Forms.Button btnInstall;
//        private System.Windows.Forms.Button btnUninstall;
//        private System.Windows.Forms.Button btnStop;
//        private System.Windows.Forms.Button btnStart;
//        private System.Windows.Forms.Label lblServiceControlTitle;

//        // SETTINGS
//        private System.Windows.Forms.Panel pageSettings;
//        private System.Windows.Forms.Button btnSaveConfig;
//        private System.Windows.Forms.GroupBox grpAux;
//        private System.Windows.Forms.TextBox txtAuxConn;
//        private System.Windows.Forms.GroupBox grpSource;
//        private System.Windows.Forms.TextBox txtSourceConn;

//        // STRUCTURE
//        private System.Windows.Forms.Panel pageStructure;
//        private System.Windows.Forms.SplitContainer splitStructure;
//        private System.Windows.Forms.ListBox lstStructureTables;
//        private System.Windows.Forms.TableLayoutPanel panelStructLeftBottom;
//        private System.Windows.Forms.Button btnAddTable;
//        private System.Windows.Forms.Button btnRemoveTable;
//        private System.Windows.Forms.Label lblStructList;

//        private System.Windows.Forms.Panel panelStructRight;
//        private System.Windows.Forms.DataGridView gridStructFields;
//        private System.Windows.Forms.Label lblGridHeader;
//        private System.Windows.Forms.TableLayoutPanel tblStructTopInputs;
//        private System.Windows.Forms.Label lblMeta1;
//        private System.Windows.Forms.Label lblMeta2;
//        private System.Windows.Forms.TextBox txtStructName1C;
//        private System.Windows.Forms.TextBox txtStructSqlTable;
//        private System.Windows.Forms.Button btnSaveStructure;
//        private System.Windows.Forms.DataGridViewTextBoxColumn colHuman;
//        private System.Windows.Forms.DataGridViewTextBoxColumn colSql;
//    }
//}