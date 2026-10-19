# 手工验证：扫码、插入结果、一分钟静默后自动出站

在本目录新建 `electric-check-selftest.db`，用SQLite管理工具执行 `01-create-source.sql` 创建空表；本机已有同名空白测试库时可以直接使用。测试数据库不提交Git，脚本可用于在新电脑重建。

测试设备号：`EC-SELFTEST-20261019-01`。

测试SN：`SELFTEST-20261019-001`。

使用已部署扫码接口及新表的E-XMOM测试服务器。当前没有未完成产品时，将C#配置切换为以上测试设备号、测试服务器和本目录SQLite文件。不要用生产设备编号上传这份测试数据，也不要覆盖生产SQLite文件。

## 正常出站

1. 打开C#的“产品扫码”页签，输入测试SN，回车或点击“扫码入站”。看到“已入站”后再插入数据。
2. 在同一程序的SQLite查询区域选择测试库，关闭“只读”选项，执行 `02-insert-results.sql` 的INSERT，然后切回“产品扫码”页签观察。也可以使用SQLite管理工具，对同一个文件执行该脚本。
3. 保持程序运行，不再写入结果。第一次自动检查读到3条时，界面显示“检测中”。从这次观察开始满一分钟，下一次检查结果仍相同时，程序自动上传本件全部明细、调用出站接口、补SN，并显示“已出站”。无需点击“检查 / 重试”，没有人工确认步骤。
4. 源SQLite中SNCode仍为空，这是预期行为；程序只读源库，补SN发生在服务器electric_check表中。

程序每60秒检查一次，计时从程序观察到最后一次变化开始。因此最后一次手动插入到界面出站通常需要约60–120秒，加上接口耗时，并非插入后精确第60秒出站。

## 服务器验收（MySQL，只读）

```sql
SELECT id, scan_id, device_no, sn_code, status,
       scan_time, out_time, record_count
FROM electric_check_scan
WHERE device_no = 'EC-SELFTEST-20261019-01'
  AND sn_code = 'SELFTEST-20261019-001'
ORDER BY id DESC;

SELECT s.scan_id, e.source_row_id, e.test_item_name, e.test_value,
       e.sn_code
FROM electric_check e
JOIN electric_check_scan s
  ON e.device_no = s.device_no
 AND e.test_datetime >= s.scan_time AND e.test_datetime <= s.out_time
 AND e.source_row_id > s.source_row_id_before_scan
 AND e.source_row_id <= s.last_source_row_id
WHERE s.device_no = 'EC-SELFTEST-20261019-01'
  AND s.sn_code = 'SELFTEST-20261019-001'
  AND s.status = 'COMPLETED'
ORDER BY e.source_row_id;
```

预期：一条入站记录为COMPLETED，out_time有值，record_count=3；三条明细都补上同一个测试SN，并能按该出站记录的时间与行号范围查询。scan_id仅保存在出入站表，electric_check表结构保持不变。界面允许扫描下一件。

## 验证持续产生结果时不会提前出站

在新的检测中，插入第一批后，每隔约20秒再执行一次02脚本，直到界面观察到新增结果。持续插入时保持“检测中”；停止插入后，程序在观察到最终数据稳定一分钟后自动出站。02脚本每次追加3条，最终明细条数应与执行次数相符。

整个过程都不插入任何记录时，产品会一直等待首条结果，不会空数据出站。读取失败或网络失败也不会被当成“没有新结果”。

## 再次测试

- 复用同一个SQLite文件时，保留已有数据，下一件使用新SN，例如 `SELFTEST-20261019-002`；扫码前行号会自动排除上一件数据。02脚本中的TestID可以重复，不作为扫码SN的关联依据。
- 如果删除并重建测试库，须同时更换未使用过的测试设备号（如末尾改为02），防止源行号归零后与服务器旧记录冲突。
- 初始化SQL是01脚本，仅在准备新空库时执行；不要在入站前执行02脚本，否则这些数据会被视为历史数据。
