# WinForms 多数据库查看工具：项目规划与架构思路

## 1. 项目定位

这是一个面向学习和实践的 C# WinForms 应用，目标是实现一个弱化版 Navicat/SQLyog。第一阶段只提供简单、安全的数据库浏览与查询能力，不追求完整的数据库管理功能。

项目目标的优先级如下：

1. 第一优先级：完成试用期要求，交付一个稳定、可演示的 MySQL 版本。
2. 第二优先级：通过亲自实现和讲解项目，系统学习 C#。
3. 后续目标：在首版可运行的基础上重构，逐步加入更规范的设计方法，而不是为了展示技术而提前增加复杂度。

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

### 里程碑 1：先只支持 MySQL

当前已经具备 MySQL 数据库和测试数据，因此第一阶段只实现 MySQL：

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

三种数据库已经明确为 Oracle、SQL Server 和 MySQL。当前先用已有的 MySQL 数据库和测试数据完成首个版本，并用统一验收场景验证 Provider 接口；MySQL 版本稳定后，再依次扩展 SQL Server 和 Oracle。

## 10. C# 技术学习地图

项目计划应写明具体 C# 技术，但不应把技术名词当成功能清单。判断某项技术是否需要加入的标准是：它是否解决当前问题、是否能由开发者解释、是否可以通过运行结果验证。

### 10.1 首版必须掌握的技术

这些技术直接关系到 MySQL 首版是否正确、稳定，必须边做边学：

| 技术 | 在项目中的使用位置 | 需要理解的重点 | 演示方式 |
|---|---|---|---|
| 类、对象、封装 | Models、Services、Provider | 数据和职责为什么要放在不同类中 | 指出一个模型、一项服务和一个 Provider 的职责 |
| 接口与多态 | `IDatabaseProvider`、`IConnectionService` | 上层为何依赖接口；接口不负责保存状态 | 说明未来替换数据库实现时哪些代码不需要改变 |
| 枚举 | `DatabaseType` | 为什么数据库类型不使用任意字符串 | 演示无效或未选择类型如何被校验 |
| 泛型集合 | 数据库、表、字段和结果集合 | `List<T>` 中 `T` 表示什么；何时返回集合 | 展示从数据库结果到界面列表的传递过程 |
| 资源释放 | 连接、命令、读取器 | `using`/`await using` 解决什么问题 | 重复连接或查询，说明资源为何不会一直占用 |
| 异常处理 | 连接失败、SQL 错误、取消操作 | `try/catch/finally` 的执行顺序；哪些异常应分别处理；不要吞掉异常 | 分别演示错误密码、数据库不可达和操作取消 |
| `async`/`await` | 测试连接、读取元数据、执行查询 | `Task` 表示尚未完成的操作；`await` 不等于新建线程；UI 线程为什么不能被阻塞 | 查询期间拖动窗口，证明界面仍可响应 |
| `CancellationToken` | 连接、预览和查询 | 取消是协作式通知；令牌如何从窗体传到服务和 Provider | 启动耗时操作后点击取消 |
| WinForms 事件 | 按钮点击、树节点展开、窗体关闭 | 事件发布者、订阅者和事件处理方法之间的关系 | 从“测试连接”按钮事件讲完整调用链 |
| 数据绑定 | `DataGridView`、`TreeView` | 数据对象和控件显示之间的边界 | 查询完成后展示结果和对象树 |

### 10.2 最小链路跑通后加入的进阶技术

这些技术纳入 4 周主计划，但必须先完成对应的最小业务链路，再通过重构加入，避免一边学习数据库流程、一边设计过多抽象：

