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
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Zongsoft.Web;
using Zongsoft.Data;
using Zongsoft.Web.Http;
using Zongsoft.Security;
using Zongsoft.Security.Privileges;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;
using Zongsoft.Web.Security;

namespace Zongsoft.Discussions.Web.Controllers;

[Authorization]
[ControllerName("Threads")]
public class ThreadController : ServiceController<Thread, ThreadService>
{
	#region 公共方法
	[HttpPatch("[area]/[controller]/{id}/Approve")]
	public object Approve(ulong id)
	{
		return this.DataService.Approve(id) ? this.NoContent() : this.NotFound();
	}

	[HttpPatch("[area]/[controller]/{id}/Hidden")]
	public object Hidden(ulong id)
	{
		return this.DataService.Visible(id, false) ? this.NoContent() : this.NotFound();
	}

	[HttpPatch("[area]/[controller]/{id}/Visible")]
	public object Visible(ulong id)
	{
		return this.DataService.Visible(id, true) ? this.NoContent() : this.NotFound();
	}

	[HttpPost("[area]/[controller]/{id}/Lock")]
	public object Lock(ulong id)
	{
		return this.DataService.SetLocked(id, true) ? this.NoContent() : this.NotFound();
	}

	[HttpPost("[area]/[controller]/{id}/Unlock")]
	public object Unlock(ulong id)
	{
		return this.DataService.SetLocked(id, false) ? this.NoContent() : this.NotFound();
	}

	[HttpPost("[area]/[controller]/{id}/Pin")]
	public object Pin(ulong id)
	{
		return this.DataService.SetPinned(id, true) ? this.NoContent() : this.NotFound();
	}

	[HttpPost("[area]/[controller]/{id}/Unpin")]
	public object Unpin(ulong id)
	{
		return this.DataService.SetPinned(id, false) ? this.NoContent() : this.NotFound();
	}

	[HttpPost("[area]/[controller]/{id}/Valued")]
	public object Valued(ulong id)
	{
		return this.DataService.SetValued(id, true) ? this.NoContent() : this.NotFound();
	}

	[HttpPost("[area]/[controller]/{id}/Unvalued")]
	public object Unvalued(ulong id)
	{
		return this.DataService.SetValued(id, false) ? this.NoContent() : this.NotFound();
	}

	[HttpPost("[area]/[controller]/{id}/Global")]
	public object Global(ulong id)
	{
		return this.DataService.SetGlobal(id, true) ? this.NoContent() : this.NotFound();
	}

	[HttpPost("[area]/[controller]/{id}/Unglobal")]
	public object Unglobal(ulong id)
	{
		return this.DataService.SetGlobal(id, false) ? this.NoContent() : this.NotFound();
	}
	#endregion
}
