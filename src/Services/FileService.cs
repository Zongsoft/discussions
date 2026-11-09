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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zongsoft.Data;
using Zongsoft.Services;
using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Services;

[Service(nameof(FileService))]
[DataService(typeof(FileCriteria))]
public class FileService : DataServiceBase<File>
{
	#region 构造函数
	public FileService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 公共方法
	public string GetDirectory(uint? folderId = null)
	{
		if(folderId.HasValue)
			return Utility.GetFilePath("/folders/{folderId}/");
		else
			return Utility.GetFilePath("files/");
	}
	#endregion

	#region 业务路径
	protected override async ValueTask<int> OnInsertAsync(IDataDictionary<File> data, ISchema schema, DataInsertOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		var filePath = data.GetValue(p => p.Path);

		try
		{
			//调用基类同名方法
			var count = await base.OnInsertAsync(data, schema, options, cancellation);

			if(count < 1)
			{
				//如果新增记录失败则删除刚创建的文件
				if(!filePath.IsEmpty)
					await Utility.DeleteFileAsync(filePath.Path, CancellationToken.None);
			}

			return count;
		}
		catch
		{
			//删除新建的文件
			if(!filePath.IsEmpty)
				await Utility.DeleteFileAsync(filePath.Path, CancellationToken.None);

			throw;
		}
	}
	#endregion
}
