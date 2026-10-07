using c_lan.Configuration;
using c_lan.Data;
using c_lan.Models;
using c_lan.Services;
using c_lan.Utilities;
using System.Data;

namespace c_lan;

public partial class Form1 : Form
{
    private readonly IConnectionService _connectionService;
    private readonly ISchemaService _schemaService;
    private readonly IQueryService _queryService;
    private CancellationTokenSource? _operation;
    private TaskCompletionSource? _operationCompleted;
    private ConnectionProfile? _activeConnectionProfile;
    private bool _filling;
    private bool _closeAfterOperation;
    private static readonly DatabaseType[] DatabaseTypes = [DatabaseType.MySQL, DatabaseType.SQLite, DatabaseType.SqlServer, DatabaseType.Oracle];
    private DatabaseType SelectedType => DatabaseTypes[Math.Max(0, _databaseTypeComboBox.SelectedIndex)];

    // 无参构造用于 WinForms 设计器；启动程序仍通过 Program 手工注入依赖。
    public Form1() : this(new ConnectionService(new ConnectionProfileStore(), new DatabaseProviderFactory()),
        new SchemaService(new DatabaseProviderFactory()), new QueryService(new DatabaseProviderFactory(), new ReadOnlySqlValidator())) { }

    public Form1(IConnectionService connectionService, ISchemaService schemaService, IQueryService queryService)
    {
        InitializeComponent();
        _connectionService = connectionService;
        _schemaService = schemaService;
        _queryService = queryService;
        Shown += async (_, _) => await RunOperationAsync(async token => await LoadProfilesAsync(token));
        ConnectButton.Click += async (_, _) => await ConnectAsync();
        TestButton.Click += async (_, _) => await RunOperationAsync(async token =>
        {
            var profile = BuildProfile();
            var result = await Task.Run(() => _connectionService.TestConnectionAsync(profile, token), token);
            token.ThrowIfCancellationRequested();
            ShowMessage(result.IsSuccess ? "连接测试成功" : result.ErrorMessage ?? "连接失败");
        });
        SaveConnectionButton.Click += async (_, _) => await RunOperationAsync(async token =>
        {
            var result = await _connectionService.SaveConnectionConfigurationAsync(BuildProfile(), token);
            ShowMessage(result.IsSuccess ? result.Message ?? "已保存" : result.ErrorMessage ?? "保存失败");
            if (result.IsSuccess) await LoadProfilesAsync(token, ConnectionnameText.Text);
        });
        DeleteConnectionButton.Click += async (_, _) =>
        {
            string name = ConnectionnameText.Text.Trim();
            if (MessageBox.Show(this, $"删除连接配置“{name}”？", "删除配置", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            await RunOperationAsync(async token =>
            {
                var result = await _connectionService.DeleteConnectionConfigurationAsync(name, token);
                ShowMessage(result.IsSuccess ? result.Message ?? "已删除" : result.ErrorMessage ?? "删除失败");
                if (result.IsSuccess) { await LoadProfilesAsync(token); ResetConnection(); }
            });
        };
        SavedConnectionsComboBox.SelectedIndexChanged += (_, _) =>
        {
            if (!_filling && SavedConnectionsComboBox.SelectedItem is ConnectionProfile profile) FillProfile(profile);
        };
        NewConnectionButton.Click += (_, _) => ResetConnection();
        _databaseTypeComboBox.SelectedIndexChanged += (_, _) =>
        {
            UpdateDatabaseTypeUi();
            if (!_filling) { PortText.Text = SelectedType switch { DatabaseType.MySQL => "3306", DatabaseType.SqlServer => "1433", DatabaseType.Oracle => "1521", _ => "" }; InvalidateConnection(); }
        };
        ShowPasswordCheckBox.CheckedChanged += (_, _) => PasswordText.UseSystemPasswordChar = !ShowPasswordCheckBox.Checked;
        IntegratedSecurityCheckBox.CheckedChanged += (_, _) => { UpdateDatabaseTypeUi(); InvalidateConnection(); };
        TrustCertificateCheckBox.CheckedChanged += (_, _) => InvalidateConnection();
        TimeoutNumericUpDown.ValueChanged += (_, _) => InvalidateConnection();
        _browseSqliteButton.Click += (_, _) =>
        {
            if (SqliteFolderDialog.ShowDialog(this) == DialogResult.OK) { HostText.Text = SqliteFolderDialog.SelectedPath; RefreshSqliteFiles(); }
        };
        HostText.Leave += (_, _) => RefreshSqliteFiles();
        _sqliteFileComboBox.Format += (_, e) => { if (e.ListItem is string file) e.Value = Path.GetFileName(file); };
        foreach (var input in new Control[] { HostText, PortText, UserText, PasswordText, DefaultDatabaseText, CharacterSetComboBox, SslModeComboBox })
            input.TextChanged += (_, _) => InvalidateConnection();
        _sqliteFileComboBox.SelectedIndexChanged += (_, _) => InvalidateConnection();
        _databaseTreeView.BeforeExpand += DatabaseTreeView_BeforeExpand;
        _databaseTreeView.NodeMouseDoubleClick += DatabaseTreeView_NodeMouseDoubleClick;
        DatabaseComboBox.SelectedIndexChanged += (_, _) => CurrentDatabaseStatusLabel.Text = $"范围：{DatabaseComboBox.Text}";
        ExecuteQueryButton.Click += async (_, _) => await ExecuteQueryAsync();
        StopQueryButton.Click += (_, _) => { _operation?.Cancel(); ResultStateLabel.Text = "正在取消…"; };
        ClearSqlButton.Click += (_, _) => SqlEditorTextBox.Clear();
        RefreshButton.Click += async (_, _) => await ConnectAsync();
        ExportButton.Click += async (_, _) => await ExportAsync();
        KeyDown += async (_, e) => { if (e.KeyCode == Keys.F5) { e.SuppressKeyPress = true; await ExecuteQueryAsync(); } };
        FormClosing += (_, e) =>
        {
            if (_operation is null) return;
            e.Cancel = true;
            _closeAfterOperation = true;
            _operation.Cancel();
        };
        CopySqlMenuItem.Click += (_, _) => SqlEditorTextBox.Copy();
        PasteSqlMenuItem.Click += (_, _) => SqlEditorTextBox.Paste(DataFormats.GetFormat(DataFormats.UnicodeText));
        SelectAllSqlMenuItem.Click += (_, _) => SqlEditorTextBox.SelectAll();
        _databaseTypeComboBox.SelectedIndex = 0;
        CharacterSetComboBox.SelectedIndex = 0;
        SslModeComboBox.SelectedIndex = 0;
        SetBusy(false);
    }

    // 一次只允许一个数据库操作，避免旧查询覆盖新连接，也避免重复写配置。
    private async Task RunOperationAsync(Func<CancellationToken, Task> action)
    {
        if (_operation is not null) return;
        using var cancellation = new CancellationTokenSource();
        _operation = cancellation;
        var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _operationCompleted = completed;
        SetBusy(true);
        try { await action(cancellation.Token); }
        catch (OperationCanceledException) { ShowMessage("操作已取消"); }
        catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex.ToString()); ShowMessage(ex.Message); }
        finally
        {
            _operation = null;
            SetBusy(false);
            completed.TrySetResult();
            if (_closeAfterOperation) Close();
        }
    }

