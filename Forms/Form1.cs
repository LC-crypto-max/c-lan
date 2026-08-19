using c_lan.Models;
using c_lan.Services;
using System.Diagnostics;

namespace c_lan
{
    public partial class Form1 : Form
    {
        private readonly IConnectionService _connectionService;
        private readonly ISchemaService _schemaService;
        private CancellationTokenSource? _cancellationTokenSource;
        private ConnectionProfile? _activeConnectionProfile;
        private readonly TreeView _databaseTreeView = new TreeView();
        private readonly TabPage _databaseObjectsTabPage = new TabPage();

        public Form1(IConnectionService connectionService, ISchemaService schemaService)
        {
            InitializeComponent();
            _connectionService = connectionService;
            _schemaService = schemaService;

            // 这些事件属于业务接线，放在 Form 代码中比手工修改 Designer 更容易阅读。
            Shown += Form1_Shown;
            ConnectButton.Click += ConnectButton_Click;
            SaveConnectionButton.Click += SaveConnectionButton_Click;
            DeleteConnectionButton.Click += DeleteConnectionButton_Click;
            ShowPasswordCheckBox.CheckedChanged += ShowPasswordCheckBox_CheckedChanged;
            InitializeDatabaseObjectBrowser();
        }

        private CancellationTokenSource CreateNewCancellationToken()
        {
            // 新操作开始时取消旧操作，避免两个配置操作同时改写同一个文件。
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            return _cancellationTokenSource;
        }

