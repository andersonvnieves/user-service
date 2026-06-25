using br.com.fiap.cloudgames.Domain.Aggregates;
using br.com.fiap.cloudgames.Domain.Exceptions;
using br.com.fiap.cloudgames.Domain.Enums;
using br.com.fiap.cloudgames.Domain.Tests.TestData;

namespace br.com.fiap.cloudgames.Domain.Tests.Aggregates;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateActiveUser_WithDefaultRoleAndCreationDate()
    {
        var name = DomainTestData.ValidName();
        var email = DomainTestData.ValidEmail();
        var identityId = DomainTestData.ValidIdentityId();

        var user = User.Create(name, email, identityId);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(UserAccountStatus.ACTIVE, user.UserAccountStatus);
        Assert.Equal(UserRoles.user, user.Role);
        Assert.Equal(identityId, user.IdentityId);
        Assert.True(user.CreationDate <= DateTime.Now);
    }

    [Fact]
    public void Create_WhenNameIsNull_ShouldThrow()
    {
        var email = DomainTestData.ValidEmail();
        var identityId = DomainTestData.ValidIdentityId();

        var ex = Assert.Throws<DomainException>(() => User.Create(null!, email, identityId));
        Assert.Contains("Name is required.", ex.Errors);
    }

    [Fact]
    public void Create_WhenEmailIsNull_ShouldThrow()
    {
        var name = DomainTestData.ValidName();
        var identityId = DomainTestData.ValidIdentityId();

        var ex = Assert.Throws<DomainException>(() => User.Create(name, null!, identityId));
        Assert.Contains("Email is required.", ex.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WhenIdentityIdIsNullOrEmpty_ShouldThrow(string? identityId)
    {
        var name = DomainTestData.ValidName();
        var email = DomainTestData.ValidEmail();

        var ex = Assert.Throws<DomainException>(() => User.Create(name, email, identityId!));
        Assert.Contains("IdentityId is required.", ex.Errors);
    }
}

