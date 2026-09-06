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
	public void OnFiltering(DataSelectContextBase context) { }
	public void OnFiltered(DataSelectContextBase context)
	{
		if(context.Result == null)
			return;

		var identity = context.Principal?.Identity;
		context.Result = FilteredResult.Create(context, item => Filter(item, identity));
	}
	#endregion

	#region 私有方法
	private static bool Filter(object item, System.Security.Principal.IIdentity identity)
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
			dictionary.SetValue(p => p.Content, string.IsNullOrEmpty(content) ? string.Empty : Utility.ReadTextFile(content));
			dictionary.SetValue(p => p.ContentType, Utility.GetContentType(contentType, true));
		}

		return true;
	}
	#endregion
}
