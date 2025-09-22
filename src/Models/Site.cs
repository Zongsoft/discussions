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
/// 表示站点的实体类。
/// </summary>
public abstract class Site
{
	#region 普通属性
	/// <summary>获取或设置站点编号。</summary>
	public abstract uint SiteId { get; set; }
	/// <summary>获取或设置站点代号。</summary>
	public abstract string SiteNo { get; set; }
	/// <summary>获取或设置站点名称。</summary>
	public abstract string Name { get; set; }
	/// <summary>获取或设置站点域名。</summary>
	public abstract string Host { get; set; }
	/// <summary>获取或设置站点图标。</summary>
	public abstract string Icon { get; set; }
	/// <summary>获取或设置所属领域。</summary>
	public abstract string Domain { get; set; }
	/// <summary>获取或设置描述信息。</summary>
	public abstract string Description { get; set; }
	#endregion

	#region 集合属性
	/// <summary>获取或设置论坛组集合。</summary>
	public abstract ICollection<ForumGroup> ForumGroups { get; set; }
	/// <summary>获取或设置论坛集合。</summary>
	public abstract ICollection<Forum> Forums { get; set; }
	#endregion

	#region 嵌套结构
	/// <summary>
	/// 表示站点用户的结构。
	/// </summary>
	public struct SiteUser : IEquatable<SiteUser>
	{
		#region 构造函数
		public SiteUser(uint siteId, uint userId, Site site = null, UserProfile user = null)
		{
			this.SiteId = siteId;
			this.UserId = userId;
			this.Site = site;
			this.User = user;
		}
		#endregion

		#region 公共属性
		public uint SiteId { get; set; }
		public uint UserId { get; set; }
		public Site Site { get; set; }
		public UserProfile User { get; set; }
		#endregion

		#region 重写方法
		public bool Equals(SiteUser other) => this.SiteId == other.SiteId && this.UserId == other.UserId;
		public override bool Equals(object obj) => obj is SiteUser other && this.Equals(other);
		public override int GetHashCode() => HashCode.Combine(this.SiteId, this.UserId);
		public override string ToString() => $"{this.SiteId}-{this.UserId}";

		public static bool operator ==(SiteUser left, SiteUser right) => left.Equals(right);
		public static bool operator !=(SiteUser left, SiteUser right) => !(left == right);
		#endregion
	}
	#endregion
}

/// <summary>
/// 表示站点查询条件的实体类。
/// </summary>
public abstract class SiteCriteria : CriteriaBase
{
	/// <summary>获取或设置站点代号。</summary>
	public abstract string SiteNo { get; set; }
	/// <summary>获取或设置站点名称。</summary>
	public abstract string Name { get; set; }
	/// <summary>获取或设置站点域名。</summary>
	public abstract string Host { get; set; }
	/// <summary>获取或设置所属领域。</summary>
	public abstract string Domain { get; set; }
}