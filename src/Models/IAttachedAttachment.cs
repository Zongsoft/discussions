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

namespace Zongsoft.Discussions.Models;

/// <summary>表示附加附件的实体接口。</summary>
public interface IAttachedAttachment
{
	/// <summary>获取或设置附件编号。</summary>
	ulong AttachmentId { get; set; }

	/// <summary>获取或设置附件对象。</summary>
	File Attachment { get; set; }

	/// <summary>获取或设置附件归属的附件目录编号。</summary>
	uint AttachmentFolderId { get; set; }

	/// <summary>获取或设置附件归属的附件目录对象。</summary>
	Folder AttachmentFolder { get; set; }

	/// <summary>获取或设置附件的排列顺序。</summary>
	short Ordinal { get; set; }
}
