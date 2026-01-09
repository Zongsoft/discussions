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
using System.Security.Claims;

using Zongsoft.Services;
using Zongsoft.Components;
using Zongsoft.Security;
using Zongsoft.Security.Privileges;

namespace Zongsoft.Discussions.Security;

public class UserIdentity : IUser
{
	#region 常量定义
	public const string Scheme = "Zongsoft.Discussions";
	#endregion

	#region 公共属性
	/// <summary>获取或设置用户所属站点编号。</summary>
	public uint SiteId { get; set; }

	/// <summary>获取或设置用户编号。</summary>
	public uint UserId { get; set; }

	/// <summary>获取或设置用户名称。</summary>
	public string Name { get; set; }

	/// <summary>获取或设置用户昵称。</summary>
	public string Nickname { get; set; }

	/// <summary>获取或设置用户绑定的邮箱地址。</summary>
	public string Email { get; set; }

	/// <summary>获取或设置用户绑定的手机号码。</summary>
	public string Phone { get; set; }

	/// <summary>获取或设置头像标识。</summary>
	public string Avatar { get; set; }

	/// <summary>获取或设置一个值，指示是否可用。</summary>
	public bool Enabled { get; set; }

	/// <summary>获取或设置用户性别。</summary>
	public Models.Gender Gender { get; set; }

	/// <summary>获取或设置用户等级，数字越大等级越高。</summary>
	public byte Grade { get; set; }

	/// <summary>获取或设置用户累计发布的帖子总数。</summary>
	public uint TotalPosts { get; set; }

	/// <summary>获取或设置用户累积发布的主题总数。</summary>
	public uint TotalThreads { get; set; }

	/// <summary>获取或设置用户描述。</summary>
	public string Description { get; set; }
	#endregion

	#region 显式实现
	bool? IUser.Gender
	{
		get => this.Gender switch
		{
			Models.Gender.Male => true,
			Models.Gender.Female => false,
			_ => null,
		};
		set => this.Gender = value switch
		{
			true => Models.Gender.Male,
			false => Models.Gender.Female,
			_ => Models.Gender.None,
		};
	}

	string IUser.Namespace { get; set; }
	Identifier IIdentifiable.Identifier
	{
		get => new(typeof(IUser), this.UserId);
		set => this.UserId = value.Validate<uint>(out var id) ? id : this.UserId;
	}
	#endregion

	#region 静态成员
	public static UserIdentity Current => ClaimsIdentityModeling.GetModel<UserIdentity>(Scheme);
	#endregion

	#region 嵌套子类
	internal sealed class Transformer : IClaimsIdentityTransformer
	{
		#region 单例字段
		public static readonly Transformer Instance = new();
		#endregion

		#region 私有构造
		private Transformer() { }
		#endregion

		#region 公共方法
		public bool CanTransform(ClaimsIdentity identity) => string.Equals(identity.AuthenticationType, UserIdentity.Scheme, StringComparison.OrdinalIgnoreCase);
		public object Transform(ClaimsIdentity identity) => identity.AsModel<UserIdentity>(this.OnTransform);
		#endregion

		#region 私有方法
		private bool OnTransform(UserIdentity user, Claim claim)
		{
			switch(claim.Type)
			{
				case nameof(UserIdentity.SiteId):
					user.SiteId = (claim.Value != null && uint.TryParse(claim.Value, out var siteId)) ? siteId : 0;
					return true;
				case nameof(UserIdentity.Gender):
					user.Gender = (claim.Value != null && Enum.TryParse<Models.Gender>(claim.Value, out var gender)) ? gender : Models.Gender.None;
					return true;
				case nameof(UserIdentity.Avatar):
					user.Avatar = claim.Value;
					return true;
				case nameof(UserIdentity.Grade):
					user.Grade = (claim.Value != null && byte.TryParse(claim.Value, out var grade)) ? grade : (byte)0;
					return true;
				case nameof(UserIdentity.TotalPosts):
					user.TotalPosts = (claim.Value != null && uint.TryParse(claim.Value, out var totalPosts)) ? totalPosts : 0;
					return true;
				case nameof(UserIdentity.TotalThreads):
					user.TotalThreads = (claim.Value != null && uint.TryParse(claim.Value, out var totalThreads)) ? totalThreads : 0;
					return true;
				default:
					return false;
			}
		}
		#endregion
	}
	#endregion
}