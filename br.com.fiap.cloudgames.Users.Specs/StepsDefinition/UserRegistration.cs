using br.com.fiap.cloudgames.Users.Application.Publishers;
using br.com.fiap.cloudgames.Users.Application.Services;
using br.com.fiap.cloudgames.Users.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Users.Application.UseCases.User.RegisterUser;
using br.com.fiap.cloudgames.Users.Domain.Aggregates;
using br.com.fiap.cloudgames.Users.Domain.Repositories;
using br.com.fiap.cloudgames.Users.Specs.ContextData;
using Microsoft.Extensions.Logging;
using Moq;
using Reqnroll;

namespace br.com.fiap.cloudgames.Users.Specs.StepsDefinition;

[Binding]
public class UserRegistration
{
    private readonly UserRegistrationContextData _userRegistrationContextData;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserAuthService> _userAuthServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserCreatedEventPublisher> _userCreatedEventPublisher = new();
    private readonly Mock<ILogger<RegisterUserUseCase>>  _loggerMock = new();
    private RegisterUserUseCase _registerUserUseCase;

    public UserRegistration()
    {
        _userRegistrationContextData = new();
        
        _registerUserUseCase = new RegisterUserUseCase(
            _userAuthServiceMock.Object,
            _unitOfWorkMock.Object,
            _userRepositoryMock.Object,
            _userCreatedEventPublisher.Object,
            _loggerMock.Object
        );
    }
    
    
    [Given("a user with first name {string}, last name {string}, email {string} and password {string}")]
    public void GivenAUserWithFirstNameLastNameEmailAndPassword(string first, string last, string email, string password)
    {
        _userRegistrationContextData.Request = new RegisterUserRequest()
        {
            FirstName = first,
            LastName = last,
            Email = email,
            Password = password
        };

        _unitOfWorkMock
            .Setup(x => x.BeginTransactionAsync());
        _userAuthServiceMock
            .Setup(x => x.CreateUserAsync(email, password, "user"))
            .ReturnsAsync(Guid.NewGuid().ToString());
        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()));
        _unitOfWorkMock.Setup(x => x.CommitAsync());
        _unitOfWorkMock.Setup(x => x.RollbackAsync());
    }

    [When("the user submits the registration request")]
    public async Task WhenTheUserSubmitsTheRegistrationRequest()
    {
        try
        {
            _userRegistrationContextData.Response = await _registerUserUseCase.ExecuteAsync(_userRegistrationContextData.Request);
        }
        catch (Exception ex)
        {
            _userRegistrationContextData.Exception = ex;
        }
    }

    [Then("the account should be created successfully")]
    public void ThenTheAccountShouldBeCreatedSuccessfully()
    {
        Assert.Null(_userRegistrationContextData.Exception);
        Assert.NotNull(_userRegistrationContextData.Response);
    }

    [Then("the response should contain the user id")]
    public void ThenTheResponseShouldContainTheUserId()
    {
        Assert.True(Guid.TryParse(_userRegistrationContextData.Response.Id,  out Guid id));
    }

    [Given("a user with email {string} already exists")]
    public void GivenAUserWithEmailAlreadyExists(string email)
    {
        _userAuthServiceMock.Reset();
        _userAuthServiceMock
            .Setup(x => x.CreateUserAsync(email, _userRegistrationContextData.Request.Password, "user"))
            .Throws(new Exception("Email already in use"));
    }

    [Then("the registration should fail")]
    public void ThenTheRegistrationShouldFail()
    {
        Assert.NotNull(_userRegistrationContextData.Exception);
        Assert.Null(_userRegistrationContextData.Response);
    }

    [Then("an error {string} should be returned")]
    public void ThenAnErrorShouldBeReturned(string error)
    {
        Assert.NotNull(_userRegistrationContextData.Exception);
        Assert.Equal(error, _userRegistrationContextData.Exception.Message);
    }
}