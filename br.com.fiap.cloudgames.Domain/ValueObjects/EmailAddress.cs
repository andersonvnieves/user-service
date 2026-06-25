using System.Net.Mail;
using br.com.fiap.cloudgames.Domain.Exceptions;

namespace br.com.fiap.cloudgames.Domain.ValueObjects;

public record EmailAddress
{
    public String Email { get; }

    public EmailAddress(String email)
    {
        var errors = new List<string>();

        if (String.IsNullOrWhiteSpace(email))
            errors.Add("Email is required.");

        if (!String.IsNullOrWhiteSpace(email) && email.Length > 254)
            errors.Add("Email must be at most 254 characters.");

        if (errors.Any())
            throw new DomainException(errors);

        try
        {
            var validEmail = new MailAddress(email);
            Email = validEmail.Address.ToLowerInvariant();
        }
        catch (FormatException)
        {
            throw new DomainException(new List<string> { "Email address must be a valid email address." });
        }
    }
    
    public override String ToString() => Email;
}