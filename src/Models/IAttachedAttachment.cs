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

namespace Zongsoft.Discussions.Models;

/// <summary>
/// 表示附加附件的实体接口。
/// </summary>
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