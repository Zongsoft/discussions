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
 * Copyright (C) 2015-2025 Zongsoft Corporation. All rights reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;

using Zongsoft.Data;

namespace Zongsoft.Discussions.Models;

/// <summary>
/// 表示访问历史的实体类。
/// </summary>
public abstract class History
{
	#region 公共属性
	/// <summary>获取或设置用户的编号。</summary>
	public abstract uint UserId { get; set; }

	/// <summary>获取或设置浏览的主题编号。</summary>
	public abstract ulong ThreadId { get; set; }

	/// <summary>获取或设置浏览的主题对象。</summary>
	public abstract Thread Thread { get; set; }

	/// <summary>获取或设置累计浏览次数。</summary>
	public abstract uint ViewedCount { get; set; }

	/// <summary>获取或设置累计发帖次数。</summary>
	public abstract uint PostedCount { get; set; }

	/// <summary>获取或设置首次浏览的时间。</summary>
	public abstract DateTime FirstViewedTime { get; set; }

	/// <summary>获取或设置首次发帖的时间。</summary>
	public abstract DateTime? FirstPostedTime { get; set; }

	/// <summary>获取或设置最后浏览的时间。</summary>
	public abstract DateTime LastViewedTime { get; set; }

	/// <summary>获取或设置最后发帖的时间。</summary>
	public abstract DateTime? LastPostedTime { get; set; }
	#endregion
}

/// <summary>
/// 表示访问历史查询条件的实体类。
/// </summary>
public abstract class HistoryCriteria : CriteriaBase
{
	#region 公共属性
	/// <summary>获取或设置浏览的主题编号。</summary>
	public abstract ulong? ThreadId { get; set; }

	/// <summary>获取或设置累计浏览次数范围。</summary>
	public abstract Range<uint>? ViewedCount { get; set; }

	/// <summary>获取或设置累计发帖次数范围。</summary>
	public abstract Range<uint>? PostedCount { get; set; }

	/// <summary>获取或设置浏览的时间范围。</summary>
	[Condition(typeof(ViewedConverter))]
	public abstract Range<DateTime>? ViewedTime { get; set; }
	#endregion

	#region 嵌套子类
	private class ViewedConverter : ConditionConverter
	{
		public override ICondition Convert(ConditionConverterContext context)
		{
			var timestamp = (Range<DateTime>)context.Value;

			if(timestamp.IsEmpty)
				return null;

			return Condition.Between(context.GetFullName(nameof(History.FirstViewedTime)), timestamp) |
			       Condition.Between(context.GetFullName(nameof(History.LastViewedTime)), timestamp);
		}
	}
	#endregion
}
