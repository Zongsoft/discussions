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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zongsoft.Data;
using Zongsoft.Security;
using Zongsoft.Services;
using Zongsoft.Collections;
using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Services;

[Service(nameof(PostService))]
[DataService(typeof(PostCriteria))]
public class PostService : DataServiceBase<Post>
{
	#region 构造函数
	public PostService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 公共方法
	public ValueTask<bool> UpvoteAsync(ulong postId, CancellationToken cancellation = default) => this.UpvoteAsync(postId, 1, cancellation);
	public async ValueTask<bool> UpvoteAsync(ulong postId, byte value, CancellationToken cancellation = default)
	{
		if(value == 0)
			value = 1;

		var userId = this.Principal.Identity.GetIdentifier<uint>();

		await using(var transaction = new Transaction())
		{
			await this.DataAccess.DeleteAsync<Post.PostVoting>(
				Condition.Equal(nameof(Post.PostVoting.PostId), postId) &
				Condition.Equal(nameof(Post.PostVoting.UserId), userId), cancellation);

			await this.DataAccess.InsertAsync(Model.Build<Post.PostVoting>(voting =>
			{
				voting.PostId = postId;
				voting.UserId = userId;
				voting.Value = (sbyte)Math.Min(value, (sbyte)100);
				voting.Timestamp = DateTime.Now;
			}), cancellation);

			//如果帖子投票统计信息更新成功
			if(await this.SetPostVotesAsync(postId, cancellation))
			{
				//提交事务
				await transaction.CommitAsync(cancellation);

				//返回成功
				return true;
			}
		}

		return false;
	}

	public ValueTask<bool> DownvoteAsync(ulong postId, CancellationToken cancellation = default) => this.DownvoteAsync(postId, 1, cancellation);
	public async ValueTask<bool> DownvoteAsync(ulong postId, byte value, CancellationToken cancellation = default)
	{
		if(value == 0)
			value = 1;

		await using(var transaction = new Transaction())
		{
			var userId = this.Principal.Identity.GetIdentifier<uint>();

			await this.DataAccess.DeleteAsync<Post.PostVoting>(
				Condition.Equal(nameof(Post.PostVoting.PostId), postId) &
				Condition.Equal(nameof(Post.PostVoting.UserId), userId), cancellation);

			await this.DataAccess.InsertAsync(Model.Build<Post.PostVoting>(voting =>
			{
				voting.PostId = postId;
				voting.UserId = userId;
				voting.Value = (sbyte)-Math.Min(value, (byte)100);
				voting.Timestamp = DateTime.Now;
			}), cancellation);

			//如果帖子投票统计信息更新成功
			if(await this.SetPostVotesAsync(postId, cancellation))
			{
				//提交事务
				await transaction.CommitAsync(cancellation);

				//返回成功
				return true;
			}
		}

		return false;
	}

	public IAsyncEnumerable<Post.PostVoting> GetUpvotesAsync(ulong postId, CancellationToken cancellation = default) => this.GetUpvotesAsync(postId, null, cancellation);
	public IAsyncEnumerable<Post.PostVoting> GetUpvotesAsync(ulong postId, Paging paging, CancellationToken cancellation = default)
	{
		return this.DataAccess.SelectAsync<Post.PostVoting>(Condition.Equal(nameof(Post.PostVoting.PostId), postId) & Condition.GreaterThan(nameof(Post.PostVoting.Value), 0), paging, cancellation);
	}

	public IAsyncEnumerable<Post.PostVoting> GetDownvotesAsync(ulong postId, CancellationToken cancellation = default) => this.GetDownvotesAsync(postId, null, cancellation);
	public IAsyncEnumerable<Post.PostVoting> GetDownvotesAsync(ulong postId, Paging paging, CancellationToken cancellation = default)
	{
		return this.DataAccess.SelectAsync<Post.PostVoting>(Condition.Equal(nameof(Post.PostVoting.PostId), postId) & Condition.LessThan(nameof(Post.PostVoting.Value), 0), paging, cancellation);
	}

