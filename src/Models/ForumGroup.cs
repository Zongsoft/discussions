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

using Zongsoft.Data;

namespace Zongsoft.Discussions.Models;

/// <summary>表示论坛分组的业务实体类。</summary>
public abstract class ForumGroup
{
	#region 公共属性
	/// <summary>获取或设置论坛组所属的站点编号，复合主键。</summary>
	public abstract uint SiteId { get; set; }

	/// <summary>获取或设置论坛组的编号，复合主键。</summary>
	public abstract ushort GroupId { get; set; }

	/// <summary>获取或设置论坛组所在站点中的排列序号。</summary>
	public abstract short Ordinal { get; set; }

	/// <summary>获取或设置论坛组的名称。</summary>
	public abstract string Name { get; set; }

	/// <summary>获取或设置论坛组的图标。</summary>
	public abstract string Icon { get; set; }

	/// <summary>获取或设置论坛组的描述文本。</summary>
	public abstract string Description { get; set; }
	#endregion

	#region 集合属性
	/// <summary>获取或设置论坛组中的论坛集。</summary>
	public abstract IEnumerable<Forum> Forums { get; set; }
	#endregion
}

/// <summary>表示论坛分组查询的实体类。</summary>
public abstract class ForumGroupCriteria : CriteriaBase
{
	#region 公共属性
	/// <summary>获取或设置论坛组的名称。</summary>
	public abstract string Name { get; set; }
	#endregion
}
