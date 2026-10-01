# Zongsoft.Discussions Community Forum

![License](https://img.shields.io/github/license/Zongsoft/Zongsoft.Discussions)
![NuGet Version](https://img.shields.io/nuget/v/Zongsoft.Discussions)
![NuGet Downloads](https://img.shields.io/nuget/dt/Zongsoft.Discussions)
![GitHub Stars](https://img.shields.io/github/stars/Zongsoft/Zongsoft.Discussions?style=social)

[English](README.md) | [简体中文](README.zh-Hans.md)

-----

[**Zongsoft.Discussions**](https://github.com/Zongsoft/Zongsoft.Discussions) is a community-forum backend that demonstrates how to build a pluggable application with the **Zongsoft** framework.

**_The documentation and implementation are still being improved. Follow the Zongsoft WeChat account and Knowledge Planet community for project updates._**

## Local build and validation

Dependency versions are defined in `Directory.Packages.props` and are restored from NuGet by default. From the discussions root, use the .NET 10 SDK:

```powershell
dotnet build src/Zongsoft.Discussions.csproj -p:GeneratePackageOnBuild=false
dotnet build src/api/Zongsoft.Discussions.Web.csproj -p:GeneratePackageOnBuild=false
dotnet build src/ui/avalonia/desktop/Zongsoft.Discussions.Presentation.Desktop.csproj
dotnet build src/ui/avalonia/browser/Zongsoft.Discussions.Presentation.Browser.csproj
```

The browser project requires a WebAssembly workload matching the SDK. The backend and Web API use their configured target frameworks; Avalonia projects declare their own targets.

To validate source outputs from the adjacent framework checkout, build the corresponding Core and Web projects before enabling local references:

```powershell
dotnet build ../framework/Zongsoft.Core/src/Zongsoft.Core.csproj -f net10.0 -p:GeneratePackageOnBuild=false
dotnet build ../framework/Zongsoft.Web/src/Zongsoft.Web.csproj -f net10.0 -p:GeneratePackageOnBuild=false
dotnet build src/api/Zongsoft.Discussions.Web.csproj -f net10.0 -p:ZongsoftFrameworkPathReferenced=true -p:GeneratePackageOnBuild=false
```

Local references do not build framework automatically. Output directories, configurations, and target frameworks must match. To validate another target framework, change the target in all three commands together.

There is currently no separate regression test project in this repository. A successful build validates compilation and code rules; it does not validate database operations, plugin loading, live requests, or the Avalonia interface at runtime.

<a name="contribution"></a>
## Contributing

Please use **Issues** for bug reports and feature requests rather than general questions or consulting requests. Contributions are welcome through [pull requests](https://github.com/Zongsoft/Zongsoft.Discussions/pulls) and [issues](https://github.com/Zongsoft/Zongsoft.Discussions/issues).

For a new feature, open an issue with a detailed proposal before implementation. Early discussion helps coordinate work, avoid duplication, and shape the proposal into a change that can be accepted by the project.

Articles, blog posts, and videos about the project are also welcome. Contact us by [email](mailto:zongsoft@qq.com) if you would like your material to be shared on the [Zongsoft blog](http://zongsoft.com/blog).

> Before asking for help, consider reading [How To Ask Questions The Smart Way](http://www.catb.org/~esr/faqs/smart-questions.html) and [How to Report Bugs Effectively](https://www.chiark.greenend.org.uk/~sgtatham/bugs.html). Clear, reproducible questions are much easier to answer.

<a name="sponsor"></a>
## Sponsorship

You can support the project in the following ways:

1. Follow the **Zongsoft WeChat** account and support its articles.
2. Join the **Zongsoft Knowledge Planet** community for online Q&A and technical support.
3. [Email us](mailto:zongsoft@qq.com) if your organization needs on-site assistance, training, a specific feature, or an expedited fix.

[![Zongsoft WeChat account](https://raw.githubusercontent.com/Zongsoft/guidelines/main/zongsoft-qrcode%28wechat%29.png)](http://weixin.qq.com/r/zy-g_GnEWTQmrS2b93rd)

<a name="license"></a>
## License

This project is licensed under the [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0).
