using br.com.fiap.cloudgames.Application.UseCases.User.RetrieveUser;
using br.com.fiap.cloudgames.Domain.Aggregates;
using br.com.fiap.cloudgames.Domain.Repositories;
using br.com.fiap.cloudgames.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace br.com.fiap.cloudgames.Application.Tests.UseCases;

public class RetrieveUserUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenUserIdIsInvalid_ShouldThrow()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<RetrieveUserUseCase>>(MockBehavior.Loose);
        var sut = new RetrieveUserUseCase(repo.Object, logger.Object);

        var request = new RetrieveUserRequest { UserId = "not-a-guid" };

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
        Assert.Contains("Invalid UserId Format", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrow()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<RetrieveUserUseCase>>(MockBehavior.Loose);
        var sut = new RetrieveUserUseCase(repo.Object, logger.Object);

        var userId = Guid.NewGuid();
        repo.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);

        var request = new RetrieveUserRequest { UserId = userId.ToString() };

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
        Assert.Contains("User not found", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldReturnUser()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<RetrieveUserUseCase>>(MockBehavior.Loose);
        var sut = new RetrieveUserUseCase(repo.Object, logger.Object);

        var user = User.Create(
            new Name("Anderson", "Silva"),
            new EmailAddress("anderson.silva@example.com"),
            "identity-123");

        repo.Setup(x => x.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        var request = new RetrieveUserRequest { UserId = user.Id.ToString() };

        var response = await sut.ExecuteAsync(request);

        Assert.Equal(user.Id.ToString(), response.Id);
        Assert.Equal("Anderson", response.FirstName);
        Assert.Equal("Silva", response.LastName);
        Assert.Equal("anderson.silva@example.com", response.Email);
        Assert.Equal(user.Role.ToString(), response.Role);
        Assert.Equal(user.UserAccountStatus, response.UserAccountStatus);
        Assert.Equal(user.CreationDate, response.CreationDate);
    }
}

