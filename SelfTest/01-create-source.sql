-- SQLite 专用：初始化独立测试源库。不要对服务器 MySQL 执行。
-- 本脚本只创建空表，不产生检测结果。SN入站后再执行02脚本。
CREATE TABLE IF NOT EXISTS ElectricCheck (
    TestDateTime TEXT NOT NULL,
    LightName TEXT NOT NULL,
    TestItemName TEXT NOT NULL,
    HighLimit TEXT,
    LowerLimit TEXT,
    TestValue TEXT,
    TestResult TEXT NOT NULL,
    LightType TEXT NOT NULL,
    TestID TEXT NOT NULL,
    TemplateItemID INTEGER NOT NULL DEFAULT 0,
    TemplateItemName TEXT,
    Remark TEXT,
    SNCode TEXT
);
