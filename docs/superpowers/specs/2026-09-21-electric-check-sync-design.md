# ElectricCheck 设备同步设计

## 目标

在现有 WinForms 数据库浏览器中增加电检数据同步能力：设备端读取可配置的 SQLite 文件，将 `ElectricCheck` 表的新记录转换为服务端已有批量 DTO，通过 HTTP POST 发送到 `172.16.28.64:8080`；程序关闭窗口后进入系统托盘并继续同步。

## 已确认范围

- 设备系统：Windows。
- 客户端：现有 C# WinForms 程序。
- 服务端：现有 Java `ElectricCheckController`/`ElectricCheckService` 接口。
- 接口：`POST /openapi/electric-check/records:batch`。
- 同步模式：自动轮询，另提供手动“立即同步”。
- 窗口关闭：隐藏到系统托盘，不退出进程；托盘菜单可显示窗口、立即同步、退出程序。
- 暂不引入 Windows Service、消息队列或独立同步进程。

## 数据流

```text
SQLite ElectricCheck
        |
        | rowid > saved cursor, limit 200
        v
C# mapper -> JSON batch -> HttpClient POST
        |
        | HTTP success only
        v
persist cursor -> next batch / next poll
```

轮询使用 `PeriodicTimer` 或等价的可取消后台循环，不使用 `FileSystemWatcher`。SQLite 写入可能触发多次文件事件且文件仍被占用，按 rowid 查询更可靠；没有新 rowid 时不产生网络请求。

## 客户端设计

### 配置

新增同步设置（存放在现有配置机制可复用的位置，避免散落注册表）：

- `Enabled`
- `DeviceNo`
- `SqliteFilePath`
- `TableName`，默认 `ElectricCheck`
- `ServerBaseUrl`，默认 `http://172.16.28.64:8080`
- `PollIntervalSeconds`，默认 5，限制为正数
- `BatchSize`，默认 200，最大 200

同步游标按“设备 + SQLite 文件路径 + 表名”保存，至少包含 `LastRowId`、最后成功时间和最后错误信息。只有整个 HTTP 请求成功后才推进游标；失败时保留原游标，下一轮重试。

### 读取与映射

使用项目已有 `Microsoft.Data.Sqlite` 和只读连接。每次批次读取：

```sql
SELECT rowid, TestDateTime, LightName, TestItemName, HighLimit,
       LowerLimit, TestValue, TestResult, LightType, TestID,
       TemplateItemID, TemplateItemName, Remark, SNCode
FROM "ElectricCheck"
WHERE rowid > $lastRowId
ORDER BY rowid ASC
LIMIT $batchSize
```

如果现场表存在显式整数主键，应优先使用该主键作为 `sourceRowId`；否则使用 SQLite `rowid`。表名和列名必须通过固定白名单/安全标识符引用，不接受任意 SQL 文本。

映射规则：

- `sourceRowId` 为主键或 rowid。
- 服务端必填文本字段在客户端校验非空。
- 空字符串转换为 `null`。
- `TemplateItemID` 的数字值转换为整数；示例中的 `缺省` 等非数字值转换为 `0`，因为服务端字段类型是 `Integer`。
- 时间按 ISO 8601/服务端 Jackson 可解析格式发送，保留毫秒。

请求体使用已有 DTO：

```json
{
  "requestId": "device-001-1001-1200",
  "deviceNo": "device-001",
  "records": [
    {
      "sourceRowId": 1001,
      "testDateTime": "2026-07-11T16:29:03.093Z",
      "lightName": "零件",
      "testItemName": "零件检测",
      "highLimit": null,
      "lowerLimit": null,
      "testValue": "True",
      "testResult": "OK",
      "lightType": "V32前位灯ISD-R",
      "testId": "20260711162903",
      "templateItemId": 9999,
      "templateItemName": "零件检测",
      "remark": null,
      "snCode": null
    }
  ]
}
```

`requestId` 由设备号和批次首尾 rowid 生成，便于日志定位；它不是唯一性判断的唯一依据。

### 后台生命周期

- 窗体加载配置后，若启用自动同步则启动一个同步循环。
- 循环通过单次锁保证手动同步与自动同步不会并发读写游标或重复上传。
- 窗体关闭事件改为取消关闭并隐藏到托盘；托盘“退出”才真正停止循环、保存配置并释放资源。
- 所有网络、SQLite、JSON 异常记录到状态栏/日志，不阻塞 UI 线程。
- HTTP 超时和临时网络错误在下一轮重试；服务端 4xx 校验错误显示为需要修配置的错误，不快速无限重试。

### 界面

在现有 WinForms 中增加同步区域：

- 自动同步复选框
- 设备编号、SQLite 文件、服务端地址、轮询间隔
- “立即同步”按钮
- 当前状态、最后成功时间、最后游标、错误信息
- 系统托盘图标及“显示窗口 / 立即同步 / 退出”菜单

## 服务端调整

现有 Controller、DTO、Service、Mapper 已覆盖接收流程，不新增接口。需要确认并补充数据库唯一约束：

```sql
UNIQUE KEY uk_electric_check_device_source (device_no, source_row_id)
```

这是现有 `ON DUPLICATE KEY UPDATE id = id` 真正生效的前提，用于设备重试时避免重复插入。若现场业务允许同一设备重复使用 source rowid，则唯一键需要改为设备编号、测试 ID 和 source rowid 的组合，并在实现前明确规则。

## 错误与安全边界

- SQLite 以只读模式打开，不修改设备原始数据库。
- 不接受任意用户 SQL；只允许固定表和列的增量查询。
- URL、设备号、文件路径在发送前验证。
- JSON 请求按 200 条以内分批，避免单次请求过大。
- 服务器地址目前是内网 HTTP；如果跨网段或存在敏感数据，应在部署前增加 HTTPS/认证，但不在第一版引入自定义认证协议。

## 验证

最小验证集：

1. 使用 `ElectricCheck.sql` 导入测试 SQLite，能读取并转换第一批记录。
2. `TemplateItemID='缺省'` 不导致序列化或服务端反序列化失败。
3. 服务端不可达时游标不前进，恢复网络后继续发送。
4. 重复发送同一批次不会产生重复行。
5. 新增一条 SQLite 记录后下一轮自动发现并上传。
6. 手动同步与自动同步同时触发时不会并发重复上传。
7. 关闭窗口后托盘继续同步，选择“退出”后循环停止。

## 第一版不做

- Windows Service 安装器。
- 消息队列、断点分片和多服务器路由。
- 服务端实时推送。
- 任意 SQLite 表的自动推断映射。

