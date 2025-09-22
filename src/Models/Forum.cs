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
/// 表示论坛的业务实体类。
/// </summary>
public abstract class Forum
{
	#region 公共属性
	/// <summary>获取或设置站点编号，主键。</summary>
	public abstract uint SiteId { get; set; }

	/// <summary>获取或设置论坛编号，主键。</summary>
	public abstract ushort ForumId { get; set; }

	/// <summary>获取或设置论坛所属的组号。</summary>
	public abstract ushort GroupId { get; set; }

	/// <summary>获取或设置论坛所属的 <see cref="ForumGroup"/> 分组对象。</summary>
	public abstract ForumGroup Group { get; set; }

	/// <summary>获取或设置论坛所在分组中的排列序号。</summary>
	public abstract short Ordinal { get; set; }

	/// <summary>获取或设置论坛的名称(标题)。</summary>
	public abstract string Name { get; set; }

	/// <summary>获取或设置论坛的描述文本。</summary>
	public abstract string Description { get; set; }

	/// <summary>获取或设置论坛的封面图片路径。</summary>
	public abstract Zongsoft.IO.PathLocation CoverPicturePath { get; set; }

	/// <summary>获取或设置一个值，表示论坛是否为热门版块。</summary>
	public abstract bool IsPopular { get; set; }

	/// <summary>获取或设置一个值，表示发帖是否需要审核。</summary>
	public abstract bool Approvable { get; set; }

	/// <summary>获取或设置论坛的可见性。</summary>
	public abstract Visibility Visibility { get; set; }

	/// <summary>获取或设置论坛的可访问性。</summary>
	public abstract Accessibility Accessibility { get; set; }

	/// <summary>获取或设置论坛中的累计帖子总数。</summary>
	public abstract uint TotalPosts { get; set; }

	/// <summary>获取或设置论坛中的累积主题总数。</summary>
	public abstract uint TotalThreads { get; set; }

	/// <summary>获取或设置论坛中最新主题的编号。</summary>
	public abstract ulong? MostRecentThreadId { get; set; }

	/// <summary>获取或设置论坛中最新发布的主题对象。</summary>
	public abstract Thread MostRecentThread { get; set; }

	/// <summary>获取或设置论坛中最新主题的标题。</summary>
	public abstract string MostRecentThreadTitle { get; set; }

	/// <summary>获取或设置论坛中最新主题的作者编号。</summary>
	public abstract uint? MostRecentThreadAuthorId { get; set; }

	/// <summary>获取或设置论坛中最新主题的作者名称。</summary>
	public abstract string MostRecentThreadAuthorName { get; set; }

	/// <summary>获取或设置论坛中最新主题的作者头像。</summary>
	public abstract string MostRecentThreadAuthorAvatar { get; set; }

	/// <summary>获取或设置论坛中最新主题的发布时间。</summary>
	public abstract DateTime? MostRecentThreadTime { get; set; }

	/// <summary>获取或设置论坛中最近回帖编号。</summary>
	public abstract ulong? MostRecentPostId { get; set; }

	/// <summary>获取或设置论坛中最近的回帖对象。</summary>
	public abstract Post MostRecentPost { get; set; }

	/// <summary>获取或设置论坛中最近回帖的作者编号。</summary>
	public abstract uint? MostRecentPostAuthorId { get; set; }

	/// <summary>获取或设置论坛中最近回帖的作者名称。</summary>
	public abstract string MostRecentPostAuthorName { get; set; }

	/// <summary>获取或设置论坛中最近回帖的作者头像。</summary>
	public abstract string MostRecentPostAuthorAvatar { get; set; }

	/// <summary>获取或设置论坛中最近回帖的时间。</summary>
	public abstract DateTime? MostRecentPostTime { get; set; }

	/// <summary>获取或设置论坛创建人编号。</summary>
	public abstract uint CreatorId { get; set; }

	/// <summary>获取或设置论坛的创建时间。</summary>
	public abstract DateTime CreatedTime { get; set; }
	#endregion

	#region 集合属性
	/// <summary>获取或设置论坛成员集。</summary>
	public abstract IEnumerable<ForumUser> Users { get; set; }

