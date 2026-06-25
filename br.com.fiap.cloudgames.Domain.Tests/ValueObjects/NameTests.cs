using br.com.fiap.cloudgames.Domain.ValueObjects;
using br.com.fiap.cloudgames.Domain.Exceptions;

namespace br.com.fiap.cloudgames.Domain.Tests.ValueObjects;

public class NameTests
{
    [Fact]
    public void ShouldTrimNames_AndBuildFullName()
    {
        var name = new Name("  Anderson ", " Silva  ");

        Assert.Equal("Anderson", name.FirstName);
        Assert.Equal("Silva", name.LastName);
        Assert.Equal("Anderson Silva", name.FullName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WhenFirstNameIsBlank_ShouldThrow(string? firstName)
    {
        var ex = Assert.Throws<DomainException>(() => new Name(firstName!, "Silva"));
        Assert.Contains("FirstName is required.", ex.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void WhenLastNameIsBlank_ShouldThrow(string? lastName)
    {
        var ex = Assert.Throws<DomainException>(() => new Name("Anderson", lastName!));
        Assert.Contains("LastName is required.", ex.Errors);
    }
}

