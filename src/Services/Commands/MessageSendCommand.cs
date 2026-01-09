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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zongsoft.Services;
using Zongsoft.Components;

namespace Zongsoft.Discussions.Services.Commands;

[CommandOption(SUBJECT_OPTION, typeof(string), Required = true)]
[CommandOption(CONTENT_OPTION, typeof(string), Required = true)]
[CommandOption(CONTENTTYPE_OPTION, typeof(string))]
[CommandOption(MESSAGETYPE_OPTION, typeof(string))]
[CommandOption(SOURCE_OPTION, typeof(string))]
public class MessageSendCommand : CommandBase<CommandContext>
{
	#region 常量定义
	private const string SUBJECT_OPTION = "subject";
	private const string CONTENT_OPTION = "content";
	private const string CONTENTTYPE_OPTION = "contentType";
	private const string MESSAGETYPE_OPTION = "messageType";
	private const string SOURCE_OPTION = "source";
	#endregion

	#region 构造函数
	public MessageSendCommand() : base("Send") { }
	public MessageSendCommand(string name) : base(name) { }
	#endregion

	#region 公共属性
	[ServiceDependency(Provider = Module.NAME)]
	public MessageService Service { get; set; }
	#endregion

	#region 执行方法
	protected override ValueTask<object> OnExecuteAsync(CommandContext context, CancellationToken cancellation)
	{
		if(context.Arguments == null || context.Arguments.IsEmpty)
			throw new CommandException("Missing arguments of the command.");

		var content = context.Options.GetValue<string>(CONTENT_OPTION);
		var contentType = context.Options.GetValue<string>(CONTENTTYPE_OPTION);

		//根据内容类型解析得到真实内容
		content = GetContent(content, ref contentType);

		var message = Zongsoft.Data.Model.Build<Models.Message>(entity =>
		{
			entity.Content = content;
			entity.ContentType = contentType;
			entity.Referer = context.Options.GetValue<string>(SOURCE_OPTION);
			entity.Subject = context.Options.GetValue<string>(SUBJECT_OPTION);
			entity.MessageType = context.Options.GetValue<string>(MESSAGETYPE_OPTION);
		});

		if(this.Service.Send(message, GetUsers(context.Arguments)) > 0)
			return ValueTask.FromResult<object>(message);

		return ValueTask.FromResult<object>(null);
	}
	#endregion

	#region 私有方法
	private static string GetContent(string content, ref string contentType)
	{
		if(string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(contentType))
			return content;

		if(contentType.Length > 5 && contentType.EndsWith("+file", StringComparison.OrdinalIgnoreCase))
		{
			contentType = contentType.Substring(0, contentType.Length - 5);

			if(Zongsoft.IO.FileSystem.File.Exists(content))
			{
				using(var stream = Zongsoft.IO.FileSystem.File.Open(content, System.IO.FileMode.Open))
				{
					using(var reader = new System.IO.StreamReader(stream))
					{
						content = reader.ReadToEnd();
					}
				}
			}
		}

		return content;
	}

	private static IEnumerable<uint> GetUsers(string[] args)
	{
		if(args == null)
			yield break;

		foreach(var arg in args)
		{
			if(arg != null && uint.TryParse(arg, out var id))
				yield return id;
		}
	}
	#endregion
}
