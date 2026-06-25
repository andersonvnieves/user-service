using br.com.fiap.cloudgames.Domain.Exceptions;

namespace br.com.fiap.cloudgames.Domain.ValueObjects;

public record Name
{
    public String FirstName { get; }
    public String LastName { get; }
    
    public String FullName  => $"{FirstName} {LastName}";

    public Name(String firstName, String lastName)
    {
        var errors = new List<string>();

        if (String.IsNullOrWhiteSpace(firstName))
            errors.Add("FirstName is required.");

        if (String.IsNullOrWhiteSpace(lastName))
            errors.Add("LastName is required.");

        if (errors.Any())
            throw new DomainException(errors);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }       
}