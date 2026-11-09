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

[ControllerName("Sites")]
public class SiteController : ServiceController<Site, SiteService>
{
	#region 公共方法
	[ActionName("Forums")]
	[HttpGet("{siteId}/[action]")]
	public IAsyncEnumerable<Forum> GetForums(uint siteId, [FromQuery] ushort group = 0, CancellationToken cancellation = default) => this.DataService.GetForumsAsync(siteId, group, cancellation);

	[ActionName("ForumGroups")]
	[HttpGet("{siteId}/[action]")]
	public IAsyncEnumerable<ForumGroup> GetForumGroups(uint siteId, CancellationToken cancellation = default) => this.DataService.GetForumGroupsAsync(siteId, cancellation);
	#endregion
}
