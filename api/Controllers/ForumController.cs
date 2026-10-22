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
using Zongsoft.Web.Http;
using Zongsoft.Data;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Web.Controllers;

[ControllerName("Forums")]
public class ForumController : ServiceController<Forum, ForumService>
{
	#region 公共方法
	[ActionName("Moderators")]
	[HttpGet("{id}/[action]")]
	public IAsyncEnumerable<UserProfile> GetModerators(ushort id, CancellationToken cancellation = default)
	{
		return this.DataService.GetModeratorsAsync(id, this.Request.Headers.GetDataSchema(), cancellation);
	}

	[ActionName("Globals")]
	[HttpGet("{id}/[action]")]
	public IAsyncEnumerable<Models.Thread> GetGlobalThreads(ushort id, [FromQuery] Paging page = null, CancellationToken cancellation = default)
	{
		return this.DataService.GetGlobalThreadsAsync(id, this.Request.Headers.GetDataSchema(), page, cancellation);
	}

	[ActionName("Pinneds")]
	[HttpGet("{id}/[action]")]
	public IAsyncEnumerable<Models.Thread> GetPinnedThreads(ushort id, [FromQuery] Paging page = null, CancellationToken cancellation = default)
	{
		return this.DataService.GetPinnedThreadsAsync(id, this.Request.Headers.GetDataSchema(), page, cancellation);
	}

	[ActionName("Topmosts")]
	[HttpGet("{id}/[action]")]
	public async ValueTask<IEnumerable<Models.Thread>> GetTopmosts(ushort id, [FromQuery]int count = 0, CancellationToken cancellation = default)
	{
		return await this.DataService.GetTopmostsAsync(id, this.Request.Headers.GetDataSchema(), count, cancellation);
	}

	[ActionName("Threads")]
	[HttpGet("{id}/[action]")]
	public async ValueTask<object> GetThreads(ushort id, [FromQuery] Paging page = null, CancellationToken cancellation = default)
	{
		return this.Paginate(page ??= Paging.First(), await this.DataService.GetThreadsAsync(id, this.Request.Headers.GetDataSchema(), page, cancellation));
	}
	#endregion
}
