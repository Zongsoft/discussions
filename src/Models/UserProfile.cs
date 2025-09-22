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
using Zongsoft.Components;
using Zongsoft.Security;
using Zongsoft.Security.Privileges;

namespace Zongsoft.Discussions.Models;

/// <summary>
/// 表示用户信息的实体类。
/// </summary>
public abstract class UserProfile : IUser
{
	#region 公共属性
	/// <summary>获取或设置用户所属站点编号。</summary>
	public abstract uint SiteId { get; set; }

	/// <summary>获取或设置用户所属站点对象。</summary>
	public abstract Site Site { get; set; }

	/// <summary>获取或设置用户编号。</summary>
	public abstract uint UserId { get; set; }

	/// <summary>获取或设置用户名称。</summary>
	[ModelProperty(ModelPropertyRole.Name, ModelPropertyFlags.Required)]
	public abstract string Name { get; set; }

	/// <summary>获取或设置用昵称。</summary>
	[ModelProperty(ModelPropertyRole.Name)]
	public abstract string Nickname { get; set; }

	/// <summary>获取或设置用户绑定的邮箱地址。</summary>
	[ModelProperty(ModelPropertyRole.Email)]
	public abstract string Email { get; set; }

	/// <summary>获取或设置用户绑定的手机号码。</summary>
	[ModelProperty(ModelPropertyRole.Phone)]
	public abstract string Phone { get; set; }

	/// <summary>获取或设置头像标识。</summary>
	public abstract string Avatar { get; set; }

	/// <summary>获取或设置一个值，指示是否可用。</summary>
	public abstract bool Enabled { get; set; }

	/// <summary>获取或设置用户性别。</summary>
	public abstract Gender Gender { get; set; }

	/// <summary>获取或设置用户等级，数字越大等级越高。</summary>
	public abstract byte Grade { get; set; }

	/// <summary>获取或设置用户的照片文件路径。</summary>
	public abstract Zongsoft.IO.PathLocation PhotoPath { get; set; }

	/// <summary>获取或设置用户累计发布的帖子总数。</summary>
	public abstract uint TotalPosts { get; set; }

	/// <summary>获取或设置用户累积发布的主题总数。</summary>
	public abstract uint TotalThreads { get; set; }

	/// <summary>获取或设置用户最后回帖的帖子编号。</summary>
	public abstract ulong? MostRecentPostId { get; set; }

	/// <summary>获取或设置用户最后回帖的时间。</summary>
	public abstract DateTime? MostRecentPostTime { get; set; }

	/// <summary>获取或设置用户最后回帖的帖子对象。</summary>
	public abstract Post MostRecentPost { get; set; }

	/// <summary>获取或设置用户最新发布的主题编号。</summary>
	public abstract ulong? MostRecentThreadId { get; set; }

	/// <summary>获取或设置用户最新发布的主题标题。</summary>
	public abstract string MostRecentThreadTitle { get; set; }

	/// <summary>获取或设置用户最新主题的发布时间。</summary>
	public abstract DateTime? MostRecentThreadTime { get; set; }

	/// <summary>获取或设置用户最新发布的主题对象。</summary>
	public abstract Thread MostRecentThread { get; set; }

	/// <summary>获取或设置用户的创建时间。</summary>
	public abstract DateTime Creation { get; set; }

	/// <summary>获取或设置用户的修改时间。</summary>
	public abstract DateTime? Modification { get; set; }

	/// <summary>获取或设置用户的描述信息。</summary>
	public abstract string Description { get; set; }
	#endregion

	#region 显式实现
	bool? IUser.Gender
	{
		get => this.Gender switch
		{
			Gender.Male => true,
			Gender.Female => false,
			_ => null,
		};
		set => this.Gender = value switch
		{
			true => Gender.Male,
			false => Gender.Female,
			_ => Gender.None,
		};
	}

	string IUser.Namespace { get; set; }
	Identifier IIdentifiable.Identifier
	{
		get => new(typeof(UserProfile), this.UserId);
		set => this.UserId = value.Validate<uint>(out var id) ? id : this.UserId;
	}
	#endregion
}

/// <summary>
/// 表示用户信息查询条件的实体类。
/// </summary>
public abstract class UserProfileCriteria : CriteriaBase
{
	#region 公共属性
	/// <summary>获取或设置用户标识。</summary>
	[Condition("Name", "Phone", "Email")]
	public abstract string Identity { get; set; }

	/// <summary>获取或设置用户性别。</summary>
	public abstract Gender? Gender { get; set; }

	/// <summary>获取或设置用户等级范围。</summary>
	public abstract Range<byte>? Grade { get; set; }
	#endregion
}

/// <summary>
/// 表示用户消息的结构。
/// </summary>
public struct UserMessage
{
	#region 构造函数
	public UserMessage(uint userId, ulong messageId, bool isRead = false)
	{
		this.MessageId = messageId;
		this.UserId = userId;
		this.IsRead = isRead;
		this.User = null;
		this.Message = null;
	}
	#endregion

	#region 公共属性
	public uint UserId { get; set; }
	/// <summary>获取或设置用户对象。</summary>
	public UserProfile User { get; set; }
	/// <summary>获取或设置一个值，指示用户是否已读该消息。</summary>
	/// <summary>获取或设置消息编号。</summary>
	public ulong MessageId { get; set; }
	/// <summary>获取或设置消息对象。</summary>
	public Message Message { get; set; }
	/// <summary>获取或设置用户编号。</summary>
	public bool IsRead { get; set; }
	#endregion
}
