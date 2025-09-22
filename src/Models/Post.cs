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

namespace Zongsoft.Discussions.Models;

/// <summary>
/// 表示帖子的业务实体类。
/// </summary>
public abstract class Post
{
	#region 公共属性
	/// <summary>获取或设置帖子编号。</summary>
	public abstract ulong PostId { get; set; }

	/// <summary>获取或设置帖子所属站点编号。</summary>
	public abstract uint SiteId { get; set; }

	/// <summary>获取或设置帖子所属的主题编号。</summary>
	public abstract ulong ThreadId { get; set; }

	/// <summary>获取或设置帖子所属的主题对象。</summary>
	public abstract Thread Thread { get; set; }

	/// <summary>获取或设置回帖引用来源的帖子编号。</summary>
	public abstract ulong RefererId { get; set; }

	/// <summary>获取或设置回帖引用来源的帖子对象。</summary>
	public abstract Post Referer { get; set; }

	/// <summary>获取或设置帖子的内容。</summary>
	public abstract string Content { get; set; }

	/// <summary>获取或设置帖子的内容类型。</summary>
	public abstract string ContentType { get; set; }

	/// <summary>获取或设置一个值，表示是否帖子是否可见。</summary>
	public abstract bool Visible { get; set; }

	/// <summary>获取或设置一个值，表示帖子是否被审核通过。</summary>
	public abstract bool Approved { get; set; }

	/// <summary>获取或设置一个值，表示帖子是否被锁定，如果锁定则不允许回复。</summary>
	public abstract bool IsLocked { get; set; }

	/// <summary>获取或设置一个值，表示帖子是否为精华帖。</summary>
	public abstract bool IsValued { get; set; }

	/// <summary>获取或设置帖子的累计点赞总数。</summary>
	public abstract uint TotalUpvotes { get; set; }

	/// <summary>获取或设置帖子的累计被踩总数。</summary>
	public abstract uint TotalDownvotes { get; set; }

	/// <summary>获取或设置访问者的位置信息。</summary>
	public abstract string VisitorAddress { get; set; }

	/// <summary>获取或设置访问者的描述信息。</summary>
	public abstract string VisitorDescription { get; set; }

	/// <summary>获取或设置帖子作者编号。</summary>
	public abstract uint CreatorId { get; set; }

	/// <summary>获取或设置帖子创建时间。</summary>
	public abstract DateTime CreatedTime { get; set; }

	/// <summary>获取或设置帖子作者对应的用户对象。</summary>
	public abstract UserProfile Creator { get; set; }
	#endregion

	#region 集合属性
	/// <summary>获取或设置帖子的附件集。</summary>
	public abstract ICollection<PostAttachment> Attachments { get; set; }

	/// <summary>获取或设置帖子的回复集。</summary>
	public abstract IEnumerable<Post> Comments { get; set; }

	/// <summary>获取或设置帖子的投票（点赞和被踩）记录集。</summary>
	public abstract IEnumerable<PostVoting> Votes { get; set; }

	/// <summary>获取帖子的点赞记录集。</summary>
	[Serialization.SerializationMember(Ignored = true)]
	public IEnumerable<PostVoting> Upvotes => this.Votes == null ? Array.Empty<PostVoting>() : this.Votes.Where(vote => vote.Value > 0);

	/// <summary>获取帖子的被踩记录集。</summary>
	[Serialization.SerializationMember(Ignored = true)]
	public IEnumerable<PostVoting> Downvotes => this.Votes == null ? Array.Empty<PostVoting>() : this.Votes.Where(vote => vote.Value < 0);
	#endregion

	#region 嵌套子类
	/// <summary>
	/// 表示帖子投票的业务实体类。
	/// </summary>
	public abstract class PostVoting
	{
		#region 公共属性
		/// <summary>获取或设置投票关联的帖子编号。</summary>
		public abstract ulong PostId { get; set; }
		/// <summary>获取或设置投票的用户编号。</summary>
		public abstract uint UserId { get; set; }
		/// <summary>获取或设置投票的用户对象。</summary>
		public abstract UserProfile User { get; set; }
		/// <summary>获取或设置投票值（正数为赞，负数为踩）。</summary>
		public abstract sbyte Value { get; set; }
		/// <summary>获取或设置投票的时间。</summary>
		public abstract DateTime Timestamp { get; set; }
		#endregion
	}
	#endregion
}

/// <summary>
/// 表示帖子查询条件的实体。
/// </summary>
public abstract class PostCriteria : CriteriaBase
{
	#region 公共属性
	/// <summary>获取或设置帖子所属站点编号。</summary>
	public abstract uint? SiteId { get; set; }

	/// <summary>获取或设置所属主题编号。</summary>
	public abstract ulong? ThreadId { get; set; }

	/// <summary>获取或设置一个值，表示是否帖子是否可见。</summary>
	public abstract bool? Visible { get; set; }

	/// <summary>获取或设置一个值，表示帖子是否被审核通过。</summary>
	public abstract bool? Approved { get; set; }

	/// <summary>获取或设置一个值，表示帖子是否被锁定，如果锁定则不允许回复。</summary>
	public abstract bool? IsLocked { get; set; }

	/// <summary>获取或设置一个值，表示帖子是否为精华帖。</summary>
	public abstract bool? IsValued { get; set; }

	/// <summary>获取或设置帖子作者编号。</summary>
	public abstract uint? CreatorId { get; set; }

	/// <summary>获取或设置帖子创建时间范围。</summary>
	public abstract Range<DateTime>? CreatedTime { get; set; }
	#endregion
}

/// <summary>
/// 表示帖子附件的关联实体类。
/// </summary>
public class PostAttachment : IAttachedAttachment
{
	#region 构造函数
	public PostAttachment() { }
	public PostAttachment(ulong postId, ulong attachmentId, uint attachmentFolderId, short ordinal = 0)
	{
		this.PostId = postId;
		this.AttachmentId = attachmentId;
		this.AttachmentFolderId = attachmentFolderId;
		this.Ordinal = ordinal;
	}
	#endregion

	#region 公共属性
	/// <summary>获取或设置帖子编号。</summary>
	public ulong PostId { get; set; }

	/// <summary>获取或设置排列顺序。</summary>
	public short Ordinal { get; set; }

	/// <summary>获取或设置附件编号。</summary>
	public ulong AttachmentId { get; set; }

	/// <summary>获取或设置附件对象。</summary>
	public File Attachment { get; set; }

	/// <summary>获取或设置附件归属的附件目录编号。</summary>
	public uint AttachmentFolderId { get; set; }

	/// <summary>获取或设置附件归属的附件目录对象。</summary>
	public Folder AttachmentFolder { get; set; }
	#endregion
}
