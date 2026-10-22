---
name: zongsoft-discussions
description: 在 Zongsoft discussions 仓库中修改、审查或排障论坛领域库、数据映射、多数据库脚本、站点租户隔离、身份转换、数据服务、Web 控制器、插件产物或 Avalonia 原型时使用；不用于修改 Zongsoft Framework 的通用能力。
---

# Zongsoft Discussions 协作流程

开始工作前阅读 [AGENTS.md](AGENTS.md) 和目标目录就近的说明。

## 定位变更

1. 先判断契约归属：
	- 领域实体与状态：`src/Models`、`src/Fields.cs`。
	- 查询、写入和业务动作：`src/Services`。
	- 租户条件、结果过滤和身份模型：`src/Data`、`src/Security`。
	- 数据结构：`src/Zongsoft.Discussions.mapping` 与 `database`。
	- HTTP 表面：`api/Controllers`、`docs/http`。
	- 插件装配和交付：`src/*.plugin`、`src/*.option`、`src/*.deploy`、对应项目文件。
	- UI 原型：`ui/avalonia`，与后端主解决方案分开处理。
2. 搜索模型属性、映射成员、SQL 列、服务选择字段和控制器动作的全部引用，再确定最窄修改范围。
3. 若需求属于 Zongsoft 通用数据、Web、插件或安全机制，应在 Framework 仓库处理；本仓只承载论坛业务规则及其适配。

## 契约检查

- 实体结构变化：同步检查模型、`.mapping`、四种数据库脚本、服务字段选择、API 输入输出和归档模板。
- 身份或权限变化：保持 `UserIdentity.Scheme`、声明转换、`UserChallenger`、插件注册和 `DataValidator` 的站点边界一致。
- 主题或帖子变化：检查审核、隐藏、锁定、置顶、精华、全局主题、回复关系、投票、附件以及论坛/用户统计的联动。
- 内容读写变化：确认内嵌内容与文件路径内容的判定、未审核内容脱敏、附件目录和 `basePath` 选项不会泄露或串站。
- API 变化：优先复用服务层语义；同步检查控制器路由、返回约定和 `docs/http` 示例。
- 插件变化：保持模块名 `Discussions`、插件依赖、挂载路径、打包产物和部署文件相互匹配。

## 验证选择

- 纯文档：检查链接、CRLF 和差异。
- 领域逻辑：构建 `src/Zongsoft.Discussions.csproj`，并对受影响服务使用隔离数据做聚焦验证。
- API：不编写单元测试；构建 `api/Zongsoft.Discussions.Web.csproj`，在本地宿主验证变更路由和授权分支。
- 映射或 SQL：至少对照全部数据库方言做静态检查；只有具备临时数据库时才执行脚本。
- Avalonia：使用 `ui/avalonia/Zongsoft.Discussions.Presentation.sln` 单独构建对应平台项目。

未经明确要求，不运行 Cake 的 `pack`、真实数据库脚本、外部对象存储请求或带真实身份信息的 HTTP 请求。

## 异步服务与内容

- 数据操作、条件/字段验证、授权及生命周期钩子使用 Async 契约；CancellationToken 位于最后，控制器等待标量结果，查询以 IAsyncEnumerable 返回。常用参数前缀支持直接传入令牌。
- 查询过滤器在异步枚举时处理记录，保留分页事件，逐条等待允许读取的外置正文。未审核内容先按身份脱敏；ThreadService 仅在作者或版主权限成立后恢复相应内容，保持 SiteId 限制。
- GetTopmostsAsync 依次读取全局主题与置顶主题，再按既有 Union、ThreadId 降序与数量限制返回数组；GetThreadsAsync 在首查询条件准备后返回原可分页查询对象。
- 内容字典回调只执行本地取值；文件写入在回调外等待。长正文的写入失败须阻止数据库写入，数据库失败或未写入时删除新建正文文件；清理使用不可取消标记，保留原错误。读取取消向上传播，其余读取失败保持空文本结果。
- 事务范围、提交条件和统计更新顺序按业务服务维护。返回失败或抛出异常时由 await using 等待未提交事务的释放；不把已有失败分支变成提交。
