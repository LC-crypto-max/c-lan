# WinForms 多数据库查看工具：项目规划与架构思路

## 1. 项目定位

这是一个面向学习和实践的 C# WinForms 应用，目标是实现一个弱化版 Navicat/SQLyog。第一阶段只提供简单、安全的数据库浏览与查询能力，不追求完整的数据库管理功能。

计划连接三种数据库：

1. Oracle
2. SQL Server
3. MySQL

三种数据库共享相同的界面和业务流程，但连接参数、驱动、元数据读取、分页语法与标识符引用由各自的 Provider 处理。

第一版的完成标准：能够配置并测试连接、浏览数据库对象、查看表结构、预览表中少量数据，并执行受限制的只读查询，同时不会让界面卡死。

## 2. 第一阶段功能范围

建议包含：

- 新建、编辑和测试数据库连接
- 选择数据库类型
- 展示数据库或 Schema
- 展示表、视图和字段
- 双击表后预览前 100～200 行
- 输入并执行单条只读查询
- 显示查询结果、行数、耗时和错误摘要
- 取消耗时较长的查询
- 手动刷新数据

暂不包含：

- 新增、修改和删除数据
- 建表、删表或修改字段
- 存储过程管理
- 用户和权限管理
- 数据库备份与恢复
- 数据同步
- 同时支持不同数据库的所有高级语法

## 3. 推荐目录与文件

```text
项目根目录
├─ Forms
│  ├─ MainForm.cs                 主界面
│  ├─ ConnectionForm.cs           新建/编辑连接
│  └─ QueryForm.cs                查询页（也可先合并到主界面）
├─ Models
│  ├─ ConnectionProfile.cs        连接配置
│  ├─ DatabaseObjectInfo.cs       数据库对象通用信息
│  ├─ TableInfo.cs                表信息
│  ├─ ColumnInfo.cs               字段信息
│  ├─ QueryRequest.cs             查询请求
│  └─ QueryResult.cs              查询结果
├─ Services
│  ├─ IConnectionService.cs       连接服务契约
│  ├─ ConnectionService.cs        测试、保存、读取连接
│  ├─ ISchemaService.cs           元数据服务契约
│  ├─ IQueryService.cs            查询服务契约
│  └─ ExportService.cs            后续导出 CSV
├─ Data
│  ├─ IDatabaseProvider.cs        各数据库适配器的统一契约
│  ├─ DatabaseProviderFactory.cs  按数据库类型创建适配器
│  ├─ OracleProvider.cs           Oracle 特有实现
│  ├─ SqlServerProvider.cs        SQL Server 特有实现
│  └─ MySqlProvider.cs            MySQL 特有实现
├─ Configuration
│  ├─ AppSettings.cs
│  └─ ConnectionProfileStore.cs
├─ Utilities
│  ├─ ReadOnlySqlValidator.cs
│  ├─ ErrorMessageHelper.cs
│  └─ PagingHelper.cs
├─ docs
│  ├─ README.md
│  └─ project-plan.md
└─ Program.cs
```

不需要一次创建所有代码文件。建议每完成一项功能，再添加对应文件，避免只有目录和空类却不理解其用途。

## 4. 分层职责

### Forms：界面层

只负责收集输入、调用服务、展示结果和控制界面状态。按钮事件里不要直接编写某一种数据库的连接和查询逻辑。

典型职责：

- 读取文本框、下拉框中的连接参数
- 在查询期间禁用“执行”按钮
- 显示加载状态
- 将结果绑定到 `DataGridView`
- 将数据库对象绑定到 `TreeView`
- 显示用户可理解的错误信息

### Models：数据模型

模型用于在界面、服务和数据库适配器之间传递数据，不操作控件，也不执行 SQL。

`ConnectionProfile` 可以表达：

```text
DatabaseType
Host
Port
ServiceName / DatabaseName
UserName
Password
ConnectionTimeout
AuthenticationMode
```

Oracle 通常需要 Service Name 或 SID；SQL Server 通常需要 Database、实例名及 Windows/SQL Server 身份验证选项；MySQL 通常需要 Host、Port 和 Database。可以保留通用字段，并用数据库类型决定界面显示哪些专用字段。

### Services：业务流程

服务负责组织完整用例，例如测试连接、加载对象树或执行查询。它不应知道具体窗体控件。

```text
ConnectionService
  - 校验连接配置
  - 请求 Provider 测试连接
  - 保存/读取连接配置

SchemaService
  - 请求 Provider 获取数据库或 Schema
  - 获取表、视图、字段和主键信息
  - 转换成统一模型

QueryService
  - 检查只读限制
  - 设置超时、最大行数和取消令牌
  - 请求 Provider 执行查询
  - 记录耗时并返回统一结果
```

### Data：数据库适配层

这是多数据库设计的核心。界面和上层服务只依赖统一接口，各 Provider 处理数据库之间的差异。

统一接口思路：

```text
interface IDatabaseProvider
    TestConnectionAsync(profile)
    GetSchemasAsync(profile)
    GetTablesAsync(profile, schema)
    GetColumnsAsync(profile, schema, table)
    PreviewTableAsync(profile, table, maxRows, cancelToken)
    ExecuteQueryAsync(profile, request, cancelToken)
```

工厂选择适配器：

```text
CreateProvider(databaseType):
    if Oracle:
        return OracleProvider
    if SqlServer:
        return SqlServerProvider
    if MySql:
        return MySqlProvider
    otherwise:
        throw UnsupportedDatabaseType
```

每个 Provider 自己负责：

- 使用正确的数据库驱动
- 生成连接字符串
- 查询本数据库的 Schema、表和字段
- 处理标识符引用规则
- 生成“预览前 N 行”的数据库专用语句
- 将查询结果转换成统一的 `QueryResult`