| 技术 | 加入时机 | 在本项目中解决的问题 | 学习边界 |
|---|---|---|---|
| 构造函数依赖注入 | 测试连接链路跑通后 | 避免窗体和服务在内部随意 `new` 具体实现，使依赖可见、便于替换和测试 | 采用手工注入，不引入 DI 容器 |
| 委托 | 已理解 WinForms 事件之后 | 理解“把方法作为参数传递”，为事件、回调和可替换行为打基础 | 先做小练习，不强行在业务中使用 |
| 自定义事件 | 出现跨组件状态通知需求时 | 让查询服务或协调对象发布进度/完成消息，降低对具体窗体的依赖 | 若普通方法返回值足够，就不创建事件 |
| 策略模式 | MySQL 功能完成并准备扩展时 | 把数据库差异、只读校验或分页规则封装成可替换策略 | 现有 Provider 接口已经具备策略思想，通过整理和讲解展示，不重复造层 |
| 工厂模式完善 | 第 4 周架构整理时 | 集中选择对应 Provider | 保留简单工厂，不提前开发 SQL Server/Oracle |
| 日志抽象 | 基本错误处理完成后 | 区分用户提示与开发调试信息 | 首版先建立清晰错误分类，再决定日志框架 |
| 自动化测试 | 业务逻辑从窗体中分离后 | 验证配置校验、只读 SQL 校验和 Provider 选择等规则 | 数据库集成测试和普通单元测试分开 |

### 10.3 暂不作为目标的高级技术

首版不要求反射、表达式树、动态代理、复杂泛型设计、事件总线、完整 IoC 容器、CQRS 或微服务。这些技术不能直接提高当前交付质量，而且会显著增加初学阶段的理解成本。以后只有在出现明确问题时再评估。

## 11. 三周双数据库压缩交付计划

默认投入调整为每天 2～3 小时、每周 6～7 天，总周期 3 周。当前交付范围为 MySQL 和 SQLite；SQL Server、Oracle 放到双数据库版本稳定之后。SQLite 是文件数据库，连接参数、数据库层级和只读方式与 MySQL 不同，正适合用来验证 Provider 抽象是否真的有效。

三周目标：第 1 周完成双数据库连接、对象树和预览；第 2 周完成双数据库只读查询；第 3 周完成稳定性、架构整理、测试和演示。每天都要产生一个能运行或能验收的增量，不单独安排整天只学概念。

### 每天固定时间盒

- 15～20 分钟：确认当天用户场景、接口和失败路径。
- 90～120 分钟：亲自编码，一次完成一条垂直调用链。
- 30～40 分钟：构建并测试成功、失败和边界场景。
- 15～20 分钟：记录问题、提交代码并用自己的话复述调用链。

如果当天只有 2 小时，先保证功能和验收，整理工作放到下一天开头；如果有 3 小时，完成当天的扩展目标，但不提前引入无关框架。

### 第 1 周（8 月 10～16 日）：MySQL + SQLite 浏览链路

本周结果：两种数据库都能完成“选择类型 → 测试连接 → 加载数据库范围 → 浏览表和字段 → 预览前 200 行”。

知识重点：Provider 策略、不同数据库的连接参数、异步资源释放、元数据映射、TreeView 延迟加载和标识符安全引用。

- 8 月 10 日：已完成 MySQL 连接链路、结果模型、简单工厂和手工依赖注入的主要骨架。
- 8 月 11 日：修正 MySQL 连接结果问题；加入 SQLite 类型、文件路径配置、Provider 和工厂分支；两种数据库都通过连接成功/失败测试。
- 8 月 12 日：整理通用元数据接口；MySQL 加载数据库和表，SQLite 将 `main` 作为默认数据库范围并加载表。
- 8 月 13 日：两种 Provider 都能加载字段并映射为统一模型；明确 SQLite 没有 MySQL 式服务器数据库列表。
- 8 月 14 日：接通 TreeView 展开事件，按需加载数据库范围、表和字段，处理重复展开。
- 8 月 15 日：实现两种数据库的表预览前 200 行并绑定 `DataGridView`；标识符由 Provider 安全引用。
- 8 月 16 日：回归空表、中文、日期、NULL、错误路径、错误密码、重复点击和取消；保留第一个双数据库稳定版本。

第一周验收：切换数据库类型时使用同一套服务流程；MySQL 和 SQLite 的差异只出现在配置校验和各自 Provider；能够解释为何 SQLite 使用文件路径，以及为何 SQLite 的 `main` 可以映射成通用数据库范围。

### 第 2 周（8 月 17～23 日）：双数据库只读查询

