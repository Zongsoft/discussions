using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;

using Zongsoft.Data;
using Zongsoft.Data.Transactions;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Tests;

public class PostVotingAsyncTest
{
	[Theory]
	[InlineData(false, 0, 1, true)]
	[InlineData(false, 127, 100, true)]
	[InlineData(true, 0, -1, true)]
	[InlineData(true, 255, -100, true)]
	[InlineData(false, 2, 2, false)]
	public async Task Vote_OrdersMutationsAndCommitsOnlyAfterStatistics(bool downvote, int value, int expected, bool updated)
	{
		await using var scope = new TestScope();
		scope.Login(42);
		using var cancellation = new CancellationTokenSource();
		var calls = new List<string>();
		var enlisted = false;
		scope.Calls.Handler = (method, args) =>
		{
			Assert.Equal(cancellation.Token, args[^1]);
			Assert.NotNull(Transaction.Current);

			if(!enlisted)
			{
				Transaction.Current.Context.Enlist(new Completion(calls));
				enlisted = true;
			}

			calls.Add(method.Name);
			switch(method.Name)
			{
				case "DeleteAsync":
					var criteria = TestRecords.Conditions(args.OfType<ICondition>().Single()).ToArray();
					Assert.Contains(criteria, item => item.Name == "PostId" && Equals(item.Value, 13UL));
					Assert.Contains(criteria, item => item.Name == "UserId" && Equals(item.Value, 42U));
					return TestRecords.Reply(1);
				case "InsertAsync":
					var vote = Assert.IsAssignableFrom<Post.PostVoting>(args[0]);
					Assert.Equal(13UL, vote.PostId);
					Assert.Equal(42U, vote.UserId);
					Assert.Equal(expected, vote.Value);
					return TestRecords.Reply(1);
				case "AggregateAsync":
					var filter = Assert.Single(TestRecords.Conditions(args.OfType<ICondition>().Single()), item => item.Name == "Value");
					return TestRecords.Reply<int?>(filter.Operator == ConditionOperator.GreaterThan ? 7 : 3);
				case "UpdateAsync":
					var data = DataDictionary.GetDictionary(args[1]);
					Assert.Equal(7, data.GetValue<int>("TotalUpvotes", -1));
					Assert.Equal(3, data.GetValue<int>("TotalDownvotes", -1));
					Assert.Equal(13UL, data.GetValue<ulong>("PostId", 0));
					return TestRecords.Reply(updated ? 1 : 0);
				default:
					throw new InvalidOperationException(method.Name);
			}
		};
		var service = new PostService(scope.Services) { DataAccess = scope.Accessor };
		var actual = downvote ? await service.DownvoteAsync(13, (byte)value, cancellation.Token) : await service.UpvoteAsync(13, (byte)value, cancellation.Token);
		Assert.Equal(updated, actual);
		Assert.Equal(["DeleteAsync", "InsertAsync", "AggregateAsync", "AggregateAsync", "UpdateAsync", updated ? "Commit" : "Rollback"], calls);
		Assert.Null(Transaction.Current);
	}

	[Fact]
	public async Task Vote_StatisticsMustCompleteBeforeCommit()
	{
		await using var scope = new TestScope();
		scope.Login(42);
		var calls = new List<string>();
		var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		var release = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
		scope.Calls.Handler = (method, _) =>
		{
			calls.Add(method.Name);
			switch(method.Name)
			{
				case "DeleteAsync":
					Transaction.Current.Context.Enlist(new Completion(calls));
					return TestRecords.Reply(1);
				case "InsertAsync":
					return TestRecords.Reply(1);
				case "AggregateAsync":
					return TestRecords.Reply<int?>(1);
				case "UpdateAsync":
					entered.TrySetResult();
					return new ValueTask<int>(release.Task);
				default:
					throw new InvalidOperationException(method.Name);
			}
		};
		var service = new PostService(scope.Services) { DataAccess = scope.Accessor };
		var operation = service.UpvoteAsync(13, TestContext.Current.CancellationToken).AsTask();

		try
		{
			await entered.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
			Assert.False(operation.IsCompleted);
			Assert.Equal(["DeleteAsync", "InsertAsync", "AggregateAsync", "AggregateAsync", "UpdateAsync"], calls);
		}
		finally { release.TrySetResult(1); }

		Assert.True(await operation);
		Assert.Equal("Commit", calls[^1]);
	}

	[Fact]
	public async Task Vote_FailureRollsBack()
	{
		await using var scope = new TestScope();
		scope.Login(42);
		var calls = new List<string>();
		var failure = new InvalidOperationException("insert failed");
		scope.Calls.Handler = (method, _) =>
		{
			calls.Add(method.Name);
			if(method.Name == "DeleteAsync")
			{
				Transaction.Current.Context.Enlist(new Completion(calls));
				return TestRecords.Reply(1);
			}

			Assert.Equal("InsertAsync", method.Name);
			return TestRecords.Failure<int>(failure);
		};
		var service = new PostService(scope.Services) { DataAccess = scope.Accessor };
		Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.UpvoteAsync(13, TestContext.Current.CancellationToken)));
		Assert.Equal(["DeleteAsync", "InsertAsync", "Rollback"], calls);
		Assert.Null(Transaction.Current);
	}

	private sealed class Completion(List<string> calls) : IEnlistment
	{
		public ValueTask OnEnlistAsync(EnlistmentContext context, CancellationToken cancellation = default)
		{
			calls.Add(context.Phase.ToString());
			return ValueTask.CompletedTask;
		}
	}
}
