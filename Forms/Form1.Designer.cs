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
            HeaderPanel = new Panel();
            HeaderSubtitleLabel = new Label();
            HeaderTitleLabel = new Label();
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
            TimeoutLabel = new Label();
            TimeoutNumericUpDown = new NumericUpDown();
            ConnectionOptionsPanel = new Panel();
            SavePasswordCheckBox = new CheckBox();
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
            HeaderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MainSplitContainer).BeginInit();
            MainSplitContainer.Panel1.SuspendLayout();
            MainSplitContainer.Panel2.SuspendLayout();
            MainSplitContainer.SuspendLayout();
            ConnectionPanel.SuspendLayout();
            ConnectionFieldsTable.SuspendLayout();
            PasswordPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TimeoutNumericUpDown).BeginInit();
            ConnectionOptionsPanel.SuspendLayout();
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
            // HeaderPanel
            //
            HeaderPanel.BackColor = Color.FromArgb(27, 43, 65);
            HeaderPanel.Controls.Add(HeaderSubtitleLabel);
            HeaderPanel.Controls.Add(HeaderTitleLabel);
            HeaderPanel.Dock = DockStyle.Top;
            HeaderPanel.Location = new Point(0, 0);
            HeaderPanel.Name = "HeaderPanel";
            HeaderPanel.Padding = new Padding(24, 11, 24, 8);
            HeaderPanel.Size = new Size(1384, 72);
            HeaderPanel.TabIndex = 0;
            //
            // HeaderSubtitleLabel
            //
            HeaderSubtitleLabel.AutoSize = true;
            HeaderSubtitleLabel.ForeColor = Color.FromArgb(174, 190, 209);
            HeaderSubtitleLabel.Location = new Point(211, 36);
            HeaderSubtitleLabel.Name = "HeaderSubtitleLabel";
            HeaderSubtitleLabel.Size = new Size(212, 20);
            HeaderSubtitleLabel.TabIndex = 1;
            HeaderSubtitleLabel.Text = "连接配置 · SQL 查询 · 结果预览";
            //
            // HeaderTitleLabel
            //
            HeaderTitleLabel.AutoSize = true;
            HeaderTitleLabel.Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold);
            HeaderTitleLabel.ForeColor = Color.White;
            HeaderTitleLabel.Location = new Point(20, 18);
            HeaderTitleLabel.Name = "HeaderTitleLabel";
            HeaderTitleLabel.Size = new Size(168, 36);
            HeaderTitleLabel.TabIndex = 0;
            HeaderTitleLabel.Text = "MySQL 工作台";
            //
            // MainSplitContainer
            //
            MainSplitContainer.Dock = DockStyle.Fill;
            MainSplitContainer.FixedPanel = FixedPanel.Panel1;
            MainSplitContainer.Location = new Point(0, 72);
            MainSplitContainer.Name = "MainSplitContainer";
            //
            // MainSplitContainer.Panel1
            //
            MainSplitContainer.Panel1.BackColor = Color.FromArgb(245, 247, 250);
            MainSplitContainer.Panel1.Controls.Add(ConnectionPanel);
            MainSplitContainer.Panel1MinSize = 360;
            //
            // MainSplitContainer.Panel2
            //
            MainSplitContainer.Panel2.BackColor = Color.White;
            MainSplitContainer.Panel2.Controls.Add(WorkspaceSplitContainer);
            MainSplitContainer.Panel2MinSize = 620;
            MainSplitContainer.Size = new Size(1384, 730);
            MainSplitContainer.SplitterDistance = 390;
            MainSplitContainer.SplitterWidth = 5;
            MainSplitContainer.TabIndex = 1;
            //
            // ConnectionPanel
            //
            ConnectionPanel.AutoScroll = true;
            ConnectionPanel.BackColor = Color.FromArgb(245, 247, 250);
            ConnectionPanel.Controls.Add(ConnectionFieldsTable);
            ConnectionPanel.Controls.Add(ConnectionOptionsPanel);
            ConnectionPanel.Controls.Add(ConnectionButtonTable);
            ConnectionPanel.Controls.Add(SecondaryButtonTable);
            ConnectionPanel.Controls.Add(ConnectionTipLabel);
            ConnectionPanel.Controls.Add(ConnectionSectionLabel);
            ConnectionPanel.Dock = DockStyle.Fill;
            ConnectionPanel.Location = new Point(0, 0);
            ConnectionPanel.Name = "ConnectionPanel";
            ConnectionPanel.Padding = new Padding(22, 20, 22, 18);
            ConnectionPanel.Size = new Size(390, 730);
            ConnectionPanel.TabIndex = 0;
            // 
            // ConnectionFieldsTable
            // 
            ConnectionFieldsTable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ConnectionFieldsTable.ColumnCount = 1;
            ConnectionFieldsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ConnectionFieldsTable.Controls.Add(ConnectionnameLabel, 0, 0);
            ConnectionFieldsTable.Controls.Add(ConnectionnameText, 0, 1);
            ConnectionFieldsTable.Controls.Add(HostLabel, 0, 2);
            ConnectionFieldsTable.Controls.Add(HostText, 0, 3);
            ConnectionFieldsTable.Controls.Add(PortLabel, 0, 4);
            ConnectionFieldsTable.Controls.Add(PortText, 0, 5);
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
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            ConnectionFieldsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            ConnectionFieldsTable.Size = new Size(346, 520);
            ConnectionFieldsTable.TabIndex = 1;
            // 
            // labels and connection inputs
            // 
            ConnectionnameLabel.AutoSize = true;
            ConnectionnameLabel.ForeColor = Color.FromArgb(61, 73, 89);
            ConnectionnameLabel.Text = "连接名称";
            ConnectionnameText.Dock = DockStyle.Fill;
            ConnectionnameText.PlaceholderText = "例如：本地开发库";
            ConnectionnameText.TabIndex = 0;
            HostLabel.AutoSize = true;
            HostLabel.ForeColor = Color.FromArgb(61, 73, 89);
            HostLabel.Text = "主机地址";
            HostText.Dock = DockStyle.Fill;
            HostText.PlaceholderText = "localhost 或服务器 IP";
            HostText.TabIndex = 1;
            PortLabel.AutoSize = true;
            PortLabel.ForeColor = Color.FromArgb(61, 73, 89);
            PortLabel.Text = "端口";
            PortText.Dock = DockStyle.Fill;
            PortText.PlaceholderText = "3306";
            PortText.TabIndex = 2;
            UserLabel.AutoSize = true;
            UserLabel.ForeColor = Color.FromArgb(61, 73, 89);
            UserLabel.Text = "用户名";
            UserText.Dock = DockStyle.Fill;
            UserText.PlaceholderText = "MySQL 用户名";
            UserText.TabIndex = 3;
            PasswordLabel.AutoSize = true;
            PasswordLabel.ForeColor = Color.FromArgb(61, 73, 89);
            PasswordLabel.Text = "密码";
            // 
            // PasswordPanel
            // 
            PasswordPanel.Controls.Add(PasswordText);
            PasswordPanel.Controls.Add(ShowPasswordCheckBox);
            PasswordPanel.Dock = DockStyle.Fill;
            PasswordPanel.Margin = new Padding(0, 0, 0, 8);
            PasswordPanel.Name = "PasswordPanel";
            PasswordText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PasswordText.Location = new Point(0, 0);
            PasswordText.Name = "PasswordText";
            PasswordText.PlaceholderText = "MySQL 密码";
            PasswordText.Size = new Size(265, 27);
            PasswordText.TabIndex = 4;
            PasswordText.UseSystemPasswordChar = true;
            ShowPasswordCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ShowPasswordCheckBox.AutoSize = true;
            ShowPasswordCheckBox.ForeColor = Color.FromArgb(90, 100, 115);
            ShowPasswordCheckBox.Location = new Point(275, 3);
            ShowPasswordCheckBox.Name = "ShowPasswordCheckBox";
            ShowPasswordCheckBox.Size = new Size(61, 24);
            ShowPasswordCheckBox.TabIndex = 5;
            ShowPasswordCheckBox.Text = "显示";
            // 
            // remaining connection inputs
            // 
            DefaultDatabaseLabel.AutoSize = true;
            DefaultDatabaseLabel.ForeColor = Color.FromArgb(61, 73, 89);
            DefaultDatabaseLabel.Text = "默认数据库（可选）";
            DefaultDatabaseText.Dock = DockStyle.Fill;
            DefaultDatabaseText.PlaceholderText = "连接后默认使用的数据库";
            DefaultDatabaseText.TabIndex = 6;
            CharacterSetLabel.AutoSize = true;
            CharacterSetLabel.ForeColor = Color.FromArgb(61, 73, 89);
            CharacterSetLabel.Text = "字符集";
            CharacterSetComboBox.Dock = DockStyle.Fill;
            CharacterSetComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            CharacterSetComboBox.Items.AddRange(new object[] { "utf8mb4", "utf8", "latin1" });
            CharacterSetComboBox.TabIndex = 7;
            SslModeLabel.AutoSize = true;
            SslModeLabel.ForeColor = Color.FromArgb(61, 73, 89);
            SslModeLabel.Text = "SSL 模式";
            SslModeComboBox.Dock = DockStyle.Fill;
            SslModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SslModeComboBox.Items.AddRange(new object[] { "Preferred", "Required", "VerifyCA", "VerifyFull", "Disabled" });
            SslModeComboBox.TabIndex = 8;
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
            TimeoutLabel.AutoSize = true;
            TimeoutLabel.ForeColor = Color.FromArgb(61, 73, 89);
            TimeoutLabel.Location = new Point(0, 6);
            TimeoutLabel.Text = "连接超时（秒）";
            TimeoutNumericUpDown.Location = new Point(116, 2);
            TimeoutNumericUpDown.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            TimeoutNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            TimeoutNumericUpDown.Size = new Size(65, 27);
            TimeoutNumericUpDown.TabIndex = 9;
            TimeoutNumericUpDown.Value = new decimal(new int[] { 10, 0, 0, 0 });
            SavePasswordCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SavePasswordCheckBox.AutoSize = true;
            SavePasswordCheckBox.ForeColor = Color.FromArgb(61, 73, 89);
            SavePasswordCheckBox.Location = new Point(239, 4);
            SavePasswordCheckBox.Text = "保存密码";
            SavePasswordCheckBox.TabIndex = 10;
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
            TestButton.BackColor = Color.White;
            TestButton.Dock = DockStyle.Fill;
            TestButton.FlatStyle = FlatStyle.Flat;
            TestButton.ForeColor = Color.FromArgb(35, 92, 151);
            TestButton.Margin = new Padding(0, 0, 6, 0);
            TestButton.Text = "测试连接";
            TestButton.TabIndex = 11;
            TestButton.UseVisualStyleBackColor = false;
            TestButton.Click += TestButton_Click;
            ConnectButton.BackColor = Color.FromArgb(35, 92, 151);
            ConnectButton.Dock = DockStyle.Fill;
            ConnectButton.FlatAppearance.BorderSize = 0;
            ConnectButton.FlatStyle = FlatStyle.Flat;
            ConnectButton.ForeColor = Color.White;
            ConnectButton.Margin = new Padding(6, 0, 0, 0);
            ConnectButton.Text = "连接 MySQL";
            ConnectButton.TabIndex = 12;
            ConnectButton.UseVisualStyleBackColor = false;
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
            SaveConnectionButton.Dock = DockStyle.Fill;
            SaveConnectionButton.FlatStyle = FlatStyle.Flat;
            SaveConnectionButton.ForeColor = Color.FromArgb(76, 87, 102);
            SaveConnectionButton.Margin = new Padding(0, 0, 6, 0);
            SaveConnectionButton.Text = "保存配置";
            SaveConnectionButton.TabIndex = 13;
            DeleteConnectionButton.Dock = DockStyle.Fill;
            DeleteConnectionButton.FlatStyle = FlatStyle.Flat;
            DeleteConnectionButton.ForeColor = Color.FromArgb(173, 58, 58);
            DeleteConnectionButton.Margin = new Padding(6, 0, 0, 0);
            DeleteConnectionButton.Text = "删除配置";
            DeleteConnectionButton.TabIndex = 14;
            ConnectionTipLabel.AutoSize = true;
            ConnectionTipLabel.ForeColor = Color.FromArgb(112, 122, 136);
            ConnectionTipLabel.Location = new Point(189, 27);
            ConnectionTipLabel.Text = "仅 MySQL";
            ConnectionSectionLabel.AutoSize = true;
            ConnectionSectionLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            ConnectionSectionLabel.ForeColor = Color.FromArgb(35, 46, 61);
            ConnectionSectionLabel.Location = new Point(20, 20);
            ConnectionSectionLabel.Text = "连接设置";
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
            WorkspaceSplitContainer.SplitterWidth = 5;
            WorkspaceSplitContainer.TabIndex = 0;
            // 
            // QueryPanel
            // 
            QueryPanel.Controls.Add(QueryEditorPanel);
            QueryPanel.Controls.Add(QueryToolbarPanel);
            QueryPanel.Controls.Add(QuerySectionLabel);
            QueryPanel.Dock = DockStyle.Fill;
            QueryPanel.Location = new Point(0, 0);
            QueryPanel.Padding = new Padding(20, 16, 20, 14);
            QueryPanel.Size = new Size(989, 335);
            QueryPanel.TabIndex = 0;
            QueryEditorPanel.BorderStyle = BorderStyle.FixedSingle;
            QueryEditorPanel.Controls.Add(SqlEditorTextBox);
            QueryEditorPanel.Dock = DockStyle.Fill;
            QueryEditorPanel.Location = new Point(20, 94);
            QueryEditorPanel.Padding = new Padding(10);
            QueryEditorPanel.Size = new Size(949, 227);
            SqlEditorTextBox.AcceptsTab = true;
            SqlEditorTextBox.BackColor = Color.FromArgb(250, 251, 253);
            SqlEditorTextBox.BorderStyle = BorderStyle.None;
            SqlEditorTextBox.Dock = DockStyle.Fill;
            SqlEditorTextBox.Font = new Font("Consolas", 11F);
            SqlEditorTextBox.ForeColor = Color.FromArgb(34, 42, 53);
            SqlEditorTextBox.Location = new Point(10, 10);
            SqlEditorTextBox.Name = "SqlEditorTextBox";
            SqlEditorTextBox.Size = new Size(927, 205);
            SqlEditorTextBox.TabIndex = 20;
            SqlEditorTextBox.Text = "-- 在此输入 MySQL 查询语句\n";
            // 
            // QueryToolbarPanel
            // 
            QueryToolbarPanel.Controls.Add(ClearSqlButton);
            QueryToolbarPanel.Controls.Add(StopQueryButton);
            QueryToolbarPanel.Controls.Add(ExecuteQueryButton);
            QueryToolbarPanel.Controls.Add(ReadOnlyCheckBox);
            QueryToolbarPanel.Controls.Add(QueryTimeoutNumericUpDown);
            QueryToolbarPanel.Controls.Add(QueryTimeoutLabel);
            QueryToolbarPanel.Controls.Add(DatabaseComboBox);
            QueryToolbarPanel.Controls.Add(DatabaseLabel);
            QueryToolbarPanel.Dock = DockStyle.Top;
            QueryToolbarPanel.Location = new Point(20, 49);
            QueryToolbarPanel.Name = "QueryToolbarPanel";
            QueryToolbarPanel.Size = new Size(949, 45);
            DatabaseLabel.AutoSize = true;
            DatabaseLabel.ForeColor = Color.FromArgb(61, 73, 89);
            DatabaseLabel.Location = new Point(0, 10);
            DatabaseLabel.Text = "数据库";
            DatabaseComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DatabaseComboBox.FormattingEnabled = true;
            DatabaseComboBox.Location = new Point(58, 6);
            DatabaseComboBox.Name = "DatabaseComboBox";
            DatabaseComboBox.Size = new Size(185, 28);
            DatabaseComboBox.TabIndex = 15;
            QueryTimeoutLabel.AutoSize = true;
            QueryTimeoutLabel.ForeColor = Color.FromArgb(61, 73, 89);
            QueryTimeoutLabel.Location = new Point(260, 10);
            QueryTimeoutLabel.Text = "超时";
            QueryTimeoutNumericUpDown.Location = new Point(306, 6);
            QueryTimeoutNumericUpDown.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            QueryTimeoutNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            QueryTimeoutNumericUpDown.Size = new Size(65, 27);
            QueryTimeoutNumericUpDown.TabIndex = 16;
            QueryTimeoutNumericUpDown.Value = new decimal(new int[] { 30, 0, 0, 0 });
            ReadOnlyCheckBox.AutoSize = true;
            ReadOnlyCheckBox.Checked = true;
            ReadOnlyCheckBox.CheckState = CheckState.Checked;
            ReadOnlyCheckBox.ForeColor = Color.FromArgb(61, 73, 89);
            ReadOnlyCheckBox.Location = new Point(386, 8);
            ReadOnlyCheckBox.Text = "只读模式";
            ReadOnlyCheckBox.TabIndex = 17;
            ExecuteQueryButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ExecuteQueryButton.BackColor = Color.FromArgb(31, 137, 89);
            ExecuteQueryButton.FlatAppearance.BorderSize = 0;
            ExecuteQueryButton.FlatStyle = FlatStyle.Flat;
            ExecuteQueryButton.ForeColor = Color.White;
            ExecuteQueryButton.Location = new Point(625, 3);
            ExecuteQueryButton.Size = new Size(118, 34);
            ExecuteQueryButton.TabIndex = 18;
            ExecuteQueryButton.Text = "▶ 执行查询";
            ExecuteQueryButton.UseVisualStyleBackColor = false;
            StopQueryButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            StopQueryButton.FlatStyle = FlatStyle.Flat;
            StopQueryButton.ForeColor = Color.FromArgb(173, 58, 58);
            StopQueryButton.Location = new Point(751, 3);
            StopQueryButton.Size = new Size(88, 34);
            StopQueryButton.TabIndex = 19;
            StopQueryButton.Text = "停止";
            ClearSqlButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ClearSqlButton.FlatStyle = FlatStyle.Flat;
            ClearSqlButton.ForeColor = Color.FromArgb(76, 87, 102);
            ClearSqlButton.Location = new Point(847, 3);
            ClearSqlButton.Size = new Size(92, 34);
            ClearSqlButton.TabIndex = 20;
            ClearSqlButton.Text = "清空";
            QuerySectionLabel.AutoSize = true;
            QuerySectionLabel.Dock = DockStyle.Top;
            QuerySectionLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            QuerySectionLabel.ForeColor = Color.FromArgb(35, 46, 61);
            QuerySectionLabel.Location = new Point(20, 16);
            QuerySectionLabel.Name = "QuerySectionLabel";
            QuerySectionLabel.Padding = new Padding(0, 0, 0, 6);
            QuerySectionLabel.Size = new Size(96, 33);
            QuerySectionLabel.Text = "SQL 查询";
            // 
            // result area
            // 
            ResultTabControl.Controls.Add(ResultTabPage);
            ResultTabControl.Controls.Add(MessageTabPage);
            ResultTabControl.Dock = DockStyle.Fill;
            ResultTabControl.Location = new Point(0, 40);
            ResultTabControl.Name = "ResultTabControl";
            ResultTabControl.Padding = new Point(16, 5);
            ResultTabControl.SelectedIndex = 0;
            ResultTabControl.Size = new Size(989, 350);
            ResultTabControl.TabIndex = 21;
            ResultTabPage.Controls.Add(dataGridView1);
            ResultTabPage.Location = new Point(4, 32);
            ResultTabPage.Name = "ResultTabPage";
            ResultTabPage.Padding = new Padding(8);
            ResultTabPage.Size = new Size(981, 314);
            ResultTabPage.Text = "查询结果";
            ResultTabPage.UseVisualStyleBackColor = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeight = 34;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.GridColor = Color.FromArgb(224, 229, 235);
            dataGridView1.Location = new Point(8, 8);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 48;
            dataGridView1.RowTemplate.Height = 30;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(965, 298);
            dataGridView1.TabIndex = 0;
            MessageTabPage.Controls.Add(MessageTextBox);
            MessageTabPage.Location = new Point(4, 32);
            MessageTabPage.Name = "MessageTabPage";
            MessageTabPage.Padding = new Padding(8);
            MessageTabPage.Size = new Size(981, 314);
            MessageTabPage.Text = "执行消息";
            MessageTabPage.UseVisualStyleBackColor = true;
            MessageTextBox.BackColor = Color.FromArgb(250, 251, 253);
            MessageTextBox.BorderStyle = BorderStyle.None;
            MessageTextBox.Dock = DockStyle.Fill;
            MessageTextBox.Font = new Font("Consolas", 10F);
            MessageTextBox.ForeColor = Color.FromArgb(61, 73, 89);
            MessageTextBox.ReadOnly = true;
            MessageTextBox.Text = "等待执行查询…";
            ResultSummaryPanel.BackColor = Color.FromArgb(248, 249, 251);
            ResultSummaryPanel.Controls.Add(ResultStateLabel);
            ResultSummaryPanel.Controls.Add(ResultSummaryLabel);
            ResultSummaryPanel.Dock = DockStyle.Top;
            ResultSummaryPanel.Location = new Point(0, 0);
            ResultSummaryPanel.Size = new Size(989, 40);
            ResultSummaryLabel.AutoSize = true;
            ResultSummaryLabel.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            ResultSummaryLabel.ForeColor = Color.FromArgb(35, 46, 61);
            ResultSummaryLabel.Location = new Point(17, 9);
            ResultSummaryLabel.Text = "结果预览";
            ResultStateLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ResultStateLabel.ForeColor = Color.FromArgb(112, 122, 136);
            ResultStateLabel.Location = new Point(690, 9);
            ResultStateLabel.Size = new Size(276, 20);
            ResultStateLabel.Text = "尚未执行查询";
            ResultStateLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MainStatusStrip
            // 
            MainStatusStrip.ImageScalingSize = new Size(20, 20);
            MainStatusStrip.Items.AddRange(new ToolStripItem[] { ConnectionStatusLabel, StatusSpringLabel, CurrentDatabaseStatusLabel });
            MainStatusStrip.Location = new Point(0, 802);
            MainStatusStrip.Name = "MainStatusStrip";
            MainStatusStrip.Size = new Size(1384, 26);
            MainStatusStrip.TabIndex = 2;
            ConnectionStatusLabel.ForeColor = Color.FromArgb(112, 122, 136);
            ConnectionStatusLabel.Text = "● 未连接";
            StatusSpringLabel.Name = "StatusSpringLabel";
            StatusSpringLabel.Size = new Size(1205, 20);
            StatusSpringLabel.Spring = true;
            CurrentDatabaseStatusLabel.ForeColor = Color.FromArgb(112, 122, 136);
            CurrentDatabaseStatusLabel.Text = "数据库：未选择";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1384, 828);
            Controls.Add(MainSplitContainer);
            Controls.Add(HeaderPanel);
            Controls.Add(MainStatusStrip);
            Font = new Font("Microsoft YaHei UI", 9F);
            MinimumSize = new Size(1120, 720);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MySQL 数据库浏览器";
            WindowState = FormWindowState.Maximized;
            HeaderPanel.ResumeLayout(false);
            HeaderPanel.PerformLayout();
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
            ((System.ComponentModel.ISupportInitialize)TimeoutNumericUpDown).EndInit();
            ConnectionOptionsPanel.ResumeLayout(false);
            ConnectionOptionsPanel.PerformLayout();
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

        private Panel HeaderPanel;
        private Label HeaderSubtitleLabel;
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
