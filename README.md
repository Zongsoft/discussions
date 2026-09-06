# Zongsoft.Discussions Community Forum

![License](https://img.shields.io/github/license/Zongsoft/Zongsoft.Discussions)
![NuGet Version](https://img.shields.io/nuget/v/Zongsoft.Discussions)
![NuGet Downloads](https://img.shields.io/nuget/dt/Zongsoft.Discussions)
![GitHub Stars](https://img.shields.io/github/stars/Zongsoft/Zongsoft.Discussions?style=social)

[English](README.md) | [简体中文](README.zh-Hans.md)

-----

[**Zongsoft.Discussions**](https://github.com/Zongsoft/Zongsoft.Discussions) is a community-forum backend that demonstrates how to build a pluggable application with the **Zongsoft** framework.

**_The documentation and implementation are still being improved. Follow the Zongsoft WeChat account and Knowledge Planet community for project updates._**

## Local build and regression checks

The current code requires **Zongsoft.Core 7.59.0** and uses Core's pagination filters and asynchronous enumeration adapters. This version was not yet available on NuGet when checked on 2026-09-06. Until it is published, use the adjacent `../framework` build outputs; do not downgrade to 7.58.0, which contains pagination filtering defects.

Run these commands from the discussions root with the .NET 10 SDK and Debug configuration:

```powershell
dotnet build ../framework/Zongsoft.Core/src/Zongsoft.Core.csproj -f net10.0 -p:GeneratePackageOnBuild=false
dotnet build ../framework/Zongsoft.Web/src/Zongsoft.Web.csproj -f net10.0 -p:GeneratePackageOnBuild=false
dotnet build src/api/Zongsoft.Discussions.Web.csproj -f net10.0 -p:ZongsoftFrameworkPathReferenced=true -p:GeneratePackageOnBuild=false
dotnet run --project test/Zongsoft.Discussions.Regression.csproj -p:ZongsoftFrameworkPathReferenced=true -p:GeneratePackageOnBuild=false
```

Local references do not build framework automatically: its output directory, configuration, and target framework must match. To check net8.0 or net9.0, change the target in the first three commands together. The regression executable targets net10.0 and does not connect to databases or external services. Once Core 7.59.0 is published, omit the local-reference property to use the default NuGet path.

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
