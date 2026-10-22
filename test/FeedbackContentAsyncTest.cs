using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

using Xunit;

using Zongsoft.Data;
using Zongsoft.Discussions.Models;
using Zongsoft.Discussions.Services;

namespace Zongsoft.Discussions.Tests;

public class FeedbackContentAsyncTest
{
	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task Feedback_WriteFailurePreservesExceptionAndNeverWritesDatabase(bool dictionary)
	{
		await using var scope = new TestScope();
		var failure = new IOException("write failed");
		scope.Files.WriteFailure = failure;
		scope.Files.DeleteFailure = new IOException("cleanup failed");
		var writes = 0;
		scope.Calls.Handler = (_, _) => { writes++; return TestRecords.Reply(1); };
		var service = new FeedbackProbe(scope);
		var data = CreateData(dictionary, new string('x', 501));

		var actual = await Assert.ThrowsAsync<IOException>(async () => await service.InsertCoreAsync(data, CancellationToken.None));

		Assert.Same(failure, actual);
		Assert.Equal(0, writes);
		Assert.Contains("DeleteAsync", scope.Files.Calls);
		Assert.Equal(CancellationToken.None, scope.Files.DeleteToken);
		Assert.Equal(new string('x', 501), data.GetValue(feedback => feedback.Content));
	}

	[Theory]
	[InlineData(false, 499)]
	[InlineData(true, 499)]
	[InlineData(false, 0)]
	[InlineData(true, 0)]
	public async Task Feedback_ShortContentStaysInline(bool dictionary, int length)
	{
		await using var scope = new TestScope();
		var data = CreateData(dictionary, new string('x', length));
		var writes = 0;
		scope.Calls.Handler = (method, args) =>
		{
			Assert.Equal("InsertAsync", method.Name);
			Assert.Same(data, args[1]);
			writes++;
			return TestRecords.Reply(1);
		};

		Assert.Equal(1, await new FeedbackProbe(scope).InsertCoreAsync(data, CancellationToken.None));
		Assert.Equal(1, writes);
		Assert.Equal(new string('x', length), data.GetValue(feedback => feedback.Content));
		Assert.Equal("text/plain+embedded", data.GetValue(feedback => feedback.ContentType));
		Assert.Empty(scope.Files.Calls);
	}

	[Theory]
	[InlineData(false, 500)]
	[InlineData(true, 500)]
	[InlineData(false, 501)]
	[InlineData(true, 501)]
	public async Task Feedback_LongContentWaitsForFileBeforeDatabase(bool dictionary, int length)
	{
		await using var scope = new TestScope();
		using var cancellation = new CancellationTokenSource();
		var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		scope.Files.WriteRelease = release.Task;
		var content = new string('x', length);
		var data = CreateData(dictionary, content);
		var writes = 0;
		scope.Calls.Handler = (method, args) =>
		{
			Assert.Equal("InsertAsync", method.Name);
			Assert.Equal(cancellation.Token, args[^1]);
			Assert.Equal(content, scope.Files.Content);
			Assert.Equal(1, scope.Files.Disposals);
			Assert.Equal(scope.Files.Path, data.GetValue(feedback => feedback.Content));
			Assert.Equal("text/plain", data.GetValue(feedback => feedback.ContentType));
			writes++;
			return TestRecords.Reply(1);
		};
		var operation = new FeedbackProbe(scope).InsertCoreAsync(data, cancellation.Token).AsTask();

		try
		{
			await scope.Files.Writing.Task.WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
			Assert.False(operation.IsCompleted);
			Assert.Equal(0, writes);
		}
		finally { release.TrySetResult(); }

		Assert.Equal(1, await operation);
		Assert.Equal(1, writes);
		Assert.DoesNotContain("DeleteAsync", scope.Files.Calls);
	}

	[Theory]
	[InlineData(false, false)]
	[InlineData(false, true)]
	[InlineData(true, false)]
	[InlineData(true, true)]
	public async Task Feedback_UnsuccessfulDatabaseWriteRemovesFile(bool dictionary, bool throws)
	{
		await using var scope = new TestScope();
		var failure = new InvalidOperationException("database failed");
		var data = CreateData(dictionary, new string('x', 501));
		var writes = 0;
		scope.Calls.Handler = (_, _) =>
		{
			writes++;
			Assert.Equal(1, scope.Files.Disposals);
			return throws ? TestRecords.Failure<int>(failure) : TestRecords.Reply(0);
		};
		var service = new FeedbackProbe(scope);

		if(throws)
			Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.InsertCoreAsync(data, CancellationToken.None)));
		else
			Assert.Equal(0, await service.InsertCoreAsync(data, CancellationToken.None));
		Assert.Equal(1, writes);
		Assert.False(scope.Files.Exists);
		Assert.Equal("DeleteAsync", scope.Files.Calls[^1]);
		Assert.Equal(CancellationToken.None, scope.Files.DeleteToken);
	}

	private static IDataDictionary<Feedback> CreateData(bool dictionary, string content) => DataDictionary.GetDictionary<Feedback>(dictionary ?
		new Hashtable { ["FeedbackId"] = 12UL, ["Content"] = content, ["ContentType"] = "text/plain" } :
		Model.Build<Feedback>(feedback =>
		{
			feedback.FeedbackId = 12;
			feedback.Content = content;
			feedback.ContentType = "text/plain";
		}));

	private sealed class FeedbackProbe : FeedbackService
	{
		private readonly string _path;
		public FeedbackProbe(TestScope scope) : base(scope.Services)
		{
			this.DataAccess = scope.Accessor;
			_path = scope.Files.Path;
		}
		protected override string GetContentFilePath(ulong feedbackId) => _path;
		public ValueTask<int> InsertCoreAsync(IDataDictionary<Feedback> data, CancellationToken cancellation) =>
			base.OnInsertAsync(data, null, new DataInsertOptions(), cancellation);
	}
}
