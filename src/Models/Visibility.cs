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

namespace Zongsoft.Discussions.Models;

/// <summary>表示可见性的枚举。</summary>
public enum Visibility : byte
{
	/// <summary>隐藏，仅限所有人可见</summary>
	Hidden,

	/// <summary>限定用户</summary>
	Specified,

	/// <summary>内部，站内用户</summary>
	Internal,

	/// <summary>公共，所有人(包括匿名访客)</summary>
	All,
}
