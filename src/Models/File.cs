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
using System.ComponentModel;

using Zongsoft.Data;

namespace Zongsoft.Discussions.Models;

/// <summary>
/// 表示文件的实体类。
/// </summary>
public abstract class File
{
	#region 公共属性
	/// <summary>获取或设置文件编号。</summary>
	public abstract ulong FileId { get; set; }

	/// <summary>获取或设置所属的站点编号。</summary>
	public abstract uint SiteId { get; set; }

	/// <summary>获取或设置用户所属文件夹编号。</summary>
	public abstract uint FolderId { get; set; }

	/// <summary>获取或设置用户所属的文件夹对象。</summary>
	public abstract Folder Folder { get; set; }

	/// <summary>获取或设置附件的原始文件名。</summary>
	public abstract string Name { get; set; }

	/// <summary>获取或设置名称缩写。</summary>
	public abstract string Acronym { get; set; }

	/// <summary>获取或设置附件文件的路径。</summary>
	public abstract Zongsoft.IO.PathLocation Path { get; set; }

	/// <summary>获取或设置附件的文件类型(MIME类型)。</summary>
	public abstract string Type { get; set; }

	/// <summary>获取或设置附件文件的大小(单位：Byte)。</summary>
	public abstract uint Size { get; set; }

	/// <summary>获取或设置标签集。</summary>
	[TypeConverter(typeof(TagsConverter))]
	public abstract string[] Tags { get; set; }

	/// <summary>获取或设置附件的上传用户编号。</summary>
	public abstract uint? CreatorId { get; set; }

	/// <summary>获取或设置创建人的<see cref="Models.UserProfile"/>用户信息。</summary>
	public abstract UserProfile Creator { get; set; }

	/// <summary>获取或设置附件的上传时间。</summary>
	public abstract DateTime CreatedTime { get; set; }

	/// <summary>获取或设置描述信息。</summary>
	public abstract string Description { get; set; }
	#endregion
}

/// <summary>
/// 表示文件查询条件的实体类。
/// </summary>
public abstract class FileCriteria : CriteriaBase
{
	#region 公共属性
	public abstract ulong? FileId { get; set; }

	[Condition(ConditionOperator.Like)]
	public abstract string Name { get; set; }

	public abstract string Type { get; set; }

	public abstract Range<uint>? Size { get; set; }

	public abstract uint? FolderId { get; set; }

	public abstract uint? CreatorId { get; set; }

	public abstract Range<DateTime>? CreatedTime { get; set; }
	#endregion
}