不要试图用一条元数据 SQL 同时兼容三种数据库。

## 5. 关键业务流程伪代码

### 测试连接

```text
用户点击“测试连接”
    从界面建立 ConnectionProfile
    校验必填项
    provider = ProviderFactory.Create(profile.DatabaseType)
    result = await provider.TestConnectionAsync(profile)

    if result.Success
        显示“连接成功”及耗时
    else
        显示整理后的错误原因
```

### 加载对象树

```text
用户连接数据库
    显示加载状态
    provider = ProviderFactory.Create(profile.DatabaseType)
    schemas = await provider.GetSchemasAsync(profile)
    将 schemas 放入 TreeView

用户展开某个 Schema
    如果该节点尚未加载
        tables = await provider.GetTablesAsync(profile, schema)
        加入“表”和“视图”子节点
```

建议延迟加载树节点，不要建立连接后一次性读取所有表和字段。

### 预览表数据

```text
用户双击表节点
    取消上一次未完成的预览
    禁用重复执行
    provider = ProviderFactory.Create(profile.DatabaseType)
    result = await provider.PreviewTableAsync(profile, table, 200, cancelToken)
    将 result.Data 绑定到 DataGridView
    状态栏显示 result.RowCount 和 result.ElapsedTime
    恢复按钮状态
```

### 执行只读查询

```text
用户点击“执行”
    sql = 读取编辑区内容

    validation = ReadOnlySqlValidator.Validate(sql)
    if validation 不通过
        显示拒绝原因
        return

    request = QueryRequest(sql, maxRows, timeout)
    result = await QueryService.ExecuteAsync(profile, request, cancelToken)

    if result.Success
        显示结果、行数和耗时
    else
        显示数据库类型、错误摘要和建议检查项
```

## 6. 三种数据库的差异点

需要在 Provider 内隔离的主要差异包括：

| 内容 | Oracle | SQL Server | MySQL |
|---|---|---|---|
| 连接参数 | Host、Port、Service Name/SID | Server、实例、Database、认证方式 | Host、Port、Database、用户名、密码 |
| 常用默认端口 | 1521 | 1433 | 3306 |
| Schema 含义 | 通常与用户关系密切 | Database 下包含 Schema | Schema 通常与 Database 基本等价 |
| 分页/限制行数 | 依版本使用 FETCH 或其他方式 | TOP 或 OFFSET/FETCH | LIMIT / OFFSET |
| 标识符引用 | 双引号 | 方括号或双引号 | 反引号 |
| 元数据来源 | 系统数据字典视图 | 系统目录或 INFORMATION_SCHEMA | INFORMATION_SCHEMA |
| 驱动职责 | Oracle 的 .NET 数据提供程序 | SQL Server 的 .NET 数据提供程序 | MySQL 的 .NET 数据提供程序 |

MySQL 对象树建议采用“连接 → 数据库 → 表/视图 → 字段”的结构；SQL Server 可采用“连接 → 数据库 → Schema → 表/视图”；Oracle 可采用“连接 → Schema → 表/视图”。界面节点可以统一，但节点背后的对象类型不能简单视为完全相同。

表名、Schema 名等对象标识符通常不能像普通值一样使用 SQL 参数，因此预览表数据时应验证对象确实来自刚刚加载的元数据，并使用 Provider 的安全引用规则，不要直接拼接用户任意输入。

## 7. 安全与稳定性原则

- 第一版使用只读数据库账号。
- 默认仅允许单条 `SELECT` 查询。
- 禁止多语句和危险关键字只是辅助措施，不能替代数据库权限。
- 每次查询设置最大返回行数和超时时间。
- 使用异步数据库 API，避免 WinForms 界面卡死。
- 支持 `CancellationToken` 或数据库命令取消。
- 连接、命令和读取器都应及时释放。
- 值条件使用参数化查询。
- 密码不要硬编码进源代码或提交到版本控制。
- 保存密码功能可以延后；若必须保存，应使用操作系统提供的安全存储能力。
- UI 只显示整理后的错误摘要，详细异常写入本地日志。
- 定时刷新时，上一次查询没有结束就不要启动下一次。

## 8. 推荐开发顺序

### 里程碑 1：先支持一种数据库

优先选择你最熟悉、手边最容易测试的数据库：

1. 完成连接配置模型。
2. 完成单一 Provider。
3. 实现测试连接。
4. 实现表和字段浏览。
5. 实现前 200 行数据预览。

先验证架构是否顺手，再复制接口实现第二、第三种数据库。不要同时开发三个 Provider，否则排错难度会明显增加。

### 里程碑 2：只读查询

1. SQL 输入区域。
2. 单条只读查询校验。
3. 查询结果展示。
4. 超时、取消、最大行数。
5. 错误显示和日志。

### 里程碑 3：加入其余数据库

每增加一种数据库，都按同一组验收场景测试：

- 正确配置能连接
- 错误密码能得到明确提示
- 能加载 Schema、表和字段
- 能预览空表和有数据的表
- 中文、日期、NULL 和大字段不会导致界面崩溃
- 超时和取消有效

### 里程碑 4：改善体验

- 手动刷新
- 分页
- 保存常用查询
- 查询历史
- CSV 导出
- 多连接或多标签页

## 9. 后续可以补充的文档

随着开发推进，可以在 `docs` 中继续添加：

```text
requirements.md          明确需求和不做的功能
database-providers.md    三种数据库差异和驱动选择
ui-flow.md               界面流程与控件事件说明
testing-checklist.md     手动测试清单
decisions.md             重要技术选择及原因
```

三种数据库已经明确为 Oracle、SQL Server 和 MySQL。下一步建议先选择其中一种作为首个实现目标，并用统一验收场景验证 Provider 接口，再扩展到另外两种。
