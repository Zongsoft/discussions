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

using Zongsoft.Data;
using Zongsoft.Services;
using Zongsoft.Components;

[assembly: ApplicationModule(Zongsoft.Discussions.Module.NAME)]

namespace Zongsoft.Discussions;

public class Module : ApplicationModule<Module.EventRegistry>
{
	#region 常量定义
	/// <summary>表示论坛模块的名称常量值。</summary>
	public const string NAME = nameof(Discussions);
	#endregion

	#region 单例字段
	public static readonly Module Current = new();
	#endregion

	#region 构造函数
	public Module() : base(NAME, Discussions.Properties.Resources.Discussions, Discussions.Properties.Resources.Discussions_Description) { }
	#endregion

	#region 公共属性
	private IDataAccess _accessor;
	public IDataAccess Accessor => _accessor ??= this.Services.ResolveRequired<IDataAccessProvider>().GetAccessor(this.Name);
	#endregion

	#region 嵌套子类
	public sealed class EventRegistry : EventRegistryBase
	{
		#region 构造函数
		public EventRegistry() : base(NAME)
		{
		}
		#endregion
	}
	#endregion
}
