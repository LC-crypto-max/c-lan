using System.Reflection;
using c_lan;
using System.Windows.Forms;
using System.Drawing;
using c_lan.Data;
using c_lan.Models;
using c_lan.Services;
using c_lan.Configuration;
using System.Data;
using c_lan.Utilities;
using Microsoft.Data.Sqlite;

var validator = new ReadOnlySqlValidator();
ColumnMetadataChecks.Run();
Assert(validator.Validate("SELECT updated_at, 'DELETE; ok' FROM t") is null, "普通列名和字符串不应误判");
Assert(validator.Validate("SELECT * INTO backup FROM t") is not null, "SELECT INTO 必须拒绝");
Assert(validator.Validate("SELECT 1; DELETE FROM t") is not null, "多语句必须拒绝");
Assert(validator.Validate("WITH x AS (SELECT 1) SELECT * FROM x") is null, "CTE 查询应可执行");

Assert(validator.Validate("SELECT ';' FROM DUAL; -- 末尾注释", out string prepared) is null && prepared == "SELECT ';' FROM DUAL -- 末尾注释", "只移除字符串外末尾分号，兼容 Oracle");
int evaluatedRows = 0;

using SqliteConnection connection = new("Data Source=:memory:");
await connection.OpenAsync();
connection.CreateFunction("count_row", (long value) =>
{
    evaluatedRows++;
    return value;
});

using SqliteCommand command = connection.CreateCommand();
command.CommandText = """
    WITH RECURSIVE numbers(value) AS
    (
        SELECT 1
        UNION ALL
        SELECT value + 1 FROM numbers WHERE value < 10000
    )
    SELECT count_row(value) AS value FROM numbers
    """;

using SqliteDataReader reader = await command.ExecuteReaderAsync();
(System.Data.DataTable table, bool isTruncated) =
    await BoundedDataTableReader.LoadAsync(reader, 2, CancellationToken.None);

Assert(table.Rows.Count == 2, $"expected 2 rows, got {table.Rows.Count}");
Assert(isTruncated, "expected the result to be marked as truncated");
Assert(evaluatedRows == 3, $"expected 3 rows to be read, got {evaluatedRows}");

Console.WriteLine("Bounded query read check passed.");


foreach (var type in new[] { DatabaseType.MySQL, DatabaseType.SQLite, DatabaseType.SqlServer, DatabaseType.Oracle })
    Assert(new DatabaseProviderFactory().CreateProvider(type).SupportedDatabaseType == type, "Provider 路由错误");
foreach (string sql in new[] { "SELECT 1 INTO OUTFILE 'x'", "SELECT 1--1 INTO OUTFILE 'x'", "SELECT 1 #alias INTO x", "WITH x AS (SELECT 1) DELETE FROM t", "SELECT seq.NEXTVAL FROM dual", "SELECT 1 /*! INTO OUTFILE 'x' */" })
    Assert(validator.Validate(sql) is not null, "应拒绝：" + sql);
foreach (string sql in new[] { "-- 注释\nSELECT 1; -- 结束", "SELECT [updated_at], 'can''t;DELETE' FROM t", "SELECT 1 /* 注释 */", "SELECT '中文 &nbsp;'" })
    Assert(validator.Validate(sql) is null, "应允许：" + sql);

