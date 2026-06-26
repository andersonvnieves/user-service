using br.com.fiap.cloudgames.Users.Domain.Enums;

namespace br.com.fiap.cloudgames.Users.Application.UseCases.User.RetrieveUser;

public class RetrieveUserResponse
{
    public String Id { get; set; }
    public String FirstName { get; set; }
    public String LastName { get; set; }
    public String Email { get; set; }
    public String Role { get; set; }
    public UserAccountStatus UserAccountStatus { get; set; }
    public DateTime CreationDate { get; set; }
}