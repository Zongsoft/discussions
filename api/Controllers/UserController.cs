/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Zongsoft.Web;
using Zongsoft.Web.Security;
using Zongsoft.Security.Privileges;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Web.Controllers;

[Authorization]
[ControllerName("Users")]
public class UserController : ServiceController<UserProfile, UserService>
{
	#region 公共方法
	[ActionName("Count")]
	[HttpGet("{id}/[action]/{args}")]
	public async ValueTask<IActionResult> GetCount(uint id, string args, CancellationToken cancellation = default)
	{
		if(string.IsNullOrEmpty(args))
			return this.BadRequest("Missing arguments of the request.");

		return args.ToLowerInvariant() switch
		{
			"unread" or "message-unread" => this.Ok(await this.DataService.GetMessageUnreadCountAsync(id, cancellation)),
			"message" or "message-total" => this.Ok(await this.DataService.GetMessageTotalCountAsync(id, cancellation)),
			_ => this.BadRequest("Invalid argument value."),
		};
	}

	[ActionName("Avatar")]
	[HttpPost("{id}/[action]")]
	public Task<IO.FileInfo> SetAvatar(uint id) => this.UploadAsync(
		this.DataService.GetFilePath(id, "avatar"),
		(file, cancellation) => file != null ? this.DataService.SetAvatarAsync(id, file.Path.Url, cancellation) : ValueTask.FromResult(false),
		this.HttpContext.RequestAborted).AsTask();

	[ActionName("Photo")]
	[HttpPost("{id}/[action]")]
	public Task<IO.FileInfo> SetPhoto(uint id) => this.UploadAsync(
		this.DataService.GetFilePath(id, "photo"),
		(file, cancellation) => file != null ? this.DataService.SetPhotoPathAsync(id, file.Path.Url, cancellation) : ValueTask.FromResult(false),
		this.HttpContext.RequestAborted).AsTask();
	#endregion
}
