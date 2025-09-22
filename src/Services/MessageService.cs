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

		using(var transaction = new Zongsoft.Transactions.Transaction())
		{
			//插入消息
			this.Insert(message);

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
		string filePath = null;

		//获取原始的内容类型
		var rawType = data.GetValue(p => p.ContentType, null);

		//调整内容类型为嵌入格式
		data.SetValue(p => p.ContentType, Utility.GetContentType(rawType, true));

		data.TryGetValue(p => p.Content, (key, value) =>
		{
			if(string.IsNullOrWhiteSpace(value) || value.Length < 500)
				return;

			//设置内容文件的存储路径
			filePath = this.GetContentFilePath(data.GetValue(p => p.MessageId), data.GetValue(p => p.ContentType));

			//将内容文本写入到文件中
			Utility.WriteTextFile(filePath, value);

			//更新内容文件的存储路径
			data.SetValue(p => p.Content, filePath);

			//更新内容类型为非嵌入格式（即外部文件）
			data.SetValue(p => p.ContentType, Utility.GetContentType(data.GetValue(p => p.ContentType), false));
		});

		var count = base.OnInsert(data, schema, options);

		if(count < 1)
		{
			//如果新增记录失败则删除刚创建的文件
			if(filePath != null && filePath.Length > 0)
				Utility.DeleteFile(filePath);

			return count;
		}

		return count;
	}

	protected override int OnUpdate(IDataDictionary<Message> data, ICondition criteria, ISchema schema, DataUpdateOptions options)
	{
		//更新内容到文本文件中
		data.TryGetValue(p => p.Content, (key, value) =>
		{
			if(string.IsNullOrWhiteSpace(value) || value.Length < 500)
				return;

			//根据当前反馈编号，获得其对应的内容文件存储路径
			var filePath = this.GetContentFilePath(data.GetValue(p => p.MessageId), data.GetValue(p => p.ContentType));

			//将反馈内容写入到对应的存储文件中
			Utility.WriteTextFile(filePath, value);

			//更新当前反馈的内容文件存储路径属性
			data.SetValue(p => p.Content, filePath);

			//更新内容类型为非嵌入格式（即外部文件）
			data.SetValue(p => p.ContentType, Utility.GetContentType(data.GetValue(p => p.ContentType), false));
		});

		//调用基类同名方法
		var count = base.OnUpdate(data, criteria, schema, options);

		if(count < 1)
			return count;

		return count;
	}
	#endregion

	#region 虚拟方法
	protected virtual string GetContentFilePath(ulong messageId, string contentType)
	{
		return Utility.GetFilePath(string.Format("messages/message-{0}.txt", messageId.ToString()));
	}
	#endregion
}
