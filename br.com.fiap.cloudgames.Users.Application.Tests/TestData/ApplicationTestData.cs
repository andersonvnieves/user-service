using br.com.fiap.cloudgames.Users.Application.UseCases.User.ChangeUserRole;
using br.com.fiap.cloudgames.Users.Application.UseCases.User.LogIn;
using br.com.fiap.cloudgames.Users.Application.UseCases.User.RegisterUser;

namespace br.com.fiap.cloudgames.Users.Application.Tests.TestData;

public static class ApplicationTestData
{
    public static RegisterUserRequest ValidRegisterUserRequest() => new()
    {
        FirstName = "Anderson",
        LastName = "Silva",
        Email = "anderson.silva@example.com",
        Password = "Pass@123!"
    };

    public static LogInRequest ValidLogInRequest() => new()
    {
        email = "anderson.silva@example.com",
        password = "Pass@123!"
    };

    public static ChangeUserRoleRequest ValidChangeUserRoleRequest() => new()
    {
        UserId = Guid.NewGuid().ToString(),
        Role = "admin"
    };
}

