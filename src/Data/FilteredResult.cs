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

using Zongsoft.Data;

namespace Zongsoft.Discussions.Data;

internal static class FilteredResult
{
	public static object Create(DataSelectContextBase context, Func<object, CancellationToken, ValueTask<bool>> filter) =>
		Activator.CreateInstance(typeof(Result<>).MakeGenericType(context.ModelType ?? typeof(object)), context.Result, filter);

	private sealed class Result<T>(object source, Func<object, CancellationToken, ValueTask<bool>> filter) : IAsyncEnumerable<T>, IPageable
	{
		public bool Suppressed => (source as IPageable)?.Suppressed ?? false;
		public event Func<object, PagingEventArgs, CancellationToken, ValueTask> Paginated
		{
			add
			{
				if(source is IPageable pageable)
					pageable.Paginated += value;
			}
			remove
			{
				if(source is IPageable pageable)
					pageable.Paginated -= value;
			}
		}

		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellation = default) =>
			Collections.Enumerable.EnumerateAsync<T>(source, cancellation)
				.FilterAsync(async (item, token) => new Pageable.FilterResult<T> { Accepted = await filter(item, token), Data = item })
				.GetAsyncEnumerator(cancellation);
	}
}