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
	public override async ValueTask<IDataTemplateModel> GetModelAsync(IDataTemplate template, object argument, CancellationToken cancellation = default)
	{
		var schema = $"*, {nameof(UserProfile.Site)}" + "{*}";

		if(argument is Stream stream)
			argument = await Serializer.Json.DeserializeAsync<UserProfileCriteria>(stream, cancellation);

		object data = argument switch
		{
			string key => await this.Services.ResolveRequired<UserService>().GetAsync(key, schema, cancellation),
			IModel model => this.Services.ResolveRequired<UserService>().SelectAsync(Criteria.Transform(model), schema, cancellation),
			_ => this.Services.ResolveRequired<UserService>().SelectAsync(null, schema, cancellation),
		};

		return new DataTemplateModel(new { Users = data });
	}
	#endregion
}
