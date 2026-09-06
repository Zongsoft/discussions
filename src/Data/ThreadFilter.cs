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
using System.Linq;
using System.Collections;
using System.Runtime.CompilerServices;

using Zongsoft.Data;
using Zongsoft.Security;

namespace Zongsoft.Discussions.Data;

[DataAccessFilter($"{Module.NAME}.{nameof(Models.Thread)}")]
public class ThreadFilter : IDataAccessFilter<DataSelectContextBase>
{
	//保留被脱敏实例的正文，供服务完成版主授权后恢复；不把原文放入可序列化的实体属性。
	private static readonly ConditionalWeakTable<Models.Thread, HiddenContent> _contents = new();

	#region 构造函数
	public ThreadFilter() { }
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
	internal static void RestoreContent(Models.Thread thread)
	{
		if(thread == null || !_contents.TryGetValue(thread, out var content))
			return;

		_contents.Remove(thread);
		if(!ReferenceEquals(thread.Post, content.Post))
			return;

		thread.Post.Content = string.IsNullOrEmpty(content.Text) || Utility.IsContentEmbedded(content.Type) ? content.Text : Utility.ReadTextFile(content.Text);
		thread.Post.ContentType = Utility.GetContentType(content.Type, true);
	}

	private static bool Filter(object item, System.Security.Principal.IIdentity identity)
	{
		var dictionary = DataDictionary.GetDictionary<Models.Thread>(item);
		if(!dictionary.TryGetValue(p => p.Post, out var post) || post == null)
			return true;

		if(!(dictionary.TryGetValue(p => p.Approved, out var approved) && approved) &&
		   !(identity?.IsAuthenticated == true && dictionary.TryGetValue(p => p.CreatorId, out var creatorId) && identity.GetIdentifier<uint>() == creatorId))
		{
			if(item is Models.Thread thread && !string.IsNullOrEmpty(post.Content))
				_contents.GetValue(thread, _ => new HiddenContent(post, post.Content, post.ContentType));

			post.Content = string.Empty;
			post.ContentType = Utility.GetContentType(post.ContentType, true);
		}
		else if(!string.IsNullOrEmpty(post.Content) && !Utility.IsContentEmbedded(post.ContentType))
		{
			post.Content = Utility.ReadTextFile(post.Content);
			post.ContentType = Utility.GetContentType(post.ContentType, true);
		}

		return true;
	}

	private sealed record HiddenContent(Models.Post Post, string Text, string Type);
	#endregion
}
