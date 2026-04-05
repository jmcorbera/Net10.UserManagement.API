using FluentAssertions;
using Moq;
using Net10.UserManagement.Application.Users.Commands.CreateUser;
using Net10.UserManagement.Application.Users.Commands.DeleteUser;
using Net10.UserManagement.Application.Users.Commands.UpdateUser;
using Net10.UserManagement.Application.Users.Queries.GetUsers;
using Net10.UserManagement.Application.Users.Queries.GetUserById;
using Net10.UserManagement.Application.Users.Models;
using Net10.UserManagement.Domain.Entities;
using Net10.UserManagement.Domain.Repositories;
using AutoMapper;

namespace Net10.UserManagement.Application.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_Should_Map_Users_To_UserDtos()
    {
        var users = new[]
        {
            User.CreatePending("john.doe@example.com", "John", "Doe")
        };

        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

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

        var handler = new GetUsersQueryHandler(repositoryMock.Object, mapperMock.Object);

        var result = (await handler.Handle(new GetUsersQuery(), CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(users[0].Id);
        result[0].Email.Should().Be(users[0].Email);
        result[0].FirstName.Should().Be(users[0].FirstName);
        result[0].LastName.Should().Be(users[0].LastName);
        result[0].CreatedAt.Should().Be(users[0].CreatedAt);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_Repository_Returns_Empty()
    {
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var mapperMock = new Mock<IMapper>();

        var handler = new GetUsersQueryHandler(repositoryMock.Object, mapperMock.Object);
    
        var result = (await handler.Handle(new GetUsersQuery(), CancellationToken.None)).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_UserDto_When_User_Exists()
    {
        // Arrange
        var user = User.CreatePending("test@example.com", "John", "Doe");
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
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

        var handler = new GetUserByIdQueryHandler(repositoryMock.Object, mapperMock.Object);
        
        // Act
        var result = await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var mapperMock = new Mock<IMapper>();

        var handler = new GetUserByIdQueryHandler(repositoryMock.Object, mapperMock.Object);
        
        // Act
        var result = await handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);
        
        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_User_And_Return_UserDto()
    {
        // Arrange
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

        var service = new CreateUserCommandHandler(repositoryMock.Object, mapperMock.Object);
        
        // Act
        var result = await service.Handle(createCommand, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(createCommand.Email);
        result.FirstName.Should().Be(createCommand.FirstName);
        result.LastName.Should().Be(createCommand.LastName);
        repositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_User_Email_And_Return_UserDto()
    {
        // Arrange
        var user = User.CreatePending("old@example.com", "John", "Doe");
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

        var service = new UpdateUserCommandHandler(repositoryMock.Object, mapperMock.Object);
        
        // Act
        var result = await service.Handle(new UpdateUserCommand(user.Id, updateCommand.Email), CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(updateCommand.Email);
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        // Arrange
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

        var service = new UpdateUserCommandHandler(repositoryMock.Object, mapperMock.Object);
        
        // Act
        var result = await service.Handle(new UpdateUserCommand(userId, updateCommand.Email), CancellationToken.None);
        
        // Assert
        result.Should().BeNull();
        repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_Should_Call_Repository_DeleteAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var repositoryMock = new Mock<IUserRepository>();
        repositoryMock
            .Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var service = new DeleteUserCommandHandler(repositoryMock.Object);
        
        // Act
        await service.Handle(new DeleteUserCommand(userId), CancellationToken.None);
        
        // Assert
        repositoryMock.Verify(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
