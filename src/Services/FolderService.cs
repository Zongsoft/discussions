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

[Service(nameof(FolderService))]
[DataService(typeof(FolderCriteria))]
public class FolderService : DataServiceBase<Folder>
{
	#region 构造函数
	public FolderService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 公共方法
	public async ValueTask<bool> SetIconAsync(uint folderId, string icon, CancellationToken cancellation = default)
	{
		if(string.IsNullOrWhiteSpace(icon))
			icon = null;

		return await this.DataAccess.UpdateAsync<Folder>(new { Icon = icon }, Condition.Equal(nameof(Folder.FolderId), folderId), cancellation) > 0;
	}

	public async ValueTask<bool> SetShareabilityAsync(uint folderId, Shareability shareability, CancellationToken cancellation = default)
	{
		return await this.DataAccess.UpdateAsync<Folder>(new { Shareability = shareability }, Condition.Equal(nameof(Folder.FolderId), folderId), cancellation) > 0;
	}
	#endregion

	protected override async ValueTask<int> OnUpdateAsync(IDataDictionary<Folder> data, ICondition criteria, ISchema schema, DataUpdateOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		await using var transaction = new Transaction();
		var count = await base.OnUpdateAsync(data, criteria, schema, options, cancellation);

		if(count < 1)
			return count;

		if(data.TryGetValue(p => p.Users, out var users) && users != null)
		{
			var folderId = data.GetValue(p => p.FolderId);
			await this.DataAccess.DeleteAsync<Folder.FolderUser>(Condition.Equal(nameof(Folder.FolderUser.FolderId), folderId), cancellation);
			await this.DataAccess.InsertManyAsync(users.Where(p => p.FolderId == folderId), cancellation);
		}

		await transaction.CommitAsync(cancellation);
		return count;
	}
}
