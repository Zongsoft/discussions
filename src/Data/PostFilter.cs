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
using System.Collections;

using Zongsoft.Data;
using Zongsoft.Security;

namespace Zongsoft.Discussions.Data;

[DataAccessFilter($"{Module.NAME}.{nameof(Models.Post)}")]
public class PostFilter : IDataAccessFilter<DataSelectContextBase>
{
	#region 构造函数
	public PostFilter() { }
	#endregion

	#region 过滤方法
	public ValueTask OnFilteringAsync(DataSelectContextBase context, CancellationToken cancellation = default) => ValueTask.CompletedTask;
	public ValueTask OnFilteredAsync(DataSelectContextBase context, CancellationToken cancellation = default)
	{
		if(context.Result == null)
			return ValueTask.CompletedTask;

		var identity = context.Principal?.Identity;
		context.Result = FilteredResult.Create(context, (item, token) => FilterAsync(item, identity, token));

		return ValueTask.CompletedTask;
	}
	#endregion

	#region 私有方法
	private static async ValueTask<bool> FilterAsync(object item, System.Security.Principal.IIdentity identity, CancellationToken cancellation)
	{
		var dictionary = DataDictionary.GetDictionary<Models.Post>(item);
		if(!dictionary.TryGetValue(p => p.Content, out var content))
			return true;

		if(!(dictionary.TryGetValue(p => p.Approved, out var approved) && approved) &&
		   !(identity?.IsAuthenticated == true && dictionary.TryGetValue(p => p.CreatorId, out var creatorId) && identity.GetIdentifier<uint>() == creatorId))
		{
			dictionary.TrySetValue(p => p.Content, string.Empty);
			if(dictionary.TryGetValue(p => p.ContentType, out var hiddenType))
				dictionary.TrySetValue(p => p.ContentType, Utility.GetContentType(hiddenType, true));
		}
		else if(dictionary.TryGetValue(p => p.ContentType, out var contentType) && !Utility.IsContentEmbedded(contentType))
		{
			dictionary.SetValue(p => p.Content, string.IsNullOrEmpty(content) ? string.Empty : await Utility.ReadTextFileAsync(content, cancellation));
			dictionary.SetValue(p => p.ContentType, Utility.GetContentType(contentType, true));
		}

		return true;
	}
	#endregion
}
