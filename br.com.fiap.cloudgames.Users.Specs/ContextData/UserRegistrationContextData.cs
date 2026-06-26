using br.com.fiap.cloudgames.Users.Application.UseCases.User.RegisterUser;

namespace br.com.fiap.cloudgames.Users.Specs.ContextData;

public class UserRegistrationContextData
{
    public RegisterUserRequest Request { get; set; }
    public RegisterUserResponse Response { get; set; }
    public Exception Exception { get; set; }
    public string RegisteredUserID { get; set; }
}