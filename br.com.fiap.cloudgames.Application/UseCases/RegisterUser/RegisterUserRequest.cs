namespace br.com.fiap.cloudgames.Application.UseCases.User.RegisterUser;

public class RegisterUserRequest
{
    public required String FirstName { get; set; }
    public required String LastName { get; set; }
    public required String Email { get; set; }
    public required String Password { get; set; }
}

