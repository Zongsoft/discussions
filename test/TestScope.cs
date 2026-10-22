using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Security.Claims;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

using Zongsoft.IO;
using Zongsoft.Data;
using Zongsoft.Data.Metadata;
using Zongsoft.Security;
using Zongsoft.Services;
using Zongsoft.Discussions.Security;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Zongsoft.Discussions.Tests;

internal sealed class TestScope : IAsyncDisposable
{
	private readonly IApplicationContext _previous = Zongsoft.Services.ApplicationContext.Current;
	private readonly Context _context;
	private readonly List<CredentialPrincipal> _principals = [];
	private readonly List<string> _entities = [];

	public TestScope()
	{
		var services = new ServiceCollection();
		services.AddSingleton<IClaimsIdentityTransformer>(new IdentityTransformer());
		this.Services = new ServiceProviderFactory().CreateServiceProvider(services);
		_context = new Context(this.Services);
		this.Accessor = DispatchProxy.Create<IDataAccess, InvocationProxy>();
		this.Calls = (InvocationProxy)(object)this.Accessor;
		this.Files = new MemoryFiles();
		FileSystem.Providers.Add(this.Files);
	}

	public IServiceProvider Services { get; }
	public IDataAccess Accessor { get; }
	public InvocationProxy Calls { get; }
	public MemoryFiles Files { get; }
	public ClaimsPrincipal Principal => _context.Principal;

	public void Login(uint userId, uint siteId = 17)
	{
		var identity = new ClaimsIdentity([
			new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
			new Claim(ClaimTypes.Name, "tester"),
			new Claim(nameof(UserIdentity.SiteId), siteId.ToString()),
		], UserIdentity.Scheme);
		var principal = new CredentialPrincipal(identity, Guid.NewGuid().ToString("N"), "renewal", "tests");
		_principals.Add(principal);
		_context.User = principal;
	}

	public SelectContext Select<T>(object source)
	{
		var entity = this.Entity();
		var context = new SelectContext(this.Accessor, entity.QualifiedName, typeof(T));
		context.Result = source;
		return context;
	}

	public DataEntity Entity(bool site = true)
	{
		var entity = new DataEntity("UnitTests", "Entity" + Guid.NewGuid().ToString("N"));
		if(site)
			entity.Properties.Add(new DataEntitySimplexProperty("SiteId", DataType.UInt32, false));
		Mapping.Entities.Add(entity);
		_entities.Add(entity.QualifiedName);
		return entity;
	}

	public async ValueTask DisposeAsync()
	{
		try
		{
			foreach(var principal in _principals)
				principal.Dispose();
			await _context.DisposeAsync();
			await ((IAsyncDisposable)this.Services).DisposeAsync();
		}
		finally
		{
			FileSystem.Providers.Remove(this.Files.Scheme);
			foreach(var entity in _entities)
				Mapping.Entities.Remove(entity);
			typeof(Zongsoft.Services.ApplicationContext).GetField("_current", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, _previous);
		}
	}

	private sealed class Context(IServiceProvider services) : Zongsoft.Services.ApplicationContext(services)
	{
		public ClaimsPrincipal User { get; set; } = new(new ClaimsIdentity());
		public override ClaimsPrincipal Principal => this.User;
	}

	private sealed class IdentityTransformer : IClaimsIdentityTransformer
	{
		public bool CanTransform(ClaimsIdentity identity) => identity.AuthenticationType == UserIdentity.Scheme;
		public object Transform(ClaimsIdentity identity) => new UserIdentity
		{
			UserId = uint.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value),
			SiteId = uint.Parse(identity.FindFirst(nameof(UserIdentity.SiteId)).Value),
		};
	}
}

public class InvocationProxy : DispatchProxy
{
	internal Func<MethodInfo, object[], object> Handler { get; set; } = (method, _) => throw new InvalidOperationException(method.Name);
	protected override object Invoke(MethodInfo method, object[] arguments) => this.Handler(method, arguments);
}

internal sealed class SelectContext(IDataAccess accessor, string name, Type type) : DataSelectContextBase(accessor, name, type, null, null, null, null, null)
{
	public override TFeature GetFeature<TFeature>() => default;
}

internal sealed class MutateContext(DataEntity entity, object data = null) : IDataMutateContextBase
{
	public IDataEntity Entity => entity;
	public object Data { get; set; } = data;
	public int Count { get; set; }
	public IDataMutateOptions Options => null;
	public IDataValidator Validator => null;
	public string Name => entity.Name;
	public DataAccessMethod Method => DataAccessMethod.Insert;
	public IDataAccess DataAccess => null;
	public ClaimsPrincipal Principal => Zongsoft.Services.ApplicationContext.Current.Principal;
	public TFeature GetFeature<TFeature>() => default;
}

