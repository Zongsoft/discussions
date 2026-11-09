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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Zongsoft.Web;
using Zongsoft.Data;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Web.Controllers;

[ControllerName("Posts")]
public class PostController : ServiceController<Post, PostService>
{
	#region 公共方法
	[ActionName("Upvote")]
	[HttpPost("{id}/[action]/{value?}")]
	public async ValueTask<IActionResult> Upvote(ulong id, byte value = 1, CancellationToken cancellation = default)
	{
		return await this.DataService.UpvoteAsync(id, value, cancellation) ? this.NoContent() : this.NotFound();
	}

	[ActionName("Downvote")]
	[HttpPost("{id}/[action]/{value?}")]
	public async ValueTask<IActionResult> Downvote(ulong id, byte value = 1, CancellationToken cancellation = default)
	{
		return await this.DataService.DownvoteAsync(id, value, cancellation) ? this.NoContent() : this.NotFound();
	}

	[ActionName("Upvotes")]
	[HttpGet("{id}/[action]")]
	public IAsyncEnumerable<Post.PostVoting> GetUpvotes(ulong id, [FromQuery] Paging page = null, CancellationToken cancellation = default)
	{
		return this.DataService.GetUpvotesAsync(id, page, cancellation);
	}

	[ActionName("Downvotes")]
	[HttpGet("{id}/[action]")]
	public IAsyncEnumerable<Post.PostVoting> GetDownvotes(ulong id, [FromQuery] Paging page = null, CancellationToken cancellation = default)
	{
		return this.DataService.GetDownvotesAsync(id, page, cancellation);
	}

	[ActionName("Comments")]
	[HttpGet("{id}/[action]")]
	public IAsyncEnumerable<Post> GetComments(ulong id, [FromQuery] Paging page = null, CancellationToken cancellation = default)
	{
		return this.DataService.GetCommentsAsync(id, page, cancellation);
	}
	#endregion
}