    private void SetBusy(bool busy)
    {
        ConnectionPanel.Enabled = !busy;
        ExecuteQueryButton.Enabled = !busy && _activeConnectionProfile is not null;
        RefreshButton.Enabled = !busy && _activeConnectionProfile is not null;
        ExportButton.Enabled = !busy && dataGridView1.DataSource is DataTable;
        ClearSqlButton.Enabled = !busy;
        DatabaseComboBox.Enabled = !busy;
        _databaseTreeView.Enabled = !busy;
        StopQueryButton.Enabled = busy;
        UseWaitCursor = busy;
    }

    private void InvalidateConnection()
    {
        if (_filling) return;
        _activeConnectionProfile = null;
        _databaseTreeView.Nodes.Clear();
        DatabaseComboBox.DataSource = null;
        dataGridView1.DataSource = null;
        ConnectionStatusLabel.Text = "● 未连接";
        ConnectionStatusLabel.ForeColor = Color.DimGray;
        SetBusy(_operation is not null);
    }

    private async Task LoadProfilesAsync(CancellationToken token, string? selectedName = null)
    {
        var profiles = await _connectionService.ReadAllConfigurationsAsync(token);
        _filling = true;
        SavedConnectionsComboBox.DataSource = null;
        SavedConnectionsComboBox.DisplayMember = nameof(ConnectionProfile.ConnectionName);
        SavedConnectionsComboBox.DataSource = profiles;
        SavedConnectionsComboBox.SelectedItem = profiles.FirstOrDefault(p => p.ConnectionName == selectedName) ?? profiles.FirstOrDefault();
        _filling = false;
        if (SavedConnectionsComboBox.SelectedItem is ConnectionProfile profile) FillProfile(profile);
    }

