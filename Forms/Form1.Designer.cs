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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            _databaseTreeView = new TreeView();
            _databaseObjectsTabPage = new TabPage();
            _databaseTypeComboBox = new ComboBox();
            _browseSqliteButton = new Button();
            _hostInputPanel = new Panel();
            _sqliteFileComboBox = new ComboBox();
            SyncLayout = new TableLayoutPanel();
            ConnectionLayout = new TableLayoutPanel();
            ConnectionHeading = new TableLayoutPanel();
            QueryParametersPanel = new FlowLayoutPanel();
            QueryActionsPanel = new FlowLayoutPanel();
            PortInputPanel = new Panel();
            TrayMenu = new ContextMenuStrip(components);
            ShowWindowMenuItem = new ToolStripMenuItem();
            SyncNowMenuItem = new ToolStripMenuItem();
            ExitMenuItem = new ToolStripMenuItem();
            SqliteFolderDialog = new FolderBrowserDialog();
            components.Add(SqliteFolderDialog);
            HeaderPanel = new Panel();
            _syncPanel = new FlowLayoutPanel();
            _syncDeviceLabel = new Label();
            _syncDeviceTextBox = new TextBox();
            _syncServerLabel = new Label();
            _syncServerTextBox = new TextBox();
            _autoSyncCheckBox = new CheckBox();
            _fullSyncButton = new Button();
            _syncNowButton = new Button();
            _syncStatusLabel = new Label();
            HeaderTitleLabel = new Label();
            _notifyIcon = new NotifyIcon(components);
            MainSplitContainer = new SplitContainer();
            ConnectionPanel = new Panel();
            ConnectionFieldsTable = new TableLayoutPanel();
            ConnectionnameLabel = new Label();
            ConnectionnameText = new TextBox();
            HostLabel = new Label();
            HostText = new TextBox();
            PortLabel = new Label();
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
            ConnectionTipLabel = new Label();
            ConnectionSectionLabel = new Label();
            WorkspaceSplitContainer = new SplitContainer();
            QueryPanel = new Panel();
            QueryEditorPanel = new Panel();
            SqlEditorTextBox = new RichTextBox();
            QueryToolbarPanel = new Panel();
            ClearSqlButton = new Button();
            StopQueryButton = new Button();
            ExecuteQueryButton = new Button();
            ReadOnlyCheckBox = new CheckBox();
            QueryTimeoutNumericUpDown = new NumericUpDown();
            QueryTimeoutLabel = new Label();
            DatabaseComboBox = new ComboBox();
            DatabaseLabel = new Label();
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
            _databaseObjectsTabPage.SuspendLayout();
            _hostInputPanel.SuspendLayout();
            SyncLayout.SuspendLayout();
            ConnectionLayout.SuspendLayout();
            ConnectionHeading.SuspendLayout();
            QueryParametersPanel.SuspendLayout();
            QueryActionsPanel.SuspendLayout();
            PortInputPanel.SuspendLayout();
            TrayMenu.SuspendLayout();
            HeaderPanel.SuspendLayout();
            _syncPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MainSplitContainer).BeginInit();
            MainSplitContainer.Panel1.SuspendLayout();
            MainSplitContainer.Panel2.SuspendLayout();
            MainSplitContainer.SuspendLayout();
            ConnectionPanel.SuspendLayout();
            ConnectionFieldsTable.SuspendLayout();
            PasswordPanel.SuspendLayout();
            ConnectionOptionsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TimeoutNumericUpDown).BeginInit();
            ConnectionButtonTable.SuspendLayout();
            SecondaryButtonTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)WorkspaceSplitContainer).BeginInit();
            WorkspaceSplitContainer.Panel1.SuspendLayout();
            WorkspaceSplitContainer.Panel2.SuspendLayout();
            WorkspaceSplitContainer.SuspendLayout();
            QueryPanel.SuspendLayout();
            QueryEditorPanel.SuspendLayout();
            QueryToolbarPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)QueryTimeoutNumericUpDown).BeginInit();
            ResultTabControl.SuspendLayout();
            ResultTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            MessageTabPage.SuspendLayout();
            ResultSummaryPanel.SuspendLayout();
            MainStatusStrip.SuspendLayout();
            SuspendLayout();
            //
            // _databaseTreeView
            //
            _databaseTreeView.Dock = DockStyle.Fill;
            _databaseTreeView.BorderStyle = BorderStyle.None;
            _databaseTreeView.HideSelection = false;
            _databaseTreeView.ShowNodeToolTips = true;
            _databaseTreeView.ItemHeight = 30;
            _databaseTreeView.ForeColor = Color.FromArgb(30, 41, 59);
            _databaseTreeView.BackColor = Color.White;
            _databaseTreeView.FullRowSelect = true;
            _databaseTreeView.Name = "_databaseTreeView";
            //
            // _databaseObjectsTabPage
            //
            _databaseObjectsTabPage.Text = "数据库对象";
            _databaseObjectsTabPage.Padding = new Padding(8);
            _databaseObjectsTabPage.UseVisualStyleBackColor = true;
            _databaseObjectsTabPage.Controls.Add(_databaseTreeView);
            _databaseObjectsTabPage.Name = "_databaseObjectsTabPage";
            //
            // _databaseTypeComboBox
            //
            _databaseTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _databaseTypeComboBox.Items.AddRange(new object[] { "MySQL", "SQLite" });
            _databaseTypeComboBox.Dock = DockStyle.Fill;
            _databaseTypeComboBox.Margin = new Padding(0, 7, 0, 0);
            _databaseTypeComboBox.Name = "_databaseTypeComboBox";
            //
            // _browseSqliteButton
            //
            _browseSqliteButton.Text = "选择文件夹";
            _browseSqliteButton.Dock = DockStyle.Right;
            _browseSqliteButton.Width = 105;
            _browseSqliteButton.Visible = false;
            _browseSqliteButton.FlatStyle = FlatStyle.Flat;
            _browseSqliteButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            _browseSqliteButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            _browseSqliteButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            _browseSqliteButton.BackColor = Color.White;
            _browseSqliteButton.ForeColor = Color.FromArgb(30, 41, 59);
            _browseSqliteButton.UseVisualStyleBackColor = false;
            _browseSqliteButton.Name = "_browseSqliteButton";
            //
            // _hostInputPanel
            //
            _hostInputPanel.Dock = DockStyle.Fill;
            _hostInputPanel.Margin = new Padding(0, 0, 0, 5);
            _hostInputPanel.Controls.Add(HostText);
            _hostInputPanel.Controls.Add(_browseSqliteButton);
            _hostInputPanel.Height = 31;
            _hostInputPanel.Name = "_hostInputPanel";
            //
            // _sqliteFileComboBox
            //
            _sqliteFileComboBox.Dock = DockStyle.Fill;
            _sqliteFileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _sqliteFileComboBox.Visible = false;
            _sqliteFileComboBox.Name = "_sqliteFileComboBox";
            //
            // SyncLayout
            //
            SyncLayout.Dock = DockStyle.Bottom;
            SyncLayout.Height = 100;
            SyncLayout.RowCount = 2;
            SyncLayout.ColumnCount = 1;
            SyncLayout.BackColor = Color.FromArgb(244, 247, 251);
            SyncLayout.Margin = Padding.Empty;
            SyncLayout.Padding = Padding.Empty;
            SyncLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            SyncLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            SyncLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            SyncLayout.Controls.Add(_syncPanel, 0, 0);
            SyncLayout.Controls.Add(_syncStatusLabel, 0, 1);
            SyncLayout.Name = "SyncLayout";
            //
            // ConnectionLayout
            //
            ConnectionLayout.Dock = DockStyle.Top;
            ConnectionLayout.AutoSize = true;
            ConnectionLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ConnectionLayout.ColumnCount = 1;
            ConnectionLayout.RowCount = 5;
            ConnectionLayout.Margin = Padding.Empty;
            ConnectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            ConnectionLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionLayout.Controls.Add(ConnectionHeading, 0, 0);
            ConnectionLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionLayout.Controls.Add(ConnectionFieldsTable, 0, 1);
            ConnectionLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionLayout.Controls.Add(ConnectionOptionsPanel, 0, 2);
            ConnectionLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionLayout.Controls.Add(ConnectionButtonTable, 0, 3);
            ConnectionLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionLayout.Controls.Add(SecondaryButtonTable, 0, 4);
            ConnectionLayout.Name = "ConnectionLayout";
            //
            // ConnectionHeading
            //
            ConnectionHeading.Height = 44;
            ConnectionHeading.ColumnCount = 2;
            ConnectionHeading.RowCount = 1;
            ConnectionHeading.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108));
            ConnectionHeading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            ConnectionHeading.Controls.Add(ConnectionSectionLabel, 0, 0);
            ConnectionHeading.Controls.Add(_databaseTypeComboBox, 1, 0);
            ConnectionHeading.Dock = DockStyle.Top;
            ConnectionHeading.Margin = new Padding(0, 0, 0, 8);
            ConnectionHeading.Name = "ConnectionHeading";
            //
            // QueryParametersPanel
            //
            QueryParametersPanel.Dock = DockStyle.Top;
            QueryParametersPanel.Height = 42;
            QueryParametersPanel.WrapContents = false;
            QueryParametersPanel.Margin = Padding.Empty;
            QueryParametersPanel.Controls.Add(DatabaseLabel);
            QueryParametersPanel.Controls.Add(DatabaseComboBox);
            QueryParametersPanel.Controls.Add(QueryTimeoutLabel);
            QueryParametersPanel.Controls.Add(QueryTimeoutNumericUpDown);
            QueryParametersPanel.Controls.Add(ReadOnlyCheckBox);
            QueryParametersPanel.Name = "QueryParametersPanel";
            //
            // QueryActionsPanel
            //
            QueryActionsPanel.Dock = DockStyle.Bottom;
            QueryActionsPanel.Height = 48;
            QueryActionsPanel.WrapContents = false;
            QueryActionsPanel.Margin = Padding.Empty;
            QueryActionsPanel.Controls.Add(ExecuteQueryButton);
            QueryActionsPanel.Controls.Add(StopQueryButton);
            QueryActionsPanel.Controls.Add(ClearSqlButton);
            QueryActionsPanel.Name = "QueryActionsPanel";
            //
            // PortInputPanel
            //
            PortInputPanel.Dock = DockStyle.Fill;
            PortInputPanel.Height = 31;
            PortInputPanel.Margin = new Padding(0, 0, 0, 5);
            PortInputPanel.Controls.Add(PortText);
            PortInputPanel.Controls.Add(_sqliteFileComboBox);
            PortInputPanel.Name = "PortInputPanel";
            //
            // TrayMenu
            //
            TrayMenu.Items.AddRange(new ToolStripItem[] { ShowWindowMenuItem, SyncNowMenuItem, ExitMenuItem });
            TrayMenu.Name = "TrayMenu";
            //
            // ShowWindowMenuItem
            //
            ShowWindowMenuItem.Text = "显示窗口";
            ShowWindowMenuItem.Name = "ShowWindowMenuItem";
            //
            // SyncNowMenuItem
            //
            SyncNowMenuItem.Text = "立即同步";
            SyncNowMenuItem.Name = "SyncNowMenuItem";
            //
            // ExitMenuItem
            //
            ExitMenuItem.Text = "退出";
            ExitMenuItem.Name = "ExitMenuItem";
            //
            // SqliteFolderDialog
            //
            SqliteFolderDialog.Description = "选择包含 SQLite 数据库文件的文件夹";
            SqliteFolderDialog.UseDescriptionForTitle = true;
            SqliteFolderDialog.ShowNewFolderButton = false;
            //
            // HeaderPanel
            //
            HeaderPanel.Controls.Add(HeaderTitleLabel);
            HeaderPanel.Dock = DockStyle.Top;
            HeaderPanel.Location = new Point(0, 0);
            HeaderPanel.Name = "HeaderPanel";
            HeaderPanel.Size = new Size(1384, 72);
            HeaderPanel.TabIndex = 0;
            HeaderPanel.BackColor = Color.FromArgb(24, 42, 66);
            HeaderPanel.Padding = Padding.Empty;
            HeaderPanel.Height = 158;
            HeaderPanel.Controls.Add(SyncLayout);
            //
            // _syncPanel
            //
            _syncPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _syncPanel.Controls.Add(_syncDeviceLabel);
            _syncPanel.Controls.Add(_syncDeviceTextBox);
            _syncPanel.Controls.Add(_syncServerLabel);
            _syncPanel.Controls.Add(_syncServerTextBox);
            _syncPanel.Controls.Add(_autoSyncCheckBox);
            _syncPanel.Controls.Add(_fullSyncButton);
            _syncPanel.Controls.Add(_syncNowButton);
            _syncPanel.Location = new Point(559, 11);
            _syncPanel.Name = "_syncPanel";
            _syncPanel.Size = new Size(801, 53);
            _syncPanel.TabIndex = 2;
            _syncPanel.WrapContents = false;
            _syncPanel.Dock = DockStyle.Fill;
            _syncPanel.AutoSize = false;
            _syncPanel.Padding = new Padding(20, 5, 12, 0);
            _syncPanel.BackColor = Color.FromArgb(244, 247, 251);
            //
            // _syncDeviceLabel
            //
            _syncDeviceLabel.AutoSize = true;
            _syncDeviceLabel.Location = new Point(8, 14);
            _syncDeviceLabel.Margin = new Padding(0, 7, 3, 0);
            _syncDeviceLabel.Name = "_syncDeviceLabel";
            _syncDeviceLabel.Size = new Size(54, 20);
            _syncDeviceLabel.TabIndex = 0;
            _syncDeviceLabel.Text = "设备号";
            _syncDeviceLabel.ForeColor = Color.FromArgb(30, 41, 59);
            //
            // _syncDeviceTextBox
            //
            _syncDeviceTextBox.Location = new Point(65, 10);
            _syncDeviceTextBox.Margin = new Padding(0, 3, 8, 0);
            _syncDeviceTextBox.Name = "_syncDeviceTextBox";
            _syncDeviceTextBox.Size = new Size(110, 27);
            _syncDeviceTextBox.TabIndex = 1;
            _syncDeviceTextBox.Text = Environment.MachineName;
            //
            // _syncServerLabel
            //
            _syncServerLabel.AutoSize = true;
            _syncServerLabel.Location = new Point(183, 14);
            _syncServerLabel.Margin = new Padding(0, 7, 3, 0);
            _syncServerLabel.Name = "_syncServerLabel";
            _syncServerLabel.Size = new Size(54, 20);
            _syncServerLabel.TabIndex = 2;
            _syncServerLabel.Text = "服务端";
            _syncServerLabel.ForeColor = Color.FromArgb(30, 41, 59);
            //
            // _syncServerTextBox
            //
            _syncServerTextBox.Location = new Point(240, 10);
            _syncServerTextBox.Margin = new Padding(0, 3, 8, 0);
            _syncServerTextBox.Name = "_syncServerTextBox";
            _syncServerTextBox.PlaceholderText = "例如：http://服务器地址:8080";
            _syncServerTextBox.Size = new Size(180, 27);
            _syncServerTextBox.TabIndex = 3;
            _syncServerTextBox.Width = 260;
            //
            // _autoSyncCheckBox
            //
            _autoSyncCheckBox.AutoSize = true;
            _autoSyncCheckBox.Location = new Point(428, 12);
            _autoSyncCheckBox.Margin = new Padding(0, 5, 8, 0);
            _autoSyncCheckBox.Name = "_autoSyncCheckBox";
            _autoSyncCheckBox.Size = new Size(91, 24);
            _autoSyncCheckBox.TabIndex = 4;
            _autoSyncCheckBox.Text = "自动同步";
            _autoSyncCheckBox.ForeColor = Color.FromArgb(30, 41, 59);
            //
            // _fullSyncButton
            //
            _fullSyncButton.AutoSize = true;
            _fullSyncButton.Location = new Point(527, 10);
            _fullSyncButton.Margin = new Padding(0, 3, 4, 0);
            _fullSyncButton.Name = "_fullSyncButton";
            _fullSyncButton.Size = new Size(85, 32);
            _fullSyncButton.TabIndex = 5;
            _fullSyncButton.Text = "全量同步";
            _fullSyncButton.FlatStyle = FlatStyle.Flat;
            _fullSyncButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            _fullSyncButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            _fullSyncButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            _fullSyncButton.BackColor = Color.White;
            _fullSyncButton.ForeColor = Color.FromArgb(30, 41, 59);
            _fullSyncButton.UseVisualStyleBackColor = false;
            //
            // _syncNowButton
            //
            _syncNowButton.AutoSize = true;
            _syncNowButton.Location = new Point(616, 10);
            _syncNowButton.Margin = new Padding(0, 3, 8, 0);
            _syncNowButton.Name = "_syncNowButton";
            _syncNowButton.Size = new Size(85, 32);
            _syncNowButton.TabIndex = 6;
            _syncNowButton.Text = "立即同步";
            _syncNowButton.FlatStyle = FlatStyle.Flat;
            _syncNowButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            _syncNowButton.UseVisualStyleBackColor = false;
            _syncNowButton.BackColor = Color.FromArgb(37, 99, 183);
            _syncNowButton.ForeColor = Color.White;
            _syncNowButton.FlatAppearance.BorderSize = 0;
            _syncNowButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 79, 149);
            _syncNowButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 63, 119);
            //
            // _syncStatusLabel
            //
            _syncStatusLabel.Location = new Point(709, 14);
            _syncStatusLabel.Name = "_syncStatusLabel";
            _syncStatusLabel.Size = new Size(84, 20);
            _syncStatusLabel.TabIndex = 7;
            _syncStatusLabel.Text = "同步未启动";
            _syncStatusLabel.AutoSize = false;
            _syncStatusLabel.Dock = DockStyle.Fill;
            _syncStatusLabel.AutoEllipsis = true;
            _syncStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            _syncStatusLabel.ForeColor = Color.FromArgb(83, 99, 119);
            _syncStatusLabel.Margin = new Padding(24, 0, 20, 0);
            //
            // _notifyIcon
            //
            _notifyIcon.Icon = (Icon)resources.GetObject("_notifyIcon.Icon");
            _notifyIcon.Text = "电检同步";
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenuStrip = TrayMenu;
            //
            // HeaderTitleLabel
            //
            HeaderTitleLabel.AutoSize = true;
            HeaderTitleLabel.ForeColor = Color.White;
            HeaderTitleLabel.Name = "HeaderTitleLabel";
            HeaderTitleLabel.Size = new Size(204, 36);
            HeaderTitleLabel.TabIndex = 0;
            HeaderTitleLabel.Text = "多数据库工作台";
            HeaderTitleLabel.Location = new Point(24, 13);
            HeaderTitleLabel.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold);
            //
            // MainSplitContainer
            //
            MainSplitContainer.Dock = DockStyle.Fill;
            MainSplitContainer.FixedPanel = FixedPanel.Panel1;
            MainSplitContainer.Location = new Point(0, 72);
            MainSplitContainer.Name = "MainSplitContainer";
            MainSplitContainer.Panel1.Controls.Add(ConnectionPanel);
            MainSplitContainer.Panel1MinSize = 360;
            MainSplitContainer.Panel2.BackColor = Color.White;
            MainSplitContainer.Panel2.Controls.Add(WorkspaceSplitContainer);
            MainSplitContainer.Panel2MinSize = 620;
            MainSplitContainer.Size = new Size(1384, 730);
            MainSplitContainer.SplitterDistance = 390;
            MainSplitContainer.TabIndex = 1;
            MainSplitContainer.BackColor = Color.FromArgb(211, 221, 233);
            MainSplitContainer.Panel1.BackColor = Color.FromArgb(244, 247, 251);
            MainSplitContainer.SplitterWidth = 6;
            //
            // ConnectionPanel
            //
            ConnectionPanel.AutoScroll = true;
            ConnectionPanel.Dock = DockStyle.Fill;
            ConnectionPanel.Location = new Point(0, 0);
            ConnectionPanel.Name = "ConnectionPanel";
            ConnectionPanel.Size = new Size(390, 730);
            ConnectionPanel.TabIndex = 0;
            ConnectionPanel.BackColor = Color.FromArgb(244, 247, 251);
            ConnectionPanel.Padding = new Padding(20, 16, 20, 20);
            ConnectionPanel.Controls.Add(ConnectionLayout);
            ConnectionPanel.Controls.Add(ConnectionTipLabel);
            //
            // ConnectionFieldsTable
            //
            ConnectionFieldsTable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            ConnectionFieldsTable.Location = new Point(22, 58);
            ConnectionFieldsTable.Name = "ConnectionFieldsTable";
            ConnectionFieldsTable.RowCount = 16;
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ConnectionFieldsTable.Size = new Size(346, 520);
            ConnectionFieldsTable.TabIndex = 1;
            ConnectionFieldsTable.AutoSize = true;
            ConnectionFieldsTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ConnectionFieldsTable.Dock = DockStyle.Top;
            ConnectionFieldsTable.Margin = new Padding(0, 0, 0, 8);
            //
            // ConnectionnameLabel
            //
            ConnectionnameLabel.AutoSize = true;
            ConnectionnameLabel.ForeColor = Color.FromArgb(61, 73, 89);
            ConnectionnameLabel.Location = new Point(3, 0);
            ConnectionnameLabel.Name = "ConnectionnameLabel";
            ConnectionnameLabel.Size = new Size(69, 20);
            ConnectionnameLabel.TabIndex = 0;
            ConnectionnameLabel.Text = "连接名称";
            ConnectionnameLabel.Margin = new Padding(0, 7, 0, 5);
            //
            // ConnectionnameText
            //
            ConnectionnameText.Dock = DockStyle.Fill;
            ConnectionnameText.Location = new Point(3, 28);
            ConnectionnameText.Name = "ConnectionnameText";
            ConnectionnameText.PlaceholderText = "例如：本地开发库";
            ConnectionnameText.Size = new Size(340, 27);
            ConnectionnameText.TabIndex = 0;
            ConnectionnameText.Margin = new Padding(0, 0, 0, 5);
            //
            // HostLabel
            //
            HostLabel.AutoSize = true;
            HostLabel.ForeColor = Color.FromArgb(61, 73, 89);
            HostLabel.Location = new Point(3, 65);
            HostLabel.Name = "HostLabel";
            HostLabel.Size = new Size(69, 20);
            HostLabel.TabIndex = 1;
            HostLabel.Text = "主机地址";
            HostLabel.Margin = new Padding(0, 7, 0, 5);
            //
            // HostText
            //
            HostText.Location = new Point(3, 93);
            HostText.Name = "HostText";
            HostText.PlaceholderText = "localhost 或服务器 IP";
            HostText.Size = new Size(340, 27);
            HostText.TabIndex = 1;
            HostText.Dock = DockStyle.Fill;
            HostText.Margin = new Padding(0, 0, 0, 5);
            //
            // PortLabel
            //
            PortLabel.AutoSize = true;
            PortLabel.ForeColor = Color.FromArgb(61, 73, 89);
            PortLabel.Location = new Point(3, 130);
            PortLabel.Name = "PortLabel";
            PortLabel.Size = new Size(39, 20);
            PortLabel.TabIndex = 2;
            PortLabel.Text = "端口";
            PortLabel.Margin = new Padding(0, 7, 0, 5);
            //
            // PortText
            //
            PortText.Dock = DockStyle.Fill;
            PortText.Location = new Point(3, 158);
            PortText.Name = "PortText";
            PortText.PlaceholderText = "3306";
            PortText.Size = new Size(340, 27);
            PortText.TabIndex = 2;
            PortText.Margin = new Padding(0, 0, 0, 5);
            //
            // UserLabel
            //
            UserLabel.AutoSize = true;
            UserLabel.ForeColor = Color.FromArgb(61, 73, 89);
            UserLabel.Location = new Point(3, 195);
            UserLabel.Name = "UserLabel";
            UserLabel.Size = new Size(54, 20);
            UserLabel.TabIndex = 3;
            UserLabel.Text = "用户名";
            UserLabel.Margin = new Padding(0, 7, 0, 5);
            //
            // UserText
            //
            UserText.Dock = DockStyle.Fill;
            UserText.Location = new Point(3, 223);
            UserText.Name = "UserText";
            UserText.PlaceholderText = "MySQL 用户名";
            UserText.Size = new Size(340, 27);
            UserText.TabIndex = 3;
            UserText.Margin = new Padding(0, 0, 0, 5);
            //
            // PasswordLabel
            //
            PasswordLabel.AutoSize = true;
            PasswordLabel.ForeColor = Color.FromArgb(61, 73, 89);
            PasswordLabel.Location = new Point(3, 260);
            PasswordLabel.Name = "PasswordLabel";
            PasswordLabel.Size = new Size(39, 20);
            PasswordLabel.TabIndex = 4;
            PasswordLabel.Text = "密码";
            PasswordLabel.Margin = new Padding(0, 7, 0, 5);
            //
            // PasswordPanel
            //
            PasswordPanel.Controls.Add(PasswordText);
            PasswordPanel.Controls.Add(ShowPasswordCheckBox);
            PasswordPanel.Dock = DockStyle.Fill;
            PasswordPanel.Location = new Point(0, 285);
            PasswordPanel.Name = "PasswordPanel";
            PasswordPanel.Size = new Size(346, 32);
            PasswordPanel.TabIndex = 5;
            PasswordPanel.Margin = new Padding(0, 0, 0, 5);
            //
            // PasswordText
            //
            PasswordText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PasswordText.Location = new Point(0, 0);
            PasswordText.Name = "PasswordText";
            PasswordText.PlaceholderText = "MySQL 密码";
            PasswordText.Size = new Size(411, 27);
            PasswordText.TabIndex = 4;
            PasswordText.UseSystemPasswordChar = true;
            PasswordText.Dock = DockStyle.Fill;
            //
            // ShowPasswordCheckBox
            //
            ShowPasswordCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ShowPasswordCheckBox.AutoSize = true;
            ShowPasswordCheckBox.ForeColor = Color.FromArgb(90, 100, 115);
            ShowPasswordCheckBox.Location = new Point(421, 3);
            ShowPasswordCheckBox.Name = "ShowPasswordCheckBox";
            ShowPasswordCheckBox.Size = new Size(61, 24);
            ShowPasswordCheckBox.TabIndex = 5;
            ShowPasswordCheckBox.Text = "显示";
            ShowPasswordCheckBox.Dock = DockStyle.Right;
            //
            // DefaultDatabaseLabel
            //
            DefaultDatabaseLabel.AutoSize = true;
            DefaultDatabaseLabel.ForeColor = Color.FromArgb(61, 73, 89);
            DefaultDatabaseLabel.Location = new Point(3, 325);
            DefaultDatabaseLabel.Name = "DefaultDatabaseLabel";
            DefaultDatabaseLabel.Size = new Size(144, 20);
            DefaultDatabaseLabel.TabIndex = 6;
            DefaultDatabaseLabel.Text = "默认数据库（可选）";
            DefaultDatabaseLabel.Margin = new Padding(0, 7, 0, 5);
            //
            // DefaultDatabaseText
            //
            DefaultDatabaseText.Dock = DockStyle.Fill;
            DefaultDatabaseText.Location = new Point(3, 353);
            DefaultDatabaseText.Name = "DefaultDatabaseText";
            DefaultDatabaseText.PlaceholderText = "连接后默认使用的数据库";
            DefaultDatabaseText.Size = new Size(340, 27);
            DefaultDatabaseText.TabIndex = 6;
            DefaultDatabaseText.Margin = new Padding(0, 0, 0, 5);
            //
            // CharacterSetLabel
            //
            CharacterSetLabel.AutoSize = true;
            CharacterSetLabel.ForeColor = Color.FromArgb(61, 73, 89);
            CharacterSetLabel.Location = new Point(3, 390);
            CharacterSetLabel.Name = "CharacterSetLabel";
            CharacterSetLabel.Size = new Size(54, 20);
            CharacterSetLabel.TabIndex = 7;
            CharacterSetLabel.Text = "字符集";
            CharacterSetLabel.Margin = new Padding(0, 7, 0, 5);
            //
            // CharacterSetComboBox
            //
            CharacterSetComboBox.Dock = DockStyle.Fill;
            CharacterSetComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            CharacterSetComboBox.Items.AddRange(new object[] { "utf8mb4", "utf8", "latin1" });
            CharacterSetComboBox.Location = new Point(3, 418);
            CharacterSetComboBox.Name = "CharacterSetComboBox";
            CharacterSetComboBox.Size = new Size(340, 28);
            CharacterSetComboBox.TabIndex = 7;
            CharacterSetComboBox.Margin = new Padding(0, 0, 0, 5);
            //
            // SslModeLabel
            //
            SslModeLabel.AutoSize = true;
            SslModeLabel.ForeColor = Color.FromArgb(61, 73, 89);
            SslModeLabel.Location = new Point(3, 455);
            SslModeLabel.Name = "SslModeLabel";
            SslModeLabel.Size = new Size(69, 20);
            SslModeLabel.TabIndex = 8;
            SslModeLabel.Text = "SSL 模式";
            SslModeLabel.Margin = new Padding(0, 7, 0, 5);
            //
            // SslModeComboBox
            //
            SslModeComboBox.Dock = DockStyle.Fill;
            SslModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SslModeComboBox.Items.AddRange(new object[] { "Preferred", "Required", "VerifyCA", "VerifyFull", "Disabled" });
            SslModeComboBox.Location = new Point(3, 483);
            SslModeComboBox.Name = "SslModeComboBox";
            SslModeComboBox.Size = new Size(340, 28);
            SslModeComboBox.TabIndex = 8;
            SslModeComboBox.Margin = new Padding(0, 0, 0, 5);
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
            // ConnectionOptionsPanel
            //
            ConnectionOptionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ConnectionOptionsPanel.Controls.Add(SavePasswordCheckBox);
            ConnectionOptionsPanel.Controls.Add(TimeoutNumericUpDown);
            ConnectionOptionsPanel.Controls.Add(TimeoutLabel);
            ConnectionOptionsPanel.Location = new Point(22, 584);
            ConnectionOptionsPanel.Name = "ConnectionOptionsPanel";
            ConnectionOptionsPanel.Size = new Size(346, 34);
            ConnectionOptionsPanel.TabIndex = 2;
            ConnectionOptionsPanel.Height = 36;
            ConnectionOptionsPanel.Dock = DockStyle.Top;
            ConnectionOptionsPanel.Margin = new Padding(0, 0, 0, 8);
            //
            // SavePasswordCheckBox
            //
            SavePasswordCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SavePasswordCheckBox.AutoSize = true;
            SavePasswordCheckBox.ForeColor = Color.FromArgb(61, 73, 89);
            SavePasswordCheckBox.Location = new Point(252, 4);
            SavePasswordCheckBox.Name = "SavePasswordCheckBox";
            SavePasswordCheckBox.Size = new Size(91, 24);
            SavePasswordCheckBox.TabIndex = 10;
            SavePasswordCheckBox.Text = "保存密码";
            //
            // ConnectionButtonTable
            //
            ConnectionButtonTable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ConnectionButtonTable.ColumnCount = 2;
            ConnectionButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            ConnectionButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            ConnectionButtonTable.Controls.Add(TestButton, 0, 0);
            ConnectionButtonTable.Controls.Add(ConnectButton, 1, 0);
            ConnectionButtonTable.Location = new Point(22, 626);
            ConnectionButtonTable.Name = "ConnectionButtonTable";
            ConnectionButtonTable.RowCount = 1;
            ConnectionButtonTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            ConnectionButtonTable.Size = new Size(346, 42);
            ConnectionButtonTable.TabIndex = 3;
            ConnectionButtonTable.Dock = DockStyle.Top;
            ConnectionButtonTable.Margin = new Padding(0, 0, 0, 8);
            //
            // TestButton
            //
            TestButton.Dock = DockStyle.Fill;
            TestButton.Location = new Point(0, 0);
            TestButton.Margin = new Padding(0, 0, 6, 0);
            TestButton.Name = "TestButton";
            TestButton.Size = new Size(149, 42);
            TestButton.TabIndex = 11;
            TestButton.Text = "测试连接";
            TestButton.Click += TestButton_Click;
            TestButton.FlatStyle = FlatStyle.Flat;
            TestButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            TestButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            TestButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            TestButton.BackColor = Color.White;
            TestButton.ForeColor = Color.FromArgb(30, 41, 59);
            TestButton.UseVisualStyleBackColor = false;
            //
            // ConnectButton
            //
            ConnectButton.Dock = DockStyle.Fill;
            ConnectButton.Location = new Point(161, 0);
            ConnectButton.Margin = new Padding(6, 0, 0, 0);
            ConnectButton.Name = "ConnectButton";
            ConnectButton.Size = new Size(185, 42);
            ConnectButton.TabIndex = 12;
            ConnectButton.Text = "连接 MySQL";
            ConnectButton.FlatStyle = FlatStyle.Flat;
            ConnectButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            ConnectButton.UseVisualStyleBackColor = false;
            ConnectButton.BackColor = Color.FromArgb(37, 99, 183);
            ConnectButton.ForeColor = Color.White;
            ConnectButton.FlatAppearance.BorderSize = 0;
            ConnectButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 79, 149);
            ConnectButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 63, 119);
            //
            // SecondaryButtonTable
            //
            SecondaryButtonTable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            SecondaryButtonTable.ColumnCount = 2;
            SecondaryButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            SecondaryButtonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            SecondaryButtonTable.Controls.Add(SaveConnectionButton, 0, 0);
            SecondaryButtonTable.Controls.Add(DeleteConnectionButton, 1, 0);
            SecondaryButtonTable.Location = new Point(22, 676);
            SecondaryButtonTable.Name = "SecondaryButtonTable";
            SecondaryButtonTable.RowCount = 1;
            SecondaryButtonTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            SecondaryButtonTable.Size = new Size(346, 34);
            SecondaryButtonTable.TabIndex = 4;
            SecondaryButtonTable.Dock = DockStyle.Top;
            SecondaryButtonTable.Margin = new Padding(0, 0, 0, 8);
            //
            // SaveConnectionButton
            //
            SaveConnectionButton.Dock = DockStyle.Fill;
            SaveConnectionButton.Location = new Point(0, 0);
            SaveConnectionButton.Margin = new Padding(0, 0, 6, 0);
            SaveConnectionButton.Name = "SaveConnectionButton";
            SaveConnectionButton.Size = new Size(167, 34);
            SaveConnectionButton.TabIndex = 13;
            SaveConnectionButton.Text = "保存配置";
            SaveConnectionButton.FlatStyle = FlatStyle.Flat;
            SaveConnectionButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            SaveConnectionButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            SaveConnectionButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            SaveConnectionButton.BackColor = Color.White;
            SaveConnectionButton.ForeColor = Color.FromArgb(30, 41, 59);
            SaveConnectionButton.UseVisualStyleBackColor = false;
            //
            // DeleteConnectionButton
            //
            DeleteConnectionButton.Dock = DockStyle.Fill;
            DeleteConnectionButton.Location = new Point(179, 0);
            DeleteConnectionButton.Margin = new Padding(6, 0, 0, 0);
            DeleteConnectionButton.Name = "DeleteConnectionButton";
            DeleteConnectionButton.Size = new Size(167, 34);
            DeleteConnectionButton.TabIndex = 14;
            DeleteConnectionButton.Text = "删除配置";
            DeleteConnectionButton.FlatStyle = FlatStyle.Flat;
            DeleteConnectionButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            DeleteConnectionButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            DeleteConnectionButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            DeleteConnectionButton.BackColor = Color.White;
            DeleteConnectionButton.UseVisualStyleBackColor = false;
            DeleteConnectionButton.ForeColor = Color.FromArgb(171, 47, 47);
            //
            // ConnectionTipLabel
            //
            ConnectionTipLabel.AutoSize = true;
            ConnectionTipLabel.ForeColor = Color.FromArgb(112, 122, 136);
            ConnectionTipLabel.Location = new Point(189, 27);
            ConnectionTipLabel.Name = "ConnectionTipLabel";
            ConnectionTipLabel.Size = new Size(157, 20);
            ConnectionTipLabel.TabIndex = 5;
            ConnectionTipLabel.Text = "支持 MySQL / SQLite";
            ConnectionTipLabel.Visible = false;
            //
            // ConnectionSectionLabel
            //
            ConnectionSectionLabel.AutoSize = true;
            ConnectionSectionLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            ConnectionSectionLabel.ForeColor = Color.FromArgb(35, 46, 61);
            ConnectionSectionLabel.Location = new Point(20, 20);
            ConnectionSectionLabel.Name = "ConnectionSectionLabel";
            ConnectionSectionLabel.Size = new Size(92, 27);
            ConnectionSectionLabel.TabIndex = 6;
            ConnectionSectionLabel.Text = "连接设置";
            ConnectionSectionLabel.Dock = DockStyle.Fill;
            ConnectionSectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            ConnectionSectionLabel.Margin = Padding.Empty;
            //
            // WorkspaceSplitContainer
            //
            WorkspaceSplitContainer.Dock = DockStyle.Fill;
            WorkspaceSplitContainer.Location = new Point(0, 0);
            WorkspaceSplitContainer.Name = "WorkspaceSplitContainer";
            WorkspaceSplitContainer.Orientation = Orientation.Horizontal;
            WorkspaceSplitContainer.Panel1.Controls.Add(QueryPanel);
            WorkspaceSplitContainer.Panel1MinSize = 250;
            WorkspaceSplitContainer.Panel2.Controls.Add(ResultTabControl);
            WorkspaceSplitContainer.Panel2.Controls.Add(ResultSummaryPanel);
            WorkspaceSplitContainer.Panel2MinSize = 240;
            WorkspaceSplitContainer.Size = new Size(989, 730);
            WorkspaceSplitContainer.SplitterDistance = 335;
            WorkspaceSplitContainer.TabIndex = 0;
            WorkspaceSplitContainer.BackColor = Color.FromArgb(211, 221, 233);
            WorkspaceSplitContainer.Panel1.BackColor = Color.White;
            WorkspaceSplitContainer.Panel2.BackColor = Color.White;
            WorkspaceSplitContainer.SplitterWidth = 6;
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
            QueryPanel.Size = new Size(989, 335);
            QueryPanel.TabIndex = 0;
            //
            // QueryEditorPanel
            //
            QueryEditorPanel.BorderStyle = BorderStyle.FixedSingle;
            QueryEditorPanel.Controls.Add(SqlEditorTextBox);
            QueryEditorPanel.Dock = DockStyle.Fill;
            QueryEditorPanel.Location = new Point(20, 94);
            QueryEditorPanel.Name = "QueryEditorPanel";
            QueryEditorPanel.Padding = new Padding(10);
            QueryEditorPanel.Size = new Size(949, 227);
            QueryEditorPanel.TabIndex = 0;
            QueryEditorPanel.BackColor = Color.FromArgb(248, 250, 253);
            //
            // SqlEditorTextBox
            //
            SqlEditorTextBox.AcceptsTab = true;
            SqlEditorTextBox.BorderStyle = BorderStyle.None;
            SqlEditorTextBox.Dock = DockStyle.Fill;
            SqlEditorTextBox.Location = new Point(10, 10);
            SqlEditorTextBox.Name = "SqlEditorTextBox";
            SqlEditorTextBox.Size = new Size(927, 205);
            SqlEditorTextBox.TabIndex = 20;
            SqlEditorTextBox.Text = "-- 在此输入 SQL 查询语句\n";
            SqlEditorTextBox.BackColor = Color.FromArgb(248, 250, 253);
            SqlEditorTextBox.ForeColor = Color.FromArgb(30, 41, 59);
            SqlEditorTextBox.Font = new Font("Consolas", 11F);
            //
            // QueryToolbarPanel
            //
            QueryToolbarPanel.Dock = DockStyle.Top;
            QueryToolbarPanel.Location = new Point(20, 49);
            QueryToolbarPanel.Name = "QueryToolbarPanel";
            QueryToolbarPanel.Size = new Size(949, 45);
            QueryToolbarPanel.TabIndex = 1;
            QueryToolbarPanel.Height = 94;
            QueryToolbarPanel.Controls.Add(QueryActionsPanel);
            QueryToolbarPanel.Controls.Add(QueryParametersPanel);
            //
            // ClearSqlButton
            //
            ClearSqlButton.Location = new Point(847, 3);
            ClearSqlButton.Name = "ClearSqlButton";
            ClearSqlButton.Size = new Size(92, 34);
            ClearSqlButton.TabIndex = 20;
            ClearSqlButton.Text = "清空";
            ClearSqlButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ClearSqlButton.Margin = new Padding(0, 4, 8, 6);
            ClearSqlButton.Height = 34;
            ClearSqlButton.FlatStyle = FlatStyle.Flat;
            ClearSqlButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            ClearSqlButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            ClearSqlButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            ClearSqlButton.BackColor = Color.White;
            ClearSqlButton.ForeColor = Color.FromArgb(30, 41, 59);
            ClearSqlButton.UseVisualStyleBackColor = false;
            //
            // StopQueryButton
            //
            StopQueryButton.Location = new Point(751, 3);
            StopQueryButton.Name = "StopQueryButton";
            StopQueryButton.Size = new Size(88, 34);
            StopQueryButton.TabIndex = 19;
            StopQueryButton.Text = "停止";
            StopQueryButton.Enabled = false;
            StopQueryButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            StopQueryButton.Margin = new Padding(0, 4, 8, 6);
            StopQueryButton.Height = 34;
            StopQueryButton.FlatStyle = FlatStyle.Flat;
            StopQueryButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            StopQueryButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
            StopQueryButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
            StopQueryButton.BackColor = Color.White;
            StopQueryButton.UseVisualStyleBackColor = false;
            StopQueryButton.ForeColor = Color.FromArgb(171, 47, 47);
            //
            // ExecuteQueryButton
            //
            ExecuteQueryButton.Location = new Point(625, 3);
            ExecuteQueryButton.Name = "ExecuteQueryButton";
            ExecuteQueryButton.Size = new Size(118, 34);
            ExecuteQueryButton.TabIndex = 18;
            ExecuteQueryButton.Text = "▶ 执行查询";
            ExecuteQueryButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ExecuteQueryButton.Margin = new Padding(0, 4, 8, 6);
            ExecuteQueryButton.Height = 34;
            ExecuteQueryButton.FlatStyle = FlatStyle.Flat;
            ExecuteQueryButton.FlatAppearance.BorderColor = Color.FromArgb(211, 221, 233);
            ExecuteQueryButton.UseVisualStyleBackColor = false;
            ExecuteQueryButton.BackColor = Color.FromArgb(37, 99, 183);
            ExecuteQueryButton.ForeColor = Color.White;
            ExecuteQueryButton.FlatAppearance.BorderSize = 0;
            ExecuteQueryButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 79, 149);
            ExecuteQueryButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 63, 119);
            //
            // ReadOnlyCheckBox
            //
            ReadOnlyCheckBox.AutoSize = true;
            ReadOnlyCheckBox.Checked = true;
            ReadOnlyCheckBox.CheckState = CheckState.Checked;
            ReadOnlyCheckBox.ForeColor = Color.FromArgb(61, 73, 89);
            ReadOnlyCheckBox.Location = new Point(386, 8);
            ReadOnlyCheckBox.Name = "ReadOnlyCheckBox";
            ReadOnlyCheckBox.Size = new Size(91, 24);
            ReadOnlyCheckBox.TabIndex = 17;
            ReadOnlyCheckBox.Text = "只读模式";
            ReadOnlyCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            ReadOnlyCheckBox.Margin = new Padding(0, 4, 10, 0);
            //
            // QueryTimeoutNumericUpDown
            //
            QueryTimeoutNumericUpDown.Location = new Point(306, 6);
            QueryTimeoutNumericUpDown.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            QueryTimeoutNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            QueryTimeoutNumericUpDown.Name = "QueryTimeoutNumericUpDown";
            QueryTimeoutNumericUpDown.Size = new Size(65, 27);
            QueryTimeoutNumericUpDown.TabIndex = 16;
            QueryTimeoutNumericUpDown.Value = new decimal(new int[] { 30, 0, 0, 0 });
            QueryTimeoutNumericUpDown.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            QueryTimeoutNumericUpDown.Margin = new Padding(0, 4, 10, 0);
            //
            // QueryTimeoutLabel
            //
            QueryTimeoutLabel.AutoSize = true;
            QueryTimeoutLabel.ForeColor = Color.FromArgb(61, 73, 89);
            QueryTimeoutLabel.Location = new Point(260, 10);
            QueryTimeoutLabel.Name = "QueryTimeoutLabel";
            QueryTimeoutLabel.Size = new Size(39, 20);
            QueryTimeoutLabel.TabIndex = 21;
            QueryTimeoutLabel.Text = "超时";
            QueryTimeoutLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            QueryTimeoutLabel.Margin = new Padding(0, 8, 10, 0);
            //
            // DatabaseComboBox
            //
            DatabaseComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DatabaseComboBox.FormattingEnabled = true;
            DatabaseComboBox.Location = new Point(58, 6);
            DatabaseComboBox.Name = "DatabaseComboBox";
            DatabaseComboBox.Size = new Size(185, 28);
            DatabaseComboBox.TabIndex = 15;
            DatabaseComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            DatabaseComboBox.Margin = new Padding(0, 4, 10, 0);
            //
            // DatabaseLabel
            //
            DatabaseLabel.AutoSize = true;
            DatabaseLabel.ForeColor = Color.FromArgb(61, 73, 89);
            DatabaseLabel.Location = new Point(0, 10);
            DatabaseLabel.Name = "DatabaseLabel";
            DatabaseLabel.Size = new Size(54, 20);
            DatabaseLabel.TabIndex = 22;
            DatabaseLabel.Text = "数据库";
            DatabaseLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            DatabaseLabel.Margin = new Padding(0, 8, 10, 0);
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
            ResultTabControl.Size = new Size(989, 350);
            ResultTabControl.TabIndex = 21;
            //
            // ResultTabPage
            //
            ResultTabPage.Controls.Add(dataGridView1);
            ResultTabPage.Location = new Point(4, 33);
            ResultTabPage.Name = "ResultTabPage";
            ResultTabPage.Padding = new Padding(8);
            ResultTabPage.Size = new Size(981, 313);
            ResultTabPage.TabIndex = 0;
            ResultTabPage.Text = "查询结果";
            ResultTabPage.UseVisualStyleBackColor = true;
            //
            // dataGridView1
            //
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeight = 34;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.Location = new Point(8, 8);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 48;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(965, 297);
            dataGridView1.TabIndex = 0;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(233, 239, 247);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(233, 239, 247);
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);
            dataGridView1.DefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(218, 232, 251);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 51, 93);
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.GridColor = Color.FromArgb(211, 221, 233);
            dataGridView1.RowTemplate.Height = 32;
            //
            // MessageTabPage
            //
            MessageTabPage.Controls.Add(MessageTextBox);
            MessageTabPage.Location = new Point(4, 33);
            MessageTabPage.Name = "MessageTabPage";
            MessageTabPage.Padding = new Padding(8);
            MessageTabPage.Size = new Size(981, 313);
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
            MessageTextBox.Location = new Point(8, 8);
            MessageTextBox.Name = "MessageTextBox";
            MessageTextBox.ReadOnly = true;
            MessageTextBox.Size = new Size(965, 297);
            MessageTextBox.TabIndex = 0;
            MessageTextBox.Text = "等待执行查询…";
            MessageTextBox.ForeColor = Color.FromArgb(83, 99, 119);
            //
            // ResultSummaryPanel
            //
            ResultSummaryPanel.Controls.Add(ResultStateLabel);
            ResultSummaryPanel.Controls.Add(ResultSummaryLabel);
            ResultSummaryPanel.Dock = DockStyle.Top;
            ResultSummaryPanel.Location = new Point(0, 0);
            ResultSummaryPanel.Name = "ResultSummaryPanel";
            ResultSummaryPanel.Size = new Size(989, 40);
            ResultSummaryPanel.TabIndex = 22;
            ResultSummaryPanel.BackColor = Color.FromArgb(244, 247, 251);
            //
            // ResultStateLabel
            //
            ResultStateLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ResultStateLabel.Location = new Point(690, 9);
            ResultStateLabel.Name = "ResultStateLabel";
            ResultStateLabel.Size = new Size(276, 20);
            ResultStateLabel.TabIndex = 0;
            ResultStateLabel.Text = "尚未执行查询";
            ResultStateLabel.TextAlign = ContentAlignment.MiddleRight;
            ResultStateLabel.ForeColor = Color.FromArgb(83, 99, 119);
            ResultStateLabel.Dock = DockStyle.Right;
            ResultStateLabel.Width = 320;
            ResultStateLabel.Padding = new Padding(0, 0, 16, 0);
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
            MainStatusStrip.ImageScalingSize = new Size(20, 20);
            MainStatusStrip.Items.AddRange(new ToolStripItem[] { ConnectionStatusLabel, StatusSpringLabel, CurrentDatabaseStatusLabel });
            MainStatusStrip.Location = new Point(0, 802);
            MainStatusStrip.Name = "MainStatusStrip";
            MainStatusStrip.Size = new Size(1384, 26);
            MainStatusStrip.TabIndex = 2;
            MainStatusStrip.BackColor = Color.FromArgb(244, 247, 251);
            MainStatusStrip.ForeColor = Color.FromArgb(83, 99, 119);
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
            ClientSize = new Size(1384, 828);
            Controls.Add(MainSplitContainer);
            Controls.Add(HeaderPanel);
            Controls.Add(MainStatusStrip);
            Font = new Font("Microsoft YaHei UI", 9F);
            MinimumSize = new Size(1120, 720);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "多数据库浏览器";
            WindowState = FormWindowState.Maximized;
            Icon = (Icon)resources.GetObject("_notifyIcon.Icon");
            BackColor = Color.FromArgb(244, 247, 251);
            ForeColor = Color.FromArgb(30, 41, 59);
            MinimumSize = new Size(1120, 800);
            _databaseObjectsTabPage.ResumeLayout(false);
            _databaseObjectsTabPage.PerformLayout();
            _hostInputPanel.ResumeLayout(false);
            _hostInputPanel.PerformLayout();
            SyncLayout.ResumeLayout(false);
            SyncLayout.PerformLayout();
            ConnectionLayout.ResumeLayout(false);
            ConnectionLayout.PerformLayout();
            ConnectionHeading.ResumeLayout(false);
            ConnectionHeading.PerformLayout();
            QueryParametersPanel.ResumeLayout(false);
            QueryParametersPanel.PerformLayout();
            QueryActionsPanel.ResumeLayout(false);
            QueryActionsPanel.PerformLayout();
            PortInputPanel.ResumeLayout(false);
            PortInputPanel.PerformLayout();
            TrayMenu.ResumeLayout(false);
            TrayMenu.PerformLayout();
            HeaderPanel.ResumeLayout(false);
            HeaderPanel.PerformLayout();
            _syncPanel.ResumeLayout(false);
            _syncPanel.PerformLayout();
            MainSplitContainer.Panel1.ResumeLayout(false);
            MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MainSplitContainer).EndInit();
            MainSplitContainer.ResumeLayout(false);
            ConnectionPanel.ResumeLayout(false);
            ConnectionPanel.PerformLayout();
            ConnectionFieldsTable.ResumeLayout(false);
            ConnectionFieldsTable.PerformLayout();
            PasswordPanel.ResumeLayout(false);
            PasswordPanel.PerformLayout();
            ConnectionOptionsPanel.ResumeLayout(false);
            ConnectionOptionsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TimeoutNumericUpDown).EndInit();
            ConnectionButtonTable.ResumeLayout(false);
            SecondaryButtonTable.ResumeLayout(false);
            WorkspaceSplitContainer.Panel1.ResumeLayout(false);
            WorkspaceSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)WorkspaceSplitContainer).EndInit();
            WorkspaceSplitContainer.ResumeLayout(false);
            QueryPanel.ResumeLayout(false);
            QueryPanel.PerformLayout();
            QueryEditorPanel.ResumeLayout(false);
            QueryToolbarPanel.ResumeLayout(false);
            QueryToolbarPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)QueryTimeoutNumericUpDown).EndInit();
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

        private TreeView _databaseTreeView;
        private TabPage _databaseObjectsTabPage;
        private ComboBox _databaseTypeComboBox;
        private Button _browseSqliteButton;
        private Panel _hostInputPanel;
        private ComboBox _sqliteFileComboBox;
        private TableLayoutPanel SyncLayout;
        private TableLayoutPanel ConnectionLayout;
        private TableLayoutPanel ConnectionHeading;
        private FlowLayoutPanel QueryParametersPanel;
        private FlowLayoutPanel QueryActionsPanel;
        private Panel PortInputPanel;
        private ContextMenuStrip TrayMenu;
        private ToolStripMenuItem ShowWindowMenuItem;
        private ToolStripMenuItem SyncNowMenuItem;
        private ToolStripMenuItem ExitMenuItem;
        private FolderBrowserDialog SqliteFolderDialog;
        private Panel HeaderPanel;
        private FlowLayoutPanel _syncPanel;
        private Label _syncDeviceLabel;
        private TextBox _syncDeviceTextBox;
        private Label _syncServerLabel;
        private TextBox _syncServerTextBox;
        private CheckBox _autoSyncCheckBox;
        private Button _fullSyncButton;
        private Button _syncNowButton;
        private Label _syncStatusLabel;
        private NotifyIcon _notifyIcon;
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
