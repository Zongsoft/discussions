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
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Zongsoft.Web;
using Zongsoft.Data;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Web.Controllers;

[ControllerName("Posts")]
public class PostController : ServiceController<Post, PostService>
{
    #region 公共方法
    [ActionName("Upvote")]
    [HttpPost("{id}/[action]/{value?}")]
    public IActionResult Upvote(ulong id, byte value = 1)
    {
        return this.DataService.Upvote(id, value) ? this.NoContent() : this.NotFound();
    }

    [ActionName("Downvote")]
    [HttpPost("{id}/[action]/{value?}")]
    public IActionResult Downvote(ulong id, byte value = 1)
    {
        return this.DataService.Downvote(id, value) ? this.NoContent() : this.NotFound();
    }

    [ActionName("Upvotes")]
    [HttpGet("{id}/[action]")]
    public IEnumerable<Post.PostVoting> GetUpvotes(ulong id, [FromQuery] Paging page = null)
    {
        return this.DataService.GetUpvotes(id, page);
    }

    [ActionName("Downvotes")]
    [HttpGet("{id}/[action]")]
    public IEnumerable<Post.PostVoting> GetDownvotes(ulong id, [FromQuery] Paging page = null)
    {
        return this.DataService.GetDownvotes(id, page);
    }

    [ActionName("Comments")]
    [HttpGet("{id}/[action]")]
    public IEnumerable<Post> GetComments(ulong id, [FromQuery] Paging page = null)
    {
        return this.DataService.GetComments(id, page);
    }
    #endregion
}
