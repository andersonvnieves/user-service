using br.com.fiap.cloudgames.Domain.Enums;
using br.com.fiap.cloudgames.Domain.Exceptions;
using br.com.fiap.cloudgames.Domain.ValueObjects;

namespace br.com.fiap.cloudgames.Domain.Aggregates;

public class User
{
    public User() { } //ORM

    #region  FactoryMethod
    public static User Create(Name name, EmailAddress email, String  identityId)
    {
        Validate(name, email, identityId);
        return new User()
        {
            Id = Guid.NewGuid(),
            Name = name, 
            Email = email,
            UserAccountStatus = UserAccountStatus.ACTIVE,
            IdentityId = identityId,
            CreationDate = DateTime.Now,
            Role = UserRoles.user
        };
    }
    #endregion
    
    #region  Properties
    public Guid Id { get; private set; }
    public Name Name { get; private set; }
    public EmailAddress Email { get; private set; }
    public UserAccountStatus UserAccountStatus { get; private set; }
    public String IdentityId { get; set; }
    public DateTime CreationDate { get; set; }
    public UserRoles Role { get; set; }
    #endregion

    private static void Validate(Name name, EmailAddress email, String identityId)
    {
        var errors = new List<string>();

        if (name is null)
            errors.Add("Name is required.");

        if (email is null)
            errors.Add("Email is required.");

        if (String.IsNullOrWhiteSpace(identityId))
            errors.Add("IdentityId is required.");

        if (errors.Any())
            throw new DomainException(errors);
    }
    
}