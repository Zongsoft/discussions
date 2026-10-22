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