	/// <summary>获取或设置论坛的版主集。</summary>
	public abstract IEnumerable<UserProfile> Moderators { get; set; }
	#endregion

	#region 嵌套结构
	/// <summary>
	/// 表示论坛用户的业务实体结构。
	/// </summary>
	public struct ForumUser : IEquatable<ForumUser>
	{
		#region 公共字段
		/// <summary>站点编号</summary>
		public uint SiteId;
		/// <summary>论坛编号</summary>
		public ushort ForumId;
		/// <summary>用户编号</summary>
		public uint UserId;
		/// <summary>权限定义</summary>
		public Permission Permission;
		/// <summary>是否为版主</summary>
		public bool IsModerator;
		/// <summary>论坛对象</summary>
		public Forum Forum;
		/// <summary>用户对象</summary>
		public UserProfile User;
		#endregion

		#region 构造函数
		private ForumUser(uint userId)
		{
			this.SiteId = 0;
			this.ForumId = 0;
			this.UserId = userId;
			this.Permission = Permission.None;
			this.IsModerator = true;
			this.Forum = null;
			this.User = null;
		}

		private ForumUser(uint siteId, ushort forumId, uint userId)
		{
			this.SiteId = siteId;
			this.ForumId = forumId;
			this.UserId = userId;
			this.Permission = Permission.None;
			this.IsModerator = true;
			this.Forum = null;
			this.User = null;
		}

		public ForumUser(uint userId, Permission permission)
		{
			this.SiteId = 0;
			this.ForumId = 0;
			this.UserId = userId;
			this.Permission = permission;
			this.IsModerator = false;
			this.Forum = null;
			this.User = null;
		}

		public ForumUser(uint siteId, ushort forumId, uint userId, Permission permission)
		{
			this.SiteId = siteId;
			this.ForumId = forumId;
			this.UserId = userId;
			this.Permission = permission;
			this.IsModerator = false;
			this.Forum = null;
			this.User = null;
		}
		#endregion

		#region 静态方法
		/// <summary>构建一个指定用户编号的版主。</summary>
		/// <param name="userId">指定的版主的用户编号。</param>
		/// <returns>返回构建成功的论坛用户成员。</returns>
		public static ForumUser Moderator(uint userId) => new(userId);

		/// <summary>构建一个指定论坛的版主。</summary>
		/// <param name="siteId">指定的站点编号。</param>
		/// <param name="forumId">指定的论坛编号。</param>
		/// <param name="userId">指定的版主的用户编号。</param>
		/// <returns>返回构建成功的论坛用户成员。</returns>
		public static ForumUser Moderator(uint siteId, ushort forumId, uint userId) => new(siteId, forumId, userId);
		#endregion

		#region 重写方法
		public bool Equals(ForumUser other) =>
			this.SiteId == other.SiteId &&
			this.ForumId == other.ForumId &&
			this.UserId == other.UserId;

		public override bool Equals(object obj) => obj is ForumUser other && this.Equals(other);
		public override int GetHashCode() => HashCode.Combine(this.SiteId, this.ForumId, this.UserId);
		public override string ToString() => this.IsModerator ?
			$"{this.SiteId}-{this.ForumId}:{this.UserId}(Moderator)" :
			$"{this.SiteId}-{this.ForumId}:{this.UserId}({this.Permission})";
		#endregion
	}
	#endregion
}

/// <summary>
/// 表示论坛查询条件的实体类。
/// </summary>
public abstract class ForumCriteria : CriteriaBase
{
	#region 公共属性
	/// <summary>获取或设置论坛的名称。</summary>
	public abstract string Name { get; set; }

	/// <summary>获取或设置论坛的可见性。</summary>
	public abstract Visibility? Visiblity { get; set; }

	/// <summary>获取或设置论坛的可访问性。</summary>
	public abstract Accessibility? Accessibility { get; set; }

	/// <summary>获取或设置一个值，表示论坛是否为热门版块。</summary>
	public abstract bool? IsPopular { get; set; }

	/// <summary>获取或设置论坛的创建时间范围。</summary>
	public abstract Range<DateTime> CreatedTime { get; set; }
	#endregion
}
