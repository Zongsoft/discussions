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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zongsoft.Data;
using Zongsoft.Services;
using Zongsoft.Security;
using Zongsoft.Collections;
using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Services;

[Service(nameof(UserService))]
[DataService(typeof(UserProfileCriteria))]
public class UserService : DataServiceBase<UserProfile>
{
	#region 成员字段
	private string _basePath;
	#endregion

	#region 构造函数
	public UserService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 公共属性
	public string BasePath
	{
		get => _basePath;
		set
		{
			if(string.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException();

			_basePath = value.Trim();
		}
	}
	#endregion

	#region 公共方法
	public IAsyncEnumerable<History> GetHistoriesAsync(uint userId, CancellationToken cancellation = default) => this.GetHistoriesAsync(userId, null, cancellation);
	public IAsyncEnumerable<History> GetHistoriesAsync(uint userId, Paging paging, CancellationToken cancellation = default)
	{
		if(userId == 0)
			userId = this.Principal.Identity.GetIdentifier<uint>();

		return this.DataAccess.SelectAsync<History>(Condition.Equal(nameof(History.UserId), userId), $"*, {nameof(History.Thread)}" + "{*}", paging, cancellation);
	}

	public ValueTask<int> GetMessageTotalCountAsync(CancellationToken cancellation = default) => this.GetMessageTotalCountAsync(0, cancellation);
	public async ValueTask<int> GetMessageTotalCountAsync(uint userId, CancellationToken cancellation = default)
	{
		if(userId == 0)
			userId = this.Principal.Identity.GetIdentifier<uint>();

		return await this.DataAccess.CountAsync<UserMessage>(Condition.Equal(nameof(UserMessage.UserId), userId), cancellation);
	}

	public ValueTask<int> GetMessageUnreadCountAsync(CancellationToken cancellation = default) => this.GetMessageUnreadCountAsync(0, cancellation);
	public async ValueTask<int> GetMessageUnreadCountAsync(uint userId, CancellationToken cancellation = default)
	{
		if(userId == 0)
			userId = this.Principal.Identity.GetIdentifier<uint>();

		return await this.DataAccess.CountAsync<UserMessage>(Condition.Equal(nameof(UserMessage.UserId), userId) & Condition.Equal(nameof(UserMessage.IsRead), false), cancellation);
	}

	public IAsyncEnumerable<Message> GetMessagesAsync(CancellationToken cancellation) => this.GetMessagesAsync(0, null, null, cancellation);
	public IAsyncEnumerable<Message> GetMessagesAsync(uint userId, CancellationToken cancellation) => this.GetMessagesAsync(userId, null, null, cancellation);
	public IAsyncEnumerable<Message> GetMessagesAsync(uint userId, bool? isRead, CancellationToken cancellation) => this.GetMessagesAsync(userId, isRead, null, cancellation);
	public IAsyncEnumerable<Message> GetMessagesAsync(uint userId = 0, bool? isRead = null, Paging paging = null, CancellationToken cancellation = default)
	{
		if(userId == 0)
			userId = this.Principal.Identity.GetIdentifier<uint>();

		var conditions = ConditionCollection.And(Condition.Equal(nameof(UserMessage.UserId), userId));

		if(isRead.HasValue)
			conditions.Add(Condition.Equal(nameof(UserMessage.IsRead), isRead.Value));

		return this.DataAccess.SelectAsync<UserMessage>(conditions, $"*, {nameof(UserMessage.Message)}" + "{*}", paging, cancellation).Map(p => p.Message);
	}

	public async ValueTask<bool> SetAvatarAsync(uint userId, string avatar, CancellationToken cancellation = default)
	{
		return await this.DataAccess.UpdateAsync(this.Name, new
		{
			Avatar = avatar
		}, Condition.Equal(nameof(UserProfile.UserId), userId), cancellation) > 0;
	}

	public async ValueTask<bool> SetPhotoPathAsync(uint userId, string path, CancellationToken cancellation = default)
	{
		return await this.DataAccess.UpdateAsync(this.Name, new
		{
			PhotoPath = path
		}, Condition.Equal(nameof(UserProfile.UserId), userId), cancellation) > 0;
	}
	#endregion

	#region 文件路径
	public string GetFilePath(uint userId, string name)
	{
		if(string.IsNullOrWhiteSpace(name))
			throw new ArgumentNullException(nameof(name));

		if(string.Equals(name, "avatar", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "photo", StringComparison.OrdinalIgnoreCase))
		{
			var basePath = string.IsNullOrWhiteSpace(_basePath) ? Utility.GetFilePath(0, userId) : _basePath;
			return Zongsoft.IO.Path.Combine(basePath, name.Trim() + "-" + userId.ToString());
		}

		throw new ArgumentNullException($"Invalid '{name}' value of the name argument.");
	}
	#endregion
	#region 异步业务路径
	protected override async ValueTask<int> OnUpdateAsync(IDataDictionary<UserProfile> data, ICondition criteria, ISchema schema, DataUpdateOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		//如果没有指定用户编号或指定的用户编号为零，则显式指定为当前用户编号
		if(!data.TryGetValue(p => p.UserId, out var userId) || userId == 0)
			data.SetValue(p => p.UserId, userId = this.Principal.Identity.GetIdentifier<uint>());

		//调用基类同名方法
		return await base.OnUpdateAsync(data, criteria, schema, options, cancellation);
	}
	#endregion
}
