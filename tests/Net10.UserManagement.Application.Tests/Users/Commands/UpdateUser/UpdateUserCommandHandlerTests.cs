using FluentAssertions;
using Moq;
using AutoMapper;
using Net10.UserManagement.Application.Users.Commands.UpdateUser;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests.Users.Commands.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    [Fact]
    public async Task UpdateAsync_Should_Update_User_Email_And_Return_UserResponse()
    {
        var user = User.CreatePending("12345678", 1, "old@example.com", "John", "Doe");
        var updateCommand = new UpdateUserCommand(
            user.Id,
            "new@example.com"
        );

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken _) => u);
        
        var mapperMock = new Mock<IMapper>();
        mapperMock
            .Setup(m => m.Map<UserResponse>(It.IsAny<User>()))
            .Returns((User u) => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CreatedAt = u.CreatedAt
            });

        var handler = new UpdateUserCommandHandler(repositoryMock.Object, mapperMock.Object);
        
        var result = await handler.Handle(new UpdateUserCommand(user.Id, updateCommand.Email), CancellationToken.None);
        
        result.Should().NotBeNull();
        result!.Email.Should().Be(updateCommand.Email);
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();
        var updateCommand = new UpdateUserCommand(
            userId,
            "new@example.com"
        );

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var mapperMock = new Mock<IMapper>();

        var handler = new UpdateUserCommandHandler(repositoryMock.Object, mapperMock.Object);
        
        var result = await handler.Handle(new UpdateUserCommand(userId, updateCommand.Email), CancellationToken.None);
        
        result.Should().BeNull();
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
