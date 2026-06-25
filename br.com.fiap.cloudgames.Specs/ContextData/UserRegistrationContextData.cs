using br.com.fiap.cloudgames.Application.UseCases.User.RegisterUser;

namespace br.com.fiap.cloudgames.Specs.ContextData;

public class UserRegistrationContextData
{
    public RegisterUserRequest Request { get; set; }
    public RegisterUserResponse Response { get; set; }
    public Exception Exception { get; set; }
    public string RegisteredUserID { get; set; }
}