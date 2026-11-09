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
	#region 构造函数
	public FileController() => this.Accessor = WebFileAccessor.Default;
	#endregion

	#region 公共属性
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
			return this.NotFound();

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

			string name = null;

			if(info.HasProperties && !info.Properties.TryGetValue("FileName", out name))
				info.Properties.TryGetValue("DispositionName", out name);

			if(string.IsNullOrWhiteSpace(name))
				name = info.Name;

			var attachment = Model.Build<File>(file =>
			{
				file.FolderId = id ?? 0;
				file.Name = info.Name;
				file.Path = (PathLocation)info.Path.Url;
				file.Type = info.Type;
				file.Size = (uint)Math.Max(0, info.Size);
			});

			if(await this.DataService.InsertAsync(attachment, cancellation) > 0)
				files.Add(attachment);
			else
				await this.Accessor.Delete(info.Path.FullPath);
		}

		return files;
	}
	#endregion
}
