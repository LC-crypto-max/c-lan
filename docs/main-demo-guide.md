# main 演示版：操作与验收

## 当前用途

main 用于多数据库浏览、只读查询及项目汇报。沿用 prod 的配色、布局和设计器控件；不包含电检同步、服务端 HTTP 请求、定时同步或托盘驻留。prod 分支未修改。

## 启动与设计器

```powershell
dotnet run --project 'c#lan.csproj'
```

Visual Studio 打开 `c#lan.slnx`，右键 `Forms/Form1.cs` → 查看设计器。固定控件定义在 `Form1.Designer.cs`，窗体有无参构造；布局可通过设计器拖动、Dock、Anchor 和 TableLayoutPanel 行列属性调整。运行时代码只切换数据库相关字段的显示和状态。

## 5 分钟演示顺序

1. 选择 SQLite，连接名称填写“本地演示”。选择项目目录（或程序输出目录），数据库文件选择 `demo_sqlite.db`。
2. 点击测试连接，再点击连接。展开 `main`，展示表、视图和字段；双击 `customers` 预览前 200 行。
3. 执行以下查询，展示结果、行数、耗时及 NULL/中文。F5 也可执行查询。

```sql
SELECT * FROM customer_order_summary;
```

4. 点击“导出 CSV”，展示当前结果的导出文件。仅导出已加载的行，截断提示意味着还有未导出的记录。
5. 输入 `DELETE FROM customers`，展示只读拦截。
6. 用下列查询展示停止按钮；也可将超时设为 1 秒演示超时。

```sql
WITH RECURSIVE n(x) AS (
    SELECT 1 UNION ALL SELECT x + 1 FROM n WHERE x < 1000000000
)
SELECT SUM(x) FROM n;
```

7. 保存连接，通过左上连接列表切换。修改连接参数后需要重新连接，避免混用旧连接和新参数。
8. 切换 MySQL、SQL Server、Oracle，说明同一套窗体和服务如何选择各自 Provider。没有可用实例时只展示配置入口和代码，不把它描述为已完成实机联调。

## 连接参数

| 数据库 | 参数与范围 |
|---|---|
| SQLite | 选择文件夹和 `.db` / `.sqlite` / `.sqlite3` 文件；以只读模式打开，范围为 `main` |
| MySQL | 主机、端口（默认 3306）、账号密码；可选默认库、字符集、SSL 模式 |
| SQL Server | 主机或 `主机\实例`；普通连接默认 1433，命名实例可留空端口；支持 Windows 或 SQL 登录；对象按 `Schema.表名` 显示 |
| Oracle | 主机、1521、Service Name、账号密码；选择可访问的 Schema；目前使用 Service Name，不提供 SID/TNS 配置入口 |

SQL Server 默认验证服务器证书；本地测试自签名证书可勾选“信任服务器证书”。MySQL 的 SSL 设置实际传入驱动。服务器数据库请使用只有查询权限的账号；客户端 SQL 检查是辅助保护，不能代替数据库权限。

## 已实现与边界

- 四种 Provider 的连接、范围、表/视图、字段、主键、预览和查询接口。
- 预览最多 200 行，手写查询最多 2000 行，多读取 1 行判断是否截断。
- 仅支持单条 SELECT / WITH 查询；禁止 SELECT INTO、多语句和修改语句。采用保守词法检查；暂不支持可执行注释、查询提示、反斜杠转义、嵌套注释及 `#` 语法。`--` 注释后请加空格。
- 查询在后台执行，支持停止和超时。SQLite 使用进度回调中断 CPU 密集查询。
- 连接列表、新建、保存、删除、刷新对象、CSV 导出和编辑区复制粘贴。
- 配置保存到 `%APPDATA%\c-lan-browser\connections.json`。第一次可读取旧 `%APPDATA%\c-lan\connections.json`，后续保存到新目录，不改写 prod 配置。密码使用 Windows 当前用户 DPAPI 加密；换用户后需重新输入密码。
- 当前提供有限行预览，未加入分页导航、查询历史、多标签页及数据库写操作。

## 汇报时讲解的代码链路

`Form1 → ConnectionService / SchemaService / QueryService → DatabaseProviderFactory → Provider → QueryResult → DataGridView`

可以展示：接口与多态、简单工厂、手工依赖注入、async/await、取消令牌、资源释放、TreeView 延迟加载、参数化元数据查询和数据库方言差异。SQL Server 与 Oracle 共用 ADO.NET 执行流程，元数据 SQL 与标识符引用分别实现。

## 回归检查

```powershell
dotnet build 'c#lan.csproj'
dotnet run --project Checks/c-lan.Checks.csproj
dotnet run --project Checks/c-lan.Checks.csproj -- --ui
```

检查使用临时 SQLite 数据库，覆盖真实查询、视图、空表、重复列名、截断、取消、超时、SQL 限制、配置加密和 CSV。`--ui` 还执行窗体连接、展开和查询流程，并在 Checks 输出目录生成界面 PNG。

可选 `--saved-databases` 会读取本机已保存的服务器数据库连接，执行只读连接、元数据、SELECT 1 和预览检查；不会创建或修改服务器数据。
## 本次验证结果

- Release 构建成功：0 警告、0 错误。
- SQLite 真实数据库、只读校验、配置加密、CSV 检查通过。
- 正常 WinForms 消息循环下，四种数据库配置界面的两种窗口尺寸，以及 SQLite 连接、展开、预览、查询流程通过，开启了跨线程访问检查。
- 本机没有已保存的服务器数据库连接，因此 MySQL、SQL Server、Oracle 实机联调尚未完成；当前验证覆盖驱动加载、工厂路由及连接字符串。
