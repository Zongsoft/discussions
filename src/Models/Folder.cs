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

namespace Zongsoft.Discussions.Models;

/// <summary>
/// 表示文件夹的业务实体类。
/// </summary>
public abstract class Folder
{
	#region 公共属性
	/// <summary>获取或设置文件夹编号。</summary>
	public abstract uint FolderId { get; set; }

	/// <summary>获取或设置文件夹名称。</summary>
	public abstract string Name { get; set; }

	/// <summary>获取或设置名称缩写。</summary>
	public abstract string Acronym { get; set; }

	/// <summary>获取或设置图标名。</summary>
	public abstract string Icon { get; set; }

	/// <summary>获取或设置所属站点编号。</summary>
	public abstract uint SiteId { get; set; }

	/// <summary>获取或设置分享性。</summary>
	public abstract Shareability Shareability { get; set; }

	/// <summary>获取或设置创建人编号。</summary>
	public abstract uint CreatorId { get; set; }

	/// <summary>获取或设置创建人对象。</summary>
	public abstract UserProfile Creator { get; set; }

	/// <summary>获取或设置创建时间。</summary>
	public abstract DateTime CreatedTime { get; set; }
	#endregion

	#region 关联属性
	/// <summary>获取或设置文件夹指定用户集合。</summary>
	public abstract IEnumerable<FolderUser> Users { get; set; }
	#endregion

	#region 嵌套结构
	/// <summary>
	/// 表示文件夹用户的实体结构。
	/// </summary>
	public struct FolderUser : IEquatable<FolderUser>
	{
		#region 公共字段
		/// <summary>文件夹编号。</summary>
		public uint FolderId;
		/// <summary>用户编号。</summary>
		public uint UserId;
		/// <summary>权限定义。</summary>
		public Permission Permission;
		/// <summary>过期时间，如果为空(null)则表示永不过期。</summary>
		public DateTime? Expiration;
		/// <summary>文件夹对象。</summary>
		public Folder Folder;
		/// <summary>用户对象。</summary>
		public UserProfile User;
		#endregion

		#region 重写方法
		public bool Equals(FolderUser other) => this.FolderId == other.FolderId && this.UserId == other.UserId;
		public override bool Equals(object obj) => obj is FolderUser other && this.Equals(other);
		public override int GetHashCode() => HashCode.Combine(this.FolderId, this.UserId);
		public override string ToString() => this.Expiration == null ?
			$"{this.FolderId}-{this.UserId}:{this.Permission}" :
			$"{this.FolderId}-{this.UserId}:{this.Permission}@{this.Expiration}";
		#endregion

		#region 重写符号
		public static bool operator ==(FolderUser left, FolderUser right) => left.Equals(right);
		public static bool operator !=(FolderUser left, FolderUser right) => !(left == right);
		#endregion
	}
	#endregion
}

/// <summary>
/// 表示文件夹的查询条件实体类。
/// </summary>
public abstract class FolderCriteria : CriteriaBase
{
	#region 公共属性
	[Condition("Name", "Acronym")]
	public abstract string Name { get; set; }

	[Condition(ConditionOperator.In)]
	public abstract Shareability[] Shareability { get; set; }

	public abstract uint? CreatorId { get; set; }

	public abstract Range<DateTime> CreatedTime { get; set; }
	#endregion
}
