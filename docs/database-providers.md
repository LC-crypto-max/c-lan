# Oracle、SQL Server、MySQL 适配思路

## 1. 设计目标

三种数据库在上层看起来应具有相同的基本能力：

- 测试连接
- 列出可访问的数据库或 Schema
- 列出表和视图
- 查看字段信息
- 预览指定表的少量数据
- 执行受限制的只读查询

差异全部收敛到 `IDatabaseProvider` 的三个实现中：

```text
IDatabaseProvider
├─ OracleProvider
├─ SqlServerProvider
└─ MySqlProvider
```

建议先把共同能力做少，不要为了统一接口而强迫所有数据库支持并不存在或含义不同的功能。

## 2. 数据库类型与连接配置

数据库类型可用枚举表达：

```text
DatabaseType
    Oracle
    SqlServer
    MySql
```

通用连接模型思路：

```text
ConnectionProfile
    Id
    DisplayName
    DatabaseType
    Host
    Port
    DatabaseName
    ServiceName
    UserName
    Password
    AuthenticationMode
    ConnectionTimeoutSeconds
```

字段使用规则：

| 字段 | Oracle | SQL Server | MySQL |
|---|---|---|---|
| Host | 需要 | 需要 | 需要 |
| Port | 常用 1521 | 常用 1433 | 常用 3306 |
| DatabaseName | 通常不用作主要连接字段 | 可选或需要 | 可选或需要 |
| ServiceName | Service Name 或 SID | 不使用 | 不使用 |
| AuthenticationMode | 通常用户名/密码 | Windows 或用户名/密码 | 通常用户名/密码 |

`ConnectionForm` 应按数据库类型动态显示相关输入项。例如选择 MySQL 时隐藏 Service Name 和 Windows 身份验证，显示 Host、Port、Database、UserName、Password。

不要让窗体直接拼接连接字符串。窗体只建立 `ConnectionProfile`，具体 Provider 再根据它创建连接。

## 3. Provider 接口

第一版可以采用以下职责：

```text
interface IDatabaseProvider
    DatabaseType SupportedType

    ValidateProfile(profile)
    TestConnectionAsync(profile, cancelToken)
    GetCatalogsAsync(profile, cancelToken)
    GetSchemasAsync(profile, catalog, cancelToken)
    GetObjectsAsync(profile, catalog, schema, cancelToken)
    GetColumnsAsync(profile, objectName, cancelToken)
    PreviewTableAsync(profile, objectName, maxRows, cancelToken)
    ExecuteQueryAsync(profile, request, cancelToken)
```

这里同时保留 Catalog 和 Schema，是因为三种数据库对这两个概念的使用方式不同。返回模型中可以允许其中一个为空，不要在界面层猜测其含义。

对象标识模型：

```text
DatabaseObjectName
    Catalog
    Schema
    Name
    ObjectType      // Table 或 View
```

## 4. Provider 工厂

```text
CreateProvider(type):
    switch type
        Oracle:
            return OracleProvider
        SqlServer:
            return SqlServerProvider
        MySql:
            return MySqlProvider
        default:
            throw UnsupportedDatabaseType
```

第一版直接使用 `switch` 足够清晰，不必为了“高级架构”立即引入依赖注入容器。

## 5. OracleProvider 思路

### 连接

主要输入通常包括：

```text
Host
Port
ServiceName 或 SID
UserName
Password
```

伪代码：

```text
BuildConnection(profile):
    验证 Host、Port、ServiceName、UserName
    根据 Service Name 或 SID 模式组织数据源
    使用 Oracle 数据提供程序创建连接
    return connection
```

### 元数据

只读账号能看见哪些对象取决于权限。第一阶段建议只显示当前用户有权访问的 Schema、表和视图，并把“没有权限”与“对象不存在”区分开。

### 数据预览

```text
PreviewTable(objectName, maxRows):
    验证 objectName 来自已加载元数据
    quotedName = 按 Oracle 规则引用 Schema 和表名
    sql = 生成仅返回 maxRows 行的 Oracle 查询
    执行并转换为 QueryResult
```

注意不同 Oracle 版本的限制行数语法可能不同。可以在第一版规定最低支持版本，避免同时维护多套分页逻辑。

## 6. SqlServerProvider 思路

### 连接

连接方式至少需要考虑：

- SQL Server 用户名/密码
- Windows 身份验证（可作为后续能力）
- Server、实例名、端口与 Database
- 是否信任服务器证书等开发环境选项

伪代码：

