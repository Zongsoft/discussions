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
