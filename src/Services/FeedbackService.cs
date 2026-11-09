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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zongsoft.Data;
using Zongsoft.Services;
using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Services;

[Service(nameof(FeedbackService))]
[DataService(typeof(FeedbackCriteria))]
public class FeedbackService : DataServiceBase<Feedback>
{
	#region 构造函数
	public FeedbackService(IServiceProvider serviceProvider) : base(serviceProvider) { }
	#endregion

	#region 虚拟方法
	protected virtual string GetContentFilePath(ulong feedbackId)
	{
		return Utility.GetFilePath($"feedbacks/feedback-{feedbackId}-{Zongsoft.Common.Randomizer.GenerateString()}.txt");
	}
	#endregion
	#region 异步业务路径
	protected override async ValueTask<Feedback> OnGetAsync(ICondition criteria, ISchema schema, DataSelectOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		//调用基类同名方法
		var feedback = await base.OnGetAsync(criteria, schema, options, cancellation);

		if(feedback == null)
			return null;

		//如果内容类型是外部文件（即非嵌入格式），则读取文件内容
		if(!Utility.IsContentEmbedded(feedback.ContentType))
			feedback.Content = await Utility.ReadTextFileAsync(feedback.Content, cancellation);

		return feedback;
	}

	protected override async ValueTask<int> OnInsertAsync(IDataDictionary<Feedback> data, ISchema schema, DataInsertOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		return await Utility.MutateContentAsync(data, () => this.GetContentFilePath(data.GetValue(p => p.FeedbackId, 0UL)), () => base.OnInsertAsync(data, schema, options, cancellation), cancellation);
	}

	protected override async ValueTask<int> OnUpdateAsync(IDataDictionary<Feedback> data, ICondition criteria, ISchema schema, DataUpdateOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		return await Utility.MutateContentAsync(data, () => this.GetContentFilePath(data.GetValue(p => p.FeedbackId, 0UL)), () => base.OnUpdateAsync(data, criteria, schema, options, cancellation), cancellation);
	}

	protected override async ValueTask<int> OnUpsertAsync(IDataDictionary<Feedback> data, ISchema schema, DataUpsertOptions options, CancellationToken cancellation)
	{
		cancellation.ThrowIfCancellationRequested();
		return await Utility.MutateContentAsync(data, () => this.GetContentFilePath(data.GetValue(p => p.FeedbackId, 0UL)), () => base.OnUpsertAsync(data, schema, options, cancellation), cancellation);
	}
	#endregion
}
