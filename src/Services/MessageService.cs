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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zongsoft.Data;
using Zongsoft.Security;
using Zongsoft.Services;
using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Services;

[Service(nameof(MessageService))]
[DataService(typeof(MessageCriteria))]
public class MessageService : DataServiceBase<Message>
{
	#region 构造函数
	public MessageService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 公共方法
	public int Send(Message message, IEnumerable<uint> users)
	{
		if(message == null)
			throw new ArgumentNullException(nameof(message));

		if(users == null || !users.Any())
			return 0;

		using(var transaction = new Transaction())
		{
			//插入消息
			if(this.Insert(message) < 1)
				return 0;

			//插入用户消息
			var count = this.DataAccess.InsertMany(users.Select(uid => new UserMessage(uid, message.MessageId)));

			//提交事务
			transaction.Commit();

			return count;
		}
	}
	#endregion

	#region 重写方法
	protected override Message OnGet(ICondition criteria, ISchema schema, DataSelectOptions options)
	{
		//调用基类同名方法
		var message = base.OnGet(criteria, schema, options);

		if(message == null)
			return null;

		//如果内容类型是外部文件（即非嵌入格式），则读取文件内容
		if(!Utility.IsContentEmbedded(message.ContentType))
			message.Content = Utility.ReadTextFile(message.Content);

		//更新当前用户对该消息的读取状态
		this.DataAccess.Update(Model.Naming.Get<UserMessage>(), new
		{
			IsRead = true,
		},
		Condition.Equal(nameof(UserMessage.MessageId), message.MessageId) &
		Condition.Equal(nameof(UserMessage.UserId), this.Principal.Identity.GetIdentifier<uint>()));

		return message;
	}

	protected override int OnInsert(IDataDictionary<Message> data, ISchema schema, DataInsertOptions options)
	{
		return Utility.MutateContent(data, () => this.GetContentFilePath(data.GetValue(p => p.MessageId, 0UL), data.GetValue(p => p.ContentType, null)), () => base.OnInsert(data, schema, options));
	}

	protected override int OnUpdate(IDataDictionary<Message> data, ICondition criteria, ISchema schema, DataUpdateOptions options)
	{
		return Utility.MutateContent(data, () => this.GetContentFilePath(data.GetValue(p => p.MessageId, 0UL), data.GetValue(p => p.ContentType, null)), () => base.OnUpdate(data, criteria, schema, options));
	}
	#endregion

	#region 虚拟方法
	protected virtual string GetContentFilePath(ulong messageId, string contentType)
	{
		return Utility.GetFilePath(string.Format("messages/message-{0}-{1}.txt", messageId.ToString(), Zongsoft.Common.Randomizer.GenerateString()));
	}
	#endregion
	#region 异步业务路径
	protected override async ValueTask<Message> OnGetAsync(ICondition criteria, ISchema schema, DataSelectOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		//调用基类同名方法
		var message = await base.OnGetAsync(criteria, schema, options, cancellation);

		if(message == null)
			return null;

		//如果内容类型是外部文件（即非嵌入格式），则读取文件内容
		if(!Utility.IsContentEmbedded(message.ContentType))
			message.Content = Utility.ReadTextFile(message.Content);

		//更新当前用户对该消息的读取状态
		await this.DataAccess.UpdateAsync(Model.Naming.Get<UserMessage>(), new
		{
			IsRead = true,
		},
		Condition.Equal(nameof(UserMessage.MessageId), message.MessageId) &
		Condition.Equal(nameof(UserMessage.UserId), this.Principal.Identity.GetIdentifier<uint>()), cancellation: cancellation);

		return message;
	}

	protected override async ValueTask<int> OnInsertAsync(IDataDictionary<Message> data, ISchema schema, DataInsertOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		return await Utility.MutateContentAsync(data, () => this.GetContentFilePath(data.GetValue(p => p.MessageId, 0UL), data.GetValue(p => p.ContentType, null)), () => base.OnInsertAsync(data, schema, options, cancellation), cancellation);
	}

	protected override async ValueTask<int> OnUpdateAsync(IDataDictionary<Message> data, ICondition criteria, ISchema schema, DataUpdateOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		return await Utility.MutateContentAsync(data, () => this.GetContentFilePath(data.GetValue(p => p.MessageId, 0UL), data.GetValue(p => p.ContentType, null)), () => base.OnUpdateAsync(data, criteria, schema, options, cancellation), cancellation);
	}
	#endregion
}
