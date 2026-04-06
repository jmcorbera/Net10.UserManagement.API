using Moq;
using Net10.UserManagement.Application.Users.Commands.DeleteUser;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests.Users.Commands.DeleteUser;

public class DeleteUserCommandHandlerTests
{
    [Fact]
    public async Task DeleteAsync_Should_Call_Repository_DeleteAsync()
    {
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var handler = new DeleteUserCommandHandler(repositoryMock.Object);
        
        await handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);
        
        repositoryMock.Verify(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
