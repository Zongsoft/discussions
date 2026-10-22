using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;

using Zongsoft.Data;
using Zongsoft.Discussions.Data;
using Zongsoft.Discussions.Models;

namespace Zongsoft.Discussions.Tests;

public class ContentFilterAsyncTest
{
	[Fact]
	public async Task Filter_PreservesTypedAsyncSource()
	{
		await using var scope = new TestScope();
		var post = NewPost("body", "text/plain+embedded", true);
		using var posts = scope.Select<Post>(new RecordStream<Post>([post]));
		await new PostFilter().OnFilteredAsync(posts, TestContext.Current.CancellationToken);
		Assert.Same(post, Assert.Single(await TestRecords.CollectAsync(Assert.IsAssignableFrom<IAsyncEnumerable<Post>>(posts.Result), TestContext.Current.CancellationToken)));

		var thread = NewThread(post, true);
		using var threads = scope.Select<Models.Thread>(new RecordStream<Models.Thread>([thread]));
		await new ThreadFilter().OnFilteredAsync(threads, TestContext.Current.CancellationToken);
		Assert.Same(thread, Assert.Single(await TestRecords.CollectAsync(Assert.IsAssignableFrom<IAsyncEnumerable<Models.Thread>>(threads.Result), TestContext.Current.CancellationToken)));
	}

	[Theory]
	[InlineData(false, 0U, false, false)]
	[InlineData(false, 42U, false, true)]
	[InlineData(false, 99U, false, false)]
	[InlineData(false, 0U, true, true)]
	[InlineData(true, 0U, false, false)]
	[InlineData(true, 42U, false, true)]
	[InlineData(true, 99U, false, false)]
	[InlineData(true, 0U, true, true)]
	public async Task Filter_ContentVisibilityDependsOnApprovalOrAuthor(bool thread, uint userId, bool approved, bool visible)
	{
		await using var scope = new TestScope();
		if(userId != 0)
			scope.Login(userId);

		var post = NewPost("private body", "text/plain+embedded", approved);

		await FilterOneAsync(scope, thread, post, approved);
		Assert.Equal(visible ? "private body" : string.Empty, post.Content);
		Assert.Equal("text/plain+embedded", post.ContentType);
		Assert.Empty(scope.Files.Calls);
	}

	[Theory]
	[InlineData(false, false)]
	[InlineData(false, true)]
	[InlineData(true, false)]
	[InlineData(true, true)]
	public async Task Filter_ExternalContentReadIsAwaited(bool thread, bool author)
	{
		await using var scope = new TestScope();
		if(author)
			scope.Login(42);

		var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		scope.Files.ReadRelease = release.Task;
		var post = NewPost(scope.Files.Path, "text/plain", !author);
		var operation = FilterOneAsync(scope, thread, post, !author);

		try
		{
			await scope.Files.Reading.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
			Assert.False(operation.IsCompleted);
			Assert.Equal(scope.Files.Path, post.Content);
		}
		finally { release.TrySetResult(); }

		await operation;
		Assert.Equal("external body", post.Content);
		Assert.Equal("text/plain+embedded", post.ContentType);
		Assert.Equal(1, scope.Files.Disposals);
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task Filter_PreservesPaginationAndDisposesOnEarlyExit(bool thread)
	{
		await using var scope = new TestScope();
		var post = NewPost("body", "text/plain+embedded", true);
		var items = thread ? new object[] { NewThread(post, true), NewThread(post, true) } : [post, post];
		var source = new RecordStream<object>(items) { Suppressed = true };
		using var context = scope.Select<object>(source);
		await ApplyFilterAsync(thread, context);

		var pageable = Assert.IsAssignableFrom<IPageable>(context.Result);
		Assert.True(pageable.Suppressed);
		using var cancellation = new CancellationTokenSource();
		var paging = new PagingEventArgs("posts", Paging.Page(2, 10));
		var events = 0;

		Func<object, PagingEventArgs, CancellationToken, ValueTask> paginated = (sender, args, token) =>
		{
			Assert.Same(source, sender);
			Assert.Same(paging, args);
			Assert.Equal(cancellation.Token, token);
			events++;
			return ValueTask.CompletedTask;
		};

		pageable.Paginated += paginated;
		await source.RaiseAsync(paging, cancellation.Token);
		pageable.Paginated -= paginated;
		await source.RaiseAsync(paging, cancellation.Token);

		await foreach(var item in ((IAsyncEnumerable<object>)context.Result).WithCancellation(cancellation.Token))
		{
			Assert.Same(items[0], item);
			break;
		}

		Assert.Equal(1, events);
		Assert.Equal(1, source.Disposals);
		Assert.Equal(cancellation.Token, source.Token);
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task Filter_CancellationDisposesSourceAndReadStream(bool thread)
	{
		await using var scope = new TestScope();
		using var cancellation = new CancellationTokenSource();
		var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		scope.Files.ReadRelease = release.Task;
		var post = NewPost(scope.Files.Path, "text/plain", true);
		var source = new RecordStream<object>([thread ? NewThread(post, true) : post]);
		using var context = scope.Select<object>(source);
		await ApplyFilterAsync(thread, context);

		var operation = TestRecords.CollectAsync((IAsyncEnumerable<object>)context.Result, cancellation.Token);
		await scope.Files.Reading.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
		cancellation.Cancel();
		await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await operation);
		Assert.Equal(1, source.Disposals);
		Assert.Equal(1, scope.Files.Disposals);
	}

	private static async Task FilterOneAsync(TestScope scope, bool thread, Post post, bool approved)
	{
		var item = thread ? (object)NewThread(post, approved) : post;
		using var context = scope.Select<object>(new RecordStream<object>([item]));
		await ApplyFilterAsync(thread, context);
		var items = await TestRecords.CollectAsync((IAsyncEnumerable<object>)context.Result);
		Assert.Same(item, Assert.Single(items));
	}
	private static ValueTask ApplyFilterAsync(bool thread, DataSelectContextBase context) => thread ?
		new ThreadFilter().OnFilteredAsync(context) : new PostFilter().OnFilteredAsync(context);

	private static Post NewPost(string content, string type, bool approved) => Model.Build<Post>(post =>
	{
		post.CreatorId = 42;
		post.Approved = approved;
		post.Content = content;
		post.ContentType = type;
	});
	private static Models.Thread NewThread(Post post, bool approved) => Model.Build<Models.Thread>(thread =>
	{
		thread.CreatorId = 42;
		thread.Approved = approved;
		thread.Post = post;
	});
}