	public IAsyncEnumerable<Post> GetCommentsAsync(ulong postId, CancellationToken cancellation = default) => this.GetCommentsAsync(postId, null, cancellation);
	public IAsyncEnumerable<Post> GetCommentsAsync(ulong postId, Paging paging, CancellationToken cancellation = default)
	{
		return this.DataAccess.SelectAsync<Post>(Condition.Equal(nameof(Post.RefererId), postId), paging, [Sorting.Descending(nameof(Post.PostId))], cancellation);
	}
	#endregion

	#region 内部方法
	internal string GetContentFilePath(IDataDictionary<Post> data) => this.GetContentFilePath(data.GetValue(p => p.PostId, 0UL), data.GetValue(p => p.ContentType, null));
	#endregion

	#region 虚拟方法
	protected virtual string GetContentFilePath(ulong postId, string contentType)
	{
		return Utility.GetFilePath(string.Format("posts/post-{0}-{1}.txt", postId.ToString(), Zongsoft.Common.Randomizer.GenerateString()));
	}
	#endregion

	#region 私有方法
	private async ValueTask<bool> SetPostVotesAsync(ulong postId, CancellationToken cancellation = default)
	{
		//获取当前帖子的点赞总数，即统计帖子投票表中投票数大于零的记录数
		var upvotes = await this.DataAccess.CountAsync<Post.PostVoting>(Condition.Equal(nameof(Post.PostVoting.PostId), postId) & Condition.GreaterThan(nameof(Post.PostVoting.Value), 0), cancellation);

		//获取当前帖子的被踩总数，即统计帖子投票表中投票数小于零的记录数
		var downvotes = await this.DataAccess.CountAsync<Post.PostVoting>(Condition.Equal(nameof(Post.PostVoting.PostId), postId) & Condition.LessThan(nameof(Post.PostVoting.Value), 0), cancellation);

		//更新指定帖子的累计点赞总数和累计被踩总数
		return await this.DataAccess.UpdateAsync(Model.Naming.Get<Post>(), new
		{
			PostId = postId,
			TotalUpvotes = upvotes,
			TotalDownvotes = downvotes,
		}, cancellation) > 0;
	}
	#endregion
	#region 异步业务路径
	protected override async ValueTask<Post> OnGetAsync(ICondition criteria, ISchema schema, DataSelectOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		//调用基类同名方法
		var post = await base.OnGetAsync(criteria, schema, options, cancellation);

		if(post == null)
			return null;

		//如果内容类型是外部文件（即非嵌入格式），则读取文件内容
		if(!Utility.IsContentEmbedded(post.ContentType))
			post.Content = await Utility.ReadTextFileAsync(post.Content, cancellation);

		return post;
	}

	protected override async ValueTask<int> OnInsertAsync(IDataDictionary<Post> data, ISchema schema, DataInsertOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		string filePath = null;

		options.Parameters.TryGetValue("Thread", out var threadObject);
		var thread = threadObject as IDataDictionary<Models.Thread> ?? (threadObject == null ? null : DataDictionary.GetDictionary<Models.Thread>(threadObject));

		if(thread == null)
		{
			var threadId = data.GetValue(p => p.ThreadId, 0UL);
			if(threadId == 0)
				throw new InvalidOperationException(Properties.Resources.PostMissingThread);

			var parent = await this.DataAccess.SelectAsync<Models.Thread>(Condition.Equal(nameof(Models.Thread.ThreadId), threadId), "SiteId,ForumId", cancellation).FirstOrDefault(cancellation);
			if(parent == null)
				throw new InvalidOperationException(Properties.Resources.ThreadNotFound);
			thread = DataDictionary.GetDictionary<Models.Thread>(parent);
		}

		//审核状态由论坛规则决定，不能采用调用方或映射中的默认值。
		data.SetValue(p => p.Approved, await this.ServiceProvider.ResolveRequired<ForumService>().CanPublishAsync(thread, cancellation));
		schema.Include(nameof(Post.Approved));

		try
		{
			filePath = await Utility.SetContentAsync(data, () => this.GetContentFilePath(data.GetValue(p => p.PostId, 0UL), data.GetValue(p => p.ContentType, null)), cancellation);

			await using(var transaction = new Transaction())
			{
				//调用基类同名方法
				var count = await base.OnInsertAsync(data, schema.Include(nameof(Post.Attachments)), options, cancellation);

				if(count > 0)
				{
					//更新发帖人的关联帖子统计信息
					//注意：只有当前帖子不是主题贴才需要更新对应的统计信息
					if(threadObject == null)
						await this.SetMostRecentPostAsync(data, cancellation);

					//提交事务
					await transaction.CommitAsync(cancellation);
				}
				else
				{
					//如果新增记录失败则删除刚创建的文件
					if(filePath != null && filePath.Length > 0)
						await Utility.DeleteFileAsync(filePath, CancellationToken.None);
				}

				return count;
			}
		}
		catch
		{
			//删除新建的文件
			if(filePath != null && filePath.Length > 0)
				await Utility.DeleteFileAsync(filePath, CancellationToken.None);

			throw;
		}
	}

