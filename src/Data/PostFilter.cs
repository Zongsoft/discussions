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
	public void OnFiltered(DataSelectContextBase context) { }
	public void OnFiltering(DataSelectContextBase context) => context.Result = new FilteredResult(context);
	#endregion

	#region 嵌套子类
	private class FilteredResult(DataSelectContextBase context) : IEnumerable
	{
		private readonly IEnumerable _result = context.Result;
		private readonly System.Security.Claims.ClaimsPrincipal _principal = context.Principal;

		public IEnumerator GetEnumerator() => new Iterator(_result, _principal);

		private sealed class Iterator(IEnumerable result, System.Security.Claims.ClaimsPrincipal principal) : IEnumerator
		{
			private readonly IEnumerator _result = result.GetEnumerator();
			private readonly System.Security.Claims.ClaimsPrincipal _principal = principal;

			public object Current
			{
				get
				{
					var dictionary = DataDictionary.GetDictionary<Models.Post>(_result.Current);

					if(dictionary.TryGetValue(p => p.Approved, out var approved) && !approved &&
					  (!_principal.Identity.IsAuthenticated || _principal.Identity.GetIdentifier<uint>() != dictionary.GetValue(p => p.CreatorId, 0U)))
					{
						dictionary.TrySetValue(p => p.Content, string.Empty);
					}
					else if(dictionary.TryGetValue(p => p.Content, out var content) && dictionary.TryGetValue(p => p.ContentType, out var contentType))
					{
						if(!Utility.IsContentEmbedded(contentType))
							dictionary.SetValue(p => p.Content, Utility.ReadTextFile(content));
					}

					return _result.Current;
				}
			}

			public bool MoveNext() => _result.MoveNext();
			public void Reset() => _result.Reset();
		}
	}
	#endregion
}
