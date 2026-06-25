using br.com.fiap.cloudgames.Application.Services;
using br.com.fiap.cloudgames.Application.Tests.TestData;
using br.com.fiap.cloudgames.Application.UseCases.User.LogIn;
using br.com.fiap.cloudgames.Domain.Aggregates;
using br.com.fiap.cloudgames.Domain.Repositories;
using br.com.fiap.cloudgames.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace br.com.fiap.cloudgames.Application.Tests.UseCases;

public class LogInUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidCredentials_ShouldAuthenticate_LoadUser_AndReturnToken()
    {
        var auth = new Mock<IUserAuthService>(MockBehavior.Strict);
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var tokens = new Mock<ITokenService>(MockBehavior.Strict);
        var logger = new Mock<ILogger<LogInUseCase>>(MockBehavior.Loose);

        var request = ApplicationTestData.ValidLogInRequest();
        var normalizedEmail = new EmailAddress(request.email).Email;

        auth.Setup(x => x.AuthenticateUserAsync(normalizedEmail, request.password))
            .ReturnsAsync("identity-123");

        var user = User.Create(new Name("Anderson", "Silva"), new EmailAddress(request.email), "identity-123");
        repo.Setup(x => x.GetByIdentityIdAsync("identity-123")).ReturnsAsync(user);

        tokens.Setup(x => x.GenerateTokenAsync(user)).ReturnsAsync("jwt-token");

        var sut = new LogInUseCase(auth.Object, repo.Object, tokens.Object, logger.Object);

        var response = await sut.ExecuteAsync(request);

        Assert.Equal("jwt-token", response.Token);

        auth.Verify(x => x.AuthenticateUserAsync(normalizedEmail, request.password), Times.Once);
        repo.Verify(x => x.GetByIdentityIdAsync("identity-123"), Times.Once);
        tokens.Verify(x => x.GenerateTokenAsync(user), Times.Once);
    }
}

