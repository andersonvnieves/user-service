using br.com.fiap.cloudgames.Domain.ValueObjects;
using br.com.fiap.cloudgames.Domain.Exceptions;

namespace br.com.fiap.cloudgames.Domain.Tests.ValueObjects;

public class EmailAddressTests
{
    [Fact]
    public void ShouldNormalizeToLowerInvariant()
    {
        var email = new EmailAddress("Anderson.Silva@Example.com");

        Assert.Equal("anderson.silva@example.com", email.Email);
        Assert.Equal("anderson.silva@example.com", email.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WhenEmailIsBlank_ShouldThrow(string? value)
    {
        var ex = Assert.Throws<DomainException>(() => new EmailAddress(value!));
        Assert.Contains("Email is required.", ex.Errors);
    }

    [Fact]
    public void WhenEmailIsTooLong_ShouldThrow()
    {
        var tooLong = new string('a', 255);
        var ex = Assert.Throws<DomainException>(() => new EmailAddress(tooLong));
        Assert.Contains("Email must be at most 254 characters.", ex.Errors);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("abc@")]
    [InlineData("@example.com")]
    public void WhenEmailIsInvalid_ShouldThrow(string value)
    {
        var ex = Assert.Throws<DomainException>(() => new EmailAddress(value));
        Assert.Contains("Email address must be a valid email address.", ex.Errors);
    }
}