internal sealed class RecordStream<T>(IEnumerable<T> items) : IAsyncEnumerable<T>, IPageable
{
	public bool Suppressed { get; set; }
	public event Func<object, PagingEventArgs, CancellationToken, ValueTask> Paginated;
	public int Disposals { get; private set; }
	public CancellationToken Token { get; private set; }
	public Action OnStarted { get; set; }
	public Task Release { get; set; } = Task.CompletedTask;
	public TaskCompletionSource Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
	public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellation = default)
	{
		this.Token = cancellation;

		try
		{
			this.OnStarted?.Invoke();
			this.Entered.TrySetResult();
			await this.Release.WaitAsync(cancellation);

			foreach(var item in items)
			{
				cancellation.ThrowIfCancellationRequested();
				yield return item;
			}
		}
		finally { this.Disposals++; }
	}
	public ValueTask RaiseAsync(PagingEventArgs args, CancellationToken cancellation) => this.Paginated?.Invoke(this, args, cancellation) ?? ValueTask.CompletedTask;
}

internal sealed class MemoryFiles : IFileSystem
{
	private readonly InvocationProxy _proxy;
	public MemoryFiles()
	{
		this.File = DispatchProxy.Create<IFile, InvocationProxy>();
		_proxy = (InvocationProxy)(object)this.File;
		_proxy.Handler = this.Invoke;
	}
	public string Scheme { get; } = "test" + Guid.NewGuid().ToString("N");
	public string Path => this.Scheme + ":/content.txt";
	public IFile File { get; }
	public IDirectory Directory => throw new NotSupportedException();
	public string GetUrl(string path) => path;
	public string GetUrl(Zongsoft.IO.Path path) => path.Url;
	public List<string> Calls { get; } = [];
	public string Content { get; set; } = "external body";
	public bool Exists { get; set; } = true;
	public Exception WriteFailure { get; set; }
	public Exception DeleteFailure { get; set; }
	public Task WriteRelease { get; set; } = Task.CompletedTask;
	public Task ReadRelease { get; set; } = Task.CompletedTask;
	public TaskCompletionSource Writing { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
	public TaskCompletionSource Reading { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
	public int Disposals { get; private set; }
	public CancellationToken DeleteToken { get; private set; }
	private object Invoke(MethodInfo method, object[] args)
	{
		var token = args.OfType<CancellationToken>().Single();
		this.Calls.Add(method.Name);
		return method.Name switch
		{
			"ExistsAsync" => TestRecords.Reply(this.Exists),
			"OpenAsync" => TestRecords.Reply<Stream>(new MemoryStreamProbe(this, args.OfType<FileAccess>().Single() == FileAccess.Read)),
			"DeleteAsync" => this.DeleteAsync(token),
			_ => throw new InvalidOperationException(method.Name),
		};
	}
	private object DeleteAsync(CancellationToken cancellation)
	{
		this.DeleteToken = cancellation;
		if(this.DeleteFailure != null)
			return TestRecords.Failure<bool>(this.DeleteFailure);
		this.Exists = false;
		return TestRecords.Reply(true);
	}
	private sealed class MemoryStreamProbe : MemoryStream
	{
		private readonly MemoryFiles _owner;
		private readonly bool _reading;
		public MemoryStreamProbe(MemoryFiles owner, bool reading)
		{
			_owner = owner;
			_reading = reading;

			if(reading)
			{
				base.Write(Encoding.UTF8.GetBytes(owner.Content));
				this.Position = 0;
			}
		}
		public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
		{
			_owner.Reading.TrySetResult();
			await _owner.ReadRelease.WaitAsync(cancellationToken);
			return await base.ReadAsync(buffer, cancellationToken);
		}
		public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
		{
			_owner.Writing.TrySetResult();
			await _owner.WriteRelease.WaitAsync(cancellationToken);
			if(_owner.WriteFailure != null)
				throw _owner.WriteFailure;
			await base.WriteAsync(buffer, cancellationToken);
		}
		public override ValueTask DisposeAsync()
		{
			_owner.Disposals++;
			if(!_reading)
				_owner.Content = Encoding.UTF8.GetString(this.ToArray()).TrimStart('\uFEFF');
			return base.DisposeAsync();
		}
	}
}

internal static class TestRecords
{
	public static object Reply<T>(T value) => new ValueTask<T>(value);
	public static object Failure<T>(Exception error) => new ValueTask<T>(Task.FromException<T>(error));
	public static async Task<T[]> CollectAsync<T>(IAsyncEnumerable<T> source, CancellationToken cancellation = default)
	{
		var result = new List<T>();
		await foreach(var item in source.WithCancellation(cancellation))
			result.Add(item);
		return result.ToArray();
	}
	public static IEnumerable<Condition> Conditions(ICondition condition) => condition switch
	{
		Condition item => [item],
		ConditionCollection items => items.SelectMany(Conditions),
		_ => [],
	};
}
