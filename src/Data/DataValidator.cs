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
using System.Collections.Generic;

using Zongsoft.Data;
using Zongsoft.Data.Metadata;

using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Security;

namespace Zongsoft.Discussions.Data;

public class DataValidator : IDataValidator
{
	#region 委托定义
	private delegate bool TryGetDelegate<in TContext>(TContext context, out object value) where TContext : IDataAccessContextBase;
	#endregion

	#region 静态字段
	private static readonly IDictionary<string, TryGetDelegate<DataImportContextBase>> _imports;
	private static readonly IDictionary<string, TryGetDelegate<IDataMutateContextBase>> _inserts;
	private static readonly IDictionary<string, TryGetDelegate<IDataMutateContextBase>> _updates;
	#endregion

	#region 静态构造
	static DataValidator()
	{
		_imports = new Dictionary<string, TryGetDelegate<DataImportContextBase>>(StringComparer.OrdinalIgnoreCase)
		{
			{ "SiteId", TryGetSiteId },

			{ "CreatorId",  TryGetUserId },
			{ "CreatedTime", TryGetTimestamp },
			{ "Creation", TryGetTimestamp },
		};

		_inserts = new Dictionary<string, TryGetDelegate<IDataMutateContextBase>>(StringComparer.OrdinalIgnoreCase)
		{
			{ "SiteId", TryGetSiteId },

			{ "CreatorId",  TryGetUserId },
			{ "CreatedTime", TryGetTimestamp },
			{ "Creation", TryGetTimestamp },
		};

		_updates = new Dictionary<string, TryGetDelegate<IDataMutateContextBase>>(StringComparer.OrdinalIgnoreCase)
		{
			{ "ModifierId",  TryGetUserId },
			{ "ModifiedTime", TryGetTimestamp },
			{ "Modification", TryGetTimestamp },
		};
	}
	#endregion

	#region 公共方法
	public ICondition Validate(IDataAccessContextBase context, ICondition criteria)
	{
		if(UserIdentity.Current == null)
			return criteria;

		if(HasProperty(context, Fields.SiteId) && (criteria == null || !criteria.Contains(Fields.SiteId, 1)))
			criteria &= Condition.Equal(Fields.SiteId, UserIdentity.Current.SiteId);

		return criteria;
	}

	public bool OnImport(DataImportContextBase context, IDataEntityProperty property, out object value)
	{
		if(_imports.TryGetValue(property.Name, out var factory))
			return factory.Invoke(context, out value);

		value = null;
		return false;
	}

	public bool OnInsert(IDataMutateContextBase context, IDataEntityProperty property, out object value)
	{
		if(_inserts.TryGetValue(property.Name, out var factory))
			return factory.Invoke(context, out value);

		value = null;
		return false;
	}

	public bool OnUpdate(IDataMutateContextBase context, IDataEntityProperty property, out object value)
	{
		if(_updates.TryGetValue(property.Name, out var factory))
			return factory.Invoke(context, out value);

		value = null;
		return false;
	}
	#endregion

	#region 私有方法
	private static bool HasProperty(IDataAccessContextBase context, string name) => context switch
	{
		DataExistContextBase exist => exist.Entity.Properties.Contains(name),
		DataSelectContextBase select => select.Entity.Properties.Contains(name),
		IDataMutateContextBase mutate => mutate.Entity.Properties.Contains(name),
		DataAggregateContextBase aggregate => aggregate.Entity.Properties.Contains(name),
		_ => false,
	};

	private static bool TryGetSiteId(DataImportContextBase context, out object value)
	{
		value = UserIdentity.Current?.SiteId;
		return value != null;
	}

	private static bool TryGetUserId(DataImportContextBase context, out object value)
	{
		value = UserIdentity.Current?.UserId;
		return value != null;
	}

	private static bool TryGetTimestamp(DataImportContextBase context, out object value)
	{
		value = DateTime.Now;
		return true;
	}

	private static bool TryGetSiteId(IDataMutateContextBase context, out object value)
	{
		value = UserIdentity.Current?.SiteId;
		return value != null;
	}

	private static bool TryGetUserId(IDataMutateContextBase context, out object value)
	{
		value = UserIdentity.Current?.UserId;
		return value != null;
	}

	private static bool TryGetTimestamp(IDataMutateContextBase context, out object value)
	{
		value = DateTime.Now;
		return true;
	}
	#endregion
}
