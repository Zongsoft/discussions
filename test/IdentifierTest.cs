using System;
using System.Security.Claims;

using Xunit;

using Zongsoft.Data;
using Zongsoft.Security;
using Zongsoft.Components;
using Zongsoft.Security.Privileges;

namespace Zongsoft.Discussions.Tests;

public class IdentifierTest
{
	[Fact]
	public void TestUserIdentity() => Verify(new Security.UserIdentity());
	[Fact]
	public void TestUserProfile() => Verify(Model.Build<Models.UserProfile>());
	[Fact]
	public void TestClaims()
	{
		var identity = new ClaimsIdentity([
			new Claim(ClaimTypes.Name, "alice"),
			new Claim(ClaimTypes.NameIdentifier, "17", ClaimValueTypes.UInteger32),
		], "Test");
		var profile = identity.AsModel<Models.UserProfile>();

		Assert.Equal(17u, profile.UserId);
		Assert.Equal(new Identifier(Users.Identifiers.User, 17u), ((IIdentifiable)profile).Identifier);
	}

	private static void Verify(IIdentifiable model)
	{
		model.Identifier = new(Users.Identifiers.User, "17");

		Assert.Equal(new Identifier(Users.Identifiers.User, 17u), model.Identifier);
		Assert.Throws<ArgumentException>(() => model.Identifier = new(Roles.Identifiers.Role, 23u));
		Assert.Throws<ArgumentException>(() => model.Identifier = new(Users.Identifiers.User, "invalid"));
		Assert.Equal(17u, model.Identifier.Value);
	}
}
