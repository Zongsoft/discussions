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