string folder = Path.Combine(Path.GetTempPath(), "c-lan-checks-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(folder);
try
{
    string db = Path.Combine(folder, "demo.db");
    using (var setup = new SqliteConnection($"Data Source={db};Pooling=False"))
    {
        setup.Open();
        using var create = setup.CreateCommand();
        create.CommandText = "CREATE TABLE sample (id INTEGER PRIMARY KEY, updated_at TEXT); WITH RECURSIVE n(x) AS (SELECT 1 UNION ALL SELECT x+1 FROM n WHERE x<250) INSERT INTO sample SELECT x,'中文' FROM n; CREATE VIEW sample_view AS SELECT * FROM sample; CREATE TABLE empty_table (id INTEGER);";
        create.ExecuteNonQuery();
    }
    var profile = new ConnectionProfile { ConnectionName = "demo", DatabaseType = DatabaseType.SQLite, DatabaseFilePath = db, ConnectionTimeout = 3 };
    var provider = new SQLiteProvider();
    Assert((await provider.GetDatabasesAsync(profile, default)).SequenceEqual(new[] { "main" }), "SQLite 范围");
    var objects = await provider.GetObjectsAsync(profile, "main", default);
    Assert(objects.Count == 3 && objects.Any(o => o.ObjectType == "View"), "表和视图元数据");
    var columns = await provider.GetColumnsAsync(profile, "main", "sample", default);
    Assert(columns.Count == 2 && columns[0].IsPrimaryKey, "字段和主键");
    var preview = await provider.PreviewAsync(profile, "main", "sample", 200, default);
    Assert(preview.IsSuccess && preview.RowCount == 200 && preview.IsTruncated, "预览限行");
    var empty = await provider.PreviewAsync(profile, "main", "empty_table", 200, default);
    Assert(empty.IsSuccess && empty.RowCount == 0 && !empty.IsTruncated, "空表");
    var service = new QueryService(new DatabaseProviderFactory(), validator);
    var result = await service.ExecuteAsync(profile, new QueryRequest { SqlText = "SELECT id AS same, id AS same, updated_at, NULL AS blank FROM sample", MaxRows = 5, TimeoutSeconds = 2 }, default);
    Assert(result.IsSuccess && result.RowCount == 5 && result.IsTruncated && result.Rows!.Columns[1].ColumnName == "same_2", "重复列名及限行");
    var literal = await service.ExecuteAsync(profile, new QueryRequest { SqlText = "SELECT '&nbsp;' AS value", TimeoutSeconds = 2 }, default);
    Assert(literal.IsSuccess && (string)literal.Rows!.Rows[0][0] == "&nbsp;", "查询不能改写字符串内容");
    var rejected = await service.ExecuteAsync(profile, new QueryRequest { SqlText = "DELETE FROM sample", TimeoutSeconds = 2 }, default);
    Assert(!rejected.IsSuccess, "只读拦截");
    const string slow = "WITH RECURSIVE n(x) AS (SELECT 1 UNION ALL SELECT x+1 FROM n WHERE x<1000000000) SELECT sum(x) FROM n";
    var timeout = await Task.Run(() => provider.ExecuteQueryAsync(profile, new QueryRequest { SqlText = slow, TimeoutSeconds = 1 }, default));
    Assert(!timeout.IsSuccess && timeout.ErrorMessage!.Contains("超时"), "SQLite CPU 查询超时");
    using var cancel = new CancellationTokenSource(100);
    var cancelled = await Task.Run(() => provider.ExecuteQueryAsync(profile, new QueryRequest { SqlText = slow, TimeoutSeconds = 30 }, cancel.Token));
    Assert(!cancelled.IsSuccess && cancelled.ErrorMessage!.Contains("取消"), "SQLite CPU 查询取消");
    var store = new ConnectionProfileStore(Path.Combine(folder, "connections.json"));
    profile.Password = "秘密-password"; profile.SavePassword = true; profile.ServiceName = "service";
    await store.SaveAsync(new() { profile }, default);
    string stored = await File.ReadAllTextAsync(Path.Combine(folder, "connections.json"));
    Assert(!stored.Contains("password") && stored.Contains("dpapi:"), "密码必须加密");
    Assert((await store.LoadAsync(default))[0].Password == profile.Password, "密码往返");
    profile.SavePassword = false;
    await store.SaveAsync(new() { profile }, default);
    Assert((await store.LoadAsync(default))[0].Password == "", "不保存密码");
    using var alreadyCancelled = new CancellationTokenSource(); alreadyCancelled.Cancel();
    try { await store.SaveAsync(new(), alreadyCancelled.Token); Assert(false, "取消保存应失败"); } catch (OperationCanceledException) { }
    Assert((await store.LoadAsync(default)).Count == 1, "取消保存不能破坏旧文件");
    DataTable csv = new(); csv.Columns.Add("列"); csv.Rows.Add("=1+1"); csv.Rows.Add("a,\"b\"\nc");
    string csvPath = Path.Combine(folder, "result.csv"); await CsvExporter.WriteAsync(csv, csvPath, default);
    string csvText = await File.ReadAllTextAsync(csvPath);
    Assert(csvText.Contains("'=1+1") && csvText.Contains("a,\"\"b\"\"\nc"), "CSV 公式及引号处理");
    var sqlProfile = new ConnectionProfile { Host = "localhost", Port = 1433, UserName = "reader", Password = "a;b", DefaultDatabase = "demo", IntegratedSecurity = true };
    var sqlBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(SqlServerProvider.BuildConnectionString(sqlProfile));
    Assert(sqlBuilder.IntegratedSecurity && sqlBuilder.InitialCatalog == "demo" && !sqlBuilder.TrustServerCertificate, "SQL Server 连接参数");
    sqlProfile.Port = 1521; sqlProfile.ServiceName = "FREEPDB1";
    var oracleBuilder = new Oracle.ManagedDataAccess.Client.OracleConnectionStringBuilder(OracleProvider.BuildConnectionString(sqlProfile));
    Assert(oracleBuilder.DataSource == "localhost:1521/FREEPDB1" && oracleBuilder.Password == "a;b", "Oracle 连接参数");
    Assert(SqlServerProvider.QuoteIdentifier("a]b") == "[a]]b]" && OracleProvider.QuoteIdentifier("a\"b") == "\"a\"\"b\"", "安全引用标识符");
    sqlProfile.SSLmode = "VerifyFull"; sqlProfile.CharacterSet = "utf8mb4";
    var mysql = new MySqlConnector.MySqlConnectionStringBuilder(MysqlProvider.MysqlConnectionStringBuilder(sqlProfile));
    Assert(mysql.SslMode == MySqlConnector.MySqlSslMode.VerifyFull && mysql.Database == "demo", "MySQL SSL/默认库");
    if (args.Contains("--ui")) UiChecks.Run(db);
    Console.WriteLine("四种 Provider 路由/参数、SQLite 实机查询/限行/取消/超时、配置加密及 CSV 检查通过。");
}
finally { Directory.Delete(folder, true); }



if (args.Contains("--saved-databases"))
{
    var profiles = await new ConnectionProfileStore().LoadAsync(default);
    if (!profiles.Any(p => p.DatabaseType is DatabaseType.MySQL or DatabaseType.SqlServer or DatabaseType.Oracle))
        Console.WriteLine("没有已保存的服务器数据库连接，跳过 MySQL / SQL Server / Oracle 实机联调。");
    foreach (var profile in profiles.Where(p => p.DatabaseType is DatabaseType.MySQL or DatabaseType.SqlServer or DatabaseType.Oracle))
    {
        var provider = new DatabaseProviderFactory().CreateProvider(profile.DatabaseType);
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var scopes = await provider.GetDatabasesAsync(profile, deadline.Token);
        string scope = scopes.FirstOrDefault(s => s == profile.DefaultDatabase) ?? scopes.First();
        var objects = await provider.GetObjectsAsync(profile, scope, deadline.Token);
        var query = await new QueryService(new DatabaseProviderFactory(), new ReadOnlySqlValidator()).ExecuteAsync(profile,
            new QueryRequest { DatabaseName = scope, SqlText = profile.DatabaseType == DatabaseType.Oracle ? "SELECT 1 FROM DUAL" : "SELECT 1", TimeoutSeconds = 5 }, deadline.Token);
        Assert(query.IsSuccess && query.RowCount == 1, profile.DatabaseType + " 实机查询失败");
        if (objects.Count > 0)
        {
            var item = objects[0];
            var columns = await provider.GetColumnsAsync(profile, scope, item.ObjectName, deadline.Token, item.SchemaName);
            var preview = await provider.PreviewAsync(profile, scope, item.ObjectName, 5, deadline.Token, item.SchemaName);
            Assert(preview.IsSuccess && columns.Count > 0, profile.DatabaseType + " 实机预览失败");
        }
        Console.WriteLine($"{profile.DatabaseType} 已保存连接的实机元数据/查询/预览检查通过。");
    }
}

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

internal static class UiChecks
{
    private static T Find<T>(Form form, string name) where T : Control => (T)form.Controls.Find(name, true).Single();
    private static async Task WaitForIdle(Form form)
    {
        var stop = Find<Button>(form, "StopQueryButton");
        var timer = System.Diagnostics.Stopwatch.StartNew();
        while (stop.Enabled && timer.Elapsed.TotalSeconds < 10) await Task.Delay(10);
        if (stop.Enabled) throw new Exception("UI 操作超过 10 秒未完成");
    }

    public static void Run(string databaseFile)
    {
        Exception? failure = null;
        Thread thread = new(() =>
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Control.CheckForIllegalCrossThreadCalls = true;
            var factory = new DatabaseProviderFactory();
            using var form = new Form1(new ConnectionService(new ConnectionProfileStore(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json")), factory),
                new SchemaService(factory), new QueryService(factory, new ReadOnlySqlValidator()));
            form.WindowState = FormWindowState.Normal;
            form.Shown += async (_, _) =>
            {
                try
                {
                    await WaitForIdle(form);
                    var types = Find<ComboBox>(form, "_databaseTypeComboBox");
                    foreach (var size in new[] { new Size(1120, 800), new Size(1400, 900) })
                    {
                        form.Size = size;
                        for (int i = 0; i < 4; i++)
                        {
                            types.SelectedIndex = i;
                            await Task.Delay(20);
                            foreach (string name in new[] { "ExecuteQueryButton", "StopQueryButton", "RefreshButton", "ExportButton" })
                            {
                                Control control = Find<Control>(form, name);
                                if (!control.Visible || control.Bounds.Right > control.Parent!.ClientSize.Width)
                                    throw new Exception($"{name} 在 {size} 下被裁切");
                            }
                            using Bitmap bitmap = new(form.Width, form.Height);
                            form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                            bitmap.Save(Path.Combine(AppContext.BaseDirectory, $"ui-{size.Width}-{i}.png"));
                        }
                    }
                    if (form.Controls.Find("_syncPanel", true).Length != 0) throw new Exception("不应包含同步控件");
                    var profile = new ConnectionProfile { ConnectionName = "SQLite 演示", DatabaseType = DatabaseType.SQLite, DatabaseFilePath = databaseFile };
                    typeof(Form1).GetMethod("FillProfile", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(form, new object[] { profile });
                    Find<Button>(form, "ConnectButton").PerformClick();
                    await WaitForIdle(form);
                    var tree = Find<TreeView>(form, "_databaseTreeView");
                    if (tree.Nodes.Count != 1 || !tree.Visible) throw new Exception("界面未能连接 SQLite 并展示对象树");
                    tree.Nodes[0].Expand(); await WaitForIdle(form);
                    var sample = tree.Nodes[0].Nodes.Cast<TreeNode>().Single(n => n.Tag is DatabaseObjectInfo o && o.ObjectName == "sample");
                    sample.Expand(); await WaitForIdle(form);
                    if (sample.Nodes.Count != 2) throw new Exception("界面未能加载字段");
                    typeof(TreeView).GetMethod("OnNodeMouseDoubleClick", BindingFlags.Instance | BindingFlags.NonPublic)!
                        .Invoke(tree, new object[] { new TreeNodeMouseClickEventArgs(sample, MouseButtons.Left, 2, 0, 0) });
                    await WaitForIdle(form);
                    var grid = Find<DataGridView>(form, "dataGridView1");
                    if (grid.DataSource is not System.Data.DataTable preview || preview.Rows.Count != 200) throw new Exception("界面预览未返回 200 行");
                    Find<RichTextBox>(form, "SqlEditorTextBox").Text = "SELECT id, updated_at FROM sample WHERE id <= 10";
                    Find<Button>(form, "ExecuteQueryButton").PerformClick(); await WaitForIdle(form);
                    if (grid.DataSource is not System.Data.DataTable result || result.Rows.Count != 10) throw new Exception("界面查询失败");
                    using Bitmap demo = new(form.Width, form.Height);
                    form.DrawToBitmap(demo, new Rectangle(Point.Empty, demo.Size));
                    demo.Save(Path.Combine(AppContext.BaseDirectory, "ui-demo.png"));
                    // 连接参数变化后必须清除旧连接，重新连接后使用新的超时。
                    var connectionTimeout = Find<NumericUpDown>(form, "TimeoutNumericUpDown");
                    connectionTimeout.Value += 1;
                    if (tree.Nodes.Count != 0 || grid.DataSource is not null || Find<Button>(form, "ExecuteQueryButton").Enabled)
                        throw new Exception("修改连接超时后旧连接仍可使用");
                    Find<Button>(form, "ConnectButton").PerformClick(); await WaitForIdle(form);
                    var active = (ConnectionProfile?)typeof(Form1)
                        .GetField("_activeConnectionProfile", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(form);
                    if (active?.ConnectionTimeout != (uint)connectionTimeout.Value || tree.Nodes.Count != 1)
                        throw new Exception("重新连接没有应用新的连接超时");
                    Find<Button>(form, "ExecuteQueryButton").PerformClick(); await WaitForIdle(form);
                    if (grid.DataSource is not DataTable afterReconnect || afterReconnect.Rows.Count != 10)
                        throw new Exception("修改连接超时并重新连接后查询失败");
                }
                catch (Exception ex) { failure = ex; }
                finally { form.Close(); }
            };
            try { Application.Run(form); }
            catch (Exception ex) { failure = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start(); thread.Join();
        if (failure is not null) throw new Exception("UI 检查失败", failure);
        Console.WriteLine("UI 四种数据库/两种尺寸、SQLite 连接/对象树/预览/查询检查通过；PNG 已写入 Checks 输出目录。");
    }
}
