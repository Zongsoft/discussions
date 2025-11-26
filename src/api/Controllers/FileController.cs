/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Zongsoft.IO;
using Zongsoft.Web;
using Zongsoft.Data;
using Zongsoft.Common;
using Zongsoft.Web.Security;
using Zongsoft.Security.Privileges;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Web.Controllers;

[Authorization]
[ControllerName("Files")]
public class FileController : ServiceController<File, FileService>
{
	#region 公共属性
	[Zongsoft.Services.ServiceDependency(IsRequired = true)]
	public WebFileAccessor Accessor { get; set; }
	#endregion

	#region 重写属性
	protected override bool CanCreate => false;
	protected override bool CanDelete => false;
	protected override bool CanUpdate => false;
	#endregion

	#region 公共方法
	[HttpGet("{id}/[action]")]
	public async Task<IActionResult> DownloadAsync(string id, CancellationToken cancellation = default)
	{
		var file = await this.DataService.GetAsync(id, $"{nameof(Models.File.FileId)},{nameof(Models.File.Path)}", Paging.Disabled, Array.Empty<Sorting>(), cancellation) as File;

		if(file == null || string.IsNullOrWhiteSpace(file.Path))
			return null;

		return this.Accessor.Read(file.Path);
	}

	[HttpPost("[action]/{id?}")]
	[HttpPost("{id:required}/[action]")]
	public async Task<IEnumerable<File>> UploadAsync(uint? id = null, CancellationToken cancellation = default)
	{
		var files = new List<File>();
		var infos = this.Accessor.Write(this.Request,
									  this.DataService.GetDirectory(id),
									  args => args.FileName = $"{Timestamp.Millennium.Epoch.GetElapsed().Days}-{Randomizer.GenerateString()}", cancellation);

		await foreach(var info in infos)
		{
			if(info == null || !info.IsFile)
				continue;

			object name = null;

			if(info.HasProperties && !info.Properties.TryGetValue("FileName", out name))
				info.Properties.TryGetValue("DispositionName", out name);

			if(string.IsNullOrWhiteSpace(name as string))
				name = info.Name;

			var attachment = Model.Build<File>(file =>
			{
				file.FolderId = id ?? 0;
				file.Name = info.Name;
				file.Path = (PathLocation)info.Path.Url;
				file.Type = info.Type;
				file.Size = (uint)Math.Max(0, info.Size);
			});

			if(this.DataService.Insert(attachment) > 0)
				files.Add(attachment);
			else
				await this.Accessor.Delete(info.Path.FullPath);
		}

		return files;
	}
	#endregion
}
