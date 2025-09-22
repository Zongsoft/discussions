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

[Service(nameof(ThreadService))]
[DataService(typeof(ThreadCriteria))]
public class ThreadService : DataServiceBase<Thread>
{
	#region 成员字段
	private PostService _posting;
	#endregion

	#region 构造函数
	public ThreadService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 公共属性
	public PostService Posting
	{
		get
		{
			if(_posting == null)
				_posting = this.ServiceProvider.ResolveRequired<PostService>();

			return _posting;
		}
	}
	#endregion

	#region 公共方法
	/// <summary>审核批准指定的主题，注：只有版主才具备调用该方法的权限。</summary>
	/// <param name="threadId">指定要审核批准的主题编号。</param>
	/// <returns>如果审核批准成功则返回真(True)，否则返回假(False)。</returns>
	public bool Approve(ulong threadId)
	{
		var criteria = Condition.Equal(nameof(Thread.ThreadId), threadId) &
		               Condition.Equal(nameof(Thread.Approved), false) &
		               GetIsModeratorCriteria();

		return this.DataAccess.Update<Thread>(new
		{
			Approved = true,
			ApprovedTime = DateTime.Now,
			Post = new
			{
				Approved = true,
			}
		}, criteria, "*,Post{Approved}") > 0;
	}

	/// <summary>设置指定主题可见，注：只有版主才具备调用该方法的权限。</summary>
	/// <param name="threadId">指定要设置的主题编号。</param>
	/// <param name="value">指定一个值，指示是否可见。</param>
	/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
	public bool Visible(ulong threadId, bool value)
	{
		return this.DataAccess.Update<Thread>(new
		{
			Visible = value,
		}, Condition.Equal(nameof(Thread.ThreadId), threadId) & GetIsModeratorCriteria()) > 0;
	}

	/// <summary>设置指定主题是否锁定，注：只有版主才具备调用该方法的权限。</summary>
	/// <param name="threadId">指定要设置的主题编号。</param>
	/// <param name="value">指定一个值，指示是否锁定。</param>
	/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
	public bool SetLocked(ulong threadId, bool value)
	{
		return this.DataAccess.Update<Thread>(new
		{
			IsLocked = value,
		}, Condition.Equal(nameof(Thread.ThreadId), threadId) & GetIsModeratorCriteria()) > 0;
	}

	/// <summary>设置指定主题是否置顶，注：只有版主才具备调用该方法的权限。</summary>
	/// <param name="threadId">指定要设置的主题编号。</param>
	/// <param name="value">指定一个值，指示是否置顶。</param>
	/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
	public bool SetPinned(ulong threadId, bool value)
	{
		return this.DataAccess.Update<Thread>(new
		{
			IsPinned = value,
		}, Condition.Equal(nameof(Thread.ThreadId), threadId) & GetIsModeratorCriteria()) > 0;
	}

	/// <summary>设置指定主题是否为精华帖，注：只有版主才具备调用该方法的权限。</summary>
	/// <param name="threadId">指定要设置的主题编号。</param>
	/// <param name="value">指定一个值，指示是否为精华帖。</param>
	/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
	public bool SetValued(ulong threadId, bool value)
	{
		return this.DataAccess.Update<Thread>(new
		{
			IsValued = value,
		}, Condition.Equal(nameof(Thread.ThreadId), threadId) & GetIsModeratorCriteria()) > 0;
	}

	/// <summary>设置指定主题是否为全局帖，注：只有超级管理员才能调用该方法。</summary>
	/// <param name="threadId">指定要设置的主题编号。</param>
	/// <param name="value">指定一个值，指示是否为全局帖。</param>
	/// <returns>如果设置成功则返回真(True)，否则返回假(False)。</returns>
	public bool SetGlobal(ulong threadId, bool value)
	{
		if(!this.Principal.IsAdministrator())
			return false;

		return this.DataAccess.Update<Thread>(new
		{
			IsGlobal = value,
		}, Condition.Equal(nameof(Thread.ThreadId), threadId)) > 0;
	}

	public IEnumerable<Post> GetPosts(ulong threadId, string schema, Paging paging = null)
	{
		return this.DataAccess.Select<Post>(
			Condition.Equal(nameof(Post.ThreadId), threadId) &
			Condition.GreaterThan(nameof(Post.RefererId), 0) &
			Condition.Equal(nameof(Post.Visible), true),
			schema, paging, Sorting.Descending(nameof(Post.PostId)));
	}
	#endregion

