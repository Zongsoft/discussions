## 硬性要求

- 新建文本文件使用 CRLF 换行符，代码文件使用 Tab 缩进；保持既有文件的局部格式，不做无关重排。
- 开始工作前阅读目标目录就近的 `AGENTS.md`、`SKILL.md`、项目文件、插件产物和相关文档。
- 保留工作树中的用户修改；不要重置、覆盖或顺手整理任务范围外的差异。

## 仓库概览

本仓库是基于 Zongsoft Framework 的社区论坛示例与业务模块，不是 Framework 公共契约的归属地。

- `src`：领域模型、数据服务、数据过滤、安全身份和插件模块；主库支持 .NET 8、9、10。
- `src/api`：基于 `Zongsoft.Web` 的 Web 控制器与 Web 插件产物。
- `database`：ClickHouse、SQL Server、MySQL、PostgreSQL 的建库脚本与说明。
- `docs/http`：API 调试请求；`docs/templates`：归档等功能使用的打包模板。
- `src/ui/avalonia`：独立的 Avalonia 桌面与浏览器原型，不属于主解决方案。

NuGet 版本由 `Directory.Packages.props` 集中管理。`Directory.Build.props` 默认引用 NuGet 包；只有明确设置 `ZongsoftFrameworkPathReferenced=true` 且相邻 `../framework` 存在时，项目才改用本地 Framework 程序集。

## 高风险契约

- `src/Zongsoft.Discussions.mapping`、`src/Models`、`database/*.sql` 和服务查询字段共同描述数据契约；更改实体、键、序列、默认值或关系时必须联动检查。
- `SiteId` 是租户边界，`DataValidator` 会依据当前 `UserIdentity` 注入或限制站点条件。不要让查询、写入、导入或新增端点绕过站点隔离。
- 帖子和主题的审核可见性、内容内嵌/外置读取、作者权限以及论坛统计字段彼此关联；修改一个环节时搜索服务、过滤器、控制器和映射中的全部调用方。
- `*.plugin`、`*.option`、`*.mapping`、`*.deploy` 作为 NuGet 产物交付。修改模块名、类型名、插件挂载点、选项键或打包路径时同步检查项目文件和调用方。
- `Properties/Resources.Designer.cs` 是生成文件；优先修改 `.resx`，仅通过相应生成流程更新设计器文件。

## 操作边界

- 不提交真实令牌、Cookie、连接字符串、对象存储凭据、NuGet API Key 或论坛用户数据；提交前检查 `.http`、配置示例和日志。
- 数据库脚本可能创建、删除或重建对象；未经明确要求，不对真实数据库执行。
- `build.cake` 的 `clean` 会清理输出，`pack` 会使用 `NUGET_API_KEY` 推送包；未经明确要求，不运行发布或推包任务。
- API 调试请求、对象存储访问和身份质询可能连接外部系统；默认只做离线或本地临时环境验证。

## 验证

- 文档改动检查相对链接、CRLF、`git diff --check` 和实际差异，不必运行构建。
- 领域库改动优先构建 `src/Zongsoft.Discussions.csproj`；API 改动构建 `src/api/Zongsoft.Discussions.Web.csproj`。
- 公共模型、映射或多目标依赖变化按需要分别验证 `net8.0`、`net9.0`、`net10.0`。
- 仓库当前没有自动化测试项目；行为验证应使用临时数据库、临时文件存储和隔离身份，不能以一次成功构建替代业务契约检查。
- Avalonia 原型按其就近说明单独验证，不因后端改动默认构建 UI。
