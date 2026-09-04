## 概述

本目录遵循 [../../../AGENTS.md](../../../AGENTS.md)，是与后端主解决方案分离的 Avalonia UI 原型。

## 项目边界

- `shared` 保存 View、ViewModel、资源和平台无关启动逻辑。
- `desktop` 是 `net9.0` 桌面入口；`browser` 是 `net9.0-browser` WebAssembly 入口及静态资源。
- `Directory.Packages.props` 在仓库中央包版本之上补充 Avalonia 与 CommunityToolkit 版本；升级时保持三个项目兼容。
- `Zongsoft.Discussions.Presentation.sln` 是本子树的解决方案，不要把 UI 文件意外纳入后端 `Zongsoft.Discussions.csproj`。

## 高风险契约

- 修改 `*.axaml` 时保持 `x:Class`、代码隐藏类型、资源 URI、绑定属性和 ViewLocator 约定一致。
- 默认启用编译绑定；改变 ViewModel 属性时检查对应 XAML 绑定和通知语义。
- 平台能力放在 `desktop` 或 `browser`，共享项目不得无条件引用平台专用 API。
- 浏览器启动文件、`runtimeconfig.template.json` 和 `wwwroot` 属于部署表面；不要把本地地址或凭据写入静态资源。

## 验证

- 共享或跨平台变化构建 `Zongsoft.Discussions.Presentation.sln`。
- 平台特定变化至少构建对应的 desktop 或 browser 项目；视觉、键盘和浏览器启动行为需要相应平台手工验证。
- 本子树当前没有自动化 UI 测试，不以另一个平台的成功构建代替目标平台验证。
