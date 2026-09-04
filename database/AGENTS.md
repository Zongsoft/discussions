## 概述

本目录遵循 [../AGENTS.md](../AGENTS.md)，保存 Discussions 数据模型的多数据库建库脚本。运行时数据映射位于 [../src/Zongsoft.Discussions.mapping](../src/Zongsoft.Discussions.mapping)。

## 职责边界

- `Zongsoft.Discussions-mssql.sql`、`-mysql.sql`、`-postgres.sql` 定义事务型论坛数据结构。
- `Zongsoft.Discussions-clickhouse.sql` 承载明确声明使用 ClickHouse 的实体或分析型结构；不要把 ClickHouse 特性机械复制到关系数据库脚本。
- `Zongsoft.Discussions.md` 记录数据库结构说明。行为和类型以映射及目标数据库脚本共同核对，发现不一致时明确指出。

## 高风险契约

- 保持表名、列名、复合主键、外键/关系、序列策略、空值、默认值和索引与 `.mapping` 及 `Models` 一致。
- `SiteId` 参与租户隔离和多组复合键；不得为了简化脚本移除其约束意义。
- 论坛、主题、帖子、附件、投票和最近活动字段存在跨表关系与冗余统计；更改列时检查对应服务的更新逻辑。
- 方言差异应在各自脚本中表达，不通过降低所有数据库的类型或约束能力来求表面一致。

## 验证

- 静态对照四份脚本、映射文件和模型属性，检查新增对象在所有适用方言中都有对应定义。
- 可执行验证必须使用一次性数据库，记录数据库产品与版本，并检查建库、约束和代表性查询。
- 未经明确授权，不在共享或生产数据库执行 DDL，不自动生成或应用破坏性迁移。
