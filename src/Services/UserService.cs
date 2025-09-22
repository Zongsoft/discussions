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
using Zongsoft.Services;
using Zongsoft.Security;
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
	public IEnumerable<History> GetHistories(uint userId, Paging paging = null)
	{
		if(userId == 0)
			userId = this.Principal.Identity.GetIdentifier<uint>();

		return this.DataAccess.Select<History>(Condition.Equal(nameof(History.UserId), userId), $"*, {nameof(History.Thread)}" + "{*}", paging);
	}

	public int GetMessageTotalCount(uint userId = 0)
	{
		if(userId == 0)
			userId = this.Principal.Identity.GetIdentifier<uint>();

		return this.DataAccess.Count<UserMessage>(Condition.Equal(nameof(UserMessage.UserId), userId));
	}

	public int GetMessageUnreadCount(uint userId = 0)
	{
		if(userId == 0)
			userId = this.Principal.Identity.GetIdentifier<uint>();

		return this.DataAccess.Count<UserMessage>(Condition.Equal(nameof(UserMessage.UserId), userId) & Condition.Equal(nameof(UserMessage.IsRead), false));
	}

	public IEnumerable<Message> GetMessages(uint userId = 0, bool? isRead = null, Paging paging = null)
	{
		if(userId == 0)
			userId = this.Principal.Identity.GetIdentifier<uint>();

		var conditions = ConditionCollection.And(Condition.Equal(nameof(UserMessage.UserId), userId));

		if(isRead.HasValue)
			conditions.Add(Condition.Equal(nameof(UserMessage.IsRead), isRead.Value));

		return this.DataAccess.Select<UserMessage>(conditions, $"*, {nameof(UserMessage.Message)}" + "{*}", paging).Select(p => p.Message);
	}

	public bool SetAvatar(uint userId, string avatar)
	{
		return this.DataAccess.Update(this.Name, new
		{
			Avatar = avatar
		}, Condition.Equal(nameof(UserProfile.UserId), userId)) > 0;
	}

	public bool SetPhotoPath(uint userId, string path)
	{
		return this.DataAccess.Update(this.Name, new
		{
			PhotoPath = path
		}, Condition.Equal(nameof(UserProfile.UserId), userId)) > 0;
	}
	#endregion

	#region 重写方法
	protected override int OnUpdate(IDataDictionary<UserProfile> data, ICondition criteria, ISchema schema, DataUpdateOptions options)
	{
		//如果没有指定用户编号或指定的用户编号为零，则显式指定为当前用户编号
		if(!data.TryGetValue(p => p.UserId, out var userId) || userId == 0)
			data.SetValue(p => p.UserId, userId = this.Principal.Identity.GetIdentifier<uint>());

		//调用基类同名方法
		return base.OnUpdate(data, criteria, schema, options);
	}
	#endregion

	#region 文件路径
	public string GetFilePath(uint userId, string name)
	{
		if(string.IsNullOrWhiteSpace(name))
			throw new ArgumentNullException(nameof(name));

		if(string.Equals(name, "avatar", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "photo", StringComparison.OrdinalIgnoreCase))
			return Zongsoft.IO.Path.Combine(_basePath, name.Trim() + "-" + userId.ToString());

		throw new ArgumentNullException($"Invalid '{name}' value of the name argument.");
	}
	#endregion
}
