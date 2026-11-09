/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (c) Zongsoft Studio. All rights reserved.
 *
 * Licensed under the MIT License (see LICENSE).
 */

using System;
using System.Collections.Generic;

namespace Zongsoft.Discussions.Configuration;

/// <summary>表示应用配置信息的接口。</summary>
public interface IConfiguration
{
	/// <summary>获取或设置默认的站点编号。</summary>
	uint SiteId { get; set; }

	/// <summary>获取或设置文件存储的基路径。</summary>
	string BasePath { get; set; }
}
