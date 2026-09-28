namespace c_lan
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SqlEditorContextMenu = new ContextMenuStrip(components);
            CopySqlMenuItem = new ToolStripMenuItem();
            PasteSqlMenuItem = new ToolStripMenuItem();
            SelectAllSqlMenuItem = new ToolStripMenuItem();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            SqliteFolderDialog = new FolderBrowserDialog();
            _databaseTreeView = new TreeView();
            _databaseObjectsTabPage = new TabPage();
            _databaseTypeComboBox = new ComboBox();
            _browseSqliteButton = new Button();
            _hostInputPanel = new Panel();
            HostText = new TextBox();
            _sqliteFileComboBox = new ComboBox();
            ConnectionLayout = new TableLayoutPanel();
            ConnectionHeading = new TableLayoutPanel();
            ConnectionSectionLabel = new Label();
            SavedConnectionsPanel = new TableLayoutPanel();
            SavedConnectionsComboBox = new ComboBox();
            NewConnectionButton = new Button();
            ConnectionFieldsTable = new TableLayoutPanel();
            ConnectionnameLabel = new Label();
            ConnectionnameText = new TextBox();
            HostLabel = new Label();
            PortLabel = new Label();
            PortInputPanel = new Panel();
            PortText = new TextBox();
            UserLabel = new Label();
            UserText = new TextBox();
            PasswordLabel = new Label();
            PasswordPanel = new Panel();
            PasswordText = new TextBox();
            ShowPasswordCheckBox = new CheckBox();
            DefaultDatabaseLabel = new Label();
            DefaultDatabaseText = new TextBox();
            CharacterSetLabel = new Label();
            CharacterSetComboBox = new ComboBox();
            SslModeLabel = new Label();
            SslModeComboBox = new ComboBox();
            ConnectionOptionsPanel = new Panel();
            SavePasswordCheckBox = new CheckBox();
            TimeoutNumericUpDown = new NumericUpDown();
            TimeoutLabel = new Label();
            ConnectionButtonTable = new TableLayoutPanel();
            TestButton = new Button();
            ConnectButton = new Button();
            SecondaryButtonTable = new TableLayoutPanel();
            SaveConnectionButton = new Button();
            DeleteConnectionButton = new Button();
            AuthenticationPanel = new FlowLayoutPanel();
            IntegratedSecurityCheckBox = new CheckBox();
            TrustCertificateCheckBox = new CheckBox();
            QueryParametersPanel = new FlowLayoutPanel();
            DatabaseLabel = new Label();
            DatabaseComboBox = new ComboBox();
            QueryTimeoutLabel = new Label();
            QueryTimeoutNumericUpDown = new NumericUpDown();
            ReadOnlyCheckBox = new CheckBox();
            QueryActionsPanel = new FlowLayoutPanel();
            ExecuteQueryButton = new Button();
            StopQueryButton = new Button();
            ClearSqlButton = new Button();
            RefreshButton = new Button();
            ExportButton = new Button();
            HeaderPanel = new Panel();
            HeaderTitleLabel = new Label();
            MainSplitContainer = new SplitContainer();
            ConnectionPanel = new Panel();
            ConnectionTipLabel = new Label();
            WorkspaceSplitContainer = new SplitContainer();
            QueryPanel = new Panel();
            QueryEditorPanel = new Panel();
            SqlEditorTextBox = new RichTextBox();
            QueryToolbarPanel = new Panel();
            QuerySectionLabel = new Label();
            ResultTabControl = new TabControl();
            ResultTabPage = new TabPage();
            dataGridView1 = new DataGridView();
            MessageTabPage = new TabPage();
            MessageTextBox = new RichTextBox();
            ResultSummaryPanel = new Panel();
            ResultStateLabel = new Label();
            ResultSummaryLabel = new Label();
            MainStatusStrip = new StatusStrip();
            ConnectionStatusLabel = new ToolStripStatusLabel();
            StatusSpringLabel = new ToolStripStatusLabel();
            CurrentDatabaseStatusLabel = new ToolStripStatusLabel();
            SqlEditorContextMenu.SuspendLayout();
            _databaseObjectsTabPage.SuspendLayout();
            _hostInputPanel.SuspendLayout();
            ConnectionLayout.SuspendLayout();
            ConnectionHeading.SuspendLayout();
            SavedConnectionsPanel.SuspendLayout();
            ConnectionFieldsTable.SuspendLayout();
            PortInputPanel.SuspendLayout();
            PasswordPanel.SuspendLayout();
            ConnectionOptionsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TimeoutNumericUpDown).BeginInit();
            ConnectionButtonTable.SuspendLayout();
            SecondaryButtonTable.SuspendLayout();
            AuthenticationPanel.SuspendLayout();
            QueryParametersPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)QueryTimeoutNumericUpDown).BeginInit();
            QueryActionsPanel.SuspendLayout();
            HeaderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MainSplitContainer).BeginInit();
            MainSplitContainer.Panel1.SuspendLayout();
            MainSplitContainer.Panel2.SuspendLayout();
            MainSplitContainer.SuspendLayout();
            ConnectionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WorkspaceSplitContainer).BeginInit();
            WorkspaceSplitContainer.Panel1.SuspendLayout();
            WorkspaceSplitContainer.Panel2.SuspendLayout();
            WorkspaceSplitContainer.SuspendLayout();
            QueryPanel.SuspendLayout();
            QueryEditorPanel.SuspendLayout();
            QueryToolbarPanel.SuspendLayout();
            ResultTabControl.SuspendLayout();
            ResultTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            MessageTabPage.SuspendLayout();
            ResultSummaryPanel.SuspendLayout();
            MainStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // SqlEditorContextMenu
            //
            SqlEditorContextMenu.Items.AddRange(new ToolStripItem[] { CopySqlMenuItem, PasteSqlMenuItem, SelectAllSqlMenuItem });
            SqlEditorContextMenu.Name = "SqlEditorContextMenu";
            CopySqlMenuItem.Name = "CopySqlMenuItem";
            CopySqlMenuItem.Text = "复制";
            PasteSqlMenuItem.Name = "PasteSqlMenuItem";
            PasteSqlMenuItem.Text = "粘贴";
            SelectAllSqlMenuItem.Name = "SelectAllSqlMenuItem";
            SelectAllSqlMenuItem.Text = "全选";
            //
            // SqliteFolderDialog
            // 
            SqliteFolderDialog.Description = "选择包含 SQLite 数据库文件的文件夹";
            SqliteFolderDialog.ShowNewFolderButton = false;
            SqliteFolderDialog.UseDescriptionForTitle = true;
            // 
            // _databaseTreeView
            // 
            _databaseTreeView.BackColor = Color.White;
            _databaseTreeView.BorderStyle = BorderStyle.None;
            _databaseTreeView.Dock = DockStyle.Fill;
            _databaseTreeView.ForeColor = Color.FromArgb(30, 41, 59);
            _databaseTreeView.FullRowSelect = true;
            _databaseTreeView.HideSelection = false;
            _databaseTreeView.ItemHeight = 30;
            _databaseTreeView.Location = new Point(8, 8);
            _databaseTreeView.Name = "_databaseTreeView";
            _databaseTreeView.ShowNodeToolTips = true;
            _databaseTreeView.Size = new Size(964, 301);
            _databaseTreeView.TabIndex = 0;
            // 
            // _databaseObjectsTabPage
            // 
            _databaseObjectsTabPage.Controls.Add(_databaseTreeView);
            _databaseObjectsTabPage.Location = new Point(4, 33);
            _databaseObjectsTabPage.Name = "_databaseObjectsTabPage";
            _databaseObjectsTabPage.Padding = new Padding(8);
            _databaseObjectsTabPage.Size = new Size(980, 317);
            _databaseObjectsTabPage.TabIndex = 0;
            _databaseObjectsTabPage.Text = "数据库对象";
            _databaseObjectsTabPage.UseVisualStyleBackColor = true;
            // 
            // _databaseTypeComboBox
            // 
            _databaseTypeComboBox.Dock = DockStyle.Fill;
            _databaseTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _databaseTypeComboBox.Items.AddRange(new object[] { "MySQL", "SQLite", "SQL Server", "Oracle" });
            _databaseTypeComboBox.Location = new Point(108, 7);
            _databaseTypeComboBox.Margin = new Padding(0, 7, 0, 0);
            _databaseTypeComboBox.Name = "_databaseTypeComboBox";
            _databaseTypeComboBox.Size = new Size(221, 28);
            _databaseTypeComboBox.TabIndex = 7;
            // 
            // _browseSqliteButton
            // 
            _browseSqliteButton.BackColor = Color.White;
            _browseSqliteButton.Dock = DockStyle.Right;
            _browseSqliteButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            _browseSqliteButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            _browseSqliteButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            _browseSqliteButton.FlatStyle = FlatStyle.Flat;
            _browseSqliteButton.ForeColor = Color.FromArgb(30, 41, 59);
            _browseSqliteButton.Location = new Point(224, 0);
            _browseSqliteButton.Name = "_browseSqliteButton";
            _browseSqliteButton.Size = new Size(105, 31);
            _browseSqliteButton.TabIndex = 2;
            _browseSqliteButton.Text = "选择文件夹";
            _browseSqliteButton.UseVisualStyleBackColor = false;
            _browseSqliteButton.Visible = false;
            // 
            // _hostInputPanel
            // 
            _hostInputPanel.Controls.Add(HostText);
            _hostInputPanel.Controls.Add(_browseSqliteButton);
            _hostInputPanel.Dock = DockStyle.Fill;
            _hostInputPanel.Location = new Point(0, 96);
            _hostInputPanel.Margin = new Padding(0, 0, 0, 5);
            _hostInputPanel.Name = "_hostInputPanel";
            _hostInputPanel.Size = new Size(329, 31);
            _hostInputPanel.TabIndex = 2;
            // 
            // HostText
            // 
            HostText.Dock = DockStyle.Fill;
            HostText.Location = new Point(0, 0);
            HostText.Margin = new Padding(0, 0, 0, 5);
            HostText.Name = "HostText";
            HostText.PlaceholderText = "localhost 或服务器 IP";
            HostText.Size = new Size(224, 27);
            HostText.TabIndex = 1;
            // 
            // _sqliteFileComboBox
            // 
            _sqliteFileComboBox.Dock = DockStyle.Fill;
            _sqliteFileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _sqliteFileComboBox.FormattingEnabled = true;
            _sqliteFileComboBox.Location = new Point(0, 0);
            _sqliteFileComboBox.Name = "_sqliteFileComboBox";
            _sqliteFileComboBox.Size = new Size(329, 28);
            _sqliteFileComboBox.TabIndex = 3;
            _sqliteFileComboBox.Visible = false;
            // 
            // ConnectionLayout
            // 
            ConnectionLayout.AutoSize = true;
            ConnectionLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ConnectionLayout.ColumnCount = 1;
            ConnectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ConnectionLayout.Controls.Add(ConnectionHeading, 0, 0);
            ConnectionLayout.Controls.Add(SavedConnectionsPanel, 0, 1);
            ConnectionLayout.Controls.Add(ConnectionFieldsTable, 0, 2);
            ConnectionLayout.Controls.Add(ConnectionOptionsPanel, 0, 4);
            ConnectionLayout.Controls.Add(ConnectionButtonTable, 0, 5);
            ConnectionLayout.Controls.Add(SecondaryButtonTable, 0, 6);
            ConnectionLayout.Controls.Add(AuthenticationPanel, 0, 3);
            ConnectionLayout.Dock = DockStyle.Top;
            ConnectionLayout.Location = new Point(20, 16);
            ConnectionLayout.Margin = new Padding(0);
            ConnectionLayout.Name = "ConnectionLayout";
            ConnectionLayout.RowCount = 7;
            ConnectionLayout.RowStyles.Add(new RowStyle());
            ConnectionLayout.RowStyles.Add(new RowStyle());
            ConnectionLayout.RowStyles.Add(new RowStyle());
            ConnectionLayout.RowStyles.Add(new RowStyle());
            ConnectionLayout.RowStyles.Add(new RowStyle());
            ConnectionLayout.RowStyles.Add(new RowStyle());
            ConnectionLayout.RowStyles.Add(new RowStyle());
            ConnectionLayout.Size = new Size(329, 833);
            ConnectionLayout.TabIndex = 0;
            // 
            // ConnectionHeading
            // 
            ConnectionHeading.ColumnCount = 2;
            ConnectionHeading.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108F));
            ConnectionHeading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ConnectionHeading.Controls.Add(ConnectionSectionLabel, 0, 0);
            ConnectionHeading.Controls.Add(_databaseTypeComboBox, 1, 0);
            ConnectionHeading.Dock = DockStyle.Top;
            ConnectionHeading.Location = new Point(0, 0);
            ConnectionHeading.Margin = new Padding(0, 0, 0, 8);
            ConnectionHeading.Name = "ConnectionHeading";
            ConnectionHeading.RowCount = 1;
            ConnectionHeading.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            ConnectionHeading.Size = new Size(329, 44);
            ConnectionHeading.TabIndex = 0;
            // 
            // ConnectionSectionLabel
            // 
            ConnectionSectionLabel.AutoSize = true;
            ConnectionSectionLabel.Dock = DockStyle.Fill;
            ConnectionSectionLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            ConnectionSectionLabel.ForeColor = Color.FromArgb(35, 46, 61);
            ConnectionSectionLabel.Location = new Point(0, 0);
            ConnectionSectionLabel.Margin = new Padding(0);
            ConnectionSectionLabel.Name = "ConnectionSectionLabel";
            ConnectionSectionLabel.Size = new Size(108, 44);
            ConnectionSectionLabel.TabIndex = 6;
            ConnectionSectionLabel.Text = "连接设置";
            ConnectionSectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // SavedConnectionsPanel
            // 
            SavedConnectionsPanel.ColumnCount = 2;
            SavedConnectionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            SavedConnectionsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            SavedConnectionsPanel.Controls.Add(SavedConnectionsComboBox, 0, 0);
            SavedConnectionsPanel.Controls.Add(NewConnectionButton, 1, 0);
            SavedConnectionsPanel.Dock = DockStyle.Top;
            SavedConnectionsPanel.Location = new Point(3, 55);
            SavedConnectionsPanel.Name = "SavedConnectionsPanel";
            SavedConnectionsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            SavedConnectionsPanel.Size = new Size(323, 38);
            SavedConnectionsPanel.TabIndex = 1;
            // 
            // SavedConnectionsComboBox
            // 
            SavedConnectionsComboBox.AccessibleName = "已保存连接";
            SavedConnectionsComboBox.Dock = DockStyle.Fill;
            SavedConnectionsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SavedConnectionsComboBox.Location = new Point(3, 3);
            SavedConnectionsComboBox.Name = "SavedConnectionsComboBox";
            SavedConnectionsComboBox.Size = new Size(247, 28);
            SavedConnectionsComboBox.TabIndex = 0;
            // 
            // NewConnectionButton
            // 
            NewConnectionButton.BackColor = Color.White;
            NewConnectionButton.Dock = DockStyle.Fill;
            NewConnectionButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            NewConnectionButton.FlatStyle = FlatStyle.Flat;
            NewConnectionButton.ForeColor = Color.FromArgb(30, 41, 59);
            NewConnectionButton.Location = new Point(256, 3);
            NewConnectionButton.Name = "NewConnectionButton";
            NewConnectionButton.Size = new Size(64, 32);
            NewConnectionButton.TabIndex = 1;
            NewConnectionButton.Text = "新建";
            NewConnectionButton.UseVisualStyleBackColor = false;
            // 
            // ConnectionFieldsTable
            // 
            ConnectionFieldsTable.AutoSize = true;
            ConnectionFieldsTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ConnectionFieldsTable.ColumnCount = 1;
            ConnectionFieldsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ConnectionFieldsTable.Controls.Add(ConnectionnameLabel, 0, 0);
            ConnectionFieldsTable.Controls.Add(ConnectionnameText, 0, 1);
            ConnectionFieldsTable.Controls.Add(HostLabel, 0, 2);
            ConnectionFieldsTable.Controls.Add(_hostInputPanel, 0, 3);
            ConnectionFieldsTable.Controls.Add(PortLabel, 0, 4);
            ConnectionFieldsTable.Controls.Add(PortInputPanel, 0, 5);
            ConnectionFieldsTable.Controls.Add(UserLabel, 0, 6);
            ConnectionFieldsTable.Controls.Add(UserText, 0, 7);
            ConnectionFieldsTable.Controls.Add(PasswordLabel, 0, 8);
            ConnectionFieldsTable.Controls.Add(PasswordPanel, 0, 9);
            ConnectionFieldsTable.Controls.Add(DefaultDatabaseLabel, 0, 10);
            ConnectionFieldsTable.Controls.Add(DefaultDatabaseText, 0, 11);
            ConnectionFieldsTable.Controls.Add(CharacterSetLabel, 0, 12);
            ConnectionFieldsTable.Controls.Add(CharacterSetComboBox, 0, 13);
            ConnectionFieldsTable.Controls.Add(SslModeLabel, 0, 14);
            ConnectionFieldsTable.Controls.Add(SslModeComboBox, 0, 15);
            ConnectionFieldsTable.Dock = DockStyle.Top;
            ConnectionFieldsTable.Location = new Point(0, 96);
            ConnectionFieldsTable.Margin = new Padding(0, 0, 0, 8);
            ConnectionFieldsTable.Name = "ConnectionFieldsTable";
            ConnectionFieldsTable.RowCount = 16;
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.RowStyles.Add(new RowStyle());
            ConnectionFieldsTable.Size = new Size(329, 527);
            ConnectionFieldsTable.TabIndex = 1;
            // 
            // ConnectionnameLabel
            // 
            ConnectionnameLabel.AutoSize = true;
            ConnectionnameLabel.ForeColor = Color.FromArgb(61, 73, 89);
            ConnectionnameLabel.Location = new Point(0, 7);
            ConnectionnameLabel.Margin = new Padding(0, 7, 0, 5);
            ConnectionnameLabel.Name = "ConnectionnameLabel";
            ConnectionnameLabel.Size = new Size(69, 20);
            ConnectionnameLabel.TabIndex = 0;
            ConnectionnameLabel.Text = "连接名称";
            // 
            // ConnectionnameText
            // 
            ConnectionnameText.Dock = DockStyle.Fill;
            ConnectionnameText.Location = new Point(0, 32);
            ConnectionnameText.Margin = new Padding(0, 0, 0, 5);
            ConnectionnameText.Name = "ConnectionnameText";
            ConnectionnameText.PlaceholderText = "例如：本地开发库";
            ConnectionnameText.Size = new Size(329, 27);
            ConnectionnameText.TabIndex = 0;
            // 
            // HostLabel
            // 
            HostLabel.AutoSize = true;
            HostLabel.ForeColor = Color.FromArgb(61, 73, 89);
            HostLabel.Location = new Point(0, 71);
            HostLabel.Margin = new Padding(0, 7, 0, 5);
            HostLabel.Name = "HostLabel";
            HostLabel.Size = new Size(69, 20);
            HostLabel.TabIndex = 1;
            HostLabel.Text = "主机地址";
            // 
            // PortLabel
            // 
            PortLabel.AutoSize = true;
            PortLabel.ForeColor = Color.FromArgb(61, 73, 89);
            PortLabel.Location = new Point(0, 139);
            PortLabel.Margin = new Padding(0, 7, 0, 5);
            PortLabel.Name = "PortLabel";
            PortLabel.Size = new Size(39, 20);
            PortLabel.TabIndex = 2;
            PortLabel.Text = "端口";
            // 
            // PortInputPanel
            // 
            PortInputPanel.Controls.Add(PortText);
            PortInputPanel.Controls.Add(_sqliteFileComboBox);
            PortInputPanel.Dock = DockStyle.Fill;
            PortInputPanel.Location = new Point(0, 164);
            PortInputPanel.Margin = new Padding(0, 0, 0, 5);
            PortInputPanel.Name = "PortInputPanel";
            PortInputPanel.Size = new Size(329, 31);
            PortInputPanel.TabIndex = 3;
            // 
            // PortText
            // 
            PortText.Dock = DockStyle.Fill;
            PortText.Location = new Point(0, 0);
            PortText.Margin = new Padding(0, 0, 0, 5);
            PortText.Name = "PortText";
            PortText.PlaceholderText = "3306";
            PortText.Size = new Size(329, 27);
            PortText.TabIndex = 2;
            // 
            // UserLabel
            // 
            UserLabel.AutoSize = true;
            UserLabel.ForeColor = Color.FromArgb(61, 73, 89);
            UserLabel.Location = new Point(0, 207);
            UserLabel.Margin = new Padding(0, 7, 0, 5);
            UserLabel.Name = "UserLabel";
            UserLabel.Size = new Size(54, 20);
            UserLabel.TabIndex = 3;
            UserLabel.Text = "用户名";
            // 
            // UserText
            // 
            UserText.Dock = DockStyle.Fill;
            UserText.Location = new Point(0, 232);
            UserText.Margin = new Padding(0, 0, 0, 5);
            UserText.Name = "UserText";
            UserText.PlaceholderText = "MySQL 用户名";
            UserText.Size = new Size(329, 27);
            UserText.TabIndex = 3;
            // 
            // PasswordLabel
            // 
            PasswordLabel.AutoSize = true;
            PasswordLabel.ForeColor = Color.FromArgb(61, 73, 89);
            PasswordLabel.Location = new Point(0, 271);
            PasswordLabel.Margin = new Padding(0, 7, 0, 5);
            PasswordLabel.Name = "PasswordLabel";
            PasswordLabel.Size = new Size(39, 20);
            PasswordLabel.TabIndex = 4;
            PasswordLabel.Text = "密码";
            // 
            // PasswordPanel
            // 
            PasswordPanel.Controls.Add(PasswordText);
            PasswordPanel.Controls.Add(ShowPasswordCheckBox);
            PasswordPanel.Dock = DockStyle.Fill;
            PasswordPanel.Location = new Point(0, 296);
            PasswordPanel.Margin = new Padding(0, 0, 0, 5);
            PasswordPanel.Name = "PasswordPanel";
            PasswordPanel.Size = new Size(329, 32);
            PasswordPanel.TabIndex = 5;
            // 
            // PasswordText
            // 
            PasswordText.Dock = DockStyle.Fill;
            PasswordText.Location = new Point(0, 0);
            PasswordText.Name = "PasswordText";
            PasswordText.PlaceholderText = "MySQL 密码";
            PasswordText.Size = new Size(268, 27);
            PasswordText.TabIndex = 4;
            PasswordText.UseSystemPasswordChar = true;
            // 
            // ShowPasswordCheckBox
            // 
            ShowPasswordCheckBox.AutoSize = true;
            ShowPasswordCheckBox.Dock = DockStyle.Right;
            ShowPasswordCheckBox.ForeColor = Color.FromArgb(90, 100, 115);
            ShowPasswordCheckBox.Location = new Point(268, 0);
            ShowPasswordCheckBox.Name = "ShowPasswordCheckBox";
            ShowPasswordCheckBox.Size = new Size(61, 32);
            ShowPasswordCheckBox.TabIndex = 5;
            ShowPasswordCheckBox.Text = "显示";
            // 
            // DefaultDatabaseLabel
            // 
            DefaultDatabaseLabel.AutoSize = true;
            DefaultDatabaseLabel.ForeColor = Color.FromArgb(61, 73, 89);
            DefaultDatabaseLabel.Location = new Point(0, 340);
            DefaultDatabaseLabel.Margin = new Padding(0, 7, 0, 5);
            DefaultDatabaseLabel.Name = "DefaultDatabaseLabel";
            DefaultDatabaseLabel.Size = new Size(144, 20);
            DefaultDatabaseLabel.TabIndex = 6;
            DefaultDatabaseLabel.Text = "默认数据库（可选）";
            // 
            // DefaultDatabaseText
            // 
            DefaultDatabaseText.Dock = DockStyle.Fill;
            DefaultDatabaseText.Location = new Point(0, 365);
            DefaultDatabaseText.Margin = new Padding(0, 0, 0, 5);
            DefaultDatabaseText.Name = "DefaultDatabaseText";
            DefaultDatabaseText.PlaceholderText = "连接后默认使用的数据库";
            DefaultDatabaseText.Size = new Size(329, 27);
            DefaultDatabaseText.TabIndex = 6;
            // 
            // CharacterSetLabel
            // 
            CharacterSetLabel.AutoSize = true;
            CharacterSetLabel.ForeColor = Color.FromArgb(61, 73, 89);
            CharacterSetLabel.Location = new Point(0, 404);
            CharacterSetLabel.Margin = new Padding(0, 7, 0, 5);
            CharacterSetLabel.Name = "CharacterSetLabel";
            CharacterSetLabel.Size = new Size(54, 20);
            CharacterSetLabel.TabIndex = 7;
            CharacterSetLabel.Text = "字符集";
            // 
            // CharacterSetComboBox
            // 
            CharacterSetComboBox.Dock = DockStyle.Fill;
            CharacterSetComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            CharacterSetComboBox.Items.AddRange(new object[] { "utf8mb4", "utf8", "latin1" });
            CharacterSetComboBox.Location = new Point(0, 429);
            CharacterSetComboBox.Margin = new Padding(0, 0, 0, 5);
            CharacterSetComboBox.Name = "CharacterSetComboBox";
            CharacterSetComboBox.Size = new Size(329, 28);
            CharacterSetComboBox.TabIndex = 7;
            // 
            // SslModeLabel
            // 
            SslModeLabel.AutoSize = true;
            SslModeLabel.ForeColor = Color.FromArgb(61, 73, 89);
            SslModeLabel.Location = new Point(0, 469);
            SslModeLabel.Margin = new Padding(0, 7, 0, 5);
            SslModeLabel.Name = "SslModeLabel";
            SslModeLabel.Size = new Size(69, 20);
            SslModeLabel.TabIndex = 8;
            SslModeLabel.Text = "SSL 模式";
            // 
            // SslModeComboBox
            // 
            SslModeComboBox.Dock = DockStyle.Fill;
            SslModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SslModeComboBox.Items.AddRange(new object[] { "Preferred", "Required", "VerifyCA", "VerifyFull", "Disabled" });
            SslModeComboBox.Location = new Point(0, 494);
            SslModeComboBox.Margin = new Padding(0, 0, 0, 5);
            SslModeComboBox.Name = "SslModeComboBox";
            SslModeComboBox.Size = new Size(329, 28);
            SslModeComboBox.TabIndex = 8;
            // 
            // ConnectionOptionsPanel
            // 
            ConnectionOptionsPanel.Controls.Add(SavePasswordCheckBox);
            ConnectionOptionsPanel.Controls.Add(TimeoutNumericUpDown);
            ConnectionOptionsPanel.Controls.Add(TimeoutLabel);
            ConnectionOptionsPanel.Dock = DockStyle.Top;
            ConnectionOptionsPanel.Location = new Point(0, 697);
            ConnectionOptionsPanel.Margin = new Padding(0, 0, 0, 8);
            ConnectionOptionsPanel.Name = "ConnectionOptionsPanel";
            ConnectionOptionsPanel.Size = new Size(329, 36);
            ConnectionOptionsPanel.TabIndex = 2;
            // 
            // SavePasswordCheckBox
            // 
            SavePasswordCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SavePasswordCheckBox.AutoSize = true;
            SavePasswordCheckBox.ForeColor = Color.FromArgb(61, 73, 89);
            SavePasswordCheckBox.Location = new Point(235, 4);
            SavePasswordCheckBox.Name = "SavePasswordCheckBox";
            SavePasswordCheckBox.Size = new Size(91, 24);
            SavePasswordCheckBox.TabIndex = 10;
            SavePasswordCheckBox.Text = "加密保存密码";
            // 
            // TimeoutNumericUpDown
            // 
            TimeoutNumericUpDown.Location = new Point(116, 2);
            TimeoutNumericUpDown.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            TimeoutNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            TimeoutNumericUpDown.Name = "TimeoutNumericUpDown";
            TimeoutNumericUpDown.Size = new Size(65, 27);
            TimeoutNumericUpDown.TabIndex = 9;
            TimeoutNumericUpDown.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // TimeoutLabel
            // 
            TimeoutLabel.AutoSize = true;
            TimeoutLabel.ForeColor = Color.FromArgb(61, 73, 89);
            TimeoutLabel.Location = new Point(0, 6);
            TimeoutLabel.Name = "TimeoutLabel";
            TimeoutLabel.Size = new Size(114, 20);
            TimeoutLabel.TabIndex = 11;
            TimeoutLabel.Text = "连接超时（秒）";
            // 
            // ConnectionButtonTable
            // 
            ConnectionButtonTable.ColumnCount = 2;
            ConnectionButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            ConnectionButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            ConnectionButtonTable.Controls.Add(TestButton, 0, 0);
            ConnectionButtonTable.Controls.Add(ConnectButton, 1, 0);
            ConnectionButtonTable.Dock = DockStyle.Top;
            ConnectionButtonTable.Location = new Point(0, 741);
            ConnectionButtonTable.Margin = new Padding(0, 0, 0, 8);
            ConnectionButtonTable.Name = "ConnectionButtonTable";
            ConnectionButtonTable.RowCount = 1;
            ConnectionButtonTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            ConnectionButtonTable.Size = new Size(329, 42);
            ConnectionButtonTable.TabIndex = 3;
            // 
            // TestButton
            // 
            TestButton.BackColor = Color.White;
            TestButton.Dock = DockStyle.Fill;
            TestButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            TestButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            TestButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            TestButton.FlatStyle = FlatStyle.Flat;
            TestButton.ForeColor = Color.FromArgb(30, 41, 59);
            TestButton.Location = new Point(0, 0);
            TestButton.Margin = new Padding(0, 0, 6, 0);
            TestButton.Name = "TestButton";
            TestButton.Size = new Size(142, 42);
            TestButton.TabIndex = 11;
            TestButton.Text = "测试连接";
            TestButton.UseVisualStyleBackColor = false;
            // 
            // ConnectButton
            // 
            ConnectButton.BackColor = Color.FromArgb(37, 99, 183);
            ConnectButton.Dock = DockStyle.Fill;
            ConnectButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            ConnectButton.FlatAppearance.BorderSize = 0;
            ConnectButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 63, 119);
            ConnectButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 79, 149);
            ConnectButton.FlatStyle = FlatStyle.Flat;
            ConnectButton.ForeColor = Color.White;
            ConnectButton.Location = new Point(154, 0);
            ConnectButton.Margin = new Padding(6, 0, 0, 0);
            ConnectButton.Name = "ConnectButton";
            ConnectButton.Size = new Size(175, 42);
            ConnectButton.TabIndex = 12;
            ConnectButton.Text = "连接 MySQL";
            ConnectButton.UseVisualStyleBackColor = false;
            // 
            // SecondaryButtonTable
            // 
            SecondaryButtonTable.ColumnCount = 2;
            SecondaryButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            SecondaryButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            SecondaryButtonTable.Controls.Add(SaveConnectionButton, 0, 0);
            SecondaryButtonTable.Controls.Add(DeleteConnectionButton, 1, 0);
            SecondaryButtonTable.Dock = DockStyle.Top;
            SecondaryButtonTable.Location = new Point(0, 791);
            SecondaryButtonTable.Margin = new Padding(0, 0, 0, 8);
            SecondaryButtonTable.Name = "SecondaryButtonTable";
            SecondaryButtonTable.RowCount = 1;
            SecondaryButtonTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            SecondaryButtonTable.Size = new Size(329, 34);
            SecondaryButtonTable.TabIndex = 4;
            // 
            // SaveConnectionButton
            // 
            SaveConnectionButton.BackColor = Color.White;
            SaveConnectionButton.Dock = DockStyle.Fill;
            SaveConnectionButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            SaveConnectionButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            SaveConnectionButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            SaveConnectionButton.FlatStyle = FlatStyle.Flat;
            SaveConnectionButton.ForeColor = Color.FromArgb(30, 41, 59);
            SaveConnectionButton.Location = new Point(0, 0);
            SaveConnectionButton.Margin = new Padding(0, 0, 6, 0);
            SaveConnectionButton.Name = "SaveConnectionButton";
            SaveConnectionButton.Size = new Size(158, 34);
            SaveConnectionButton.TabIndex = 13;
            SaveConnectionButton.Text = "保存配置";
            SaveConnectionButton.UseVisualStyleBackColor = false;
            // 
            // DeleteConnectionButton
            // 
            DeleteConnectionButton.BackColor = Color.White;
            DeleteConnectionButton.Dock = DockStyle.Fill;
            DeleteConnectionButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            DeleteConnectionButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            DeleteConnectionButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            DeleteConnectionButton.FlatStyle = FlatStyle.Flat;
            DeleteConnectionButton.ForeColor = Color.FromArgb(171, 47, 47);
            DeleteConnectionButton.Location = new Point(170, 0);
            DeleteConnectionButton.Margin = new Padding(6, 0, 0, 0);
            DeleteConnectionButton.Name = "DeleteConnectionButton";
            DeleteConnectionButton.Size = new Size(159, 34);
            DeleteConnectionButton.TabIndex = 14;
            DeleteConnectionButton.Text = "删除配置";
            DeleteConnectionButton.UseVisualStyleBackColor = false;
            // 
            // AuthenticationPanel
            // 
            AuthenticationPanel.AutoSize = true;
            AuthenticationPanel.Controls.Add(IntegratedSecurityCheckBox);
            AuthenticationPanel.Controls.Add(TrustCertificateCheckBox);
            AuthenticationPanel.Dock = DockStyle.Top;
            AuthenticationPanel.Location = new Point(3, 634);
            AuthenticationPanel.Name = "AuthenticationPanel";
            AuthenticationPanel.Size = new Size(323, 60);
            AuthenticationPanel.TabIndex = 5;
            // 
            // IntegratedSecurityCheckBox
            // 
            IntegratedSecurityCheckBox.AutoSize = true;
            IntegratedSecurityCheckBox.Location = new Point(3, 3);
            IntegratedSecurityCheckBox.Name = "IntegratedSecurityCheckBox";
            IntegratedSecurityCheckBox.Size = new Size(162, 24);
            IntegratedSecurityCheckBox.TabIndex = 0;
            IntegratedSecurityCheckBox.Text = "Windows 身份验证";
            // 
            // TrustCertificateCheckBox
            // 
            TrustCertificateCheckBox.AutoSize = true;
            TrustCertificateCheckBox.Location = new Point(3, 33);
            TrustCertificateCheckBox.Name = "TrustCertificateCheckBox";
            TrustCertificateCheckBox.Size = new Size(226, 24);
            TrustCertificateCheckBox.TabIndex = 1;
            TrustCertificateCheckBox.Text = "信任服务器证书（本地测试）";
            // 
            // QueryParametersPanel
            // 
            QueryParametersPanel.Controls.Add(DatabaseLabel);
            QueryParametersPanel.Controls.Add(DatabaseComboBox);
            QueryParametersPanel.Controls.Add(QueryTimeoutLabel);
            QueryParametersPanel.Controls.Add(QueryTimeoutNumericUpDown);
            QueryParametersPanel.Controls.Add(ReadOnlyCheckBox);
            QueryParametersPanel.Dock = DockStyle.Top;
            QueryParametersPanel.Location = new Point(0, 0);
            QueryParametersPanel.Margin = new Padding(0);
            QueryParametersPanel.Name = "QueryParametersPanel";
            QueryParametersPanel.Size = new Size(948, 42);
            QueryParametersPanel.TabIndex = 1;
            QueryParametersPanel.WrapContents = false;
            // 
            // DatabaseLabel
            // 
            DatabaseLabel.AutoSize = true;
            DatabaseLabel.ForeColor = Color.FromArgb(61, 73, 89);
            DatabaseLabel.Location = new Point(0, 8);
            DatabaseLabel.Margin = new Padding(0, 8, 10, 0);
            DatabaseLabel.Name = "DatabaseLabel";
            DatabaseLabel.Size = new Size(54, 20);
            DatabaseLabel.TabIndex = 22;
            DatabaseLabel.Text = "数据库";
            // 
            // DatabaseComboBox
            // 
            DatabaseComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DatabaseComboBox.FormattingEnabled = true;
            DatabaseComboBox.Location = new Point(64, 4);
            DatabaseComboBox.Margin = new Padding(0, 4, 10, 0);
            DatabaseComboBox.Name = "DatabaseComboBox";
            DatabaseComboBox.Size = new Size(185, 28);
            DatabaseComboBox.TabIndex = 15;
            // 
            // QueryTimeoutLabel
            // 
            QueryTimeoutLabel.AutoSize = true;
            QueryTimeoutLabel.ForeColor = Color.FromArgb(61, 73, 89);
            QueryTimeoutLabel.Location = new Point(259, 8);
            QueryTimeoutLabel.Margin = new Padding(0, 8, 10, 0);
            QueryTimeoutLabel.Name = "QueryTimeoutLabel";
            QueryTimeoutLabel.Size = new Size(39, 20);
            QueryTimeoutLabel.TabIndex = 21;
            QueryTimeoutLabel.Text = "超时";
            // 
            // QueryTimeoutNumericUpDown
            // 
            QueryTimeoutNumericUpDown.Location = new Point(308, 4);
            QueryTimeoutNumericUpDown.Margin = new Padding(0, 4, 10, 0);
            QueryTimeoutNumericUpDown.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            QueryTimeoutNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            QueryTimeoutNumericUpDown.Name = "QueryTimeoutNumericUpDown";
            QueryTimeoutNumericUpDown.Size = new Size(65, 27);
            QueryTimeoutNumericUpDown.TabIndex = 16;
            QueryTimeoutNumericUpDown.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // ReadOnlyCheckBox
            // 
            ReadOnlyCheckBox.AutoSize = true;
            ReadOnlyCheckBox.Enabled = false;
            ReadOnlyCheckBox.Checked = true;
            ReadOnlyCheckBox.CheckState = CheckState.Checked;
            ReadOnlyCheckBox.ForeColor = Color.FromArgb(61, 73, 89);
            ReadOnlyCheckBox.Location = new Point(383, 4);
            ReadOnlyCheckBox.Margin = new Padding(0, 4, 10, 0);
            ReadOnlyCheckBox.Name = "ReadOnlyCheckBox";
            ReadOnlyCheckBox.Size = new Size(91, 24);
            ReadOnlyCheckBox.TabIndex = 17;
            ReadOnlyCheckBox.Text = "只读模式";
            // 
            // QueryActionsPanel
            // 
            QueryActionsPanel.Controls.Add(ExecuteQueryButton);
            QueryActionsPanel.Controls.Add(StopQueryButton);
            QueryActionsPanel.Controls.Add(ClearSqlButton);
            QueryActionsPanel.Controls.Add(RefreshButton);
            QueryActionsPanel.Controls.Add(ExportButton);
            QueryActionsPanel.Dock = DockStyle.Bottom;
            QueryActionsPanel.Location = new Point(0, 46);
            QueryActionsPanel.Margin = new Padding(0);
            QueryActionsPanel.Name = "QueryActionsPanel";
            QueryActionsPanel.Size = new Size(948, 48);
            QueryActionsPanel.TabIndex = 0;
            QueryActionsPanel.WrapContents = false;
            // 
            // ExecuteQueryButton
            // 
            ExecuteQueryButton.BackColor = Color.FromArgb(37, 99, 183);
            ExecuteQueryButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            ExecuteQueryButton.FlatAppearance.BorderSize = 0;
            ExecuteQueryButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 63, 119);
            ExecuteQueryButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 79, 149);
            ExecuteQueryButton.FlatStyle = FlatStyle.Flat;
            ExecuteQueryButton.ForeColor = Color.White;
            ExecuteQueryButton.Location = new Point(0, 4);
            ExecuteQueryButton.Margin = new Padding(0, 4, 8, 6);
            ExecuteQueryButton.Name = "ExecuteQueryButton";
            ExecuteQueryButton.Size = new Size(118, 34);
            ExecuteQueryButton.TabIndex = 18;
            ExecuteQueryButton.Text = "▶ 执行查询";
            ExecuteQueryButton.UseVisualStyleBackColor = false;
            // 
            // StopQueryButton
            // 
            StopQueryButton.BackColor = Color.White;
            StopQueryButton.Enabled = false;
            StopQueryButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            StopQueryButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            StopQueryButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            StopQueryButton.FlatStyle = FlatStyle.Flat;
            StopQueryButton.ForeColor = Color.FromArgb(171, 47, 47);
            StopQueryButton.Location = new Point(126, 4);
            StopQueryButton.Margin = new Padding(0, 4, 8, 6);
            StopQueryButton.Name = "StopQueryButton";
            StopQueryButton.Size = new Size(88, 34);
            StopQueryButton.TabIndex = 19;
            StopQueryButton.Text = "停止";
            StopQueryButton.UseVisualStyleBackColor = false;
            // 
            // ClearSqlButton
            // 
            ClearSqlButton.BackColor = Color.White;
            ClearSqlButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            ClearSqlButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            ClearSqlButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            ClearSqlButton.FlatStyle = FlatStyle.Flat;
            ClearSqlButton.ForeColor = Color.FromArgb(30, 41, 59);
            ClearSqlButton.Location = new Point(222, 4);
            ClearSqlButton.Margin = new Padding(0, 4, 8, 6);
            ClearSqlButton.Name = "ClearSqlButton";
            ClearSqlButton.Size = new Size(92, 34);
            ClearSqlButton.TabIndex = 20;
            ClearSqlButton.Text = "清空";
            ClearSqlButton.UseVisualStyleBackColor = false;
            // 
            // RefreshButton
            // 
            RefreshButton.BackColor = Color.White;
            RefreshButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            RefreshButton.FlatStyle = FlatStyle.Flat;
            RefreshButton.ForeColor = Color.FromArgb(30, 41, 59);
            RefreshButton.Location = new Point(325, 3);
            RefreshButton.Name = "RefreshButton";
            RefreshButton.Size = new Size(100, 36);
            RefreshButton.TabIndex = 21;
            RefreshButton.Text = "刷新对象";
            RefreshButton.UseVisualStyleBackColor = false;
            // 
            // ExportButton
            // 
            ExportButton.BackColor = Color.White;
            ExportButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            ExportButton.FlatStyle = FlatStyle.Flat;
            ExportButton.ForeColor = Color.FromArgb(30, 41, 59);
            ExportButton.Location = new Point(431, 3);
            ExportButton.Name = "ExportButton";
            ExportButton.Size = new Size(100, 36);
            ExportButton.TabIndex = 22;
            ExportButton.Text = "导出 CSV";
            ExportButton.UseVisualStyleBackColor = false;
            // 
            // HeaderPanel
            // 
            HeaderPanel.BackColor = Color.FromArgb(24, 42, 66);
            HeaderPanel.Controls.Add(HeaderTitleLabel);
            HeaderPanel.Dock = DockStyle.Top;
            HeaderPanel.Location = new Point(0, 0);
            HeaderPanel.Name = "HeaderPanel";
            HeaderPanel.Size = new Size(1384, 64);
            HeaderPanel.TabIndex = 0;
            // 
            // HeaderTitleLabel
            // 
            HeaderTitleLabel.AutoSize = true;
            HeaderTitleLabel.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold);
            HeaderTitleLabel.ForeColor = Color.White;
            HeaderTitleLabel.Location = new Point(24, 13);
            HeaderTitleLabel.Name = "HeaderTitleLabel";
            HeaderTitleLabel.Size = new Size(190, 33);
            HeaderTitleLabel.TabIndex = 0;
            HeaderTitleLabel.Text = "多数据库查询工作台";
            // 
            // MainSplitContainer
            // 
            MainSplitContainer.BackColor = Color.FromArgb(211, 221, 233);
            MainSplitContainer.Dock = DockStyle.Fill;
            MainSplitContainer.FixedPanel = FixedPanel.Panel1;
            MainSplitContainer.Location = new Point(0, 64);
            MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            MainSplitContainer.Panel1.BackColor = Color.FromArgb(244, 247, 251);
            MainSplitContainer.Panel1.Controls.Add(ConnectionPanel);
            MainSplitContainer.Panel1MinSize = 360;
            // 
            // MainSplitContainer.Panel2
            // 
            MainSplitContainer.Panel2.BackColor = Color.White;
            MainSplitContainer.Panel2.Controls.Add(WorkspaceSplitContainer);
            MainSplitContainer.Panel2MinSize = 620;
            MainSplitContainer.Size = new Size(1384, 738);
            MainSplitContainer.SplitterDistance = 390;
            MainSplitContainer.SplitterWidth = 6;
            MainSplitContainer.TabIndex = 1;
            // 
            // ConnectionPanel
            // 
            ConnectionPanel.AutoScroll = true;
            ConnectionPanel.BackColor = Color.FromArgb(244, 247, 251);
            ConnectionPanel.Controls.Add(ConnectionLayout);
            ConnectionPanel.Controls.Add(ConnectionTipLabel);
            ConnectionPanel.Dock = DockStyle.Fill;
            ConnectionPanel.Location = new Point(0, 0);
            ConnectionPanel.Name = "ConnectionPanel";
            ConnectionPanel.Padding = new Padding(20, 16, 20, 20);
            ConnectionPanel.Size = new Size(390, 738);
            ConnectionPanel.TabIndex = 0;
            // 
            // ConnectionTipLabel
            // 
            ConnectionTipLabel.AutoSize = true;
            ConnectionTipLabel.ForeColor = Color.FromArgb(112, 122, 136);
            ConnectionTipLabel.Location = new Point(189, 27);
            ConnectionTipLabel.Name = "ConnectionTipLabel";
            ConnectionTipLabel.Size = new Size(157, 20);
            ConnectionTipLabel.TabIndex = 5;
            ConnectionTipLabel.Text = "只读查询 · 双击表预览前 200 行 · F5 执行";
            ConnectionTipLabel.Visible = false;
            // 
            // WorkspaceSplitContainer
            // 
            WorkspaceSplitContainer.BackColor = Color.FromArgb(211, 221, 233);
            WorkspaceSplitContainer.Dock = DockStyle.Fill;
            WorkspaceSplitContainer.Location = new Point(0, 0);
            WorkspaceSplitContainer.Name = "WorkspaceSplitContainer";
            WorkspaceSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // WorkspaceSplitContainer.Panel1
            // 
            WorkspaceSplitContainer.Panel1.BackColor = Color.White;
            WorkspaceSplitContainer.Panel1.Controls.Add(QueryPanel);
            WorkspaceSplitContainer.Panel1MinSize = 250;
            // 
            // WorkspaceSplitContainer.Panel2
            // 
            WorkspaceSplitContainer.Panel2.BackColor = Color.White;
            WorkspaceSplitContainer.Panel2.Controls.Add(ResultTabControl);
            WorkspaceSplitContainer.Panel2.Controls.Add(ResultSummaryPanel);
            WorkspaceSplitContainer.Panel2MinSize = 240;
            WorkspaceSplitContainer.Size = new Size(988, 738);
            WorkspaceSplitContainer.SplitterDistance = 338;
            WorkspaceSplitContainer.SplitterWidth = 6;
            WorkspaceSplitContainer.TabIndex = 0;
            // 
            // QueryPanel
            // 
            QueryPanel.Controls.Add(QueryEditorPanel);
            QueryPanel.Controls.Add(QueryToolbarPanel);
            QueryPanel.Controls.Add(QuerySectionLabel);
            QueryPanel.Dock = DockStyle.Fill;
            QueryPanel.Location = new Point(0, 0);
            QueryPanel.Name = "QueryPanel";
            QueryPanel.Padding = new Padding(20, 16, 20, 14);
            QueryPanel.Size = new Size(988, 338);
            QueryPanel.TabIndex = 0;
            // 
            // QueryEditorPanel
            // 
            QueryEditorPanel.BackColor = Color.FromArgb(248, 250, 253);
            QueryEditorPanel.BorderStyle = BorderStyle.FixedSingle;
            QueryEditorPanel.Controls.Add(SqlEditorTextBox);
            QueryEditorPanel.Dock = DockStyle.Fill;
            QueryEditorPanel.Location = new Point(20, 143);
            QueryEditorPanel.Name = "QueryEditorPanel";
            QueryEditorPanel.Padding = new Padding(10);
            QueryEditorPanel.Size = new Size(948, 181);
            QueryEditorPanel.TabIndex = 0;
            // 
            // SqlEditorTextBox
            // 
            SqlEditorTextBox.AcceptsTab = true;
            SqlEditorTextBox.BackColor = Color.FromArgb(248, 250, 253);
            SqlEditorTextBox.BorderStyle = BorderStyle.None;
            SqlEditorTextBox.Dock = DockStyle.Fill;
            SqlEditorTextBox.Font = new Font("Consolas", 11F);
            SqlEditorTextBox.ForeColor = Color.FromArgb(30, 41, 59);
            SqlEditorTextBox.Location = new Point(10, 10);
            SqlEditorTextBox.Name = "SqlEditorTextBox";
            SqlEditorTextBox.Size = new Size(926, 159);
            SqlEditorTextBox.TabIndex = 20;
            SqlEditorTextBox.ContextMenuStrip = SqlEditorContextMenu;
            SqlEditorTextBox.ShortcutsEnabled = true;
            SqlEditorTextBox.Text = "-- 在此输入 SQL 查询语句\n";
            // 
            // QueryToolbarPanel
            // 
            QueryToolbarPanel.Controls.Add(QueryActionsPanel);
            QueryToolbarPanel.Controls.Add(QueryParametersPanel);
            QueryToolbarPanel.Dock = DockStyle.Top;
            QueryToolbarPanel.Location = new Point(20, 49);
            QueryToolbarPanel.Name = "QueryToolbarPanel";
            QueryToolbarPanel.Size = new Size(948, 94);
            QueryToolbarPanel.TabIndex = 1;
            // 
            // QuerySectionLabel
            // 
            QuerySectionLabel.AutoSize = true;
            QuerySectionLabel.Dock = DockStyle.Top;
            QuerySectionLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            QuerySectionLabel.ForeColor = Color.FromArgb(35, 46, 61);
            QuerySectionLabel.Location = new Point(20, 16);
            QuerySectionLabel.Name = "QuerySectionLabel";
            QuerySectionLabel.Padding = new Padding(0, 0, 0, 6);
            QuerySectionLabel.Size = new Size(97, 33);
            QuerySectionLabel.TabIndex = 2;
            QuerySectionLabel.Text = "SQL 查询";
            // 
            // ResultTabControl
            // 
            ResultTabControl.Controls.Add(_databaseObjectsTabPage);
            ResultTabControl.Controls.Add(ResultTabPage);
            ResultTabControl.Controls.Add(MessageTabPage);
            ResultTabControl.Dock = DockStyle.Fill;
            ResultTabControl.Location = new Point(0, 40);
            ResultTabControl.Name = "ResultTabControl";
            ResultTabControl.Padding = new Point(16, 5);
            ResultTabControl.SelectedIndex = 0;
            ResultTabControl.Size = new Size(988, 354);
            ResultTabControl.TabIndex = 21;
            // 
            // ResultTabPage
            // 
            ResultTabPage.Controls.Add(dataGridView1);
            ResultTabPage.Location = new Point(4, 33);
            ResultTabPage.Name = "ResultTabPage";
            ResultTabPage.Padding = new Padding(8);
            ResultTabPage.Size = new Size(980, 317);
            ResultTabPage.TabIndex = 0;
            ResultTabPage.Text = "查询结果";
            ResultTabPage.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(247, 249, 252);
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(233, 239, 247);
            dataGridViewCellStyle2.Font = new Font("Microsoft YaHei UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(233, 239, 247);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.ColumnHeadersHeight = 34;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Microsoft YaHei UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(218, 232, 251);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(20, 51, 93);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.GridColor = Color.FromArgb(211, 221, 233);
            dataGridView1.Location = new Point(8, 8);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 48;
            dataGridView1.RowTemplate.Height = 32;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(964, 301);
            dataGridView1.TabIndex = 0;
            // 
            // MessageTabPage
            // 
            MessageTabPage.Controls.Add(MessageTextBox);
            MessageTabPage.Location = new Point(4, 33);
            MessageTabPage.Name = "MessageTabPage";
            MessageTabPage.Padding = new Padding(8);
            MessageTabPage.Size = new Size(980, 317);
            MessageTabPage.TabIndex = 1;
            MessageTabPage.Text = "执行消息";
            MessageTabPage.UseVisualStyleBackColor = true;
            // 
            // MessageTextBox
            // 
            MessageTextBox.BackColor = Color.FromArgb(250, 251, 253);
            MessageTextBox.BorderStyle = BorderStyle.None;
            MessageTextBox.Dock = DockStyle.Fill;
            MessageTextBox.Font = new Font("Consolas", 10F);
            MessageTextBox.ForeColor = Color.FromArgb(83, 99, 119);
            MessageTextBox.Location = new Point(8, 8);
            MessageTextBox.Name = "MessageTextBox";
            MessageTextBox.ReadOnly = true;
            MessageTextBox.Size = new Size(964, 301);
            MessageTextBox.TabIndex = 0;
            MessageTextBox.Text = "等待执行查询…";
            // 
            // ResultSummaryPanel
            // 
            ResultSummaryPanel.BackColor = Color.FromArgb(244, 247, 251);
            ResultSummaryPanel.Controls.Add(ResultStateLabel);
            ResultSummaryPanel.Controls.Add(ResultSummaryLabel);
            ResultSummaryPanel.Dock = DockStyle.Top;
            ResultSummaryPanel.Location = new Point(0, 0);
            ResultSummaryPanel.Name = "ResultSummaryPanel";
            ResultSummaryPanel.Size = new Size(988, 40);
            ResultSummaryPanel.TabIndex = 22;
            // 
            // ResultStateLabel
            // 
            ResultStateLabel.Dock = DockStyle.Right;
            ResultStateLabel.ForeColor = Color.FromArgb(83, 99, 119);
            ResultStateLabel.Location = new Point(668, 0);
            ResultStateLabel.Name = "ResultStateLabel";
            ResultStateLabel.Padding = new Padding(0, 0, 16, 0);
            ResultStateLabel.Size = new Size(320, 40);
            ResultStateLabel.TabIndex = 0;
            ResultStateLabel.Text = "尚未执行查询";
            ResultStateLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ResultSummaryLabel
            // 
            ResultSummaryLabel.AutoSize = true;
            ResultSummaryLabel.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            ResultSummaryLabel.ForeColor = Color.FromArgb(35, 46, 61);
            ResultSummaryLabel.Location = new Point(17, 9);
            ResultSummaryLabel.Name = "ResultSummaryLabel";
            ResultSummaryLabel.Size = new Size(78, 24);
            ResultSummaryLabel.TabIndex = 1;
            ResultSummaryLabel.Text = "结果预览";
            // 
            // MainStatusStrip
            // 
            MainStatusStrip.BackColor = Color.FromArgb(244, 247, 251);
            MainStatusStrip.ForeColor = Color.FromArgb(83, 99, 119);
            MainStatusStrip.ImageScalingSize = new Size(20, 20);
            MainStatusStrip.Items.AddRange(new ToolStripItem[] { ConnectionStatusLabel, StatusSpringLabel, CurrentDatabaseStatusLabel });
            MainStatusStrip.Location = new Point(0, 802);
            MainStatusStrip.Name = "MainStatusStrip";
            MainStatusStrip.Size = new Size(1384, 26);
            MainStatusStrip.TabIndex = 2;
            // 
            // ConnectionStatusLabel
            // 
            ConnectionStatusLabel.ForeColor = Color.FromArgb(112, 122, 136);
            ConnectionStatusLabel.Name = "ConnectionStatusLabel";
            ConnectionStatusLabel.Size = new Size(70, 20);
            ConnectionStatusLabel.Text = "● 未连接";
            // 
            // StatusSpringLabel
            // 
            StatusSpringLabel.Name = "StatusSpringLabel";
            StatusSpringLabel.Size = new Size(1178, 20);
            StatusSpringLabel.Spring = true;
            // 
            // CurrentDatabaseStatusLabel
            // 
            CurrentDatabaseStatusLabel.ForeColor = Color.FromArgb(112, 122, 136);
            CurrentDatabaseStatusLabel.Name = "CurrentDatabaseStatusLabel";
            CurrentDatabaseStatusLabel.Size = new Size(121, 20);
            CurrentDatabaseStatusLabel.Text = "数据库：未选择";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 247, 251);
            ClientSize = new Size(1384, 828);
            Controls.Add(MainSplitContainer);
            Controls.Add(HeaderPanel);
            Controls.Add(MainStatusStrip);
            Font = new Font("Microsoft YaHei UI", 9F);
            ForeColor = Color.FromArgb(30, 41, 59);
            MinimumSize = new Size(1120, 800);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            Text = "多数据库浏览器 · MySQL / SQLite / SQL Server / Oracle";
            WindowState = FormWindowState.Maximized;
            SqlEditorContextMenu.ResumeLayout(false);
            _databaseObjectsTabPage.ResumeLayout(false);
            _hostInputPanel.ResumeLayout(false);
            _hostInputPanel.PerformLayout();
            ConnectionLayout.ResumeLayout(false);
            ConnectionLayout.PerformLayout();
            ConnectionHeading.ResumeLayout(false);
            ConnectionHeading.PerformLayout();
            SavedConnectionsPanel.ResumeLayout(false);
            ConnectionFieldsTable.ResumeLayout(false);
            ConnectionFieldsTable.PerformLayout();
            PortInputPanel.ResumeLayout(false);
            PortInputPanel.PerformLayout();
            PasswordPanel.ResumeLayout(false);
            PasswordPanel.PerformLayout();
            ConnectionOptionsPanel.ResumeLayout(false);
            ConnectionOptionsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TimeoutNumericUpDown).EndInit();
            ConnectionButtonTable.ResumeLayout(false);
            SecondaryButtonTable.ResumeLayout(false);
            AuthenticationPanel.ResumeLayout(false);
            AuthenticationPanel.PerformLayout();
            QueryParametersPanel.ResumeLayout(false);
            QueryParametersPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)QueryTimeoutNumericUpDown).EndInit();
            QueryActionsPanel.ResumeLayout(false);
            HeaderPanel.ResumeLayout(false);
            HeaderPanel.PerformLayout();
            MainSplitContainer.Panel1.ResumeLayout(false);
            MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MainSplitContainer).EndInit();
            MainSplitContainer.ResumeLayout(false);
            ConnectionPanel.ResumeLayout(false);
            ConnectionPanel.PerformLayout();
            WorkspaceSplitContainer.Panel1.ResumeLayout(false);
            WorkspaceSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)WorkspaceSplitContainer).EndInit();
            WorkspaceSplitContainer.ResumeLayout(false);
            QueryPanel.ResumeLayout(false);
            QueryPanel.PerformLayout();
            QueryEditorPanel.ResumeLayout(false);
            QueryToolbarPanel.ResumeLayout(false);
            ResultTabControl.ResumeLayout(false);
            ResultTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            MessageTabPage.ResumeLayout(false);
            ResultSummaryPanel.ResumeLayout(false);
            ResultSummaryPanel.PerformLayout();
            MainStatusStrip.ResumeLayout(false);
            MainStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ContextMenuStrip SqlEditorContextMenu;
        private ToolStripMenuItem CopySqlMenuItem;
        private ToolStripMenuItem PasteSqlMenuItem;
        private ToolStripMenuItem SelectAllSqlMenuItem;
        private ComboBox SavedConnectionsComboBox;
        private Button NewConnectionButton;
        private TableLayoutPanel SavedConnectionsPanel;
        private FlowLayoutPanel AuthenticationPanel;
        private CheckBox IntegratedSecurityCheckBox;
        private CheckBox TrustCertificateCheckBox;
        private Button RefreshButton;
        private Button ExportButton;
        private TreeView _databaseTreeView;
        private TabPage _databaseObjectsTabPage;
        private ComboBox _databaseTypeComboBox;
        private Button _browseSqliteButton;
        private Panel _hostInputPanel;
        private ComboBox _sqliteFileComboBox;
        private TableLayoutPanel ConnectionLayout;
        private TableLayoutPanel ConnectionHeading;
        private FlowLayoutPanel QueryParametersPanel;
        private FlowLayoutPanel QueryActionsPanel;
        private Panel PortInputPanel;
        private FolderBrowserDialog SqliteFolderDialog;
        private Panel HeaderPanel;
        private Label HeaderTitleLabel;
        private SplitContainer MainSplitContainer;
        private Panel ConnectionPanel;
        private TableLayoutPanel ConnectionFieldsTable;
        private Label ConnectionnameLabel;
        private TextBox ConnectionnameText;
        private Label HostLabel;
        private TextBox HostText;
        private Label PortLabel;
        private TextBox PortText;
        private Label UserLabel;
        private TextBox UserText;
        private Label PasswordLabel;
        private Panel PasswordPanel;
        private TextBox PasswordText;
        private CheckBox ShowPasswordCheckBox;
        private Label DefaultDatabaseLabel;
        private TextBox DefaultDatabaseText;
        private Label CharacterSetLabel;
        private ComboBox CharacterSetComboBox;
        private Label SslModeLabel;
        private ComboBox SslModeComboBox;
        private Label TimeoutLabel;
        private NumericUpDown TimeoutNumericUpDown;
        private Panel ConnectionOptionsPanel;
        private CheckBox SavePasswordCheckBox;
        private TableLayoutPanel ConnectionButtonTable;
        private Button TestButton;
        private Button ConnectButton;
        private TableLayoutPanel SecondaryButtonTable;
        private Button SaveConnectionButton;
        private Button DeleteConnectionButton;
        private Label ConnectionTipLabel;
        private Label ConnectionSectionLabel;
        private SplitContainer WorkspaceSplitContainer;
        private Panel QueryPanel;
        private Panel QueryEditorPanel;
        private RichTextBox SqlEditorTextBox;
        private Panel QueryToolbarPanel;
        private Button ClearSqlButton;
        private Button StopQueryButton;
        private Button ExecuteQueryButton;
        private CheckBox ReadOnlyCheckBox;
        private NumericUpDown QueryTimeoutNumericUpDown;
        private Label QueryTimeoutLabel;
        private ComboBox DatabaseComboBox;
        private Label DatabaseLabel;
        private Label QuerySectionLabel;
        private TabControl ResultTabControl;
        private TabPage ResultTabPage;
        private DataGridView dataGridView1;
        private TabPage MessageTabPage;
        private RichTextBox MessageTextBox;
        private Panel ResultSummaryPanel;
        private Label ResultStateLabel;
        private Label ResultSummaryLabel;
        private StatusStrip MainStatusStrip;
        private ToolStripStatusLabel ConnectionStatusLabel;
        private ToolStripStatusLabel StatusSpringLabel;
        private ToolStripStatusLabel CurrentDatabaseStatusLabel;
    }
}