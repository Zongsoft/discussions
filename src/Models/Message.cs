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
using System.ComponentModel;

using Zongsoft.Data;

namespace Zongsoft.Discussions.Models;

/// <summary>表示消息的业务实体类。</summary>
public abstract class Message
{
	#region 公共属性
	/// <summary>获取或设置消息编号。</summary>
	public abstract ulong MessageId { get; set; }

	/// <summary>获取或设置所属站点编号。</summary>
	public abstract uint SiteId { get; set; }

	/// <summary>获取或设置消息标题。</summary>
	public abstract string Subject { get; set; }

	/// <summary>获取或设置消息内容。</summary>
	public abstract string Content { get; set; }

	/// <summary>获取或设置内容类型。</summary>
	public abstract string ContentType { get; set; }

	/// <summary>获取或设置消息类型。</summary>
	public abstract string MessageType { get; set; }

	/// <summary>获取或设置消息来源。</summary>
	public abstract string Referer { get; set; }

	/// <summary>获取或设置标签数组。</summary>
	[TypeConverter(typeof(TagsConverter))]
	public abstract string[] Tags { get; set; }

	/// <summary>获取或设置创建人编号。</summary>
	public abstract uint? CreatorId { get; set; }

	/// <summary>获取或设置创建人对象。</summary>
	public abstract UserProfile Creator { get; set; }

	/// <summary>获取或设置创建时间。</summary>
	public abstract DateTime CreatedTime { get; set; }
	#endregion
}

/// <summary>表示消息查询条件的实体类。</summary>
public abstract class MessageCriteria : CriteriaBase
{
	#region 公共属性
	/// <summary>获取或设置消息标题。</summary>
	[Condition(ConditionOperator.Like)]
	public abstract string Subject { get; set; }

	/// <summary>获取或设置消息类型。</summary>
	public abstract string MessageType { get; set; }

	/// <summary>获取或设置消息来源。</summary>
	public abstract string Referer { get; set; }

	/// <summary>获取或设置创建人编号。</summary>
	public abstract uint? CreatorId { get; set; }

	/// <summary>获取或设置创建时间范围。</summary>
	public abstract Range<DateTime>? CreatedTime { get; set; }
	#endregion
}
