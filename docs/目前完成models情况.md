目前的 Models 已经足够支撑第一阶段的 MySQL 最小闭环：

```text
界面收集连接信息
→ ConnectionProfile
→ 发起 QueryRequest
→ 数据服务执行 SQL
→ 返回 QueryResult
→ DataTable 绑定 DataGridView
```

从“先连接 MySQL 并返回查询结果”的目标看，模型层大约完成了 70%。项目构建成功，0 个错误，但有 21 个警告，其中 20 个属于非空属性未初始化。

## 做得比较好的地方

- 已补充 `QueryRequest` 和 `QueryResult`，查询流程基本闭环。
- `QueryResult.Rows` 使用 `DataTable`，很适合第一版直接绑定 `DataGridView`。
- `TableInfo` 已经从空类扩充为具有实际用途的 MySQL 表信息模型。
- 正确地把部分可能缺失的值声明为可空，例如注释、默认值、字符集。
- 命名有所改善：`IsComplete`、`SchemaName`、`IsSystemObject` 已符合常见 C# 规范。
- 当前没有过早引入复杂的继承、泛型仓储或数据库抽象，适合初学阶段。

## 现在必须优先考虑的问题

### 1. 非空属性没有初始化

由于项目启用了可空引用类型，构建报告了大量 `CS8618` 警告。

例如：

- [ConnectionProfile.cs](E:/试用期项目实战/c%23lan/Models/ConnectionProfile.cs:9)
- [QueryRequest.cs](E:/试用期项目实战/c%23lan/Models/QueryRequest.cs:12)
- [TableInfo.cs](E:/试用期项目实战/c%23lan/Models/TableInfo.cs:9)
- [ColumnInfo.cs](E:/试用期项目实战/c%23lan/Models/ColumnInfo.cs:9)

不要简单地把所有属性都改成可空。应该逐项分类：

```text
创建对象时必须提供：
    连接名称、主机、数据库类型、SQL 文本、表名、字段名

数据库可能不返回：
    注释、默认值、字符集、排序规则、创建时间

由程序提供默认值：
    MySQL 端口、连接超时、查询超时
```

你的目标应该是：创建出来的模型尽量处于可用状态，而不是允许所有内容为空。

### 2. `ConnectionId` 暂时没有来源

[QueryRequest.cs](E:/试用期项目实战/c%23lan/Models/QueryRequest.cs:9) 有 `ConnectionId`，但 [ConnectionProfile.cs](E:/试用期项目实战/c%23lan/Models/ConnectionProfile.cs:7) 没有对应的 `Id`。

你需要选择一种流程：

```text
简单版本：
    查询服务直接接收当前 ConnectionProfile
    QueryRequest 只保存 SQL 和查询选项

连接管理版本：
    ConnectionProfile 有唯一 Id
    查询请求通过 ConnectionId 找到已保存连接
```

第一阶段只有一个当前连接时，前一种思路更容易理解。等你开始保存多条连接配置，再引入 ID。

### 3. `IsReadOnly` 不应完全相信请求提供的值

[QueryRequest.cs](E:/试用期项目实战/c%23lan/Models/QueryRequest.cs:14) 中的 `IsReadOnly` 容易产生语义问题：

```text
用户传入：
    IsReadOnly = true

实际 SQL：
    可能仍然是 DELETE 或 UPDATE
```

只读性应该由查询服务根据 SQL 和数据库账号权限判断，而不是由请求自己声明。模型可以表达“请求只读执行”，但不能把它当成 SQL 已经安全的证明。

### 4. `QueryResult` 存在重复信息

[QueryResult.cs](E:/试用期项目实战/c%23lan/Models/QueryResult.cs:11) 同时包含：

- `Columns`
- `Rows`，类型为 `DataTable`

但 `DataTable` 自身已经具有列信息。

第一版要决定：

```text
方案一：
    DataTable 负责列和行
    直接绑定 DataGridView
    最适合当前目标

方案二：
    Columns 保存数据库元数据
    Rows 保存展示数据
    适合以后显示字段类型、主键等额外信息
```

如果 `Columns` 只是重复 `DataTable.Columns`，第一版可以不急着使用。若它表示查询结果以外的详细数据库元数据，就需要明确这个差异。

### 5. `ExecutionTime` 缺少单位

[QueryResult.cs](E:/试用期项目实战/c%23lan/Models/QueryResult.cs:14) 的 `ExecutionTime` 无法看出是秒还是毫秒。

思路上可以：

- 在名称中明确单位；或者
- 使用能够表达时间长度的类型。

对于界面状态栏，毫秒通常更直观。

