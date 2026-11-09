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

[ControllerName("ForumGroups")]
public class ForumGroupController : ServiceController<ForumGroup, ForumGroupService>
{
	#region 公共方法
	[ActionName("Forums")]
	[HttpGet("{siteId}-{groupId}/[action]")]
	public IAsyncEnumerable<Forum> GetForums(uint siteId, ushort groupId, CancellationToken cancellation = default)
	{
		return this.DataService.GetForumsAsync(siteId, groupId, cancellation);
	}
	#endregion
}