	#region 重写方法
	protected override Thread OnGet(ICondition criteria, ISchema schema, DataSelectOptions options)
	{
		//调用基类同名方法
		var thread = base.OnGet(criteria, schema, options);

		if(thread == null)
			return null;

		//如果指定主题尚未审核通过并且创建人不是当前用户，则需要进行权限判断
		if(!thread.Approved && (this.Principal.Identity.GetIdentifier<uint>() != thread.CreatorId))
		{
			//判断当前用户是否为该论坛的版主
			var isModerator = this.ServiceProvider.ResolveRequired<ForumService>().IsModerator(thread.ForumId);

			//当前用户不是版主，并且该主题未审核通过则抛出授权异常
			if(!isModerator && !thread.Approved)
				throw new Zongsoft.Security.Privileges.AuthorizationException("The specified thread has not been approved, so it cannot be viewed.");
		}

		//递增当前主题的累计阅读量并更新最后查看时间
		this.DataAccess.Update<Thread>(new
		{
			TotalViews = Operand.Field(nameof(Thread.TotalViews)) + 1,
			ViewedTime = DateTime.Now,
		}, Condition.Equal(nameof(Thread.ThreadId), thread.ThreadId));

		//更新查询到的实体属性
		thread.TotalViews += 1;
		thread.ViewedTime = DateTime.Now;

		//更新当前用户的浏览记录
		this.SetHistory(thread.ThreadId);

		return thread;
	}

	protected override int OnInsert(IDataDictionary<Thread> data, ISchema schema, DataInsertOptions options)
	{
		if(!data.TryGetValue(p => p.Post, out var post) || string.IsNullOrEmpty(post.Content))
			throw new InvalidOperationException("Missing content of the thread.");

		//确保数据模式含有“主题内容贴”复合属性
		schema.Include("Post{*}");

		//更新主题内容贴的相关属性
		post.Visible = false;

		using(var transaction = new Zongsoft.Transactions.Transaction())
		{
			//调用基类同名方法，插入主题数据
			var count = base.OnInsert(data, schema, options);

			if(count < 1)
				return count;

			//更新发帖人关联的主题统计信息
			this.SetMostRecentThread(data);

			//提交事务
			transaction.Commit();

			return count;
		}
	}
	#endregion

	#region 私有方法
	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	private Zongsoft.Data.Condition GetIsModeratorCriteria()
	{
		return Condition.Exists("Forum.Users",
		         Condition.Equal(nameof(Forum.ForumUser.UserId), this.Principal.Identity.GetIdentifier<uint>()) &
		         Condition.Equal(nameof(Forum.ForumUser.IsModerator), true));
	}

	private bool SetMostRecentThread(IDataDictionary<Thread> data)
	{
		var count = 0;
		var userId = data.GetValue(p => p.CreatorId, this.Principal.Identity.GetIdentifier<uint>());
		var user = this.DataAccess.Select<UserProfile>(
			Condition.Equal(nameof(UserProfile.UserId), userId),
			$"{nameof(UserProfile.UserId)}," +
			$"{nameof(UserProfile.Name)}," +
			$"{nameof(UserProfile.Nickname)}," +
			$"{nameof(UserProfile.Avatar)}").FirstOrDefault();

		//更新当前主题所属论坛的最后发帖信息
		count += this.DataAccess.Update<Forum>(new
		{
			SiteId = data.GetValue(p => p.SiteId),
			ForumId = data.GetValue(p => p.ForumId),
			MostRecentThreadId = data.GetValue(p => p.ThreadId),
			MostRecentThreadTitle = data.GetValue(p => p.Title),
			MostRecentThreadTime = data.GetValue(p => p.CreatedTime),
			MostRecentThreadAuthorId = userId,
			MostRecentThreadAuthorName = user?.Nickname,
			MostRecentThreadAuthorAvatar = user?.Avatar,
		});

		//递增当前发帖人的累计主题数及最后发表的主题信息
		count += this.DataAccess.Update<UserProfile>(new
		{
			UserId = userId,
			TotalThreads = Operand.Field(nameof(UserProfile.TotalThreads)) + 1,
			MostRecentThreadId = data.GetValue(p => p.ThreadId),
			MostRecentThreadTitle = data.GetValue(p => p.Title),
			MostRecentThreadTime = data.GetValue(p => p.CreatedTime),
		});

		return count > 0;
	}

	private void SetHistory(ulong threadId)
	{
		//新增或更新当前用户对指定主题的浏览记录（自动递增浏览次数）
		this.DataAccess.Upsert<History>(new
		{
			UserId = this.Principal.Identity.GetIdentifier<uint>(),
			ThreadId = threadId,
			ViewedCount = Operand.Field(nameof(History.ViewedCount)) + 1,
			MostRecentViewedTime = DateTime.Now,
		});
	}
	#endregion
}