## `TableInfo` 当前评价

[TableInfo.cs](E:/试用期项目实战/c%23lan/Models/TableInfo.cs:7) 目前偏向 MySQL，这是符合你现阶段目标的，不必为了 Oracle 和 SQL Server 立即抽象。

现有字段可以分为三组：

```text
基本身份：
    TableName
    DatabaseName
    ObjectType

说明信息：
    Comment
    Engine
    Collation
    CreateTime

统计信息：
    ColumnCount
    RowCount
    DataLength
    IndexLength
```

这个结构作为 MySQL 表总览已经比较完整。不过还需注意以下问题。

### `CreateTime` 不适合长期使用字符串

创建时间本质上是时间，不是普通文本。字符串会带来：

- 排序容易出错
- 格式不统一
- 时区和空值难以表达
- 后续计算不方便

而且部分对象可能没有创建时间，因此它还可能需要允许“没有值”。

### 统计信息不一定精确，也不一定存在

MySQL 返回的 `RowCount` 对某些存储引擎可能只是估算值。视图的行数、数据长度、索引长度也可能没有意义。

因此界面不要把它表达成绝对准确的“总行数”。可以把这些字段理解为元数据统计，而不是执行 `COUNT(*)` 得到的精确结果。

同时思考：

```text
数据库没有返回统计值：
    0 是真实的零？
    还是未知？
```

如果需要区分，就应该允许“未知”。

### `ObjectType` 和 `IsView()` 容易受大小写影响

[TableInfo.cs](E:/试用期项目实战/c%23lan/Models/TableInfo.cs:23) 只在 `ObjectType` 完全等于 `"View"` 时返回真。

MySQL 元数据实际可能返回不同形式，例如大写文本。普通字符串也可能被写成：

```text
View
VIEW
view
Table
BASE TABLE
```

建议尽早统一 Provider 映射规则：

```text
读取 MySQL 原始类型
→ Provider 转换成程序内部统一对象类型
→ TableInfo 只保存统一结果
```

以后可考虑使用枚举，但第一版也可以先规定固定字符串，只要不要让数据库原始值到处传播。

### `TableInfo` 还没有字段集合

如果它仅用于“表列表总览”，目前这样可以。

如果它代表“完整的表详情”，后续才需要加入：

```text
Columns
    包含该表的 ColumnInfo 集合
```

建议现在先把它定位为表列表模型，不要为了完整而立刻加载每张表的所有字段。用户选中表后，再单独查询字段信息。

## 其他可以后置的问题

### `ColumnInfo`

[ColumnInfo.cs](E:/试用期项目实战/c%23lan/Models/ColumnInfo.cs:23) 的 `IsNormalColumn` 仍然存在：

- 参数没有使用。
- 主键仍然可能是普通可查询字段。
- 方法名称表达不出真实用途。

当前查询结果展示暂时不需要它，可以等真正出现“判断字段能否编辑或插入”的需求时再定义。

另外 `Collation` 和 `CharacterSet` 一样，并非所有字段都有，应重新判断是否允许为空。

### `DatabaseObjectInfo`

[DatabaseObjectInfo.cs](E:/试用期项目实战/c%23lan/Models/DatabaseObjectInfo.cs:7) 与 `TableInfo` 有部分重复。

现阶段可以这样区分：

```text
DatabaseObjectInfo：
    TreeView 中的轻量节点
    表示表、视图等不同对象

TableInfo：
    选中表后展示的 MySQL 表详情
```

只要职责如此明确，重复少量身份字段是可以接受的。

### 项目依赖警告

构建还提示 `System.Data.Common` 在当前目标框架中已经自动可用，不需要额外引用。这不是 Models 的问题，也不阻碍当前进度，可以在以后整理项目依赖时处理。

## 推荐的下一步顺序

1. 先处理 Models 中“必填还是可空”的设计，消除有意义的可空警告。
2. 决定查询服务使用 `ConnectionId` 还是直接使用 `ConnectionProfile`。
3. 明确 `QueryResult.Columns` 与 `DataTable.Columns` 是否重复。
4. 明确执行时间单位。
5. 统一表/视图类型的内部表示。
6. 暂时保留 `TableInfo` 的 MySQL 特征，不急着做三数据库通用化。
7. 接下来设计 MySQL 连接服务的职责和调用流程。
8. 你自行完成后，可以再让我按同一边界审阅连接服务。

目前最重要的不是继续增加模型，而是用现有模型真正走通一次：**连接 MySQL → 执行简单 SELECT → 得到 `QueryResult` → 将 `DataTable` 显示到界面。**