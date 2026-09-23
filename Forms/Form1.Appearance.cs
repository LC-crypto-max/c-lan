namespace c_lan
{
    public partial class Form1
    {
        private void InitializeAppearance()
        {
            SuspendLayout();
            Color accent = Color.FromArgb(37, 99, 183);
            Color ink = Color.FromArgb(30, 41, 59);
            Color muted = Color.FromArgb(83, 99, 119);
            Color surface = Color.FromArgb(244, 247, 251);
            Color border = Color.FromArgb(211, 221, 233);

            BackColor = surface;
            ForeColor = ink;
            MinimumSize = new Size(1120, 800);
            HeaderPanel.BackColor = Color.FromArgb(24, 42, 66);
            HeaderPanel.Padding = Padding.Empty;
            HeaderPanel.Height = 158;
            HeaderTitleLabel.Location = new Point(24, 13);
            HeaderTitleLabel.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold);

            // 同步配置独占一行，状态独占下一行，较长的服务端消息不会挤走操作按钮。
            HeaderPanel.Controls.Remove(_syncPanel);
            _syncPanel.Controls.Remove(_syncStatusLabel);
            _syncPanel.Dock = DockStyle.Fill;
            _syncPanel.AutoSize = false;
            _syncPanel.Padding = new Padding(20, 5, 12, 0);
            _syncPanel.BackColor = surface;
            _syncDeviceLabel.ForeColor = _syncServerLabel.ForeColor = _autoSyncCheckBox.ForeColor = ink;
            _syncServerTextBox.Width = 260;
            _syncStatusLabel.AutoSize = false;
            _syncStatusLabel.Dock = DockStyle.Fill;
            _syncStatusLabel.AutoEllipsis = true;
            _syncStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            _syncStatusLabel.ForeColor = muted;
            _syncStatusLabel.Margin = new Padding(24, 0, 20, 0);
            TableLayoutPanel syncLayout = new()
            {
                Dock = DockStyle.Bottom, Height = 100, RowCount = 2, ColumnCount = 1,
                BackColor = surface, Margin = Padding.Empty, Padding = Padding.Empty
            };
            syncLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            syncLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            syncLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            syncLayout.Controls.Add(_syncPanel, 0, 0);
            syncLayout.Controls.Add(_syncStatusLabel, 0, 1);
            HeaderPanel.Controls.Add(syncLayout);

            ConnectionPanel.BackColor = surface;
            MainSplitContainer.BackColor = border;
            MainSplitContainer.Panel1.BackColor = surface;
            WorkspaceSplitContainer.BackColor = border;
            WorkspaceSplitContainer.Panel1.BackColor = Color.White;
            WorkspaceSplitContainer.Panel2.BackColor = Color.White;
            MainSplitContainer.SplitterWidth = WorkspaceSplitContainer.SplitterWidth = 6;
            // 连接区用原生表格自动计算高度；SQLite 隐藏的字段不再留下大块空白。
            ConnectionPanel.Controls.Clear();
            ConnectionPanel.Padding = new Padding(20, 16, 20, 20);
            TableLayoutPanel connectionLayout = new()
            {
                Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1, RowCount = 5, Margin = Padding.Empty
            };
            connectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            TableLayoutPanel connectionHeading = new()
            {
                Dock = DockStyle.Fill, Height = 44, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty
            };
            connectionHeading.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108));
            connectionHeading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            ConnectionSectionLabel.Dock = DockStyle.Fill;
            ConnectionSectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            ConnectionSectionLabel.Margin = Padding.Empty;
            _databaseTypeComboBox.Dock = DockStyle.Fill;
            _databaseTypeComboBox.Margin = new Padding(0, 7, 0, 0);
            connectionHeading.Controls.Add(ConnectionSectionLabel, 0, 0);
            connectionHeading.Controls.Add(_databaseTypeComboBox, 1, 0);
            ConnectionFieldsTable.AutoSize = true;
            ConnectionFieldsTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ConnectionFieldsTable.Dock = DockStyle.Fill;
            ConnectionFieldsTable.Margin = Padding.Empty;
            foreach (RowStyle row in ConnectionFieldsTable.RowStyles) row.SizeType = SizeType.AutoSize;
            foreach (Control field in ConnectionFieldsTable.Controls)
                field.Margin = field is Label ? new Padding(0, 7, 0, 5) : new Padding(0, 0, 0, 5);
            _hostInputPanel.Height = HostText.PreferredHeight + 4;
            ShowPasswordCheckBox.Dock = DockStyle.Right;
            PasswordText.Dock = DockStyle.Fill;
            ConnectionOptionsPanel.Height = 36;
            foreach (Control section in new Control[] { connectionHeading, ConnectionFieldsTable, ConnectionOptionsPanel,
                ConnectionButtonTable, SecondaryButtonTable })
            {
                section.Dock = DockStyle.Top;
                section.Margin = new Padding(0, 0, 0, 8);
                connectionLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                connectionLayout.Controls.Add(section, 0, connectionLayout.Controls.Count);
            }
            ConnectionPanel.Controls.Add(connectionLayout);
            ConnectionPanel.Controls.Add(ConnectionTipLabel);
            _databaseTreeView.ItemHeight = 30;
            _databaseTreeView.ForeColor = ink;
            _databaseTreeView.BackColor = Color.White;
            _databaseTreeView.FullRowSelect = true;

            // 原生流式布局分开查询参数和操作，缩窄窗口时不会与右侧按钮重叠。
            QueryToolbarPanel.Controls.Clear();
            QueryToolbarPanel.Height = 94;
            FlowLayoutPanel parameters = new()
            {
                Dock = DockStyle.Top, Height = 42, WrapContents = false, Margin = Padding.Empty
            };
            FlowLayoutPanel actions = new()
            {
                Dock = DockStyle.Bottom, Height = 48, WrapContents = false, Margin = Padding.Empty
            };
            foreach (Control control in new Control[] { DatabaseLabel, DatabaseComboBox, QueryTimeoutLabel, QueryTimeoutNumericUpDown, ReadOnlyCheckBox })
            {
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                control.Margin = new Padding(0, control is Label ? 8 : 4, 10, 0);
                parameters.Controls.Add(control);
            }
            foreach (Button button in new[] { ExecuteQueryButton, StopQueryButton, ClearSqlButton })
            {
                button.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                button.Margin = new Padding(0, 4, 8, 6);
                button.Height = 34;
                actions.Controls.Add(button);
            }
            QueryToolbarPanel.Controls.Add(actions);
            QueryToolbarPanel.Controls.Add(parameters);

            foreach (Button button in new[] { ConnectButton, TestButton, SaveConnectionButton, DeleteConnectionButton,
                ExecuteQueryButton, StopQueryButton, ClearSqlButton, _syncNowButton, _fullSyncButton, _browseSqliteButton })
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = border;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 239, 249);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(217, 229, 244);
                button.BackColor = Color.White;
                button.ForeColor = ink;
                button.UseVisualStyleBackColor = false;
            }
            foreach (Button button in new[] { ConnectButton, ExecuteQueryButton, _syncNowButton })
            {
                button.BackColor = accent;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 79, 149);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 63, 119);
            }
            DeleteConnectionButton.ForeColor = StopQueryButton.ForeColor = Color.FromArgb(171, 47, 47);
            SqlEditorTextBox.BackColor = QueryEditorPanel.BackColor = Color.FromArgb(248, 250, 253);
            SqlEditorTextBox.ForeColor = ink;
            SqlEditorTextBox.Font = new Font("Consolas", 11F);
            ResultSummaryPanel.BackColor = surface;
            ResultStateLabel.ForeColor = muted;
            ResultStateLabel.Dock = DockStyle.Right;
            ResultStateLabel.Width = 320;
            ResultStateLabel.Padding = new Padding(0, 0, 16, 0);
            MessageTextBox.ForeColor = muted;
            MainStatusStrip.BackColor = surface;
            MainStatusStrip.ForeColor = muted;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(233, 239, 247);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = ink;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(233, 239, 247);
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = ink;
            dataGridView1.DefaultCellStyle.ForeColor = ink;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(218, 232, 251);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromArgb(20, 51, 93);
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.GridColor = border;
            dataGridView1.RowTemplate.Height = 32;

            using Stream? iconStream = typeof(Form1).Assembly.GetManifestResourceStream("c_lan.Assets.app.ico");
            if (iconStream is not null)
            {
                using Icon source = new(iconStream);
                Icon appIcon = (Icon)source.Clone();
                Icon = appIcon;
                _notifyIcon.Icon = appIcon;
                Disposed += (_, _) => appIcon.Dispose();
            }
            ResumeLayout(true);
        }
    }
}
