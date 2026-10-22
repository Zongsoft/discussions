using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;

using Zongsoft.Data;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Tests;

public class ForumServiceAsyncTest
{
	[Theory]
	[InlineData(null, 10)]
	[InlineData(-1, 5)]
	[InlineData(5, 5)]
	[InlineData(50, 50)]
	[InlineData(51, 50)]
	public async Task Topmosts_ClampCountUnionSortAndAwaitGlobalBeforePinned(int? requested, int count)
	{
		await using var scope = new TestScope();
		using var cancellation = new CancellationTokenSource();
		var shared = NewThread(100);
		var globals = new RecordStream<Models.Thread>(new[] { shared }.Concat(Enumerable.Range(1, 60).Select(value => NewThread((ulong)value))));
		var pinned = new RecordStream<Models.Thread>([shared, NewThread(99), NewThread(98)]);
		var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		globals.Release = release.Task;
		var calls = new List<string>();
		scope.Calls.Handler = (method, args) =>
		{
			Assert.Equal("SelectAsync", method.Name);
			Assert.Equal(cancellation.Token, args[^1]);
			var conditions = TestRecords.Conditions(args.OfType<ICondition>().Single()).ToArray();
			Assert.Contains(conditions, condition => condition.Name == "Visible" && Equals(condition.Value, true));
			var paging = Assert.Single(args.OfType<Paging>());
			Assert.Equal(1, paging.Index);
			Assert.Equal(count, paging.Size);

			if(conditions.Any(condition => condition.Name == "IsGlobal"))
			{
				Assert.DoesNotContain(conditions, condition => condition.Name == "ForumId");
				calls.Add("global");
				return globals;
			}

			Assert.Equal(1, globals.Disposals);
			Assert.Contains(conditions, condition => condition.Name == "ForumId" && Equals(condition.Value, (ushort)23));
			Assert.Contains(conditions, condition => condition.Name == "IsPinned" && Equals(condition.Value, true));
			calls.Add("pinned");
			return pinned;
		};
		var service = new ForumService(scope.Services) { DataAccess = scope.Accessor };
		var operation = requested.HasValue ? service.GetTopmostsAsync(23, "*", requested.Value, cancellation.Token).AsTask() : service.GetTopmostsAsync(23, "*", cancellation.Token).AsTask();

		try
		{
			await globals.Entered.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
			Assert.False(operation.IsCompleted);
			Assert.Equal(["global"], calls);
		}
		finally { release.TrySetResult(); }

		var result = await operation;
		Assert.Equal(["global", "pinned"], calls);
		Assert.Equal(new ulong[] { 100, 99, 98 }.Concat(Enumerable.Range(1, 60).Reverse().Select(value => (ulong)value)).Take(count), result.Select(thread => thread.ThreadId));
		Assert.Single(result, thread => ReferenceEquals(thread, shared));
		Assert.Equal(1, pinned.Disposals);
	}

	[Theory]
	[InlineData(null, true)]
	[InlineData(1, true)]
	[InlineData(2, false)]
	public async Task Threads_ExcludesTopmostOnlyForFirstPageAndPreservesSource(int? page, bool topmost)
	{
		await using var scope = new TestScope();
		var queries = 0;
		var result = new RecordStream<Models.Thread>([NewThread(7)]);
		var paging = page.HasValue ? Paging.Page(page.Value, 20) : null;
		using var cancellation = new CancellationTokenSource();
		scope.Calls.Handler = (method, args) =>
		{
			Assert.Equal("SelectAsync", method.Name);
			Assert.Equal(cancellation.Token, args[^1]);
			queries++;
			var criteria = TestRecords.Conditions(args.OfType<ICondition>().Single()).ToArray();

			if(criteria.Any(condition => condition.Name == "IsGlobal"))
				return new RecordStream<Models.Thread>([NewThread(100)]);
			if(criteria.Any(condition => condition.Name == "IsPinned"))
				return new RecordStream<Models.Thread>([NewThread(90)]);
			var exclusions = criteria.Where(condition => condition.Name == "ThreadId").ToArray();
			if(topmost)
			{
				var excluded = Assert.Single(exclusions);
				Assert.Equal(ConditionOperator.NotIn, excluded.Operator);
				Assert.Equal(new ulong[] { 100, 90 }, Assert.IsAssignableFrom<IEnumerable<ulong>>(excluded.Value));
			}
			else
				Assert.Empty(exclusions);

			Assert.Contains(criteria, condition => condition.Name == "ForumId" && Equals(condition.Value, (ushort)23));
			Assert.Contains(criteria, condition => condition.Name == "Visible" && Equals(condition.Value, true));
			Assert.Same(paging, args[2]);
			return result;
		};
		var service = new ForumService(scope.Services) { DataAccess = scope.Accessor };
		var actual = await service.GetThreadsAsync(23, "*", paging, cancellation.Token);
		Assert.Same(result, actual);
		Assert.IsAssignableFrom<IPageable>(actual);
		Assert.Equal(topmost ? 3 : 1, queries);
		Assert.Equal(7UL, Assert.Single(await TestRecords.CollectAsync(actual, TestContext.Current.CancellationToken)).ThreadId);
	}

	private static Models.Thread NewThread(ulong id) => Model.Build<Models.Thread>(thread => thread.ThreadId = id);
}
