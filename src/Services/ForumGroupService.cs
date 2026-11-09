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

using Zongsoft.Data;
using Zongsoft.Services;
using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Services;

[Service(nameof(ForumGroupService))]
[DataService(typeof(ForumGroupCriteria))]
public class ForumGroupService : DataServiceBase<ForumGroup>
{
	#region 构造函数
	public ForumGroupService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 公共方法
	public IAsyncEnumerable<Forum> GetForumsAsync(uint siteId, ushort groupId, CancellationToken cancellation = default)
	{
		return this.DataAccess.SelectAsync<Forum>(
			Condition.Equal(nameof(Forum.SiteId), siteId) & Condition.Equal(nameof(Forum.GroupId), groupId), cancellation);
	}
	#endregion
}