        private async void Form1_Shown(object? sender, EventArgs e)
        {
            try
            {
                List<ConnectionProfile> profiles = await _connectionService.ReadAllConfigurationsAsync(CancellationToken.None);

                // 当前界面还没有“已保存连接”下拉框，先回填第一条配置完成读取闭环。
                // 第三关之后如果增加连接列表，可把这里改为回填用户选中的配置。
                ConnectionProfile? firstProfile = profiles.FirstOrDefault();
                if (firstProfile is not null)
                {
                    FillFormFromProfile(firstProfile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"读取连接配置失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private async void TestButton_Click(object sender, EventArgs e)
        {
            CancellationTokenSource cancellation = CreateNewCancellationToken();

            try
            {
                TestButton.Enabled = false;
                TestButton.Text = "测试中...";

                ConnectionProfile profile = BuildConnectionProfileFromForm();

                Debug.WriteLine("开始测试连接");
                ConnectionResult result = await _connectionService.TestConnectionAsync(profile,cancellation.Token);
                Debug.WriteLine("测试完成");

                MessageBox.Show(result.IsSuccess ? "连接成功" : result.ErrorMessage);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("测试连接已取消");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("测试连接遇到未知错误");
                MessageBox.Show(ex.Message,"测试连接失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                TestButton.Enabled = true;
                TestButton.Text = "测试连接";
                DisposeCancellationToken(cancellation);
            }
        }

        //连接按钮负责加载数据库范围，并为TreeView建立第一层节点
        private async void ConnectButton_Click(object? sender, EventArgs e)
        {
            CancellationTokenSource cancellation = CreateNewCancellationToken();

            try
            {
                ConnectButton.Enabled = false;
                ConnectButton.Text = "连接中...";
                ConnectionStatusLabel.Text = "● 正在连接";

                ConnectionProfile profile = BuildConnectionProfileFromForm();
                List<string> databases = await _schemaService.GetDatabasesAsync(profile, cancellation.Token);

                _activeConnectionProfile = profile;
                FillDatabaseTree(databases);

                //数据库下拉框仍会在后续查询功能中使用，因此这里同步填充。
                DatabaseComboBox.DataSource = null;
                DatabaseComboBox.DataSource = databases;
                if (!String.IsNullOrWhiteSpace(profile.DefaultDatabase) && databases.Contains(profile.DefaultDatabase))
                {
                    DatabaseComboBox.SelectedItem = profile.DefaultDatabase;
                }

                ConnectionStatusLabel.Text = "● 已连接";
                ConnectionStatusLabel.ForeColor = Color.FromArgb(31, 137, 89);
                ResultStateLabel.Text = $"已加载 {databases.Count} 个数据库";
                ResultTabControl.SelectedTab = _databaseObjectsTabPage;
            }
            catch (OperationCanceledException)
            {
                ConnectionStatusLabel.Text = "● 连接已取消";
            }
            catch (Exception ex)
            {
                _activeConnectionProfile = null;
                _databaseTreeView.Nodes.Clear();
                ConnectionStatusLabel.Text = "● 连接失败";
                ConnectionStatusLabel.ForeColor = Color.FromArgb(173, 58, 58);
                MessageBox.Show(ex.Message,"加载数据库失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                ConnectButton.Enabled = true;
                ConnectButton.Text = "连接 MySQL";
                DisposeCancellationToken(cancellation);
            }
        }

        private void InitializeDatabaseObjectBrowser()
        {
            //不重做现有布局，只在结果区增加一个对象树页签。
            _databaseObjectsTabPage.Text = "数据库对象";
            _databaseObjectsTabPage.Padding = new Padding(8);
            _databaseObjectsTabPage.UseVisualStyleBackColor = true;

            _databaseTreeView.Dock = DockStyle.Fill;
            _databaseTreeView.BorderStyle = BorderStyle.None;
            _databaseTreeView.HideSelection = false;
            _databaseTreeView.ShowNodeToolTips = true;
            _databaseTreeView.BeforeExpand += DatabaseTreeView_BeforeExpand;
            _databaseTreeView.NodeMouseDoubleClick += DatabaseTreeView_NodeMouseDoubleClick;

            _databaseObjectsTabPage.Controls.Add(_databaseTreeView);
            ResultTabControl.TabPages.Insert(0, _databaseObjectsTabPage);
        }

        private void FillDatabaseTree(List<string> databases)
        {
            _databaseTreeView.BeginUpdate();
            try
            {
                _databaseTreeView.Nodes.Clear();
                foreach (string database in databases)
                {
                    TreeNode databaseNode = new TreeNode(database){Tag = new BrowserNodeInfo{NodeType = BrowserNodeType.Database,DatabaseName = database}};

                    //占位节点会让TreeView显示展开箭头，真正展开时再访问数据库。
                    databaseNode.Nodes.Add(CreatePlaceholderNode());
                    _databaseTreeView.Nodes.Add(databaseNode);
                }
            }
            finally
            {
                _databaseTreeView.EndUpdate();
            }
        }

        private async void DatabaseTreeView_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            TreeNode? treeNode = e.Node;
            if (_activeConnectionProfile is null || treeNode?.Tag is not BrowserNodeInfo nodeInfo || !HasPlaceholderNode(treeNode))
            {
                return;
            }

            CancellationTokenSource cancellation = CreateNewCancellationToken();
            treeNode.Nodes.Clear();

            try
            {
                if (nodeInfo.NodeType == BrowserNodeType.Database)
                {
                    await LoadObjectNodesAsync(treeNode, nodeInfo, cancellation.Token);
                }
                else if (nodeInfo.NodeType == BrowserNodeType.DatabaseObject)
                {
                    await LoadColumnNodesAsync(treeNode, nodeInfo, cancellation.Token);
                }
            }
            catch (OperationCanceledException)
            {
                //取消后重新放回占位节点，下次展开还可以再次加载。
                treeNode.Nodes.Add(CreatePlaceholderNode());
            }
            catch (Exception ex)
            {
                treeNode.Nodes.Add(CreatePlaceholderNode());
                MessageBox.Show(ex.Message,"加载数据库对象失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                DisposeCancellationToken(cancellation);
            }
        }

        private async Task LoadObjectNodesAsync(TreeNode databaseNode, BrowserNodeInfo nodeInfo, CancellationToken token)
        {
            List<DatabaseObjectInfo> objects = await _schemaService.GetObjectsAsync(_activeConnectionProfile!, nodeInfo.DatabaseName, token);

            foreach (DatabaseObjectInfo databaseObject in objects)
            {
                string iconText = databaseObject.ObjectType == "View" ? "视图" : "表";
                TreeNode objectNode = new TreeNode($"[{iconText}] {databaseObject.ObjectName}")
                {
                    ToolTipText = databaseObject.Description,
                    Tag = new BrowserNodeInfo{NodeType = BrowserNodeType.DatabaseObject,DatabaseName = databaseObject.DatabaseName,ObjectName = databaseObject.ObjectName,ObjectType = databaseObject.ObjectType}
                };
                objectNode.Nodes.Add(CreatePlaceholderNode());
                databaseNode.Nodes.Add(objectNode);
            }

            if (objects.Count == 0)
            {
                databaseNode.Nodes.Add(new TreeNode("（没有表或视图）"));
            }
        }

        private async Task LoadColumnNodesAsync(TreeNode objectNode, BrowserNodeInfo nodeInfo, CancellationToken token)
        {
            List<ColumnInfo> columns = await _schemaService.GetColumnsAsync(_activeConnectionProfile!,nodeInfo.DatabaseName,nodeInfo.ObjectName,token);

            foreach (ColumnInfo column in columns)
            {
                string nullableText = column.IsNullable ? "NULL" : "NOT NULL";
                string keyText = column.IsPrimaryKey ? " PK" : String.Empty;
                TreeNode columnNode = new TreeNode(
                    $"{column.ColumnName} : {column.FullColumnType} {nullableText}{keyText}")
                {
                    ToolTipText = column.Comment ?? String.Empty,
                    Tag = new BrowserNodeInfo{NodeType = BrowserNodeType.Column,DatabaseName = nodeInfo.DatabaseName,ObjectName = nodeInfo.ObjectName}
                };
                objectNode.Nodes.Add(columnNode);
            }

            if (columns.Count == 0)
            {
                objectNode.Nodes.Add(new TreeNode("（没有字段）"));
            }
        }

        //双击表或视图后，在已有DataGridView中显示前200行
        private async void DatabaseTreeView_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode? treeNode = e.Node;
            if (_activeConnectionProfile is null || treeNode?.Tag is not BrowserNodeInfo nodeInfo || nodeInfo.NodeType != BrowserNodeType.DatabaseObject)
            {
                return;
            }

            CancellationTokenSource cancellation = CreateNewCancellationToken();
            try
            {
                ResultStateLabel.Text = "正在加载预览...";
                QueryResult result = await _schemaService.PreviewAsync(_activeConnectionProfile,nodeInfo.DatabaseName,nodeInfo.ObjectName,200,cancellation.Token);

                if (!result.IsSuccess)
                {
                    MessageBox.Show(result.ErrorMessage, "预览失败");
                    ResultStateLabel.Text = "预览失败";
                    return;
                }

                dataGridView1.DataSource = result.Rows;
                ResultSummaryLabel.Text = $"{nodeInfo.DatabaseName}.{nodeInfo.ObjectName}";
                string truncatedText = result.IsTruncated ? "，仅显示前200行" : String.Empty;
                ResultStateLabel.Text = $"{result.RowCount ?? 0} 行，{result.ExecutionTime} ms{truncatedText}";
                CurrentDatabaseStatusLabel.Text = $"数据库：{nodeInfo.DatabaseName}";
                ResultTabControl.SelectedTab = ResultTabPage;
            }
            catch (OperationCanceledException)
            {
                ResultStateLabel.Text = "预览已取消";
            }
            catch (Exception ex)
            {
                ResultStateLabel.Text = "预览失败";
                MessageBox.Show(ex.Message,"预览失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                DisposeCancellationToken(cancellation);
            }
        }

        private static bool HasPlaceholderNode(TreeNode node)
        {
            return node.Nodes.Count == 1 && node.Nodes[0].Tag is BrowserNodeInfo placeholder && placeholder.NodeType == BrowserNodeType.Placeholder;
        }

        private static TreeNode CreatePlaceholderNode()
        {
            return new TreeNode("展开后加载...")
            {
                Tag = new BrowserNodeInfo { NodeType = BrowserNodeType.Placeholder }
            };
        }

        private async void SaveConnectionButton_Click(object? sender, EventArgs e)
        {
            CancellationTokenSource cancellation = CreateNewCancellationToken();

            try
            {
                SetConfigurationButtonsEnabled(false);
                SaveConnectionButton.Text = "保存中...";

                ConnectionProfile profile = BuildConnectionProfileFromForm();
                SaveConfigurationResult result =await _connectionService.SaveConnectionConfigurationAsync(profile,cancellation.Token);

                ShowConfigurationResult(result, "保存连接配置");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("保存操作已取消");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"保存连接配置失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                SaveConnectionButton.Text = "保存配置";
                SetConfigurationButtonsEnabled(true);
                DisposeCancellationToken(cancellation);
            }
        }

        private async void DeleteConnectionButton_Click(object? sender, EventArgs e)
        {
            string connectionName = ConnectionnameText.Text.Trim();
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                MessageBox.Show("请先填写要删除的连接名称");
                return;
            }

            DialogResult confirmation = MessageBox.Show($"确定删除连接配置“{connectionName}”吗？","确认删除",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            CancellationTokenSource cancellation = CreateNewCancellationToken();

            try
            {
                SetConfigurationButtonsEnabled(false);
                DeleteConnectionButton.Text = "删除中...";

                SaveConfigurationResult result = await _connectionService.DeleteConnectionConfigurationAsync(connectionName,cancellation.Token);

                ShowConfigurationResult(result, "删除连接配置");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("删除操作已取消");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"删除连接配置失败",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            finally
            {
                DeleteConnectionButton.Text = "删除配置";
                SetConfigurationButtonsEnabled(true);
                DisposeCancellationToken(cancellation);
            }
        }

        private ConnectionProfile BuildConnectionProfileFromForm()
        {
            // 测试连接和保存配置共用这一份映射，避免以后新增字段时只改到其中一处。
            return new ConnectionProfile
            {
                ConnectionName = ConnectionnameText.Text,
                DatabaseType = DatabaseType.MySQL,
                Host = HostText.Text,
                Port = uint.TryParse(PortText.Text, out uint port) ? port : 0,
                UserName = UserText.Text,
                Password = PasswordText.Text,
                DefaultDatabase = NullIfWhiteSpace(DefaultDatabaseText.Text),
                CharacterSet = NullIfWhiteSpace(CharacterSetComboBox.Text),
                SSLmode = SslModeComboBox.Text,
                SavePassword = SavePasswordCheckBox.Checked,
                ConnectionTimeout = (uint)TimeoutNumericUpDown.Value
            };
        }

        private void FillFormFromProfile(ConnectionProfile profile)
        {
            ConnectionnameText.Text = profile.ConnectionName;
            HostText.Text = profile.Host;
            PortText.Text = profile.Port == 0 ? String.Empty : profile.Port.ToString();
            UserText.Text = profile.UserName;
            PasswordText.Text = profile.Password;
            DefaultDatabaseText.Text = profile.DefaultDatabase ?? String.Empty;
            CharacterSetComboBox.Text = profile.CharacterSet ?? String.Empty;
            SslModeComboBox.Text = profile.SSLmode;
            SavePasswordCheckBox.Checked = profile.SavePassword;

            decimal timeout = Math.Clamp((decimal)profile.ConnectionTimeout,TimeoutNumericUpDown.Minimum,TimeoutNumericUpDown.Maximum);
            TimeoutNumericUpDown.Value = timeout;
        }

        private void ShowPasswordCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            PasswordText.UseSystemPasswordChar = !ShowPasswordCheckBox.Checked;
        }

        private void SetConfigurationButtonsEnabled(bool enabled)
        {
            SaveConnectionButton.Enabled = enabled;
            DeleteConnectionButton.Enabled = enabled;
        }

        private static void ShowConfigurationResult(
            SaveConfigurationResult result,
            string caption)
        {
            MessageBox.Show(
                result.IsSuccess ? result.Message : result.ErrorMessage,
                caption,
                MessageBoxButtons.OK,
                result.IsSuccess ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        private void DisposeCancellationToken(CancellationTokenSource cancellation)
        {
            if (_cancellationTokenSource == cancellation)
            {
                cancellation.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private static string? NullIfWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        //Tag用来保存节点身份，避免后续再从“[表] 用户”这样的显示文字中拆名称。
        private sealed class BrowserNodeInfo
        {
            public BrowserNodeType NodeType { get; set; }
            public string DatabaseName { get; set; } = String.Empty;
            public string ObjectName { get; set; } = String.Empty;
            public string ObjectType { get; set; } = String.Empty;
        }

        private enum BrowserNodeType
        {
            Placeholder,
            Database,
            DatabaseObject,
            Column
        }
    }
}
