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
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Data;
using Zongsoft.Security;
using Zongsoft.Services;
using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Services;

[Service(nameof(ForumService))]
[DataService(typeof(ForumCriteria))]
public class ForumService : DataServiceBase<Forum>
{
	#region 构造函数
	public ForumService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 公共方法
	public bool IsModerator(ushort forumId, uint? userId = null)
	{
		if(userId == null || userId.Value == 0)
			userId = this.Principal.Identity.GetIdentifier<uint>();

		return this.DataAccess.Exists<Forum.ForumUser>(
			Condition.Equal(nameof(Forum.ForumUser.ForumId), forumId) &
			Condition.Equal(nameof(Forum.ForumUser.UserId), userId) & Condition.Equal(nameof(Forum.ForumUser.IsModerator), true));
	}

	public IEnumerable<UserProfile> GetModerators(ushort forumId, string schema)
	{
		return this.DataAccess.Select<UserProfile>(nameof(Forum.ForumUser),
			Condition.Equal(nameof(Forum.ForumUser.ForumId), forumId) &
			Condition.Equal(nameof(Forum.ForumUser.IsModerator), true),
			schema);
	}

	public IEnumerable<Thread> GetGlobalThreads(ushort forumId, string schema, Paging paging = null)
	{
		if(forumId == 0)
			return this.DataAccess.Select<Thread>(
				Condition.Equal(nameof(Thread.IsGlobal), true) &
				Condition.Equal(nameof(Thread.Visible), true),
				schema, paging, Sorting.Descending(nameof(Thread.ThreadId)));
		else
			return this.DataAccess.Select<Thread>(
				Condition.Equal(nameof(Thread.ForumId), forumId) &
				Condition.Equal(nameof(Thread.IsGlobal), true) &
				Condition.Equal(nameof(Thread.Visible), true),
				schema, paging, Sorting.Descending(nameof(Thread.ThreadId)));
	}

	public IEnumerable<Thread> GetPinnedThreads(ushort forumId, string schema, Paging paging = null)
	{
		return this.DataAccess.Select<Thread>(
			Condition.Equal(nameof(Thread.ForumId), forumId) &
			Condition.Equal(nameof(Thread.IsPinned), true) &
			Condition.Equal(nameof(Thread.Visible), true),
			schema, paging, Sorting.Descending(nameof(Thread.ThreadId)));
	}

	public Thread[] GetTopmosts(ushort forumId, string schema, int count = 10)
	{
		count = Math.Max(5, Math.Min(50, count));

		var globals = this.GetGlobalThreads(0, schema, Paging.Page(1, count));
		var pinneds = this.GetPinnedThreads(forumId, schema, Paging.Page(1, count));

		return globals.Union(pinneds).OrderByDescending(t => t.ThreadId).Take(count).ToArray();
	}

	public IEnumerable<Thread> GetThreads(ushort forumId, string schema, Paging paging = null)
	{
		var criteria =
			Condition.Equal(nameof(Thread.ForumId), forumId) &
			Condition.Equal(nameof(Thread.Visible), true);

		//只有第一页或者不分页才需要加载最顶部主题集
		if(paging == null || paging.PageIndex == 1)
		{
			//获取指定论坛中最顶部的主题集（全局贴+本论坛的置顶贴）
			var topmosts = this.GetTopmosts(forumId, schema);

			if(topmosts != null && topmosts.Length > 0)
				criteria.Add(Condition.NotIn(nameof(Thread.ThreadId), topmosts.Select(p => p.ThreadId)));
		}

		return this.DataAccess.Select<Thread>(criteria, schema, paging);
	}
	#endregion

	#region 重写方法
	protected override ICondition OnValidate(DataServiceMethod method, ICondition criteria, IDataOptions options)
	{
		//调用基类同名方法
		criteria = base.OnValidate(method, criteria, options);

		//匿名用户只能获取公共数据
		if(!this.Principal.Identity.IsAuthenticated)
			return criteria.And(Condition.Equal(nameof(Forum.Visibility), Visibility.All));
		else
			return criteria.And(Condition.In(nameof(Forum.Visibility), (byte)Visibility.Internal, (byte)Visibility.All) |
				(
					Condition.Equal(nameof(Forum.Visibility), Visibility.Specified) &
					Condition.Exists(nameof(Forum.Users),
						Condition.Equal(nameof(Forum.ForumUser.UserId), this.Principal.Identity.GetIdentifier<uint>()) &
						(
							Condition.Equal(nameof(Forum.ForumUser.IsModerator), true) |
							Condition.In(nameof(Forum.ForumUser.Permission), (byte)Permission.Read, (byte)Permission.Write)
						)
					)
				));
	}
	#endregion
}
