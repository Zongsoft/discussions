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

using System.IO;
using Zongsoft.Data;
using Zongsoft.Data.Archiving;
using Zongsoft.Services;
using Zongsoft.Serialization;

using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Features.Archiving;

[Service(typeof(IDataTemplateModelProvider))]
public class UserDataTemplateModelProvider : DataTemplateModelProviderBase
{
	#region 构造函数
	public UserDataTemplateModelProvider(IServiceProvider services) : base("User-List", services) { }
	#endregion

	#region 公共属性
	[ServiceDependency("~", IsRequired = true)]
	public IDataAccess DataAccess { get; set; }
	#endregion

	#region 公共方法
	public override IDataTemplateModel GetModel(IDataTemplate template, object argument)
	{
		var schema = $"*, {nameof(UserProfile.Site)}" + "{*}";

		if(argument is Stream stream)
			argument = Serializer.Json.Deserialize<UserProfileCriteria>(stream);

		var data = argument switch
		{
			string key => this.Services.ResolveRequired<UserService>().Get(key, schema),
			IModel model => this.Services.ResolveRequired<UserService>().Select(Criteria.Transform(model), schema),
			_ => this.Services.ResolveRequired<UserService>().Select(null, schema),
		};

		return new DataTemplateModel(new { Users = data });
	}
	#endregion
}