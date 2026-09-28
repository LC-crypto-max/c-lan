# main 多数据库演示版实施计划

目标：迁移 prod 的 WinForms 设计器布局，保留纯数据库浏览用途，补齐四种数据库。

1. 迁移 Form1.Designer.cs 和资源，删除同步、托盘及服务端相关控件；所有固定控件留在设计器。
2. 扩展已有 Provider 接口的 Schema 参数，加入 SQL Server、Oracle 驱动、连接校验、元数据、预览和查询。
3. 修正只读 SQL 校验、重复列名、SQLite 后台执行与取消、MySQL SSL 配置。
4. 完成连接选择、新建、刷新、CSV 导出和一致的忙碌状态；密码使用 Windows 当前用户加密保存，配置原子写入。
5. 扩展现有 Checks 控制台检查，验证真实 SQLite 查询、四种 Provider、只读限制及界面布局；构建并生成演示说明。

验收：dotnet build；dotnet run --project Checks；窗体在不同尺寸下布局正常。外部数据库若没有可用测试实例，明确标注尚未实机联调。

## 实施结果

已完成上述 5 项。真实 SQLite 查询及 WinForms 消息循环下的连接、展开、预览、执行流程通过；外部数据库实机检查只在存在已保存连接时运行。