本周结果：MySQL 和 SQLite 都能执行受限制的单条只读查询，显示结果、行数、耗时和明确错误，并能取消耗时操作。

知识重点：请求/结果模型、只读边界、最大行数、超时、取消传播、DataTable 绑定，以及数据库权限与 SQL 字符串校验的区别。

- 8 月 17 日：增加独立查询服务和最小只读校验流程，拒绝空 SQL、多语句和明显写操作。
- 8 月 18 日：完成 MySQL 查询链路，应用数据库选择、超时、最大行数和结果统计。
- 8 月 19 日：完成 SQLite 查询链路；连接以只读方式打开，复用相同的请求和结果模型。
- 8 月 20 日：接通执行、停止和重复执行状态；令牌从窗体传到服务和 Provider。
- 8 月 21 日：统一成功、SQL 错误、超时、取消和连接失败的用户提示；控件在所有出口恢复。
- 8 月 22 日：测试合法查询、非只读语句、错误 SQL、大结果集、NULL、中文、超时和取消。
- 8 月 23 日：回归连接、对象树、预览、查询完整链路并提交第二个稳定版本。

第二周验收：同一查询界面可在 MySQL 和 SQLite 间切换；能够说明字符串校验为什么不能替代 MySQL 只读账号，以及 SQLite 只读连接提供了哪一层额外保护。

### 第 3 周（8 月 24～30 日）：架构收口、测试与演示

本周结果：不再增加核心功能，集中消除错误成功状态、资源泄漏、UI 卡顿和重复逻辑，形成可交付、可讲解的双数据库版本。

知识重点：依赖倒置、策略模式、简单工厂、错误分类、单元测试与数据库集成测试的区别。

- 8 月 24 日：检查 Form 是否只负责输入、服务调用、数据绑定和控件状态；迁出残留数据库逻辑。
- 8 月 25 日：整理 `IDatabaseProvider`，确认 MySQL/SQLite 差异没有泄漏到服务层；删除无效或提前设计的字段。
- 8 月 26 日：整理工厂和程序入口组装；为未来新增 SQL Server Provider 写出扩展说明，但不实现 SQL Server。
- 8 月 27 日：为配置校验、Provider 选择和只读 SQL 规则补少量自动化测试；数据库行为使用集成测试清单。
- 8 月 28 日：处理可空警告、命名、用户错误提示和调试记录；不为了清零警告改变合理模型。
- 8 月 29 日：按最终演示顺序完整彩排 MySQL 和 SQLite，记录缺陷并修正高优先级问题。
- 8 月 30 日：冻结功能、整理 README、架构图、已知限制和 5～8 分钟讲解稿，形成试用期交付版本。

第三周验收：不用查看实现也能画出“Form → Service → Provider → Database”的调用链；能指出新增 SQL Server 时哪些代码不变、哪些连接和 SQL 规则必须单独实现。

### 8 月 11 日详细计划（2～3 小时）

今日必须结果：MySQL 和 SQLite 都能从同一测试连接流程得到正确的成功或失败结果。对象树和查询不是今天的必须项。

1. 0:00～0:25：修正现有连接正确性。无效 Profile 必须立即返回；不支持类型不得返回成功；错误原因不能写成与实际情况无关的“连接超时”。
2. 0:25～0:45：明确 SQLite 最小配置。通用字段保留 `ConnectionName`、`DatabaseType` 和超时；SQLite 增加数据库文件路径，不要求 Host、Port、用户名和密码。
3. 0:45～1:35：加入 SQLite Provider，只实现配置校验、连接字符串和异步测试连接；加入对应数据库驱动和工厂分支。
4. 1:35～2:10：让窗体根据用户选择创建 MySQL 或 SQLite Profile，并仍然只调用 `IConnectionService.TestConnectionAsync`。
5. 2:10～2:40：手工验收 MySQL 正确账号、错误密码，SQLite 有效文件、无效路径；再次构建并确认 0 错误。
6. 2:40～3:00（扩展）：若前五项全部通过，补可操作的连接取消入口并区分“取消”与普通失败；未通过则用于排错，不开始对象树。

今日建议的方法形状：

