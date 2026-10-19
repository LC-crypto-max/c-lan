-- SQLite 专用：先扫描 SELFTEST-20261019-001 入站，再执行本脚本。
-- 时间即时生成，使用服务端现有的Asia/Shanghai时间口径。
-- 故意将SNCode设为NULL，验证服务器能从扫码记录补SN。
-- 每执行一次追加3条结果，不删除旧数据、不固定rowid。
INSERT INTO ElectricCheck (
    TestDateTime, LightName, TestItemName,
    HighLimit, LowerLimit, TestValue, TestResult, LightType,
    TestID, TemplateItemID, TemplateItemName, Remark, SNCode
)
VALUES
    (strftime('%Y-%m-%d %H:%M:%f','now','+8 hours'), 'SELFTEST_LAMP', 'Voltage',
     '14', '10', '12', 'OK', 'SELFTEST', 'SELFTEST-20261019-001', 1, 'Voltage', 'Manual self-test', NULL),
    (strftime('%Y-%m-%d %H:%M:%f','now','+8 hours'), 'SELFTEST_LAMP', 'Current',
     '2', '0.1', '1', 'OK', 'SELFTEST', 'SELFTEST-20261019-001', 2, 'Current', 'Manual self-test', NULL),
    (strftime('%Y-%m-%d %H:%M:%f','now','+8 hours'), 'SELFTEST_LAMP', 'Power',
     '20', '1', '12', 'OK', 'SELFTEST', 'SELFTEST-20261019-001', 3, 'Power', 'Manual self-test', NULL);