```text
BuildConnection(profile):
    验证 Server 信息
    if AuthenticationMode == Windows:
        使用集成身份验证配置
    else:
        验证 UserName 和 Password
        使用 SQL Server 身份验证配置
    使用 SQL Server 数据提供程序创建连接
```

不要为了让连接“先成功”而默认关闭所有加密或证书验证。开发环境所需选项应清楚显示，并与生产连接配置区分。

### 对象层次

推荐层次：

```text
连接
└─ Database
   └─ Schema
      ├─ Tables
      └─ Views
```

### 数据预览

第一版只需生成限制行数的查询。后续真正分页时，需要稳定排序；如果表没有主键或唯一排序字段，应提示分页结果可能不稳定。

## 7. MySqlProvider 思路

### 连接

主要输入：

```text
Host
Port（常用 3306）
Database（可以允许测试服务器连接时留空）
UserName
Password
ConnectionTimeout
```

伪代码：

```text
BuildConnection(profile):
    验证 Host、Port、UserName
    DatabaseName 可选
    使用 MySQL 数据提供程序创建连接
    return connection
```

如果 Database 为空，测试连接成功后可以加载该账号有权访问的数据库列表；若已指定 Database，则进入连接后优先选中它。

### 对象层次

MySQL 中 Schema 通常与 Database 基本等价，界面建议显示：

```text
连接
└─ Database
   ├─ Tables
   └─ Views
```

上层模型仍可保留 Schema 字段，但 MySQL Provider 应明确自己的映射规则，避免界面代码对 MySQL 写特殊判断。

### 元数据

可以通过 MySQL 的元数据集合或 `INFORMATION_SCHEMA` 获取：

- 当前账号可见的数据库
- 表和视图
- 字段名、数据类型、可空性、默认值
- 主键信息

元数据查询条件也要使用参数，不要直接把 Database 名拼进普通筛选条件。

### 数据预览与分页

```text
PreviewTable(objectName, maxRows):
    验证对象来自已加载元数据
    使用反引号安全引用 Database 和表名
    sql = SELECT ... LIMIT maxRows
    执行并返回 QueryResult
```

后续分页思路：

```text
SELECT ...
FROM 已安全引用的表
ORDER BY 稳定且唯一的字段
LIMIT pageSize OFFSET pageIndex * pageSize
```

没有稳定排序时，翻页可能出现重复或遗漏，应在产品设计中说明。

## 8. 查询结果统一模型

三种 Provider 应返回同一种结果结构：

```text
QueryResult
    Success
    Columns
    Rows 或 DataTable
    RowCount
    ElapsedMilliseconds
    WasTruncated
    ErrorCategory
    UserMessage
    TechnicalMessage
```

`WasTruncated` 用来提示用户结果达到最大行数，并不代表数据库中只有这些数据。

错误分类可以统一为：

```text
ConfigurationError
AuthenticationError
NetworkError
PermissionError
TimeoutError
SqlSyntaxError
Cancelled
Unknown
```

底层数据库的错误编号和异常类型由 Provider 转换为统一分类，界面只负责展示。

## 9. 只读限制

仅检查 SQL 是否以 `SELECT` 开头并不可靠。第一版建议同时使用多层保护：

1. 为三种数据库分别创建只读账号。
2. 界面只开放查询入口。
3. 默认只接受单条查询语句。
4. 拒绝明显的写入、结构变更和管理语句。
5. 设置查询超时与最大返回行数。
6. 不允许用户在表预览功能中直接提供任意对象名。

SQL 文本校验是减少误操作的辅助措施，数据库账号权限才是最终边界。

## 10. 推荐实现顺序

推荐按以下顺序逐个完成 Provider：

```text
SQL Server 或 MySQL（选择本地最方便测试的一个）
    ↓ 验证统一接口与界面流程
另一个常用数据库
    ↓ 修正 Catalog/Schema 抽象
Oracle
    ↓ 处理 Oracle 专有连接和元数据差异
三库统一回归测试
```

不要同时写三个 Provider。每完成一个，都先通过测试清单，再开始下一个。

## 11. 每个 Provider 的验收清单

- 正确配置可以连接。
- 错误主机、端口、用户名和密码分别能得到可理解的提示。
- 可以加载该账号有权限查看的对象。
- 表和视图能够区分。
- 字段类型、NULL、默认值和主键信息基本正确。
- 空表可以正常显示。
- 中文、日期、NULL、二进制和大文本不会让界面崩溃。
- 预览结果不会超过最大行数。
- 查询超时和用户取消有效。
- 连接、命令和读取器在成功、失败、取消时都能释放。
- 无权限访问时不会被误报为程序崩溃。