    private void ResetConnection()
    {
        _filling = true;
        SavedConnectionsComboBox.SelectedIndex = -1;
        _databaseTypeComboBox.SelectedIndex = 0;
        ConnectionnameText.Clear(); HostText.Text = "localhost"; PortText.Text = "3306";
        UserText.Clear(); PasswordText.Clear(); DefaultDatabaseText.Clear();
        IntegratedSecurityCheckBox.Checked = TrustCertificateCheckBox.Checked = SavePasswordCheckBox.Checked = false;
        _filling = false;
        InvalidateConnection();
        UpdateDatabaseTypeUi();
    }

    private ConnectionProfile BuildProfile() => new()
    {
        ConnectionName = ConnectionnameText.Text.Trim(), DatabaseType = SelectedType,
        Host = HostText.Text.Trim(), Port = ParsePort(),
        UserName = UserText.Text.Trim(), Password = PasswordText.Text,
        DefaultDatabase = SelectedType == DatabaseType.Oracle ? null : NullIfBlank(DefaultDatabaseText.Text),
        ServiceName = SelectedType == DatabaseType.Oracle ? DefaultDatabaseText.Text.Trim() : "",
        IntegratedSecurity = IntegratedSecurityCheckBox.Checked, TrustServerCertificate = TrustCertificateCheckBox.Checked,
        CharacterSet = CharacterSetComboBox.Text, SSLmode = SslModeComboBox.Text,
        ConnectionTimeout = (uint)TimeoutNumericUpDown.Value, SavePassword = SavePasswordCheckBox.Checked,
        DatabaseFilePath = _sqliteFileComboBox.SelectedItem?.ToString() ?? ""
    };
    private uint ParsePort()
    {
        if (SelectedType == DatabaseType.SQLite || (SelectedType == DatabaseType.SqlServer && string.IsNullOrWhiteSpace(PortText.Text))) return 0;
        if (!uint.TryParse(PortText.Text, out uint value) || value is 0 or > 65535) throw new ArgumentException("端口必须是 1～65535 的数字");
        return value;
    }
    private static string? NullIfBlank(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void FillProfile(ConnectionProfile profile)
    {
        _filling = true;
        _databaseTypeComboBox.SelectedIndex = Math.Max(0, Array.IndexOf(DatabaseTypes, profile.DatabaseType));
        ConnectionnameText.Text = profile.ConnectionName;
        HostText.Text = profile.DatabaseType == DatabaseType.SQLite ? Path.GetDirectoryName(profile.DatabaseFilePath) : profile.Host;
        PortText.Text = profile.Port == 0 ? "" : profile.Port.ToString();
        UserText.Text = profile.UserName; PasswordText.Text = profile.Password;
        DefaultDatabaseText.Text = profile.DatabaseType == DatabaseType.Oracle ? profile.ServiceName : profile.DefaultDatabase;
        IntegratedSecurityCheckBox.Checked = profile.IntegratedSecurity;
        TrustCertificateCheckBox.Checked = profile.TrustServerCertificate;
        CharacterSetComboBox.Text = profile.CharacterSet ?? "utf8mb4";
        SslModeComboBox.Text = string.IsNullOrWhiteSpace(profile.SSLmode) ? "Preferred" : profile.SSLmode;
        TimeoutNumericUpDown.Value = Math.Clamp(profile.ConnectionTimeout, (uint)TimeoutNumericUpDown.Minimum, (uint)TimeoutNumericUpDown.Maximum);
        SavePasswordCheckBox.Checked = profile.SavePassword;
        RefreshSqliteFiles();
        if (profile.DatabaseType == DatabaseType.SQLite) _sqliteFileComboBox.SelectedItem = profile.DatabaseFilePath;
        _filling = false;
        InvalidateConnection();
        UpdateDatabaseTypeUi();
    }

    private void UpdateDatabaseTypeUi()
    {
        bool sqlite = SelectedType == DatabaseType.SQLite;
        bool mysql = SelectedType == DatabaseType.MySQL;
        bool sqlServer = SelectedType == DatabaseType.SqlServer;
        HostLabel.Text = sqlite ? "SQLite 文件夹" : "主机 / 实例";
        HostText.PlaceholderText = sqlite ? "选择包含数据库文件的目录" : "localhost";
        _browseSqliteButton.Visible = sqlite;
        PortLabel.Text = sqlite ? "数据库文件" : sqlServer ? "端口（命名实例可留空）" : "端口";
        PortText.Visible = !sqlite; _sqliteFileComboBox.Visible = sqlite;
        UserLabel.Visible = UserText.Visible = PasswordLabel.Visible = PasswordPanel.Visible = !sqlite;
        UserText.Enabled = PasswordPanel.Enabled = !(sqlServer && IntegratedSecurityCheckBox.Checked);
        DefaultDatabaseLabel.Text = SelectedType == DatabaseType.Oracle ? "Service Name" : "默认数据库（可选）";
        DefaultDatabaseLabel.Visible = DefaultDatabaseText.Visible = !sqlite;
        CharacterSetLabel.Visible = CharacterSetComboBox.Visible = SslModeLabel.Visible = SslModeComboBox.Visible = mysql;
        AuthenticationPanel.Visible = sqlServer;
        SavePasswordCheckBox.Visible = !sqlite;
        ConnectButton.Text = "连接 " + _databaseTypeComboBox.Text;
        DatabaseLabel.Text = SelectedType == DatabaseType.Oracle ? "Schema" : "数据库";
        if (sqlite) RefreshSqliteFiles();
    }

    private void RefreshSqliteFiles()
    {
        if (SelectedType != DatabaseType.SQLite) return;
        string? selected = _sqliteFileComboBox.SelectedItem?.ToString();
        bool wasFilling = _filling;
        _filling = true;
        _sqliteFileComboBox.Items.Clear();
        if (!Directory.Exists(HostText.Text)) { _filling = wasFilling; return; }
        try
        {
            var files = Directory.EnumerateFiles(HostText.Text).Where(f => new[] { ".db", ".sqlite", ".sqlite3" }.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)).OrderBy(Path.GetFileName).ToArray();
            _sqliteFileComboBox.Items.AddRange(files);
            _sqliteFileComboBox.SelectedItem = selected;
            if (_sqliteFileComboBox.SelectedIndex < 0 && files.Length > 0) _sqliteFileComboBox.SelectedIndex = 0;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { ShowMessage(ex.Message); }
        finally { _filling = wasFilling; }
        if (selected != _sqliteFileComboBox.SelectedItem?.ToString()) InvalidateConnection();
    }

    private async Task ConnectAsync() => await RunOperationAsync(async token =>
    {
        var profile = BuildProfile();
        InvalidateConnection();
        ConnectionStatusLabel.Text = "● 正在连接";
        try
        {
            var databases = await Task.Run(() => _schemaService.GetDatabasesAsync(profile, token), token);
            token.ThrowIfCancellationRequested();
            _activeConnectionProfile = profile;
            DatabaseComboBox.DataSource = databases;
            string? preferred = profile.DatabaseType == DatabaseType.Oracle ? profile.UserName.ToUpperInvariant() : profile.DefaultDatabase;
            if (preferred is not null && databases.Contains(preferred)) DatabaseComboBox.SelectedItem = preferred;
            foreach (string database in databases)
            {
                TreeNode node = new(database) { Tag = database };
                node.Nodes.Add(new TreeNode("展开后加载…"));
                _databaseTreeView.Nodes.Add(node);
            }
            ConnectionStatusLabel.Text = "● 已连接 " + _databaseTypeComboBox.Text;
            ConnectionStatusLabel.ForeColor = Color.SeaGreen;
            ResultStateLabel.Text = $"已加载 {databases.Count} 个数据库 / Schema";
            ResultTabControl.SelectedTab = _databaseObjectsTabPage;
        }
        catch { ConnectionStatusLabel.Text = "● 未连接"; throw; }
    });

    private async void DatabaseTreeView_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
    {
        var node = e.Node;
        if (node is null || node.Nodes.Count != 1 || node.Nodes[0].Tag is not null || node.Nodes[0].Text != "展开后加载…") return;
        e.Cancel = true;
        if (_activeConnectionProfile is null) return;
        await RunOperationAsync(async token =>
        {
            var profile = _activeConnectionProfile;
            List<TreeNode> children = new();
            if (node.Tag is string database)
            {
                var objects = await Task.Run(() => _schemaService.GetObjectsAsync(profile, database, token), token);
                foreach (var item in objects)
                {
                    string name = profile.DatabaseType == DatabaseType.SqlServer ? item.SchemaName + "." + item.ObjectName : item.ObjectName;
                    TreeNode child = new($"[{(item.ObjectType == "View" ? "视图" : "表")}] {name}") { Tag = item, ToolTipText = item.Description };
                    child.Nodes.Add(new TreeNode("展开后加载…")); children.Add(child);
                }
            }
            else if (node.Tag is DatabaseObjectInfo item)
            {
                var columns = await Task.Run(() => _schemaService.GetColumnsAsync(profile, item.DatabaseName, item.ObjectName, token, item.SchemaName), token);
                children.AddRange(columns.Select(c => new TreeNode($"{c.ColumnName} : {c.FullColumnType} {(c.IsNullable ? "NULL" : "NOT NULL")}{(c.IsPrimaryKey ? " PK" : "")}") { Tag = c, ToolTipText = c.Comment ?? "" }));
            }
            token.ThrowIfCancellationRequested();
            node.Nodes.Clear();
            node.Nodes.AddRange(children.Count > 0 ? children.ToArray() : [new TreeNode("（无可见对象）")]);
            node.Expand();
        });
    }

    private async void DatabaseTreeView_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (_activeConnectionProfile is null || e.Node?.Tag is not DatabaseObjectInfo item) return;
        var expectedProfile = _activeConnectionProfile;
        if (_operationCompleted is not null) await _operationCompleted.Task;
        if (IsDisposed || _closeAfterOperation || _activeConnectionProfile != expectedProfile) return;
        await RunOperationAsync(async token =>
        {
            var profile = _activeConnectionProfile;
            var result = await Task.Run(() => _schemaService.PreviewAsync(profile, item.DatabaseName, item.ObjectName, 200, token, item.SchemaName), token);
            token.ThrowIfCancellationRequested();
            DatabaseComboBox.SelectedItem = item.DatabaseName;
            ShowResult(result, $"{item.SchemaName}.{item.ObjectName} · 前 200 行");
        });
    }

