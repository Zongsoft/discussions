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
using Zongsoft.Common;
using Zongsoft.Services;
using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Services;

[Service(nameof(SiteService))]
[DataService(typeof(SiteCriteria))]
public class SiteService : DataServiceBase<Site>
{
	#region 构造函数
	public SiteService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 公共方法
	public IAsyncEnumerable<ForumGroup> GetForumGroupsAsync(uint siteId, CancellationToken cancellation = default) =>
		this.DataAccess.SelectAsync<ForumGroup>(Condition.Equal(nameof(ForumGroup.SiteId), siteId), cancellation);

	public IAsyncEnumerable<Forum> GetForumsAsync(uint siteId, CancellationToken cancellation = default) => this.GetForumsAsync(siteId, 0, cancellation);
	public IAsyncEnumerable<Forum> GetForumsAsync(uint siteId, ushort groupId, CancellationToken cancellation = default)
	{
		return groupId == 0 ?
			this.DataAccess.SelectAsync<Forum>(
				Condition.Equal(nameof(Forum.SiteId), siteId), cancellation) :
			this.DataAccess.SelectAsync<Forum>(
				Condition.Equal(nameof(Forum.SiteId), siteId) &
				Condition.Equal(nameof(Forum.GroupId), groupId), cancellation);
	}
	#endregion

	#region 重写方法
	protected override async ValueTask OnValidateAsync(DataServiceMethod method, ISchema schema, IDataDictionary<Site> data, IDataMutateOptions options, CancellationToken cancellation = default)
	{
		if(method.IsInsert)
		{
			data.TrySetValue(nameof(Site.SiteNo), () => $"S{Randomizer.GenerateString()}", string.IsNullOrWhiteSpace);
		}

		await base.OnValidateAsync(method, schema, data, options, cancellation);
	}
	#endregion
}
