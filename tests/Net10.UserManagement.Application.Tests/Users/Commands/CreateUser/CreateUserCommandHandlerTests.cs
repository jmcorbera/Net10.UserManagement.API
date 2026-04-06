using FluentAssertions;
using Moq;
using AutoMapper;
using Net10.UserManagement.Application.Users.Commands.CreateUser;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;

namespace Net10.UserManagement.Application.Tests.Users.Commands.CreateUser;

public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_User_And_Return_UserResponse()
    {
        var createCommand = new CreateUserCommand(
            "john.doe@example.com",
            "John",
            "Doe"
        );

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);
        
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

        var handler = new CreateUserCommandHandler(repositoryMock.Object, mapperMock.Object);
        
        var result = await handler.Handle(createCommand, CancellationToken.None);
        
        result.Should().NotBeNull();
        result!.Email.Should().Be(createCommand.Email);
        result.FirstName.Should().Be(createCommand.FirstName);
        result.LastName.Should().Be(createCommand.LastName);
        repositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
