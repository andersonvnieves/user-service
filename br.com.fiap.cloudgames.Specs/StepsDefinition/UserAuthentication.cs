using br.com.fiap.cloudgames.Application.Services;
using br.com.fiap.cloudgames.Application.UseCases.User.LogIn;
using br.com.fiap.cloudgames.Domain.Aggregates;
using br.com.fiap.cloudgames.Domain.Repositories;
using br.com.fiap.cloudgames.Domain.ValueObjects;
using br.com.fiap.cloudgames.Specs.ContextData;
using Microsoft.Extensions.Logging;
using Moq;
using Reqnroll;

namespace br.com.fiap.cloudgames.Specs.StepsDefinition;

[Binding]
public class UserAuthentication
{
    private readonly UserAuthenticationContextData _context;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserAuthService> _userAuthServiceMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<ILogger<LogInUseCase>> _loggerMock = new();
    private LogInUseCase _logInUseCase;
    private LogInResponse _authenticationResult;

    public UserAuthentication()
    {
        _context = new UserAuthenticationContextData();

        _logInUseCase = new LogInUseCase(
            _userAuthServiceMock.Object,
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object
        );
    }
    [Given("a user with email {string} and password {string} exists")]
    public void GivenAUserWithEmailAndPasswordExists(string email, string password)
    {
        _context.Email = email;
        _context.Password = password;
        var identityId = Guid.NewGuid().ToString();
        var name = new Name("FirstName", "LastName");
        var emailAddress = new EmailAddress(email);
        var user = User.Create(name, emailAddress, identityId);
        var token = "FAKE TOKEN";
        _userAuthServiceMock
            .Setup(x => x.AuthenticateUserAsync(email,password))
            .ReturnsAsync(identityId);

        _userRepositoryMock
            .Setup(x => x.GetByIdentityIdAsync(identityId))
            .ReturnsAsync(user);
        
        _tokenServiceMock
            .Setup(x => x.GenerateTokenAsync(user))
            .ReturnsAsync(token);
    }

    [When("the user submits the login request with email {string} and password {string}")]
    public async Task WhenTheUserSubmitsTheLoginRequestWithEmailAndPassword(string email, string password)
    {
        try
        {
            _authenticationResult = await _logInUseCase.ExecuteAsync(new LogInRequest()
            {
                email = _context.Email,
                password = _context.Password
            });

            _context.GeneratedToken = _authenticationResult.Token;
        }
        catch (Exception ex)
        {
            _context.Exception = ex;
        }
    }

    [Then("the authentication should be successful")]
    public void ThenTheAuthenticationShouldBeSuccessful()
    {
        Assert.Null(_context.Exception);
        Assert.NotNull(_context.GeneratedToken);
    }

    [Then("the response should contain an access token")]
    public void ThenTheResponseShouldContainAnAccessToken()
    {
        Assert.False(string.IsNullOrWhiteSpace(_context.GeneratedToken));
        Assert.Equal("FAKE TOKEN", _context.GeneratedToken);
    }

    [Then("the authentication should fail")]
    public void ThenTheAuthenticationShouldFail()
    {
        Assert.NotNull(_context.Exception);
        Assert.IsType<NullReferenceException>(_context.Exception);
    }

    [Given("no user exists with email {string}")]
    public void GivenNoUserExistsWithEmail(string email)
    {
        _context.Email = email;
        var identityId = Guid.NewGuid().ToString();
        _userAuthServiceMock.Setup(x => x.AuthenticateUserAsync(email, _context.Password))
            .Throws(new NullReferenceException());
    }

    [When("the user submits the login request with email {string} and wrong password {string}")]
    public async Task WhenTheUserSubmitsTheLoginRequestWithEmailAndWrongPassword(string email, string password)
    {
        _userAuthServiceMock
            .Setup(x => x.AuthenticateUserAsync(email,password))
            .ThrowsAsync(new NullReferenceException());
        try
        {
            _authenticationResult = await _logInUseCase.ExecuteAsync(new LogInRequest()
            {
                email = email,
                password = password
            });

            _context.GeneratedToken = _authenticationResult.Token;
        }
        catch (Exception ex)
        {
            _context.Exception = ex;
        }
    }
}