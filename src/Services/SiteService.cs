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
	public IEnumerable<ForumGroup> GetForumGroups(uint siteId) =>
		this.DataAccess.Select<ForumGroup>(Condition.Equal(nameof(ForumGroup.SiteId), siteId));

	public IEnumerable<Forum> GetForums(uint siteId, ushort groupId = 0)
	{
		return groupId == 0 ?
			this.DataAccess.Select<Forum>(
				Condition.Equal(nameof(Forum.SiteId), siteId)) :
			this.DataAccess.Select<Forum>(
				Condition.Equal(nameof(Forum.SiteId), siteId) &
				Condition.Equal(nameof(Forum.GroupId), groupId));
	}
	#endregion

	#region 重写方法
	protected override void OnValidate(DataServiceMethod method, ISchema schema, IDataDictionary<Site> data, IDataMutateOptions options)
	{
		if(method.IsInsert)
		{
			data.TrySetValue(nameof(Site.SiteNo), () => $"S{Randomizer.GenerateString()}", string.IsNullOrWhiteSpace);
		}

		base.OnValidate(method, schema, data, options);
	}
	#endregion
}