    private async Task ExecuteQueryAsync()
    {
        if (_activeConnectionProfile is null) { ShowMessage("请先连接数据库"); return; }
        await RunOperationAsync(async token =>
        {
            var profile = _activeConnectionProfile;
            var request = new QueryRequest { DatabaseName = DatabaseComboBox.Text, SqlText = SqlEditorTextBox.Text,
                MaxRows = 2000, TimeoutSeconds = (int)QueryTimeoutNumericUpDown.Value, IsReadOnly = true };
            ResultStateLabel.Text = "查询执行中…";
            var result = await Task.Run(() => _queryService.ExecuteAsync(profile, request, token), token);
            token.ThrowIfCancellationRequested();
            ShowResult(result, "查询结果 · 最多 2000 行");
        });
    }

    private void ShowResult(QueryResult result, string title)
    {
        if (!result.IsSuccess) { dataGridView1.DataSource = null; ShowMessage(result.ErrorMessage ?? "查询失败"); return; }
        dataGridView1.DataSource = result.Rows;
        ResultSummaryLabel.Text = title;
        ResultStateLabel.Text = $"{result.RowCount} 行 · {result.ExecutionTime} ms{(result.IsTruncated ? " · 结果已截断" : "")}";
        MessageTextBox.Text = "查询成功。";
        ResultTabControl.SelectedTab = ResultTabPage;
    }
    private void ShowMessage(string message)
    {
        MessageTextBox.Text = message; ResultStateLabel.Text = message;
        ResultTabControl.SelectedTab = MessageTabPage;
    }

    private async Task ExportAsync()
    {
        if (dataGridView1.DataSource is not DataTable table) return;
        using SaveFileDialog dialog = new() { Filter = "CSV 文件|*.csv", FileName = "查询结果.csv" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        await RunOperationAsync(async token => { await CsvExporter.WriteAsync(table, dialog.FileName, token); ShowMessage("已导出当前显示的结果：" + dialog.FileName); });
    }
}
