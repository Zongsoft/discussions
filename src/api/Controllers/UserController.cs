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
using Zongsoft.Data;
using Zongsoft.Security.Privileges;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;
using Zongsoft.Web.Security;

namespace Zongsoft.Discussions.Web.Controllers;

[Authorization]
[ControllerName("Users")]
public class UserController : ServiceController<UserProfile, UserService>
{
    #region 公共方法
    [ActionName("Count")]
    [HttpGet("[area]/[controller]/{id}/[action]/{args}")]
    public IActionResult GetCount(uint id, string args)
    {
        if (string.IsNullOrEmpty(args))
            return this.BadRequest("Missing arguments of the request.");

		return args.ToLowerInvariant() switch
		{
			"unread" or "message-unread" => this.Ok(this.DataService.GetMessageUnreadCount(id)),
			"message" or "message-total" => this.Ok(this.DataService.GetMessageTotalCount(id)),
			_ => this.BadRequest("Invalid argument value."),
		};
	}

    [ActionName("Avatar")]
    [HttpPost("[area]/[controller]/{id}/[action]")]
    public Task<IO.FileInfo> SetAvatar(uint id) => SetAvatar(id);

    [ActionName("Photo")]
    [HttpPost("[area]/[controller]/{id}/[action]")]
    public Task<IO.FileInfo> SetPhoto(uint id) => SetPhoto(id);
    #endregion
}
