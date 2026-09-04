## 概述

本目录遵循 [../AGENTS.md](../AGENTS.md)，是 Discussions 领域模块和 Web 扩展的源码根。领域协作流程见 [../SKILL.md](../SKILL.md)。

## 职责边界

- `Models` 定义论坛、站点、用户、主题、帖子、附件、反馈和消息的数据形态。
- `Services` 负责数据访问与业务动作；控制器应保持为薄适配层。
- `Data` 负责站点条件注入和查询结果过滤；`Security` 负责论坛身份质询与声明转换。
- `Features` 放置对 Framework 扩展点的业务实现；`Module.cs` 暴露模块访问器、事件和属性。
- `api` 与 `ui` 有独立的就近说明，不应被主库项目编译进来。

## 插件和数据产物

- 修改模型属性时检查 `Zongsoft.Discussions.mapping`、`../database/*.sql`、服务选择字段和 API 表面。
- 修改模块、过滤器、质询器或身份转换器时同步检查 `Zongsoft.Discussions.plugin` 中的类型名和挂载路径。
- 修改配置时保持 `Configuration/IConfiguration.cs` 与 `Zongsoft.Discussions.option` 的路径、节名和键一致。
- 保持 `.deploy`、项目文件中的 `Pack` 项与实际交付的 `.plugin`、`.option`、`.mapping` 一致。

## 验证

- 最小构建：`dotnet build Zongsoft.Discussions.csproj -f net10.0`。
- 公共模型或依赖兼容性变化再覆盖 net8.0、net9.0、net10.0。
- 数据过滤、安全或内容读写变化需验证匿名用户、当前作者、其他已登录用户和跨站点访问分支。
