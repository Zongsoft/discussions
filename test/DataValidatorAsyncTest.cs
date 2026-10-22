using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;

using Zongsoft.Data;
using Zongsoft.Data.Metadata;
using Zongsoft.Discussions.Data;

namespace Zongsoft.Discussions.Tests;

public class DataValidatorAsyncTest
{
	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task TenantCriteria_AlwaysConjoinsCurrentSite(bool existingSite)
	{
		await using var scope = new TestScope();
		scope.Login(42, 17);
		var validator = new DataValidator();
		var context = new MutateContext(scope.Entity());
		ICondition criteria = existingSite ? Condition.Equal("SiteId", 99U) : Condition.Equal("Title", "sample");

		var result = await validator.ValidateAsync(context, criteria, TestContext.Current.CancellationToken);

		var conjunction = Assert.IsType<ConditionCollection>(result);
		Assert.Equal(ConditionCombination.And, conjunction.Combination);
		Assert.Contains(criteria, conjunction);
		var enforced = Assert.Single(TestRecords.Conditions(result), item => item.Name == "SiteId" && Equals(item.Value, 17U));
		Assert.Equal(ConditionOperator.Equal, enforced.Operator);
	}

	[Theory]
	[InlineData(false, "SiteId", true, 17U)]
	[InlineData(false, "CreatorId", true, 42U)]
	[InlineData(false, "ModifierId", false, 0U)]
	[InlineData(true, "ModifierId", true, 42U)]
	[InlineData(true, "SiteId", false, 0U)]
	[InlineData(false, "Unknown", false, 0U)]
	public async Task FieldValidation_PreservesSuppliedAndAbsentValues(bool update, string name, bool hasValue, uint value)
	{
		await using var scope = new TestScope();
		scope.Login(42, 17);
		var validator = new DataValidator();
		var context = new MutateContext(scope.Entity());
		var property = new DataEntitySimplexProperty(name, DataType.UInt32, false);

		var result = update ? await validator.OnUpdateAsync(context, property, TestContext.Current.CancellationToken) : await validator.OnInsertAsync(context, property, TestContext.Current.CancellationToken);

		Assert.Equal(hasValue, result.HasValue);
		if(hasValue)
			Assert.Equal(value, result.Value);
		else
			Assert.Null(result.Value);
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task GlobalAdmin_RespectsExplicitSite(bool explicitSite)
	{
		await using var scope = new TestScope();
		scope.Login(42, 0);
		var data = explicitSite ? new Dictionary<string, object> { ["SiteId"] = 23U } : null;
		var context = new MutateContext(scope.Entity(), data);
		var validator = new DataValidator();
		var property = new DataEntitySimplexProperty("SiteId", DataType.UInt32, false);

		var result = await validator.OnInsertAsync(context, property, TestContext.Current.CancellationToken);

		Assert.Equal(!explicitSite, result.HasValue);
		Assert.Equal(explicitSite ? null : 0U, result.Value);
		var criteria = Condition.Equal("SiteId", 23U);
		Assert.Same(criteria, await validator.ValidateAsync(context, criteria, TestContext.Current.CancellationToken));
	}

	[Fact]
	public async Task AnonymousAndUnscopedEntities_DoNotInjectTenantOrCreator()
	{
		await using var scope = new TestScope();
		var validator = new DataValidator();
		var context = new MutateContext(scope.Entity());
		var criteria = Condition.Equal("Title", "sample");
		Assert.Same(criteria, await validator.ValidateAsync(context, criteria, TestContext.Current.CancellationToken));
		Assert.False((await validator.OnInsertAsync(context, new DataEntitySimplexProperty("CreatorId", DataType.UInt32, false), TestContext.Current.CancellationToken)).HasValue);
		scope.Login(42);
		Assert.Same(criteria, await validator.ValidateAsync(new MutateContext(scope.Entity(false)), criteria, TestContext.Current.CancellationToken));
	}

	[Theory]
	[InlineData(17U, "SiteId", true, 17U)]
	[InlineData(0U, "SiteId", false, 0U)]
	[InlineData(17U, "CreatorId", true, 42U)]
	[InlineData(17U, "Unknown", false, 0U)]
	public async Task ImportFields_UseIdentityWithoutSupplyingUnknownValues(uint siteId, string name, bool supplied, uint value)
	{
		await using var scope = new TestScope();
		scope.Login(42, siteId);
		using var context = new ImportContext(scope.Accessor, scope.Entity().QualifiedName);
		var result = await new DataValidator().OnImportAsync(context, new DataEntitySimplexProperty(name, DataType.UInt32, false), TestContext.Current.CancellationToken);
		Assert.Equal(supplied, result.HasValue);
		Assert.Equal(supplied ? value : null, result.Value);
	}

	[Fact]
	public async Task TimestampFields_SupplyCurrentTime()
	{
		await using var scope = new TestScope();
		var context = new MutateContext(scope.Entity());
		var before = DateTime.Now;
		var result = await new DataValidator().OnInsertAsync(context, new DataEntitySimplexProperty("CreatedTime", DataType.DateTime, false), TestContext.Current.CancellationToken);
		Assert.True(result.HasValue);
		Assert.InRange(Assert.IsType<DateTime>(result.Value), before, DateTime.Now);
	}

	private sealed class ImportContext(IDataAccess accessor, string name) : DataImportContextBase(accessor, name, Array.Empty<object>(), null)
	{
		public override TFeature GetFeature<TFeature>() => default;
	}
}
