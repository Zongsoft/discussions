## 概述

本目录遵循 [../AGENTS.md](../AGENTS.md)。该项目将领域服务暴露为 Zongsoft Web 控制器，并通过 `Zongsoft.Discussions.Web.plugin` 装配到宿主。

## 工作边界

- 控制器继承 `ServiceController<TEntity, TService>`，通用 CRUD、查询和返回约定优先由基类处理；这里只实现论坛特有动作。
- 保持 `[ControllerName]`、路由模板、动作名、参数绑定和服务方法一致。改变公开路由时同步检查 [../../docs/http](../../docs/http) 中的请求示例。
- 授权、站点隔离、审核和内容脱敏应由既有安全/数据/服务契约保证；不要只在单个控制器中复制一套易绕过的规则。
- 修改控制器、程序集名或依赖时检查 `.plugin`、`.deploy` 和项目文件的打包项。

## 外部依赖与安全

- 本项目是可打包的 Web 类库，不是完整宿主；运行验证需要兼容的 Zongsoft Web 宿主和插件目录。
- `.http` 文件只使用示例或本地凭据，不提交令牌、Cookie、个人数据和真实服务地址。
- 文件上传/下载和消息接口可能触发存储或外部服务访问；测试使用临时目录、替身或隔离服务。

## 验证

- 最小构建：`dotnet build Zongsoft.Discussions.Web.csproj -f net10.0`。
- 路由或绑定变化在本地宿主验证正常、未认证、无权限、跨站点和无效输入分支。
- 多目标兼容性变化再分别验证 net8.0、net9.0、net10.0。
