using br.com.fiap.cloudgames.Application.Services;
using br.com.fiap.cloudgames.Application.Tests.TestData;
using br.com.fiap.cloudgames.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Application.UseCases.User.ChangeUserRole;
using br.com.fiap.cloudgames.Domain.Aggregates;
using br.com.fiap.cloudgames.Domain.Enums;
using br.com.fiap.cloudgames.Domain.Repositories;
using br.com.fiap.cloudgames.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace br.com.fiap.cloudgames.Application.Tests.UseCases;

public class ChangeUserRoleUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldRollback_AndThrow()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var auth = new Mock<IUserAuthService>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var logger = new Mock<ILogger<ChangeUserRoleUseCase>>(MockBehavior.Loose);

        var request = ApplicationTestData.ValidChangeUserRoleRequest();

        uow.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        repo.Setup(x => x.GetUserByIdAsync(Guid.Parse(request.UserId))).ReturnsAsync((User)null!);
        uow.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

        var sut = new ChangeUserRoleUseCase(repo.Object, auth.Object, uow.Object, logger.Object);

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => sut.ExecuteAsync(request));
        Assert.Contains("User not found", ex.Message);

        uow.Verify(x => x.BeginTransactionAsync(), Times.Once);
        uow.Verify(x => x.RollbackAsync(), Times.Once);
        uow.Verify(x => x.CommitAsync(), Times.Never);
        auth.Verify(x => x.ReplaceUserRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        repo.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldReplaceRole_UpdateUser_AndCommit()
    {
        var repo = new Mock<IUserRepository>(MockBehavior.Strict);
        var auth = new Mock<IUserAuthService>(MockBehavior.Strict);
        var uow = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var logger = new Mock<ILogger<ChangeUserRoleUseCase>>(MockBehavior.Loose);

        var request = ApplicationTestData.ValidChangeUserRoleRequest();
        var user = User.Create(new Name("Anderson", "Silva"), new EmailAddress("anderson.silva@example.com"), "identity-123");

        uow.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        repo.Setup(x => x.GetUserByIdAsync(Guid.Parse(request.UserId))).ReturnsAsync(user);
        auth.Setup(x => x.ReplaceUserRoleAsync(user.IdentityId, request.Role.ToLower())).Returns(Task.CompletedTask);
        repo.Setup(x => x.Update(It.IsAny<User>()));
        uow.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        var sut = new ChangeUserRoleUseCase(repo.Object, auth.Object, uow.Object, logger.Object);

        var response = await sut.ExecuteAsync(request);

        Assert.Equal(user.Id.ToString(), response.Id);
        Assert.Equal("Anderson", response.FirstName);
        Assert.Equal("Silva", response.LastName);
        Assert.Equal("anderson.silva@example.com", response.Email);
        Assert.Equal(UserRoles.admin.ToString(), response.Role);

        auth.Verify(x => x.ReplaceUserRoleAsync(user.IdentityId, request.Role.ToLower()), Times.Once);
        repo.Verify(x => x.Update(It.Is<User>(u => u.Role == UserRoles.admin)), Times.Once);
        uow.Verify(x => x.CommitAsync(), Times.Once);
        uow.Verify(x => x.RollbackAsync(), Times.Never);
    }
}