```text
ValidateProfile(profile):
    根据 DatabaseType 校验不同必填项

CreateProvider(databaseType):
    MySQL -> MySQL Provider
    SQLite -> SQLite Provider
    其他 -> 明确不支持

TestConnection(profile, token):
    校验配置
    选择 Provider
    异步测试
    返回成功、失败或取消结果
```

今日验收门槛：两种数据库各有一条成功证据和一条失败证据；UI 不直接创建数据库连接；切换到 SQLite 后不会要求填写 MySQL 专用参数。

### 后续数据库扩展顺序

1. 当前版本：MySQL + SQLite。
2. 双数据库版本稳定后：优先 SQL Server，因为其数据库/Schema 层级可以进一步验证元数据抽象。
3. Oracle 最后加入，单独处理 Service Name/SID、Schema 语义、标识符大小写、分页和驱动差异。

当前三周不得同时实现 SQL Server 和 Oracle，也不引入 DI 容器、事件总线或复杂插件系统。自定义事件、日志框架和高级设计模式只有在现有功能出现明确需求时再加入。

## 12. 每项功能的完整实施过程

以后无论实现测试连接、加载表还是执行查询，都使用以下过程，避免“代码能跑但自己讲不清”：

```text
明确用户场景和完成标准
    写出正常流程、失败流程和取消流程
    确认本次只需要学习的 C# 概念
    定义输入模型、输出模型和负责该流程的对象
    用伪代码写调用顺序
    从最小成功场景开始亲自实现
    编译并处理第一个错误，不同时改很多位置
    测试成功、失败、边界和取消场景
    记录问题的现象、原因、验证和修复思路
    用自己的话讲解设计和取舍
    完成验收后再进入下一项功能
```

每个功能开始前回答：

- 用户做了什么操作？
- 输入数据来自哪里，最终结果显示在哪里？
- 哪个对象负责组织流程，哪个对象负责访问数据库？
- 哪些地方可能失败、超时或被取消？
- 为什么此处选择接口、异步或事件；如果不用会发生什么？

每个功能结束后提交以下学习证据：

- 一段不超过一页的流程说明或伪代码。
- 至少一条调试记录。
- 成功、失败和边界场景的验收结果。
- 一段 3～5 分钟的口头讲解稿。
- 本周仍未理解的问题清单。

## 13. 理解检查与演示标准

“使用了某项技术”不等于“理解了某项技术”。一项技术只有同时满足以下条件，才算在项目中掌握：

1. 能不用术语堆砌，说明它解决了什么问题。
2. 能指出项目中的具体使用位置和调用链。
3. 能说明不使用它会带来什么后果。
4. 能演示正常场景和至少一个异常或边界场景。
5. 能对代码做一个小改动，并预测影响范围。

首版最终演示建议按以下顺序进行：

1. 说明项目目标、首版范围和分层结构。
2. 配置并测试 MySQL 连接。
3. 展开数据库、表和字段。
4. 预览普通表、空表以及包含中文/NULL 的数据。
5. 执行合法只读查询并展示行数和耗时。
6. 演示错误密码、错误 SQL 和取消耗时查询。
7. 选择一条调用链，从 UI 事件讲到 Provider，再讲结果如何返回 UI。
8. 说明当前限制、下一步重构计划以及未来如何扩展 SQL Server/Oracle。

## 14. 实施原则与学习边界

- 先完成可工作的 MySQL 垂直流程，再追求设计完整度。
- 每次只引入一个新的主要概念，并通过当前功能验证它。
- 首版必须学好异常处理、异步和取消；依赖注入、事件和策略重构在最小业务链路跑通后，于第 1～4 周逐步加入，不能早于它们要解决的问题。
- 不因为“高级”就使用某项技术，也不为了套设计模式增加只有一个实现的无意义接口。
- UI 事件处理方法只组织界面行为，不直接堆积连接、元数据和查询实现。
- 遇到问题时先记录和定位，再修改；一次只验证一个原因。
- 可以请求代码审查、概念解释、伪代码和调试提示，但核心业务实现由学习者亲自完成并讲解。
- 每周结束只进入下一阶段的条件是：功能通过验收，并且能够回答本周的理解检查问题。