	protected override async ValueTask<int> OnUpdateAsync(IDataDictionary<Post> data, ICondition criteria, ISchema schema, DataUpdateOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		return await Utility.MutateContentAsync(data, () => this.GetContentFilePath(data.GetValue(p => p.PostId, 0UL), data.GetValue(p => p.ContentType, null)), () => base.OnUpdateAsync(data, criteria, schema, options, cancellation), cancellation);
	}

	private async ValueTask<bool> SetMostRecentPostAsync(IDataDictionary<Post> data, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		//注意：如果当前帖子是主题内容贴则不需要更新对应的统计信息
		if(data == null)
			return false;

		//如果当前帖子没有指定对应的主题编号，则返回失败
		var threadId = data.GetValue(p => p.ThreadId, (ulong)0);

		if(threadId == 0)
			return false;

		//如果当前帖子对应的主题是不存在的，则返回失败
		var thread = await this.DataAccess.SelectAsync<Models.Thread>(Condition.Equal(nameof(Models.Thread.ThreadId), threadId), cancellation).FirstOrDefault(cancellation);

		if(thread == null)
			return false;

		//递增新增贴所属的主题的累计回帖总数
		if(await this.DataAccess.UpdateAsync<Models.Thread>(new
		{
			TotalReplies = Operand.Field(nameof(Models.Thread.TotalReplies)) + 1,
		}, Condition.Equal(nameof(Models.Thread.ThreadId), threadId), cancellation) < 1)
			return false;

		var userId = data.GetValue(p => p.CreatorId);
		var user = await this.DataAccess.SelectAsync<UserProfile>(Condition.Equal(nameof(UserProfile.UserId), userId), cancellation).FirstOrDefault(cancellation);
		var count = 0;

		//更新当前帖子所属主题的最后回帖信息
		count += await this.DataAccess.UpdateAsync(Model.Naming.Get<Models.Thread>(), new
		{
			ThreadId = threadId,
			MostRecentPostId = data.GetValue(p => p.PostId),
			MostRecentPostTime = data.GetValue(p => p.CreatedTime),
			MostRecentPostAuthorId = userId,
			MostRecentPostAuthorName = user?.Nickname,
			MostRecentPostAuthorAvatar = user?.Avatar,
		}, cancellation);

		//更新当前帖子所属论坛的最后回帖信息
		count += await this.DataAccess.UpdateAsync(Model.Naming.Get<Forum>(), new
		{
			SiteId = thread.SiteId,
			ForumId = thread.ForumId,
			MostRecentPostId = data.GetValue(p => p.PostId),
			MostRecentPostTime = data.GetValue(p => p.CreatedTime),
			MostRecentPostAuthorId = userId,
			MostRecentPostAuthorName = user?.Nickname,
			MostRecentPostAuthorAvatar = user?.Avatar,
		}, cancellation);

		//递增当前发帖人的累计回帖数，并且更新发帖人的最后回帖信息
		if(await this.DataAccess.UpdateAsync<UserProfile>(new
		{
			TotalPosts = Operand.Field(nameof(UserProfile.TotalPosts)) + 1,
		}, Condition.Equal(nameof(UserProfile.UserId), data.GetValue(p => p.CreatorId)), cancellation) > 0)
		{
			count += await this.DataAccess.UpdateAsync(Model.Naming.Get<UserProfile>(), new
			{
				UserId = data.GetValue(p => p.CreatorId),
				MostRecentPostId = data.GetValue(p => p.PostId),
				MostRecentPostTime = data.GetValue(p => p.CreatedTime),
			}, cancellation);
		}

		return count > 0;
	}
	#endregion
}
