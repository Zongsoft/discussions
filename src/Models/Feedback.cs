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

/// <summary>表示意见反馈的业务实体类。</summary>
public abstract class Feedback
{
	#region 公共属性
	/// <summary>获取或设置反馈编号。</summary>
	public abstract ulong FeedbackId { get; set; }
	/// <summary>获取或设置站点编号。</summary>
	public abstract uint SiteId { get; set; }
	/// <summary>获取或设置反馈种类。</summary>
	public abstract byte Kind { get; set; }
	/// <summary>获取或设置反馈标题。</summary>
	public abstract string Subject { get; set; }
	/// <summary>获取或设置反馈内容。</summary>
	public abstract string Content { get; set; }
	/// <summary>获取或设置内容类型。</summary>
	public abstract string ContentType { get; set; }
	/// <summary>获取或设置联系人姓名。</summary>
	public abstract string ContactName { get; set; }
	/// <summary>获取或设置联系人方式。</summary>
	public abstract string ContactText { get; set; }
	/// <summary>获取或设置创建时间。</summary>
	public abstract DateTime CreatedTime { get; set; }
	#endregion
}

/// <summary>表示意见反馈查询条件的实体类。</summary>
public abstract class FeedbackCriteria : CriteriaBase
{
	#region 公共属性
	[Condition("Subject", "ContactName", "ContactText")]
	public abstract string Key { get; set; }
	public abstract byte? Kind { get; set; }
	public abstract string Subject { get; set; }
	public abstract string ContactName { get; set; }
	public abstract string ContactText { get; set; }
	public abstract Range<DateTime> CreatedTime { get; set; }
	#endregion
}
