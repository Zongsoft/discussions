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
using System.Security.Claims;
using System.Security.Principal;
using Zongsoft.Collections;

using Zongsoft.Data;
using Zongsoft.Services;
using Zongsoft.Security;
using Zongsoft.Security.Privileges;

using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Security;

public class UserChallenger : IChallenger
{
	#region 单例字段
	public static readonly UserChallenger Instance = new();
	#endregion

	#region 私有构造
	private UserChallenger() { }
	#endregion

	#region 公共方法
	public async ValueTask ChallengeAsync(ClaimsPrincipal principal, string scenario, CancellationToken cancellation = default)
	{
		if(principal.Identity == null)
			throw new AuthenticationException(SecurityReasons.InvalidIdentity);

		var userId = principal.Identity.GetIdentifier<uint>();

		if(userId == 0)
			throw new AuthenticationException(SecurityReasons.InvalidIdentity);

		//获取当前主身份所对应的用户信息，如果没有对应的用户则创建一条对应的新用户信息a
		var user = await this.GetUserAsync(userId, cancellation) ?? await this.CreateUserAsync(principal.Identity, cancellation);

		if(user == null)
			throw new AuthenticationException(SecurityReasons.InvalidIdentity);

		//设置用户的命名空间
		if(string.IsNullOrEmpty(((IUser)user).Namespace))
			((IUser)user).Namespace = principal.Identity.GetNamespace();

		//执行身份校验
		this.OnVerify(user, scenario);

		//构建新的用户身份
		var identity = this.Identity(user);

		//将新的身份加入到主体中
		principal.AddIdentity(identity);
	}
	#endregion

	#region 虚拟方法
	protected virtual ValueTask<UserProfile> GetUserAsync(uint userId, CancellationToken cancellation) =>
		Module.Current.Accessor.SelectAsync<UserProfile>(
			Condition.Equal(nameof(UserProfile.UserId), userId),
			Paging.Limit(1),
			cancellation).FirstOrDefault(cancellation);

	protected virtual async ValueTask<UserProfile> CreateUserAsync(IIdentity identity, CancellationToken cancellation)
	{
		var user = identity.AsModel<UserProfile>();

		if(user.SiteId == 0)
		{
			var @namespace = identity.GetNamespace();

			if(!string.IsNullOrEmpty(@namespace))
			{
				if(uint.TryParse(@namespace, out var id))
					user.SiteId = id;
				else
				{
					var site = await Module.Current.Accessor.SelectAsync<Site>(
						Condition.Equal(nameof(Site.SiteNo), @namespace),
						$"{nameof(Site.SiteId)},{nameof(Site.SiteNo)}",
						Paging.Limit(1), cancellation).FirstOrDefault(cancellation);

					if(site != null)
						user.SiteId = site.SiteId;
				}
			}
		}

		return await Module.Current.Accessor.UpsertAsync(user, cancellation) > 0 ? user : null;
	}

	protected virtual void OnClaims(ClaimsIdentity identity, UserProfile user) { }
	protected virtual void OnVerify(UserProfile user, string scenario) { }
	#endregion

	#region 私有方法
	private ClaimsIdentity Identity(UserProfile user)
	{
		var identity = user.Identity(UserIdentity.Scheme, "Zongsoft");

		identity.SetClaim(nameof(UserProfile.SiteId), user.SiteId);
		identity.SetClaim(nameof(UserProfile.Gender), user.Gender);
		identity.SetClaim(nameof(UserProfile.Avatar), user.Avatar);
		identity.SetClaim(nameof(UserProfile.Grade), user.Grade);
		identity.SetClaim(nameof(UserProfile.TotalPosts), user.TotalPosts);
		identity.SetClaim(nameof(UserProfile.TotalThreads), user.TotalThreads);

		//进行其他声明定义
		this.OnClaims(identity, user);

		//返回新构建的身份
		return identity;
	}
	#endregion
}
