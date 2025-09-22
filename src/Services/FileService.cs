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

	#region 重写方法
	protected override int OnInsert(IDataDictionary<File> data, ISchema schema, DataInsertOptions options)
	{
		var filePath = data.GetValue(p => p.Path);

		try
		{
			//调用基类同名方法
			var count = base.OnInsert(data, schema, options);

			if(count < 1)
			{
				//如果新增记录失败则删除刚创建的文件
				if(!filePath.IsEmpty)
					Utility.DeleteFile(filePath.Path);
			}

			return count;
		}
		catch
		{
			//删除新建的文件
			if(!filePath.IsEmpty)
				Utility.DeleteFile(filePath.Path);

			throw;
		}
	}
	#endregion
}
