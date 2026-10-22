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
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Zongsoft.Web;
using Zongsoft.Web.Security;
using Zongsoft.Security.Privileges;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Web.Controllers;

[Authorization]
[ControllerName("Folders")]
public class FolderController : ServiceController<Folder, FolderService>
{
	#region 公共方法
	[ActionName("Icon")]
	[HttpPost("{id}/[action]/{value}")]
	public async ValueTask<IActionResult> SetIcon(uint id, string value = null, CancellationToken cancellation = default)
	{
		return await this.DataService.SetIconAsync(id, value, cancellation) ? this.NoContent() : this.NotFound();
	}

	[ActionName("Shareability")]
	[HttpPost("{id}/[action]/{value}")]
	public async ValueTask<IActionResult> SetShareability(uint id, Shareability value, CancellationToken cancellation = default)
	{
		return await this.DataService.SetShareabilityAsync(id, value, cancellation) ? this.NoContent() : this.NotFound();
	}
	#endregion
}
