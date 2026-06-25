using br.com.fiap.cloudgames.Domain.Enums;
using br.com.fiap.cloudgames.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Domain.Tests.TestData;

public static class DomainTestData
{
    public static Name ValidName() => new("Anderson", "Silva");

    public static EmailAddress ValidEmail() => new("Anderson.Silva@Example.com");

    public static string ValidIdentityId() => "identity-123";
}

