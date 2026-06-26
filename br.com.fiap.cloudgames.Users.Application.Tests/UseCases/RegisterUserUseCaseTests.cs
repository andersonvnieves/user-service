using br.com.fiap.cloudgames.Users.Application.Publishers;
using br.com.fiap.cloudgames.Users.Application.Services;
using br.com.fiap.cloudgames.Users.Application.Tests.TestData;
using br.com.fiap.cloudgames.Users.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Users.Application.UseCases.User.RegisterUser;
using br.com.fiap.cloudgames.Users.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace br.com.fiap.cloudgames.Users.Application.Tests.UseCases;

public class RegisterUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateIdentityUser_ThenPersistDomainUser_AndCommit()
    {
        var auth = new Mock<IUserAuthService>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var publisher = new Mock<IUserCreatedEventPublisher>(MockBehavior.Strict);
        var logger = new Mock<ILogger<RegisterUserUseCase>>(MockBehavior.Loose);

        var request = ApplicationTestData.ValidRegisterUserRequest();

        uow.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        auth.Setup(x => x.CreateUserAsync(request.Email, request.Password, "user"))
            .ReturnsAsync("identity-123");
        repo.Setup(x => x.AddAsync(It.IsAny<br.com.fiap.cloudgames.Users.Domain.Aggregates.User>()))
            .Returns(Task.CompletedTask);
        uow.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
        publisher.Setup(x => x.PublishAsync(It.IsAny<br.com.fiap.cloudgames.Users.Application.Events.UserCreatedEvent>()))
            .Returns(Task.CompletedTask);

        var sut = new RegisterUserUseCase(auth.Object, uow.Object, repo.Object, publisher.Object, logger.Object);

        var response = await sut.ExecuteAsync(request);

        Assert.False(string.IsNullOrWhiteSpace(response.Id));
        Assert.Equal(request.FirstName, response.FirstName);
        Assert.Equal(request.LastName, response.LastName);
        Assert.Equal(request.Email, response.Email);
        Assert.Equal("user", response.Role);

        uow.Verify(x => x.BeginTransactionAsync(), Times.Once);
        auth.Verify(x => x.CreateUserAsync(request.Email, request.Password, "user"), Times.Once);
        repo.Verify(x => x.AddAsync(It.IsAny<br.com.fiap.cloudgames.Users.Domain.Aggregates.User>()), Times.Once);
        uow.Verify(x => x.CommitAsync(), Times.Once);
        uow.Verify(x => x.RollbackAsync(), Times.Never);
        publisher.Verify(x => x.PublishAsync(It.IsAny<br.com.fiap.cloudgames.Users.Application.Events.UserCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthServiceThrows_ShouldRollback_AndRethrow()
    {
        var auth = new Mock<IUserAuthService>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var publisher = new Mock<IUserCreatedEventPublisher>(MockBehavior.Strict);
        var logger = new Mock<ILogger<RegisterUserUseCase>>(MockBehavior.Loose);

        var request = ApplicationTestData.ValidRegisterUserRequest();

        uow.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        auth.Setup(x => x.CreateUserAsync(request.Email, request.Password, "user"))
            .ThrowsAsync(new InvalidOperationException("boom"));
        uow.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

        var sut = new RegisterUserUseCase(auth.Object, uow.Object, repo.Object, publisher.Object, logger.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.ExecuteAsync(request));

        uow.Verify(x => x.BeginTransactionAsync(), Times.Once);
        uow.Verify(x => x.RollbackAsync(), Times.Once);
        repo.Verify(x => x.AddAsync(It.IsAny<br.com.fiap.cloudgames.Users.Domain.Aggregates.User>()), Times.Never);
        uow.Verify(x => x.CommitAsync(), Times.Never);
    }
}

